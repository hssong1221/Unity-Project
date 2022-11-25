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
        protected PlayerInfo playerInfo;
        protected Interaction interaction;
        protected UI ui;
        protected MineUI mineUI;
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
            playerInfo = GetComponent<PlayerInfo>();
            interaction = GetComponent<Interaction>();
            ui = GetComponent<UI>();
            dataSync = GetComponent<DataSync>();

            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
        }
    }
}