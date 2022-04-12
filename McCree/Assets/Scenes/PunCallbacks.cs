using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class PunCallbacks : MonoBehaviourPunCallbacks
    {

        private static PunCallbacks pInstance;

        public static PunCallbacks Instance
        {
            get { return pInstance; }
        }


        private Transform[] canvasChildrens;

        private void Awake()
        {
            pInstance = this;

            DontDestroyOnLoad(this.gameObject);
        }

        // 마스터 서버와 연결 시 작동
        public override void OnConnectedToMaster()
        {
            Debug.Log("서버 연결 성공");
            LoadingUI.Instance.msg_Text.text = "서버와 연결 성공!";
            LoadingUI.Instance.msg_Text.text = "로비에 접속 중...";
            PhotonNetwork.JoinLobby();
        }

        // 서버 접속 실패시 자동 실행
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Connecting failed!! Trying re-connect..");
            LoadingUI.Instance.msg_Text.text = "서버와 연결 실패, 접속 재시도 중...";
            LoadingUI.Instance.msg_Canvas.SetActive(true);
            // 설정한 정보로 마스터 서버 접속 시도
            PhotonNetwork.ConnectUsingSettings();
        }

        // 로비에 참가했한 후에 실행 (룸에서 나왓을 때도 실행)
        public override void OnJoinedLobby()
        {
            //base.OnJoinedLobby();
            Debug.Log("로비 참가 성공");
            LoadingUI.Instance.msg_Text.text = "로비와 연결 성공!";
            SceneManager.LoadScene("Lobby");
        }

        // 로비에서 나가면서 실행
        public override void OnLeftLobby()
        {
            base.OnLeftLobby();
            Debug.Log("로비 떠나기 성공");
            LoadingUI.Instance.msg_Text.text = "로비 떠나기 성공";
            LoadingUI.Instance.msg_Canvas.SetActive(false);
        }

        // 방에 접속 한 후에 실행
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log("방 참가 성공");
            LoadingUI.Instance.msg_Text.text = "방 참가 성공";
            LoadingUI.Instance.msg_Canvas.SetActive(false);

            PunChat.Instance.behave = "EnterRoom";
            PunChat.Instance.chatClient.Subscribe(new string[] { PhotonNetwork.CurrentRoom.Name }, 10);
        }


        // 방에서 나가면서 실행
        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            Debug.Log("방 나가기 성공");
            LoadingUI.Instance.msg_Text.text = "방 나가기 성공";
            LoadingUI.Instance.msg_Canvas.SetActive(false);
            // 저절로 OnConnectedToMaster 실행한다.
        }

        // 방을 만든 후에 실행
        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            Debug.Log("방 만들기 성공");
            LoadingUI.Instance.msg_Text.text = "방 생성 성공";
            LoadingUI.Instance.msg_Canvas.SetActive(false);
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