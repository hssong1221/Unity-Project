using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace com.ThreeCS.McCree
{
    public class LoadingUI : MonoBehaviour
    {
        private static LoadingUI pInstance;

        public static LoadingUI Instance
        {
            get { return pInstance; }
        }


        [Header("메세지 팝업 패널")]
        public GameObject msg_Canvas;
        public Text msg_Text;
        public Button close_Btn;


        private Transform[] canvasChildrens;

        void Awake()
        {
            pInstance = this;

            canvasChildrens = transform.GetComponentsInChildren<Transform>();
            foreach (Transform child in canvasChildrens)
            {
                if (child.name == "MsgCanvas")
                {
                    msg_Canvas = child.gameObject;
                    msg_Canvas.SetActive(false);
                }
                else if (child.name == "Message")
                {
                    msg_Text = child.GetComponent<Text>();
                }
                else if (child.name == "CloseBtn")
                {
                    close_Btn = child.GetComponent<Button>();
                }
            }
            close_Btn.onClick.AddListener(Close_Btn);
            DontDestroyOnLoad(this.gameObject);
        }

        void Update()
        {
            //Debug.Log(msg_Canvas.activeSelf + "  " + close_Btn.gameObject.activeSelf);
            //if (msg_Canvas.activeSelf && close_Btn.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Return))
            //{
            //    Debug.Log("?????");
            //    // 엔터키를 치면 로그인 (제출) 버튼을 클릭
            //    close_Btn.onClick.Invoke();
            //}
        }



        private void Close_Btn()
        {
            msg_Canvas.SetActive(false);
        }
    }
}