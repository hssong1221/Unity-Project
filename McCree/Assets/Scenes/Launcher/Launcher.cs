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
        #region Variable Fields

        // 게임 버젼인데 포톤 다큐먼트에서 있으면 좋다고 하더라고 뭐하는지 잘 모름
        string gameVersion = "1";

        [SerializeField]
        protected Button joinBtn;  // 입장 버튼
        [SerializeField]
        protected InputField NickName; // 닉네임
        #endregion

        private void Awake()
        {
            joinBtn.onClick.AddListener(Join_Lobby);
        }

        #region Public Methods

        private void Join_Lobby()
        {
            // 닉네임이 같으면 중복된다는 코드 구현예정
            PhotonNetwork.NickName = NickName.text;
            PhotonNetwork.ConnectUsingSettings();
            PunCallbacks.statusText.text = "로비에 참가 중...";
            PunCallbacks.statusUI.SetActive(true);
            Debug.Log(NickName.text);
        }

        #endregion
    }

}

