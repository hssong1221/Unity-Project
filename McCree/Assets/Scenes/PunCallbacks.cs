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
        public static GameObject statusUI;  // 네트워크 상태 UI
        public static Text statusText;      // 네트워크 상태 텍스트
        // stataic 변수는 처음에 지정을 못해주는듯
        // Awake함수에서 하위요소를 불러와서 각각 저장

        private Transform[] canvasChildrens;

        private void Awake()
        {
            canvasChildrens = transform.GetComponentsInChildren<Transform>();
            foreach (Transform child in canvasChildrens)
            {
                if (child.name == "BackGround")
                {
                    statusUI = child.gameObject;
                    statusUI.SetActive(false);
                }
                else if (child.name == "StatusText")
                    statusText = child.gameObject.GetComponent<Text>();
            }
            DontDestroyOnLoad(this.gameObject);
        }

        // 마스터 서버와 연결 시 작동
        public override void OnConnectedToMaster()
        {
            Debug.Log("서버 연결 성공");
            statusText.text = "서버에 연결 성공!";
            statusUI.SetActive(false);
            PhotonNetwork.JoinLobby();
        }

        // 서버 접속 실패시 자동 실행
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Connecting failed!! Trying re-connect..");
            statusText.text = "서버와 연결 실패, 접속 재시도 중...";
            statusUI.SetActive(true);
            // 설정한 정보로 마스터 서버 접속 시도
            PhotonNetwork.ConnectUsingSettings();
        }

        // 로비에 참가했한 후에 실행 (룸에서 나왓을 때도 실행)
        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            Debug.Log("로비 참가 성공");
            statusText.text = "로비 접속 성공";
            statusUI.SetActive(false);
            SceneManager.LoadScene("Lobby");
        }

        // 로비에서 나가면서 실행
        public override void OnLeftLobby()
        {
            base.OnLeftLobby();
            Debug.Log("로비 떠나기 성공");
            statusText.text = "로비 떠나기 성공";
            statusUI.SetActive(false);
        }

        // 방에 접속 한 후에 실행
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log("방 참가 성공");
            statusText.text = "방 참가 성공";
            statusUI.SetActive(false);
            SceneManager.LoadScene("Room");
        }

        // 방에서 나가면서 실행
        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            Debug.Log("방 나가기 성공");
            statusText.text = "방 나가기 성공";
            statusUI.SetActive(false);
            //SceneManager.LoadScene("Lobby");
        }

        // 방을 만든 후에 실행
        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            Debug.Log("방 만들기 성공");
            statusText.text = "방 생성 성공";
            statusUI.SetActive(false);
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