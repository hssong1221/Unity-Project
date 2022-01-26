using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class Lobby : MonoBehaviourPunCallbacks
    {
        #region Private Fields

        [SerializeField]    // 동시 접속자 수 
        private Text curPlayerText;     

        [SerializeField]     // 룸 리스트 prefab 붙여질 위치
        private Transform content;
        
        [SerializeField]   // 룸 리스트 하나의 Prefab
        private GameObject roomListing;

        private Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>();

        private RoomOptions roomOptions; // 방 옵션

        [SerializeField]   // 방 만들기 팝업
        protected GameObject PopUp;

        [SerializeField]    // 방 만들기 메뉴 안에 방 제목 입력칸
        protected InputField roomName;

        [SerializeField]    // 방 만들기 메뉴 안에 방 비밀번호 입력칸
        protected InputField roomPwd;

        [SerializeField]   // 방 만들기 버튼
        protected Button createRoomBtn;

        [SerializeField]   // 방 만들기 팝업 열기
        protected Button openPopUpBtn;

        [SerializeField]   // 방 만들기 팝업 닫기
        protected Button closePopUpBtn;

        #endregion



        #region 생성자, AddListener

        private void Awake()
        {
            createRoomBtn.onClick.AddListener(CreateRoom);
            openPopUpBtn.onClick.AddListener(Open_PopUp);
            closePopUpBtn.onClick.AddListener(Close_PopUp);
            PopUp.SetActive(false);

        }


        void FixedUpdate()
        {
            CheckPlayerCount();  // update말고 좋은게 있지않을까? 나중에 찾아보자
            
            
        }

        #endregion


        #region 룸에 관련된 사항이 바뀔때 호출되는 함수 
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            GameObject tempRoom = null;

            foreach (var room in roomList)
            {
                // 룸이 삭제된 경우
                if (room.RemovedFromList == true)
                {
                    roomDict.TryGetValue(room.Name, out tempRoom);
                    Destroy(tempRoom);
                    roomDict.Remove(room.Name);
                }
                // 룸 정보가 갱신된경우
                else
                {
                    // 룸이 하나도 없을 경우
                    if (roomDict.ContainsKey(room.Name) == false)
                    {
                        GameObject _room = Instantiate(roomListing, content);
                        _room.GetComponent<RoomList>().myRoomInfo = room;
                        roomDict.Add(room.Name, _room);
                    }
                    // 룸 정보를 갱신하는 경우
                    else
                    {
                        roomDict.TryGetValue(room.Name, out tempRoom);
                        tempRoom.GetComponent<RoomList>().myRoomInfo = room;
                    }
                }
            }

        }
        #endregion

        #region Public Methods

        // 동시 접속자 수 표현
        public void CheckPlayerCount()
        {
            int connectPlayer = PhotonNetwork.CountOfPlayers;
            //Debug.Log("현재 동시접속자수 " + connectPlayer);
            curPlayerText.text = "현재 접속자 수 : " + connectPlayer.ToString();
        }
        public void Open_PopUp()
        {
            CommonFunction.clear(roomName);
            CommonFunction.clear(roomPwd);
            roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 7; // 7명이 기본값
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;
            roomOptions.PublishUserId = true;
            PopUp.SetActive(true);
        }

        public void Close_PopUp()
        {
            PopUp.SetActive(false);
        }

        // 방 만들기 버튼 눌렀을 때
        public void CreateRoom()
        {
            string RoomName = roomName.text;

            // 방 이름이 빈칸이면 못 만들게
            if (string.IsNullOrEmpty(roomName.text))
                return;
            Debug.Log(RoomName);

            // 방 이름으로 방 생성
            PhotonNetwork.JoinOrCreateRoom(RoomName, roomOptions, TypedLobby.Default);

            PunCallbacks.statusText.text = "방 생성 중...";
            PunCallbacks.statusUI.SetActive(true);
        }


        #endregion


        
    }


}
