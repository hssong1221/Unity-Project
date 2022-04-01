using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class PlayerAutoMove : Controller
    {
        public GameObject targetedEnemy;
        //public bool isPlayerAlive;


        private void Awake()
        {
            base.Awake();
        }

        void Start()
        {

        }
        
        void Update()
        {
            if (photonView.IsMine)
            {
                if (targetedEnemy != null) // PlayerManager에서 isAiming상태에서 적을 클릭했을시에만 발생
                {
                    // 뱅 범위 사거리안에 적 플레이어가 안들어와있을때
                    if (Vector3.Distance(character.transform.position,
                        targetedEnemy.transform.position) > playerManager.maxAttackDistance)
                    {
                        // 이동
                        playerManager.agent.SetDestination(targetedEnemy.transform.position);
                        playerManager.agent.stoppingDistance = playerManager.maxAttackDistance;
                    }
                    // 뱅 범위 사거리안에 적이 들어와 있을때
                    else
                    {
                        playerManager.agent.SetDestination(transform.position); // 쏠때 제자리
                        transform.LookAt(targetedEnemy.transform); // 쏠때 적 바라보기

                        if (playerInfo.myItemList[0].itemCount > 0) // 뱅 있으면
                        {

                            //GameObject bullet = ObjectPool.Instance.GetObject(1);
                            //bullet.transform.position = 

                            animator.SetTrigger("Bang");

                            playerInfo.myItemList[0].itemCount -= 1;


                            string shooterNick = PhotonNetwork.LocalPlayer.NickName;
                            string targetNick = targetedEnemy.GetComponent<PhotonView>().Owner.NickName;

                            // Bang 애니메이션 실행
                            targetedEnemy.GetComponent<PhotonView>().RPC("Bang_Trigger", RpcTarget.All, shooterNick, targetNick);

                            // 적 한테 데미지
                            //targetedEnemy.GetComponent<PhotonView>().RPC("Damaged", RpcTarget.All);
                            targetedEnemy = null;
                        }
                        else
                        {
                            playerManager.Character_Notice_Text("<color=#FF2D2D>" + "뱅 이 존재하지 않습니다!" + "</color>");
                        }
                    }
                }
            }
        }


        public void TurnOnBangBubble()
        {
            Debug.Log("떠야함22");
            ui.bangGifImg.enabled = true;
            ui.bangGifImg.GetComponent<Animator>().Play("BangAnim", -1, 0);
            // 두번째 인자-1일경우 첫 state재생 
            // 세번째 인자 표준화된(normalized) 애니메이션 시간 0 처음 1 끝
        }
    }
}
