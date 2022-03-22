using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

using Photon.Chat;

namespace com.ThreeCS.McCree
{
    public class PlayFabModule : MonoBehaviourPunCallbacks
    {

        private static PlayFabModule pInstance;

        public static PlayFabModule Instance
        {
            get { return pInstance; }
        }

        void Awake()
        {
            pInstance = this;
        }


        private string player_ID;
        private string player_NickName;
        private string player_FabID;

        //private ChatClient chatClient;

        // Start is called before the first frame update


        public string Return_player_ID()
        {
            return player_ID;
        }
        public string Return_player_NickName()
        {
            return player_NickName;
        }
        public string Return_player_FabID()
        {
            return player_FabID;
        }

        public void GetAccountSuccess(GetAccountInfoResult result)
        {
            Debug.Log("성공적으로 데이터를 받아옴"); 
            player_ID = result.AccountInfo.Username;
            player_NickName = result.AccountInfo.TitleInfo.DisplayName;
            player_FabID = result.AccountInfo.PlayFabId;

            PhotonNetwork.NickName = player_NickName;
            LoadingUI.Instance.msg_Text.text = "유저 정보 불러오기 성공!";

            PunChat.Instance.Connect();
        }

        public void GetAccountFailure(PlayFabError error)
        {
            Debug.Log("데이터를 받아오는데 문제가 생김");
        }


        public void OnLoginSuccess(LoginResult result)
        {
            LoadingUI.Instance.msg_Text.text = "로그인 성공!";
            Debug.Log("로그인 성공");
            Debug.Log(result.PlayFabId);

            var request = new GetAccountInfoRequest {
                PlayFabId = result.PlayFabId
            };

            LoadingUI.Instance.msg_Text.text = "해당 유저 정보 불러오는 중...";
            PlayFabClientAPI.GetAccountInfo(request, GetAccountSuccess, GetAccountFailure);
        }

        public void OnLoginFailure(PlayFabError error)
        {
            Debug.LogWarning("로그인 실패");
            Debug.LogWarning(error.GenerateErrorReport());
            Debug.Log(error.Error);

            if (error.Error == PlayFabErrorCode.AccountNotFound)
            {
                LoadingUI.Instance.msg_Text.text = "해당 정보와 일치하는 계정이 없습니다.";
            }

            else if (error.Error == PlayFabErrorCode.AccountNotFound)
            {
                LoadingUI.Instance.msg_Text.text = "아이디 혹은 비밀번호가 일치하지 않습니다.";
            }

            else 
            {
                LoadingUI.Instance.msg_Text.text = "해당 정보와 일치하는 계정이 없습니다.";
            }
            LoadingUI.Instance.msg_Canvas.SetActive(true);
            LoadingUI.Instance.close_Btn.gameObject.SetActive(true);
        }


        public void RegisterSuccess(RegisterPlayFabUserResult result)
        {
            Debug.Log("가입 성공");
            LoadingUI.Instance.msg_Text.text = "가입이 완료되었습니다."; 
            LoadingUI.Instance.close_Btn.gameObject.SetActive(true);
            LoadingUI.Instance.msg_Canvas.SetActive(true);
            LoadingUI.Instance.close_Btn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Login");
            }); // 아니 이게 addlistner 되었는데 누른다음 기능이 없어짐 
                // 되서 좋긴한데 왜 없어지는지 모르겠음
        }

        public void RegisterFailure(PlayFabError error)
        {
            Debug.LogWarning("가입 실패");
            Debug.LogWarning(error.GenerateErrorReport());

            Debug.Log(error.Error);

            if (error.Error == PlayFabErrorCode.UsernameNotAvailable)
            {
                LoadingUI.Instance.msg_Text.text = "중복된 아이디 입니다.";
            }
            else if (error.Error == PlayFabErrorCode.NameNotAvailable)
            {
                LoadingUI.Instance.msg_Text.text = "중복된 닉네임 입니다.";
            }
            else if (error.Error == PlayFabErrorCode.EmailAddressNotAvailable)
            {
                LoadingUI.Instance.msg_Text.text = "중복된 이메일 입니다.";
            }
            else if (error.Error == PlayFabErrorCode.InvalidParams)
            {   // 아이디 닉네임 패스워드를 직접 예외처리해서 이메일 형식 invalidparams밖에 안뜰거임 아마
                LoadingUI.Instance.msg_Text.text = "유효하지않은 이메일 형식 입니다.";
            }
            LoadingUI.Instance.close_Btn.gameObject.SetActive(true);
            LoadingUI.Instance.msg_Canvas.SetActive(true);
        }

        
    }
}
