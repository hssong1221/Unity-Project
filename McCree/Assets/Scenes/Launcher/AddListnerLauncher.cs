using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class AddListnerLauncher : MonoBehaviour
    {
        #region Variable Fields

        [SerializeField]
        protected Text statusText; // 서버 상태 텍스트
        [SerializeField]
        protected Button joinBtn;  // 입장 버튼
        [SerializeField]
        protected InputField NickName; // 닉네임

        #endregion

        private void Awake()
        {
            joinBtn.onClick.AddListener(Join_Lobby);
        }

        #region OnClick Function
        private void Join_Lobby()
        {
            // 닉네임이 같으면 중복된다는 코드 구현예정
            PhotonNetwork.NickName = NickName.text;
            statusText.text = "서버에 접속 중...";
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log(NickName.text);
        }
        #endregion
    }
}
