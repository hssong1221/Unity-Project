using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Chat;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace com.ThreeCS.McCree
{
    public class MasterKey : MonoBehaviour, IChatClientListener
    {
        // PlayFab에 로그인은 안하고 7명 방에 자동 참여 방 나갈려하면 오류뜰꺼임

        public static ChatClient chatClient;
        public static string behave;

        private List<string> chatList = new List<string>();
        private Button sendBtn;
        private Text chatLog;
        private InputField chatInput;
        private ScrollRect scr;

        private string lobbyName = "Lobby";

        void Start()
        {
            DontDestroyOnLoad(this.gameObject);
            chatClient = new ChatClient(this);
            chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,
                "1.0", new Photon.Chat.AuthenticationValues(PhotonNetwork.LocalPlayer.NickName));
            PhotonNetwork.IsMessageQueueRunning = true;
            LoadingUI.msg_Canvas.SetActive(true);
            LoadingUI.close_Btn.gameObject.SetActive(false);
            LoadingUI.msg_Text.text = "채팅 서버에 접속 중...";
            enabled = true;
            behave = "InitialConnecting";
        }
        void Update()
        {
            if (chatClient != null)
                chatClient.Service();
            if (Input.GetKeyDown(KeyCode.Return) && sendBtn != null)
            {
                Send_Chat();
            }
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Register" || scene.name == "Login")
            {
                LoadingUI.msg_Canvas.SetActive(false);
                LoadingUI.close_Btn.gameObject.SetActive(true);
            }
            else if (scene.name == "Lobby" || scene.name == "Room")
            {
                LoadingUI.msg_Canvas.SetActive(false);
                LoadingUI.close_Btn.gameObject.SetActive(false);

                Debug.Log("씬 교체됨, 현재 씬: " + scene.name);
                // 씬 교체될때 채팅 창에 관련된 오브젝트 다시 지정
                // chatLog - 대화창
                // sendBtn - 전송 버튼
                // chatInput - 대화 인풋 필드
                // scr - 챗 scrollReact
                var tempchatLog = GameObject.Find("ChatLog");
                if (tempchatLog != null)
                    chatLog = tempchatLog.GetComponent<UIText>();

                var tempsendBtn = GameObject.Find("sendBtn");
                if (tempsendBtn != null)
                {
                    sendBtn = tempsendBtn.GetComponent<Button>();
                    sendBtn.onClick.AddListener(Send_Chat);
                }

                var tempchatInput = GameObject.Find("chatInputField");
                if (tempchatInput != null)
                    chatInput = tempchatInput.GetComponent<InputField>();

                var tempsrc = GameObject.Find("ChatScroll");
                if (tempsrc != null)
                    scr = tempsrc.GetComponent<ScrollRect>();

                chatList = new List<string>();
            }
            else if (scene.name == "Game")
            {
                LoadingUI.msg_Canvas.SetActive(false);
            }



        }
        public void Send_Chat()
        {
            if (chatInput.text.Equals(""))
                return;
            string msgs = string.Format("[{0}]", PhotonNetwork.LocalPlayer.NickName);
            msgs += " " + chatInput.text;

            if (PhotonNetwork.InLobby)
                chatClient.PublishMessage(lobbyName, msgs);
            else if (PhotonNetwork.InRoom)
                chatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, msgs);
            chatInput.ActivateInputField();
            CommonFunction.clear(chatInput);
        }

        public void AddLine(string lineString)
        {
            if (chatLog != null && scr != null)
            {
                chatLog.text += lineString;
                scr.verticalNormalizedPosition = 0.0f;
            }
        }


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
            LoadingUI.msg_Text.text = "채팅 서버 연결 성공!";
            //Debug.Log("포톤 채팅 연결 완료");
            chatClient.Subscribe(new string[] { lobbyName }, 10);
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
            string msgs = messages[senders.Length - 1] + "\n";

            // !(룸에 없을 때 && 메세지가 로비로 부터왔을때)
            // == 룸에 있거나 메세지가 로비로부터 온게 아닐때 대화 로그 붙이기

            // 로비일때는 로비 대화만 받고
            // 룸에있을때는 로비 대화무시하고 방에있는 대화만 받는다는 뜻
            if (!(PhotonNetwork.InRoom && channelName == lobbyName))
                AddLine(msgs);
            //throw new System.NotImplementedException();
        }

        public void OnPrivateMessage(string sender, object message, string channelName)
        {
            //throw new System.NotImplementedException();
        }

        public void OnSubscribed(string[] channels, bool[] results)
        {
            // 로비는 최초 연결되어있는 상태고
            // 방에 들어갔을때 해당 방을 구독하고
            // 방에서 나갔을때 해당 방을 구독취소하는 식

            if (behave == "EnterRoom") // 방에 입장하였을때
            {
                LoadingUI.msg_Canvas.SetActive(false);
                SceneManager.LoadScene("Room");
                string msgs = string.Format("​<color=navy>[{0}]님이 입장하셨습니다.</color>", PhotonNetwork.LocalPlayer.NickName);
                chatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, msgs);
            }

            else if (behave == "InitialConnecting") // 최초 연결할때
            {
                LoadingUI.msg_Text.text = "로비 채팅에 접속 중...";
                PhotonNetwork.ConnectUsingSettings();
            }
            //throw new System.NotImplementedException();
        }

        public void OnUnsubscribed(string[] channels)
        {

            if (behave == "ExitRoom") // 방에 나갈때
            {
                PhotonNetwork.LeaveRoom();
            }
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


        
    }
}