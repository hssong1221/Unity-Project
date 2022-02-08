using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class Controller : MonoBehaviourPunCallbacks
    {
        protected GameObject character; // Character객체 (상속가능) 
        protected Transform rootPos;
        protected Animator animator;
        public static bool isDeath;

        protected Controller controller;
        protected PlayerManager playerManager;
        protected PlayerAutoMove playerAutoMove;
        protected HpUI hpUI;


        protected void Awake()
        {
            character = GameObject.FindWithTag("Player");
            rootPos = character.transform.Find("Root");

            controller = GetComponent<Controller>();
            playerManager = GetComponent<PlayerManager>();
            playerAutoMove = GetComponent<PlayerAutoMove>();
            hpUI = GetComponent<HpUI>();


            animator = character.GetComponent<Animator>();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}