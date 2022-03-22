using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class Room : MonoBehaviourPunCallbacks
    {

        #region  Variable Fields

        [SerializeField]     // 플레이어 prefab 붙여질 위치
        private Transform content;

        [SerializeField]   // 플레이어 하나의 Prefab
        private GameObject playerListing;

        private Dictionary<string, GameObject> playerDict = new Dictionary<string, GameObject>();


        [SerializeField]
        protected Text playerCount;  // 현재인원 / Max인원

        protected string playerCountText;  // 인원 수  텍스트

        [SerializeField]
        protected Text roomName;  // 방 제목

        [SerializeField]
        protected Button startBtn;  // 준비 버튼

        [SerializeField]
        protected Text startText; // 준비 버튼 내용 

        [SerializeField]
        protected Button exitBtn;  // 나가기 버튼

        private PunChat punChat;

        #endregion

        #region 생성자, AddListner, MonoBehavior

        private void Awake()
        {
            exitBtn.onClick.AddListener(Exit_Room);
            startBtn.onClick.AddListener(Start_Game);

            
            playerCountText = "인원 수 : ";
            InvokeRepeating("RoomStatTrans", 1f, 3.0f);
            PhotonNetwork.AutomaticallySyncScene = true;

        }

        private void Start()
        {
            punChat = GameObject.Find("LoadingUI").GetComponent<PunChat>();


            GetCurrentRoomName();      // 현재 방에 이름 가져오기
            GetCurrentRoomPlayers();   // 현재 방에있는 플레이어들
            GetCurrentPlayersCount();  // 현재 방에있는 플레이어 수

            if(!PhotonNetwork.IsMasterClient)
            {
                startText.text = "방장만 시작 가능";
            }
        }

        #endregion

        #region 룸에 플레이어가 나가거나 들어왔을 경우

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            //base.OnPlayerEnteredRoom(newPlayer);
            AddPlayerListing(newPlayer);
            GetCurrentPlayersCount();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            GameObject tempPlayer = null;
            playerDict.TryGetValue(otherPlayer.UserId, out tempPlayer);
            Destroy(tempPlayer);
            playerDict.Remove(otherPlayer.UserId);
            GetCurrentPlayersCount();
        }

        #endregion


        #region Public Methods

        // 방 나가기 버튼 눌렀을 때
        public void Exit_Room()
        {
            LoadingUI.Instance.msg_Text.text = "방 떠나는 중...";
            LoadingUI.Instance.msg_Canvas.SetActive(true);
            string msgs = string.Format("​<color=navy>[{0}]님이 퇴장하셨습니다.</color>", PhotonNetwork.LocalPlayer.NickName);
            PunChat.Instance.chatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, msgs);
            PunChat.Instance.behave = "ExitRoom";
            PunChat.Instance.chatClient.Unsubscribe(new string[] {PhotonNetwork.CurrentRoom.Name});
        }

        // 시작하기 버튼을 눌렀을 때
        public void Start_Game()
        {
            // 마스터 클라이언트만 로드레벨을 해야함
            if (PhotonNetwork.IsMasterClient) 
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;

                GameLoading(); // 자기화면 로딩
                punChat.Function_Loading_GameScene(); // PunChat에서 남의화면 로딩하라고 일러줌

                PhotonNetwork.IsMessageQueueRunning = false;
                PhotonNetwork.LoadLevel("Game");
            }
        }

        private void GameLoading()
        {
            LoadingUI.Instance.msg_Text.text = "게임에 참여하는 중...";
            LoadingUI.Instance.msg_Canvas.SetActive(true);
        }

        public void AddPlayerListing(Player playerList)
        {
            GameObject _playerDict = Instantiate(playerListing, content);
            _playerDict.GetComponent<PlayerList>().myPlayer = playerList;
            playerDict.Add(playerList.UserId, _playerDict);
        }

        public void GetCurrentPlayersCount()
        {
            playerCount.text = (playerCountText + 
            PhotonNetwork.CurrentRoom.PlayerCount + " / " + 
            PhotonNetwork.CurrentRoom.MaxPlayers);
        }

        public void GetCurrentRoomPlayers()
        {
            foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
            {
                AddPlayerListing(playerInfo.Value);
            }
        }

        public void GetCurrentRoomName()
        {
            roomName.text = PhotonNetwork.CurrentRoom.Name;
        }

        // 룸 상태를 변화를 시켜서 로비에있는 OnRoomListUpdate()함수가 알수 있음
        public void RoomStatTrans()
        {
            //Debug.Log("룸 상태 변경");
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsOpen = true;
            }
        }

        #endregion


    }

}