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

            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
                animator.SetTrigger("Banged");
        }



        #region 뱅 쏠때 (Shooting Gun)
        protected void Bang_Speech_Bubble_Anim_Start()
        {
            if (photonView.IsMine)
            {
                playerManager.isBanging = true;
                playerManager.isAiming = false;
            }
        }

        protected void Bang_Speech_Bubble_Anim()
        {
            playerAutoMove.TurnOnBangBubble();
        }

        protected void Bang_Speech_Bubble_Anim_End()
        {
            if (photonView.IsMine)
            {
                playerManager.isBanging = false;
            }
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