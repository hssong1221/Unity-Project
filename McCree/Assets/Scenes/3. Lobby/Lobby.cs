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

        [SerializeField]   // 방 최대 인원 수 설정 버튼들
        protected List<Button> maxPlayerCountBtns;

        #endregion



        #region 생성자, AddListener

        private void Awake()
        {
            createRoomBtn.onClick.AddListener(CreateRoom);
            openPopUpBtn.onClick.AddListener(Open_PopUp);
            closePopUpBtn.onClick.AddListener(Close_PopUp);
            PopUp.SetActive(false);


            // 방 최대인원수 버튼 onclick addlistener
            for (int i=0; i<maxPlayerCountBtns.Count; i++)
            {
                Button btn = maxPlayerCountBtns[i];
                int index = i;
                btn.onClick.AddListener(() => UpdateMaxPlayerCount(index + 4));
            }

            // 본래 포톤 플레이어 체크는 5초마다 한번씩 이루어진다고 한다.
            InvokeRepeating("CheckPlayerCount", 0f, 5.0f); 
        }

        #endregion



        #region 룸에 관련된 사항이 바뀔때 호출되는 함수 


        // 룸 리스트들를 업데이트하는것이 아니고 변경된 roomList의 정보를 받아오는것이다.
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            GameObject tempRoom = null;

            foreach (var room in roomList)
            {
                // 룸이 삭제된 경우
                if (room.RemovedFromList == true)
                {
                    // room.Name을 찾아서 tempRoom(gameObject)에 저장한뒤 파괴하고
                    // 리스트에서도 삭제한다.
                    roomDict.TryGetValue(room.Name, out tempRoom);
                    Destroy(tempRoom);
                    roomDict.Remove(room.Name);
                }
                // 룸 정보가 갱신된경우
                else
                {
                    // 룸이 새로 생성되었을 경우
                    if (roomDict.ContainsKey(room.Name) == false)
                    {
                        // roomListing을 생성하고 content위치에 생성되는
                        // _room (gameObject)를 생성한다
                        GameObject _room = Instantiate(roomListing, content);
                        // 룸 정보를 저장 (방 인원수, 이름)
                        _room.GetComponent<RoomList>().myRoomInfo = room;
                        roomDict.Add(room.Name, _room);
                    }
                    // 룸 정보를 갱신하는 경우 (방 정보 수정은 아직 구현안함)
                    else
                    {
                        // 변경된 해당 room.Name을 찾아 방 정보를 다시 저장한다
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
            curPlayerText.text = "현재 접속자 수 : " + connectPlayer.ToString();
        }

        public void Open_PopUp()
        {
            Debug.Log("열기버튼누름");
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
            Debug.Log("닫기버튼누름");
            PopUp.SetActive(false);
        }

        // 방 만들기 버튼 눌렀을 때
        public void CreateRoom()
        {
            Debug.Log("생성버튼누름");
            string RoomName = roomName.text;

            // 방 이름이 빈칸이면 못 만들게
            if (string.IsNullOrEmpty(roomName.text))
                return;

            // 방 이름으로 방 생성
            PhotonNetwork.JoinOrCreateRoom(RoomName, roomOptions, TypedLobby.Default);
            LoadingUI.Instance.msg_Text.text = "방 생성 중...";
            LoadingUI.Instance.msg_Canvas.SetActive(true);
        }

        // 방 최대인원수 선택
        public void UpdateMaxPlayerCount(int count)
        {
            Debug.Log(count);
            roomOptions.MaxPlayers = (byte)count;

            for (int i=0; i< maxPlayerCountBtns.Count; i++)
            {
                if (i == count - 4)
                {
                    maxPlayerCountBtns[i].image.color = new Color(0f, 0f, 0f, 1f);
                }
                else
                {
                    maxPlayerCountBtns[i].image.color = new Color(0f, 0f, 0f, 0f);
                }
            }
        }

        #endregion


        
    }


}
