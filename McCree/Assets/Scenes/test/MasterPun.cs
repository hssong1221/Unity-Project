using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

namespace com.ThreeCS.McCree
{
    public class MasterPun : MonoBehaviourPunCallbacks
    {

        string player = "플레이어";
        int random;
        RoomOptions roomOptions; // 방 옵션

        private void Start()
        {
            random = Random.Range(1, 20);
        }

        // Start is called before the first frame update
        public override void OnConnectedToMaster()
        {
            PhotonNetwork.NickName = player + " " + random;
            Debug.Log("서버 연결 성공");
            LoadingUI.msg_Text.text = "서버와 연결 성공!";
            LoadingUI.msg_Text.text = "로비에 접속 중...";
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("로비 참가 성공");
            LoadingUI.msg_Text.text = "로비와 연결 성공!";


            roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 7; // 7명이 기본값
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;
            roomOptions.PublishUserId = true;

            string RoomName = "test";

            // 방 이름으로 방 생성
            PhotonNetwork.JoinOrCreateRoom(RoomName, roomOptions, TypedLobby.Default);
            LoadingUI.msg_Text.text = "방 생성 중...";
            LoadingUI.msg_Canvas.SetActive(true);

        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Connecting failed!! Trying re-connect..");
            LoadingUI.msg_Text.text = "서버와 연결 실패, 접속 재시도 중...";
            LoadingUI.msg_Canvas.SetActive(true);
            // 설정한 정보로 마스터 서버 접속 시도
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            Debug.Log("방 참가 성공");
            LoadingUI.msg_Text.text = "방 참가 성공";
            LoadingUI.msg_Canvas.SetActive(false);

            MasterKey.behave = "EnterRoom";
            MasterKey.chatClient.Subscribe(new string[] { PhotonNetwork.CurrentRoom.Name }, 10);
        }


        // 방을 만든 후에 실행
        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            Debug.Log("방 만들기 성공");
            LoadingUI.msg_Text.text = "방 생성 성공";
            LoadingUI.msg_Canvas.SetActive(false);
            //SceneManager.LoadScene("Room");
        }

        // 방 만들기 실패하면 실행
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);
            Debug.Log("방 만들기 실패");
        }
    }
}