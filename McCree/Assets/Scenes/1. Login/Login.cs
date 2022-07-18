using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro inputfield

using Photon.Pun;
using Photon.Realtime;

using PlayFab;
using PlayFab.ClientModels;

namespace com.ThreeCS.McCree
{
    public class Login : MonoBehaviourPunCallbacks
    {
        #region Variable Fields

        // 게임 버젼인데 포톤 다큐먼트에서 있으면 좋다고 하더라고 뭐하는지 잘 모름
        string gameVersion = "1";

        [Header("로그인 패널")]
        public GameObject login_Canvas;
        public InputField ID_Input;
        public Button loginBtn;  // 로그인 버튼
        private string username;
        private string password;
        #endregion


        private void Awake()
        {
            // 게임 버젼
            PhotonNetwork.GameVersion = gameVersion;

            loginBtn.onClick.AddListener(Login_Btn);
        }

        private void Start()
        {
            
        }

        /*void Update()
        {   
            
        }*/


        #region Public Methods

        private void Login_Btn()
        {
            username = ID_Input.text.ToString();
            //password = Pwd_Input.text.ToString();

            /*var request = new LoginWithPlayFabRequest { 
                Username = username, 
                Password = password
            };*/
            //PlayFabClientAPI.LoginWithPlayFab(request, PlayFabModule.Instance.OnLoginSuccess, PlayFabModule.Instance.OnLoginFailure);

            LoadingUI.Instance.msg_Text.text = "로그인 시도 중...";
            LoadingUI.Instance.close_Btn.gameObject.SetActive(false);
            LoadingUI.Instance.msg_Canvas.SetActive(true);

            // 포톤 서버 연결
            PhotonNetwork.ConnectUsingSettings();
        }


        #endregion
    }

}

