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

        [SerializeField]    // 방 만들기 메뉴
        private GameObject createRoomMenu;

        [SerializeField]    // 방 만들기 메뉴 안에 입력칸
        private InputField roomName;


        #endregion


        #region MonoBehaviour CallBacks

        void FixedUpdate()
        {
            CheckPlayerCount();
        }

        #endregion

        #region MonoBehaviourPunCallbacks Callbacks

        // 방에 들어가면 작동
        public override void OnJoinedRoom()
        {
            Debug.Log("Room Create!");

            // 방 만들기 창 끔
            createRoomMenu.SetActive(false);
            // 룸 씬으로 이동
            SceneManager.LoadScene("Room");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("Room create Failed!!");
            Debug.LogError(message);
        }
        #endregion

        #region Public Methods

        // 동시 접속자 수 표현
        public void CheckPlayerCount()
        {
            int connectPlayer = PhotonNetwork.CountOfPlayers;
            Debug.Log("현재 동시접속자수 " + connectPlayer);
            curPlayerText.text = "현재 접속자 수 : " + connectPlayer.ToString();
        }

        // 방 만들기 버튼 눌렀을 때
        public void CreateRoom()
        {
            // 방 만들기 메뉴 보이게 함
            createRoomMenu.SetActive(true);

            
            Debug.Log("roomname : " + roomName.text);

            // 방 이름이 빈칸이면 못 만들게
            if (string.IsNullOrEmpty(roomName.text))
                return;

            // 방 이름으로 방 생성
            PhotonNetwork.CreateRoom(roomName.text);

        }

        #endregion
    }

}
