using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class AddListnerLobby : MonoBehaviour
    {
        #region Private Fields

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

        private void Awake()
        {
            createRoomBtn.onClick.AddListener(CreateRoom);
            openPopUpBtn.onClick.AddListener(Open_PopUp);
            closePopUpBtn.onClick.AddListener(Close_PopUp);
        }

        #region Function Fields
        public void Open_PopUp()
        {
            CommonFunction.clear(roomName);
            CommonFunction.clear(roomPwd);
            roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 7; // 7명이 기본값
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;
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
        }
        #endregion

        
    }
}