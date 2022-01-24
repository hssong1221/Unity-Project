using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Fields

        // 게임 버젼인데 포톤 다큐먼트에서 있으면 좋다고 하더라고 뭐하는지 잘 모름
        string gameVersion = "1";

        [SerializeField]
        private Text statusText; // 서버 상태 텍스트

        #endregion


        #region MonoBehaviour CallBacks

        void Start()
        {
            // 방장이 혼자 씬을 로딩 하면, 나머지 사람들은 자동으로 싱크됨
            //PhotonNetwork.AutomaticallySyncScene = true;

            // 포톤 서버 설정에 따라 마스터 서버에 연결
            Debug.Log("Connect to MasterServer....");
            statusText.text = "서버에 접속 중..";
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }

        #endregion

        #region MonoBehaviourPunCallbacks Callbacks

        // 마스터 서버와 연결 시 작동
        public override void OnConnectedToMaster()
        {
            // 로비에 연결
            Debug.Log("Connected MasterServer");
            statusText.text = "서버에 연결 성공!";
        }

        // 서버 접속 실패시 자동 실행
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Connecting failed!! Trying re-connect..");
            statusText.text = "오프라인 : 서버와 연결 실패\n 접속 재시도 중...";
            // 설정한 정보로 마스터 서버 접속 시도
            PhotonNetwork.ConnectUsingSettings();
        }

        // 로비와 연결 시 작동
        public override void OnJoinedLobby()
        {
            // 로비에 들어오면 필요한 메뉴를 보여줌
            Debug.Log("Connected Lobby");
            
        }

        #endregion

        #region Public Methods

        public void JoinLobby()
        {
            PhotonNetwork.JoinLobby();
            SceneManager.LoadScene("Lobby");
        }

        #endregion
    }

}

