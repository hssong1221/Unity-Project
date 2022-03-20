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

        protected Controller controller;
        protected PlayerManager playerManager;
        protected PlayerAutoMove playerAutoMove;
        protected PlayerInfo playerInfo;
        protected Interaction interaction;
        protected UI ui;
        protected MineUI mineUI;

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

            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            playerManager.isBanging = false;
        }



        #region 애니메이션 제어 
        protected void Bang_Speech_Bubble_Anim_Start()
        {
            playerManager.isBanging = true;
        }

        protected void Bang_Speech_Bubble_Anim()
        {
            photonView.RPC("TurnOnBangBubble", RpcTarget.All);
        }

        protected void Bang_Speech_Bubble_Anim_End()
        {
            playerManager.isBanging = false;
        }
        #endregion
    }

}