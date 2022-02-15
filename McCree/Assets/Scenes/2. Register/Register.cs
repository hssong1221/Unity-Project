using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro; // TextMeshPro inputfield


using PlayFab;
using PlayFab.ClientModels;

namespace com.ThreeCS.McCree
{
    public class Register : MonoBehaviour
    {
        [Header("등록 패널")]
        public TMP_InputField ID_Input;
        public TMP_InputField NickName_Input;
        public TMP_InputField Email_Input;
        public TMP_InputField Pwd_Input;
        public TMP_InputField PwdConfirm_Input;
        public Button backBtn;  // 로그인 버튼
        public Button registerCompleteBtn;  // 등록 버튼

        private string username;
        private string password;
        private string email;
        private string nickName;
        private string confirm_password;

        private PlayFabModule playFabModule;
        EventSystem system; // tab키 enter키 사용할수있는 그런것

        void Awake()
        {
            backBtn.onClick.AddListener(Back_Btn);
            registerCompleteBtn.onClick.AddListener(RegisterComplete_Btn);
            playFabModule = GameObject.Find("LoadingUI").GetComponent<PlayFabModule>();
        }

        private void Start()
        {
            system = EventSystem.current;
            ID_Input.Select();
        }

        // Update is called once per frame
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
            //    registerCompleteBtn.onClick.Invoke();
            //}
        }

        private void Back_Btn()
        {
            LoadingUI.msg_Text.text = "로그인 화면으로 이동 중...";
            LoadingUI.msg_Canvas.SetActive(true);
            LoadingUI.close_Btn.gameObject.SetActive(false);
            SceneManager.LoadScene("Login");
        }

        private void RegisterComplete_Btn()
        {

            bool idChecker = Regex.IsMatch(ID_Input.text, @"[^a-zA-Z0-9]");
            bool nickChecker = Regex.IsMatch(NickName_Input.text, @"[^a-zA-Z0-9가-힣]");
            // [^a-zA-Z0-9가-힣] 은 앞에 ^ 표시가 붙어 부정을 의미한다.
            // 그러므로 a-z A - Z 0 - 9 가 - 힣 사이에 있는문자가 아닌게 있는지 확인하는 것

            username = ID_Input.text.ToString();
            nickName = NickName_Input.text.ToString();
            password = Pwd_Input.text.ToString();
            confirm_password = PwdConfirm_Input.text.ToString();
            email = Email_Input.text.ToString();

            if (username.Trim() == ""
                || nickName.Trim() == ""
                || password.Trim() == ""
                || confirm_password.Trim() == ""
                || email.Trim() == "")
            {
                LoadingUI.msg_Text.text = "빈칸없이 입력해 주세요.";
                LoadingUI.msg_Canvas.SetActive(true);
                return;
            }


            if (idChecker)
            {
                LoadingUI.msg_Text.text = "아이디에 한글, 특수문자, 공백은 허용되지 않습니다.";
                LoadingUI.msg_Canvas.SetActive(true);
                return;
            }

            if (username.Length < 3 || 20 < username.Length)
            {
                LoadingUI.msg_Text.text = "아이디는 3 ~ 20사이의 문자로 입력해주세요.";
                LoadingUI.msg_Canvas.SetActive(true);
                return;
            }

            if (nickChecker)
            {
                LoadingUI.msg_Text.text = "닉네임에 특수문자, 공백은 허용되지 않습니다.";
                LoadingUI.msg_Canvas.SetActive(true);
                return;
            }

            if (nickName.Length < 3 || 20 < nickName.Length)
            {
                LoadingUI.msg_Text.text = "닉네임은 3 ~ 20사이의 문자로 입력해주세요.";
                LoadingUI.msg_Canvas.SetActive(true);
                return;
            }

            Debug.Log(password);
            Debug.Log(confirm_password);

            if (password == confirm_password)
            {
                if (password.Length < 6 || 20 < password.Length)
                {
                    LoadingUI.msg_Text.text = "비밀번호는 6 ~ 20사이의 문자로 입력해주세요.";
                    LoadingUI.msg_Canvas.SetActive(true);
                    return;
                }

                var request = new RegisterPlayFabUserRequest
                {
                    Username = username,
                    DisplayName = nickName,
                    Password = password,
                    Email = email
                };
                PlayFabClientAPI.RegisterPlayFabUser(request, playFabModule.RegisterSuccess, playFabModule.RegisterFailure);

                LoadingUI.msg_Text.text = "회원 등록 중...";
                LoadingUI.close_Btn.gameObject.SetActive(false);
                LoadingUI.msg_Canvas.SetActive(true);

                return;
            }
            else
            {
                LoadingUI.msg_Text.text = "비밀번호를 확인해 주세요";
                LoadingUI.msg_Canvas.SetActive(true);
                return;
            }
        }
    }
}