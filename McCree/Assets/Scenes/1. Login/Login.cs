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

        EventSystem system; // tab키 enter키 사용할수있는 그런것

        [SerializeField]
        protected Button joinBtn;  // 입장 버튼

        
        private PlayFabModule playFabModule;



        [Header("로그인 패널")]
        public GameObject login_Canvas;
        public TMP_InputField ID_Input;
        public TMP_InputField Pwd_Input;
        public Button loginBtn;  // 로그인 버튼
        public Button registerBtn;  // 등록 버튼
        private string username;
        private string password;




        #endregion


        private void Awake()
        {
            // 게임 버젼
            PhotonNetwork.GameVersion = gameVersion;

            playFabModule = GameObject.Find("LoadingUI").GetComponent<PlayFabModule>();

            loginBtn.onClick.AddListener(Login_Btn);
            registerBtn.onClick.AddListener(Register_Btn);
        }

        private void Start()
        {
            // Awake에 넣으면 작동 안하드라
            system = EventSystem.current;
            ID_Input.Select();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
            {
                // Tab + LeftShift는 위의 Selectable 객체를 선택
                Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
                if (next != null)
                {
                    next.Select();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                // Tab은 아래의 Selectable 객체를 선택
                Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
                if (next != null)
                {
                    next.Select();
                }
            }
            //else if (Input.GetKeyDown(KeyCode.Return))
            //{
            //    // 엔터키를 치면 로그인 (제출) 버튼을 클릭
            //    loginBtn.onClick.Invoke();
            //}
        }


        #region Public Methods

        private void Login_Btn()
        {
            username = ID_Input.text.ToString();
            password = Pwd_Input.text.ToString();

            var request = new LoginWithPlayFabRequest { 
                Username = username, 
                Password = password
            };
            PlayFabClientAPI.LoginWithPlayFab(request, playFabModule.OnLoginSuccess, playFabModule.OnLoginFailure);

            LoadingUI.Instance.msg_Text.text = "로그인 시도 중...";
            LoadingUI.Instance.close_Btn.gameObject.SetActive(false);
            LoadingUI.Instance.msg_Canvas.SetActive(true);
        }

        private void Register_Btn()
        {
            LoadingUI.Instance.msg_Text.text = "등록화면으로 이동 중...";
            LoadingUI.Instance.msg_Canvas.SetActive(true);
            LoadingUI.Instance.close_Btn.gameObject.SetActive(false);

            SceneManager.LoadScene("Register");
        }

        

        

       

        #endregion
    }

}

