using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;


using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Audio;

namespace com.ThreeCS.McCree 
{
    public class SettingMaster : MonoBehaviour
    {
        #region Variable Fields


        [Header("환경설정 관련 UI")]
        public GameObject SettingPanel;     // 전체 설정 패널
        private bool isSettingOpen = false;

        public Button setBtn;   // 톱니바퀴 모양 버튼


        public GameObject gPanel;   //그래픽 패널
        public GameObject sPanel;   //사운드
        public GameObject mPanel;   //마우스
        public GameObject kPanel;   //키보드

        [SerializeField] private Button gButton;
        [SerializeField] private Button sButton;
        [SerializeField] private Button mButton;
        [SerializeField] private Button kButton;

        [Header("그래픽설정 관련 UI")]
        [SerializeField] private Transform graphicObject;

        private int graphicOpt;         // 그래픽 프리셋
        private Text graphicTxt;
        private Button graphicDownBtn;
        private Button graphicUpBtn;

        [Header("사운드설정 관련 UI")]
        public AudioMixer audioMixer;

        [SerializeField] private Transform soundObject;

        private int soundOpt;       // 전체 볼륨
        private Text soundTxt;
        private Button soundDownBtn;
        private Button soundUpBtn;
        private Slider soundSld;

        [Header("마우스설정 관련 UI")]

        [SerializeField] private Transform mouseObject;

        private int mouseOpt;       // 마우스 감도
        private Text mouseTxt;
        private Button mouseDownBtn;
        private Button mouseUpBtn;
        private Slider mouseSld;


        // 저장버튼
        [Header("저장버튼")]
        [SerializeField] private Button saveButton;


        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
            //환경설정 UI init
            InitContent(graphicObject, out graphicTxt, out graphicDownBtn, out graphicUpBtn, GraphicOptDown, GraphicOptUp);

            InitContent(soundObject, out soundTxt, out soundDownBtn, out soundUpBtn, out soundSld, SoundOptDown, SoundOptUp, SliderValueChangeSound);

            //InitContent(mouseObject, out mouseTxt, out mouseDownBtn, out mouseUpBtn, out mouseSld, );

            // 설정 메뉴 버튼 
            gButton.onClick.AddListener(Gpanel);
            sButton.onClick.AddListener(Spanel);
            mButton.onClick.AddListener(Mpanel);
            kButton.onClick.AddListener(Kpanel);

            setBtn.onClick.AddListener(Setting);

            // 저장버튼 버튼 
            saveButton.onClick.AddListener(SaveSetting);

            Setting();

        }

        void Start()
        {
            Setting();
            DontDestroyOnLoad(this.gameObject);
        }


        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
                Setting();
        }

        #endregion

        #region 환경설정 UI 관련 method

        // UI 오브젝트 공통 초기화
        // 그래픽설정
        void InitContent(Transform obj, out Text text, out Button downBtn, out Button upBtn, UnityAction btnDown, UnityAction btnUp)
        {
            text = obj.Find("Text").GetComponent<Text>();
            downBtn = obj.Find("DownBtn").GetComponent<Button>();
            upBtn = obj.Find("UpBtn").GetComponent<Button>();

            downBtn.onClick.AddListener(btnDown);
            upBtn.onClick.AddListener(btnUp);
        }
        // 이거에 오바로오딩인거시다
        // 사운드 설정 + 마우스 설정
        void InitContent(Transform obj, out Text text, out Button downBtn, out Button upBtn, out Slider slider, UnityAction btnDown, UnityAction btnUp, UnityAction<float> sliderChange)
        {
            text = obj.Find("Text").GetComponent<Text>();
            downBtn = obj.Find("DownBtn").GetComponent<Button>();
            upBtn = obj.Find("UpBtn").GetComponent<Button>();
            slider = obj.Find("Slider").GetComponent<Slider>();

            downBtn.onClick.AddListener(btnDown);
            upBtn.onClick.AddListener(btnUp);
            slider.onValueChanged.AddListener(sliderChange);
        }


        // 환경 설정 UI on/off
        void Setting()
        {
            // 본인 컴퓨터의 저장된 설정 값 안불러와질 경우를 대비해서 한번 더 부름
            graphicOpt = PlayerPrefs.GetInt("graphicOpt", 0);
            soundOpt = PlayerPrefs.GetInt("soundOpt", 0);

            UpdateGraphicOpt();
            UpdateSoundOpt();

            if (isSettingOpen)
            {
                SettingPanel.SetActive(false);
                isSettingOpen = false;

            }
            else
            {
                SettingPanel.SetActive(true);
                isSettingOpen = true;
            }
        }


        // 설정 종류 버튼 하드코딩인데 이거보다 좋은방법이 있나
        public void Gpanel()
        {
            gPanel.SetActive(true);
            sPanel.SetActive(false);
            mPanel.SetActive(false);
            kPanel.SetActive(false);
        }

        public void Spanel()
        {
            gPanel.SetActive(false);
            sPanel.SetActive(true);
            mPanel.SetActive(false);
            kPanel.SetActive(false);
        }
        public void Mpanel()
        {
            gPanel.SetActive(false);
            sPanel.SetActive(false);
            mPanel.SetActive(true);
            kPanel.SetActive(false);
        }
        public void Kpanel()
        {
            gPanel.SetActive(false);
            sPanel.SetActive(false);
            mPanel.SetActive(false);
            kPanel.SetActive(true);
        }

        // -------------------------------------- 버튼 동작 -------------------------------------- 
        void GraphicOptDown()
        {
            --graphicOpt;
            UpdateGraphicOpt();
        }
        void GraphicOptUp()
        {
            ++graphicOpt;
            UpdateGraphicOpt();
        }

        void SoundOptDown()
        {
            if (--soundOpt < -80) soundOpt = -80;
            Data.SoundOpt = soundOpt;

            UpdateSoundOpt();

        }
        void SoundOptUp()
        {
            if (20 < ++soundOpt) soundOpt = 20;
            Data.SoundOpt = soundOpt;

            UpdateSoundOpt();
        }


        // -------------------------------------- 슬라이더 동작 ----------------------------------------------

        void SliderValueChangeSound(float volume)
        {
            Data.SoundOpt = soundOpt = Mathf.RoundToInt(volume);
            UpdateSoundOpt();
        }

        // -------------------------------------- 내부 텍스트 값( 또는 슬라이더 값도 포함) --------------------------------------
        void UpdateGraphicOpt()
        {
            graphicDownBtn.interactable = graphicOpt != 0;
            graphicUpBtn.interactable = graphicOpt != 5;

            switch (graphicOpt)
            {
                case 0:
                    graphicTxt.text = "Very Low";
                    break;
                case 1:
                    graphicTxt.text = "Low";
                    break;
                case 2:
                    graphicTxt.text = "Medium";
                    break;
                case 3:
                    graphicTxt.text = "High";
                    break;
                case 4:
                    graphicTxt.text = "Very High";
                    break;
                case 5:
                    graphicTxt.text = "Ultra";
                    break;
                default:
                    graphicTxt.text = "Error";
                    break;
            }

        }

        void UpdateSoundOpt()
        {
            // 저장 값 변경
            soundSld.value = soundOpt;
            soundTxt.text = (soundOpt + 80).ToString();

            // 실제 소리 적용

            audioMixer.SetFloat("Master", soundOpt);

        }

        // 환경설정 저장
        void SaveSetting()
        {
            QualitySettings.SetQualityLevel(graphicOpt);

            Data.GraphicOpt = graphicOpt;
            Data.SoundOpt = soundOpt;
        }

    #endregion

    }

    // Playerprefs에 저장될 값 처리 클래스
    public static class Data
    {
        #region Variable

        private static int graphicOpt;
        private static int soundOpt;

        #endregion

        #region get/set

        // 그래픽 프리셋 값
        public static int GraphicOpt
        {
            get
            {
                //Debug.Log("Playerfreps에서 저장된 값을 줌 : " + graphicOpt);
                return graphicOpt;
            }
            set
            {
                graphicOpt = value;
                //Debug.Log("게임에서 준 값을 저장중 : " + graphicOpt);

                PlayerPrefs.SetInt(GetMemberName(() => graphicOpt), value);

            }
        }

        public static int SoundOpt
        {
            get
            {
                return soundOpt;
            }
            set
            {
                soundOpt = value;
                PlayerPrefs.SetInt(GetMemberName(() => soundOpt), value);
            }
        }

        private static string GetMemberName<T>(Expression<Func<T>> memberExpression)    //변수명을 string으로 리턴해주는 함수. 변수명을 그대로 key로 쓰기 위함. 
        {
            MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
            return expressionBody.Member.Name;
        }

        #endregion
    }

}


