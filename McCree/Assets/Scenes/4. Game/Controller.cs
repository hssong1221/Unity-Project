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
        protected Transform rootPos;
        protected Animator animator;

        protected Controller controller;
        protected PlayerManager playerManager;
        protected PlayerAutoMove playerAutoMove;
        protected UI ui;


        protected void Awake()
        {
            character = GameObject.FindWithTag("Player");
            rootPos = character.transform.Find("Root");

            controller = GetComponent<Controller>();
            playerManager = GetComponent<PlayerManager>();
            playerAutoMove = GetComponent<PlayerAutoMove>();
            ui = GetComponent<UI>();


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

        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(this.ui.hp);

                //stream.SendNext(this.ui.bangSpeechBubble);
                //stream.SendNext(this.ui.bangText);

            }
            else
            {
                // Network player, receive data
                this.ui.hp = (int)stream.ReceiveNext();

                //this.ui.bangSpeechBubble = (Image)stream.ReceiveNext();
                //this.ui.bangText = (Text)stream.ReceiveNext();
            }
        }

        #endregion
    }

}