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
            ui = GetComponent<UI>();

            animator = GetComponent<Animator>();
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