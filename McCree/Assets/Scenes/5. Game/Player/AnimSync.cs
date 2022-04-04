using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;


namespace com.ThreeCS.McCree
{
    public class AnimSync : Controller
    {
        private const byte ShootBulletEventCode = 98;
        private const byte PlayAnimationEventCode = 99;

        void Awake()
        {
            base.Awake();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            PhotonNetwork.NetworkingClient.EventReceived += AnymSyncFun;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            PhotonNetwork.NetworkingClient.EventReceived -= AnymSyncFun;
        }

        public void AnymSyncFun(EventData obj)
        {
            if (obj.Code == PlayAnimationEventCode)
            {
                object[] data = (object[])obj.CustomData;
                int targetPhotonView = (int)data[0];
                if (targetPhotonView == photonView.ViewID)
                {
                    string animatorParameter = (string)data[1];
                    string parameterType = (string)data[2];
                    object parameterValue = (object)data[3];
                    switch (parameterType)
                    {
                        case "Trigger":
                            animator.SetTrigger(animatorParameter);
                            break;
                        case "Bool":
                            animator.SetBool(animatorParameter, (bool)parameterValue);
                            break;
                        case "Float":
                            animator.SetFloat(animatorParameter, (float)parameterValue);
                            break;
                        case "Int":
                            animator.SetInteger(animatorParameter, (int)parameterValue);
                            break;
                        default:
                            break;
                    }
                }
            }
            else if(obj.Code == ShootBulletEventCode)
            {
                object[] data = (object[])obj.CustomData;
                int shooterPhotonView = (int)data[0];
                int targetPhotonView = (int)data[1];

                if (shooterPhotonView == photonView.ViewID)
                {
                    if (controller.animcoroutine != null)
                        Stop_Anim_Coroutine();
                    controller.animcoroutine = Set_Bullet_Target(PhotonView.Find(targetPhotonView).gameObject.GetComponent<PlayerManager>().bulletAttackedPos);
                    StartCoroutine(controller.animcoroutine);
                }
            }
        }



        // 애니메이션 동기화
        public void SendPlayAnimationEvent(int photonViewID, string animatorParameter, string parameterType, object parameterValue = null)
        {
            if (photonView.IsMine)
            {
                switch (parameterType)
                {
                    case "Trigger":
                        animator.SetTrigger(animatorParameter);
                        break;
                    case "Bool":
                        animator.SetBool(animatorParameter, (bool)parameterValue);
                        break;
                    case "Float":
                        animator.SetFloat(animatorParameter, (float)parameterValue);
                        break;
                    case "Int":
                        animator.SetInteger(animatorParameter, (int)parameterValue);
                        break;
                    default:
                        break;
                }
                object[] content = new object[] { photonViewID, animatorParameter, parameterType, parameterValue };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(PlayAnimationEventCode, content, raiseEventOptions, SendOptions.SendReliable);
            }
            
        }

        public void Shooting_Bullet(int photonViewID, int targetPvID ,GameObject targetpos)
        {
            if (photonView.IsMine)
            {
                if (controller.animcoroutine != null)
                    Stop_Anim_Coroutine();
                controller.animcoroutine = Set_Bullet_Target(targetpos);
                StartCoroutine(controller.animcoroutine);

                object[] content = new object[] { photonViewID, targetPvID };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(ShootBulletEventCode, content, raiseEventOptions, SendOptions.SendReliable);
            }
        }
    }
}