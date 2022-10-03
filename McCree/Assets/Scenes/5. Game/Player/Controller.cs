using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class Controller : MonoBehaviourPunCallbacks
    {
        protected GameObject character; // Character객체 (상속가능) 
        //protected Transform rootPos;
        protected Animator animator;
        protected Rigidbody rb;

        protected Controller controller;
        protected PlayerManager playerManager;
        protected PlayerAutoMove playerAutoMove;
        protected PlayerInfo playerInfo;
        protected Interaction interaction;
        protected UI ui;
        protected MineUI mineUI;
        protected AnimSync animSync;

        protected DataSync dataSync; 

        protected bool trigger;

        private IEnumerator _animcoroutine;
        public IEnumerator animcoroutine
        {
            get { return _animcoroutine; }
            set
            {
                _animcoroutine = value;
            }
        }

        protected void Awake()
        {

            character = transform.gameObject;
            //character = GameObject.FindWithTag("Player");
            //rootPos = character.transform.Find("Root");

            controller = GetComponent<Controller>();
            playerManager = GetComponent<PlayerManager>();
            playerAutoMove = GetComponent<PlayerAutoMove>();
            playerInfo = GetComponent<PlayerInfo>();
            interaction = GetComponent<Interaction>();
            ui = GetComponent<UI>();
            animSync = GetComponent<AnimSync>();

            dataSync = GetComponent<DataSync>(); 

            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
        }


        #region 뱅 쏠때 (Shooting Gun)
        protected void Bang_Speech_Bubble_Anim_Start()
        {
            Debug.Log("7번 뜨는 이유??");
            playerManager.isBanging = true;
            playerManager.isAiming = false;
        }

        protected IEnumerator Set_Bullet_Target(GameObject target)
        {
            yield return new WaitUntil(() => trigger == true);

            GameObject bullet = ObjectPool.Instance.GetObject(4);
            bullet.GetComponent<Bullet>().target = target;
            bullet.transform.position = playerInfo.equipedWeapon.bulletPos.position;
            bullet.GetComponent<Bullet>().enabled = true; // FixedUpdate활성화
        }

        protected void Stop_Anim_Coroutine() // 코루틴은 해당 파일에서 꺼야함
        {
            if (animcoroutine != null)
            {
                StopCoroutine(animcoroutine);
            }
        }

        protected void Bang_Speech_Bubble_Anim()
        {
            trigger = true;
            playerAutoMove.TurnOnBangBubble();
        }

        protected void Bang_Speech_Bubble_Anim_End()
        {
            Stop_Anim_Coroutine();

            trigger = false;
            playerManager.isBanging = false;
        }

        #endregion

        #region 뱅 맞을때 (Flying Death, Stand Up)
        protected void IsAttacked_True()
        {
            if (photonView.IsMine)
            {
                playerManager.isBangeding = true;
            }
        }

        protected void IsAttacked_False()
        {
            if (photonView.IsMine)
            {
                playerManager.isBangeding = false;
            }
        }

        #endregion
    }

}