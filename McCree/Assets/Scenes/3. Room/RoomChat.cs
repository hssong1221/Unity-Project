using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{   
    //룸 챗은 Photon Chat 사용하는것이 아니라 Rpc 사용하는것임
    //rpc는 같은 룸에있는 대상으로 가는것이기 때문에
    public class RoomChat : MonoBehaviourPunCallbacks
    {

        public List<string> chatList = new List<string>();
        public Button sendBtn;
        public Text chatLog;
        public InputField chatInput;
        public ScrollRect scr;

        private PhotonView pv;


        private void Awake()
        {
            sendBtn.onClick.AddListener(Send_Chat);
        }

        void Start()
        {
            pv = PhotonView.Get(this);
        }

        void Update()
        {
            //chatInput.ActivateInputField();
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Send_Chat();
            }
        }

        public void Send_Chat()
        {
            if (chatInput.text.Equals(""))
            {
                Debug.Log("Empty Chat");
                return;
            }
            string msg = string.Format("[{0}] {1}",
                PhotonNetwork.LocalPlayer.NickName, chatInput.text);
            pv.RPC("ReceiveMsg", RpcTarget.All, msg); // (같은곳에)연결되어있는 모두한테 보냄
            chatInput.ActivateInputField();
            CommonFunction.clear(chatInput);
        }


        [PunRPC] // PunRPC를 통하여 (같은룸에)연결되어있는 모든 클라이언트 ReceiveMsg 실행
        public void ReceiveMsg(string msg)
        {
            chatLog.text += msg + "\n";
            scr.verticalNormalizedPosition = 0.0f;
        }


        // 룸도 Photon Chat으로 구현하면 입장, 퇴장 알림문 가능함
        // 일단 보류


        //[PunRPC]
        //public void InfoMsg(string msg)
        //{
        //    chatLog.text += string.Format("​<color=navy>{0}</color>\n", msg);
        //    scr.verticalNormalizedPosition = 0.0f;
        //}


        // 최초로 Lobby에서 Room으로 가기전까지 RoomChat생성이 되질않으므로
        // 클라이언트가 최초로 방에 들어갈때까진 OnJoinedRoom이 실행되질않음 
        //public override void OnJoinedRoom()  
        //{
        //    Debug.Log("방에 들어옴");
        //    string msg = string.Format("{0}",
        //    PhotonNetwork.LocalPlayer.NickName + "님이 입장하셨습니다.");
        //    pv.RPC("InfoMsg", RpcTarget.All, msg); 
        //}


    }
}