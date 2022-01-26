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

        [SerializeField]
        protected Text roomName;  // 방 제목

        [SerializeField]
        protected Button readyBtn;  // 준비 버튼

        [SerializeField]
        protected Button exitBtn;  // 나가기 버튼

        #endregion

        #region 생성자, AddListner

        private void Awake()
        {
            exitBtn.onClick.AddListener(Exit_Room);
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void Start()
        {
            GetCurrentRoomName();      // 현재 방에 이름 가져오기
            GetCurrentRoomPlayers();   // 현재 방에있는 플레이어들
        }

        #endregion



        #region 룸에 플레이어가 나가거나 들어왔을 경우

        public void AddPlayerListing(Player playerList)
        {
            GameObject tempPlayer = null;

            GameObject _playerDict = Instantiate(playerListing, content);
            _playerDict.GetComponent<PlayerList>().myPlayer = playerList;
            Debug.Log(playerList.UserId + "    " + playerList.NickName);
            playerDict.Add(playerList.UserId, _playerDict);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            //base.OnPlayerEnteredRoom(newPlayer);
            AddPlayerListing(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            GameObject tempPlayer = null;
            playerDict.TryGetValue(otherPlayer.UserId, out tempPlayer);
            Destroy(tempPlayer);
            playerDict.Remove(otherPlayer.UserId);
        }
        #endregion


        #region Public Methods
        
        public void Exit_Room()
        {
            PhotonNetwork.LeaveRoom();
            PunCallbacks.statusText.text = "방 떠나는 중...";
            PunCallbacks.statusUI.SetActive(true);
            //PhotonNetwork.JoinLobby();
        }
        public void GetCurrentRoomPlayers()
        {
            Debug.Log(PhotonNetwork.CurrentRoom.Players);
            foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
            {
                AddPlayerListing(playerInfo.Value);
            }
        }

        public void GetCurrentRoomName()
        {
            roomName.text = PhotonNetwork.CurrentRoom.Name;
        }

        #endregion
    }

}