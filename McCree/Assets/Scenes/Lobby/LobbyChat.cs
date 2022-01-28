using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

using Photon.Chat;
using ExitGames.Client.Photon;

namespace com.ThreeCS.McCree
{
    // 로비 채팅은 Photon Chat을 이용하여 사용
    public class LobbyChat : MonoBehaviour, IChatClientListener
    {
        public List<string> chatList = new List<string>();
        public Button sendBtn;
        public Text chatLog;
        public InputField chatInput;
        public ScrollRect scr;

        private string roomName = "Lobby";

        ChatClient chatClient;

        private void Awake()
        {
            sendBtn.onClick.AddListener(Send_Chat);
        }


        void Start()
        {
            //Photon Chat 연결 시도 
            this.chatClient = new ChatClient(this);
            this.chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, 
                "1.0", new Photon.Chat.AuthenticationValues(PhotonNetwork.NickName));
            PhotonNetwork.IsMessageQueueRunning = true;

            AddLine("연결시도\n");
        }

        void Update()
        {
            chatClient.Service();
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
            chatClient.PublishMessage(roomName, chatInput.text);
            chatInput.ActivateInputField();
            CommonFunction.clear(chatInput);
        }

        // ChatLog에 추가
        public void AddLine(string lineString)
        {
            chatLog.text += lineString;
            scr.verticalNormalizedPosition = 0.0f;
        }

        #region IChatClientListener
        // 이 밑은 IChatClientListener 인터페이스
        // OnJoinedRoom exitroom 뭐시기처럼 메세지 받거나 친추등 기능 구현할수있는 인터페이스
        public void DebugReturn(DebugLevel level, string message)
        {
            if (level == ExitGames.Client.Photon.DebugLevel.ERROR)
            {
                Debug.LogError(message);
            }
            else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
            {
                Debug.LogWarning(message);
            }
            else
            {
                Debug.Log(message);
            }
        }

        public void OnConnected()
        {
            AddLine("서버에 연결되었습니다.\n");
            chatClient.Subscribe(new string[] { roomName }, 10);
        }

        public void OnDisconnected()
        {
            AddLine("서버에 연결이 끊어졌습니다.\n");
            //throw new System.NotImplementedException();
        }

        public void OnChatStateChange(ChatState state)
        {
            Debug.Log("OnChatStateChange = " + state);
            //throw new System.NotImplementedException();
        }

        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            string msgs = "";
            for (int i = 0; i < senders.Length; i++)
            {
                msgs += "[" + senders[i] + "] " + messages[i] + "\n";
            }
            AddLine(msgs);
            //throw new System.NotImplementedException();
        }

        public void OnPrivateMessage(string sender, object message, string channelName)
        {
            //throw new System.NotImplementedException();
        }

        public void OnSubscribed(string[] channels, bool[] results)
        {
            AddLine(string.Format("채널 입장 ({0})\n", string.Join(",", channels)));
            //throw new System.NotImplementedException();
        }

        public void OnUnsubscribed(string[] channels)
        {
            AddLine(string.Format("채널 퇴장 ({0})\n", string.Join(",", channels)));
            //throw new System.NotImplementedException();
        }

        public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {
            //throw new System.NotImplementedException();
        }

        public void OnUserSubscribed(string channel, string user)
        {
            //throw new System.NotImplementedException();
        }

        public void OnUserUnsubscribed(string channel, string user)
        {
            //throw new System.NotImplementedException();
        }
        #endregion
    }
}