using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

using Photon.Chat;

namespace com.ThreeCS.McCree
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Variable Fields

        // 게임 버젼인데 포톤 다큐먼트에서 있으면 좋다고 하더라고 뭐하는지 잘 모름
        string gameVersion = "1";

        [SerializeField]
        protected Button joinBtn;  // 입장 버튼
        [SerializeField]
        protected InputField NickName; // 닉네임

        private PunChat punChat;
        private ChatClient chatClient;
        #endregion

        private void Awake()
        {
            // 게임 버젼
            PhotonNetwork.GameVersion = gameVersion;
            joinBtn.onClick.AddListener(Join_Lobby);

            punChat = GameObject.Find("LoadingUI").GetComponent<PunChat>();
        }

        #region Public Methods

        // join버튼 누르면 실행
        private void Join_Lobby()
        {
            // 닉네임이 같으면 중복된다는 코드 구현예정
            PhotonNetwork.NickName = NickName.text;
            //PhotonNetwork.ConnectUsingSettings();
            PunCallbacks.statusText.text = "서버에 접속 중...";
            PunCallbacks.statusUI.SetActive(true);
            punChat.Connect();
            Debug.Log(NickName.text);
        }

        #endregion
    }

}

