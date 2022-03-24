using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.Audio;

using Photon.Pun;
using Photon.Realtime;

using Newtonsoft.Json;

namespace com.ThreeCS.McCree
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Variable Fields

        // 게임 매니저 어디서든 사용가능
        private static GameManager pInstance;

        public static GameManager Instance
        {
            get { return pInstance; }
        }

        // 내 카메라
        private CameraWork cameraWork;

        // 플레이어 리스트
        public GameObject[] playerList;

        // 자기 자신(자기 포톤 뷰) 찾아서 Player관련 저장
        private PhotonView photonView;
        private PlayerManager playerManager;
        private PlayerInfo playerInfo;

        // 게임 배경 음악
        private GameObject BGMLobby;
        private AudioSource bgm;

        

        public enum jType
        {
            Error,
            Sheriff,
            Vice,
            Outlaw,
            Renegade
        } //직업 타입

        public enum aType
        {
            HumanVolcanic,
            BangMissed,
            ThreeCard,
            OnehpOnecard,
            TwocardOnehp,
            TwocardOnecard,
            DrinkBottle
        } //능력 타입


        [Header("직업 관련 UI")]
        public GameObject jobPanel;
        public RectTransform jobBoard;
        public Text jobText;
        public Image jobImage1;         // 직업마다 이미지가 다르게 할 것
        public Image jobImage2;
        public Image jobImage3;
        private Animator jobUIAnimator;
        private Animator abilUIAnimator;


        [Header("고유 능력 관련 UI")]
        public GameObject abilPanel;
        public Image abilImage;
        public Text abilText;


        [Header("직업 일러스트")]
        public Sprite sheriff1;  // 보안관 일러스트
        public Sprite sheriff2;
        public Sprite sheriff3;

        public Sprite deputy1;     // 부관   일러스트
        public Sprite deputy2;
        public Sprite deputy3;

        public Sprite outlaw1;   // 무법자 일러스트
        public Sprite outlaw2;
        public Sprite outlaw3;

        public Sprite renegade1;  // 배신자 일러스트
        public Sprite renegade2;
        public Sprite renegade3;

        [Header("어빌 일러스트")]
        public Sprite sheriff4;  // 능력 나올 때의 직업 일러
        public Sprite deputy4;
        public Sprite outlaw4;
        public Sprite renegade4;

        [Header("맵 기준점")]
        public Transform[] points;
        public List<GameObject> maps;

        // 한 게임에서 사용할 전체 아이템 세트
        public ItemSet entireItemSet; 

        [Header("환경설정 관련 UI")]
        public GameObject SettingPanel;     // 전체 설정 패널
        private bool isSettingOpen = false;

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

        private int graphicOpt; // 그래픽 프리셋
        private Text graphicTxt;

        private Button graphicDownBtn;
        private Button graphicUpBtn;

        [Header("사운드설정 관련 UI")]
        public AudioMixer audioMixer;

        [SerializeField] private Transform soundObject;

        private int soundOpt;
        private Text soundTxt;

        private Button soundDownBtn;
        private Button soundUpBtn;
        private Slider soundSld;

        // 저장버튼
        [SerializeField] private Button saveButton;

        #endregion


        #region MonoBehaviour CallBacks

        void Awake()
        {
            // 어디서든 쓸 수 있게 인스턴스화
            pInstance = this;

            jobUIAnimator = jobPanel.GetComponent<Animator>();
            abilUIAnimator = abilPanel.GetComponent<Animator>();

            // 로비브금 끄기 
            BGMLobby = GameObject.Find("BGMLobby");
            Destroy(BGMLobby);
            // 게임브금 켜기
            bgm = gameObject.GetComponent<AudioSource>();
            bgm.Play();

            // 접속 못하면 초기화면으로 쫓아냄
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene("Launcher");
                return;
            }

            //환경설정 UI init
            InitContent(graphicObject, out graphicTxt ,out graphicDownBtn, out graphicUpBtn, GraphicOptDown, GraphicOptUp);

            InitContent(soundObject, out soundTxt, out soundDownBtn, out soundUpBtn, out soundSld, SoundOptDown, SoundOptUp, SliderValueChangeSound);

            // 설정 메뉴 버튼 
            gButton.onClick.AddListener(Gpanel);
            sButton.onClick.AddListener(Spanel);
            mButton.onClick.AddListener(Mpanel);
            kButton.onClick.AddListener(Kpanel);

            // 저장버튼 버튼 
            saveButton.onClick.AddListener(SaveSetting);
        }

        public override void OnEnable()
        {
            base.OnEnable();

            // 본인 환경설정 세팅값 가져오기
            graphicOpt = Data.GraphicOpt;
            soundOpt = Data.SoundOpt;
        }

        void Start()
        {
            PhotonNetwork.IsMessageQueueRunning = true;

            if (PlayerManager.LocalPlayerInstance == null)
            {
                StartCoroutine(InstantiateResource()); 

                cameraWork = GetComponent<CameraWork>(); // 본인 카메라 가져오기
            }
            StartCoroutine(WaitAllPlayers()); // 다른 플레이어 기다리기

        }

        void Update()
        {
            // 환경설정 UI on/off
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Setting();
            }
        }
        
        #endregion

        #region Coroutine

        // 캐릭터 생성 후 맵 스폰 순서대로
        IEnumerator InstantiateResource()
        {
            yield return new WaitForEndOfFrame();

            StartCoroutine(SpawnPlayer());

            StartCoroutine(SpawnMap());
        }

        // 맵 생성
        IEnumerator SpawnMap()
        {
            // 방장만 맵 스폰
            if(PhotonNetwork.IsMasterClient)
            {
                // 맵 랜덤화
                maps = CommonFunction.ShuffleList(maps);

                // 맵은 8칸 + 가운데 마을1칸(이건 중앙 고정)
                for (int i = 0; i < 8; i++)
                {
                    // 맵 모듈을 스폰해서 동기화
                    PhotonNetwork.Instantiate("World/" + maps[i].name, points[i].position, Quaternion.identity, 0);
                }
                yield return new WaitForEndOfFrame();
            }
            
        }

        IEnumerator SpawnPlayer()
        {
            RaiseEventManager.Instance.Spwan_Player();
            yield return new WaitForEndOfFrame();
        }

        IEnumerator FindMinePv()
        {
            foreach (GameObject player in playerList)
            {
                if (player.GetComponent<PhotonView>().IsMine)
                {
                    photonView = player.GetComponent<PhotonView>();
                    playerManager = player.GetComponent<PlayerManager>();
                    playerInfo = player.GetComponent<PlayerInfo>();

                    MineUI.Instance.FindMinePv(player);
                    break;
                }
            }

            yield return new WaitForEndOfFrame();
        }

        IEnumerator WaitAllPlayers()
        {
            while (PhotonNetwork.PlayerList.Length != playerList.Length)
            {
                Debug.Log("PM 방 총원 : " + PhotonNetwork.PlayerList.Length);
                Debug.Log("PM 현재 로딩된 인원 수 : " + playerList.Length);
                yield return null;
            }
            // 플레이어들이 모두 들어오면 밑에 실행 가능

            StartCoroutine(FindMinePv());  // 자기 자신의 PhotonView, 관련 스크립트 찾기

            if (PhotonNetwork.IsMasterClient && photonView.IsMine)
            {

                // 직업이랑 능력을 나누어 준다.
                StartCoroutine(JobandAbility());

                StartCoroutine(AnimPlay());

                // 카드 나눠주는것
                //StartCoroutine(Cards());
                
                
                //StartCoroutine(GiveCardSet());
                
            }
        }

        IEnumerator JobandAbility()
        {
            yield return new WaitForEndOfFrame();

            // (인구수에 맞게 하는 거 추가하기)
            List<int> jobList = new List<int>() { 1, 2, 3, 4, 5, 6, 7 };

            // 능력 갯수에 맞게 해야함
            List<int> abilityList = new List<int>() { 1, 2, 3, 4, 5, 6, 7 };

            jobList = CommonFunction.ShuffleList(jobList);
            //Debug.Log("잡리스트" + jobList[0] + " " + jobList[1] + " " + jobList[2]);
            abilityList = CommonFunction.ShuffleList(abilityList);
            //Debug.Log("어빌리스트" + abilityList[0] + " " + abilityList[1] + " " + abilityList[2]);


            // 직업을 나눠주고 동기화 시킴
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Debug.Log("잡 리스트 번호 : " + jobList[i]);
                playerList[i].GetComponent<PhotonView>().RPC("JobSelect", RpcTarget.All, jobList[i]);
            }

            // 능력을 나눠주고 동기화 시킴
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Debug.Log("어빌 리스트 번호 : " + abilityList[i]);
                playerList[i].GetComponent<PhotonView>().RPC("AbilitySelect", RpcTarget.All, abilityList[i]);
            }

            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Debug.Log("체력 동기화 : " + playerList[i]);
                playerList[i].GetComponent<PhotonView>().RPC("SyncHp", RpcTarget.All);
            }
            yield return new WaitForEndOfFrame();
        }

        // 애니메이션 스타트
        IEnumerator AnimPlay()
        {
            yield return new WaitForEndOfFrame();

            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                playerList[i].GetComponent<PhotonView>().RPC("AnimStart", RpcTarget.All);
            }

            yield return new WaitForEndOfFrame();
        }

        public IEnumerator GameStart()
        {
            // 직업 선택 텍스트랑 애니메이션 재생

            yield return new WaitForEndOfFrame();
            jobPanel.SetActive(true);
            jobText.text = JobText();

            yield return new WaitForSeconds(6f);
            jobPanel.SetActive(false);
            abilPanel.SetActive(true);

            yield return new WaitForSeconds(0.5f);
            abilUIAnimator.SetTrigger("Abil");
            abilText.text += AblityText();
            abilText.text += "\n3. 당신의 능력을 잘 활용하십시오";


            // Status 창 (인벤토리창 동기화)
            StatusUI.Instance.job_Name.text = jobText.text;
            StatusUI.Instance.job_Explain.text = abilText.text;


            // ---------------------- 사람들이 텍스트를 읽을 시간 부여(나중에 다시 활성화) ----------------------------
            //yield return new WaitForSeconds(12f);
            abilPanel.SetActive(false);
            MineUI.Instance.leftTopPanel.SetActive(true);
            MineUI.Instance.rightBottomPanel.SetActive(true);
            MineUI.Instance.rightTop.SetActive(true);
        }


        #endregion

        #region Public Methods

        // 직업 관련 텍스트
        public string JobText()
        {
            string temp = "";
            Debug.Log(playerManager.playerType);
            switch (playerManager.playerType)
            {
                // 플레이어 타입마다 다른 그림 부여 (애니메이션도 다 다르게 하려 햇는데 일단 하나로 함)
                case jType.Sheriff:
                    Debug.Log("당신은 보안관입니다.");
                    jobUIAnimator.SetTrigger("Sheriff");
                    temp = "SHERIFF\n보 안 관.";

                    // 직업 이미지 3장 
                    jobImage1.sprite = sheriff1;
                    jobImage2.sprite = sheriff2;
                    jobImage3.sprite = sheriff3;

                    // 능력 이미지랑 텍스트
                    abilImage.sprite = sheriff4;
                    abilText.text = "1. 부관을 찾고 무법자를 모두 사살하십시오.";
                    break;
                case jType.Vice:
                    Debug.Log("당신은 부관입니다.");
                    jobUIAnimator.SetTrigger("Deputy");
                    temp = "DEPUTY\n 부  관";

                    jobImage1.sprite = deputy1;
                    jobImage2.sprite = deputy2;
                    jobImage3.sprite = deputy3;

                    abilImage.sprite = deputy4;
                    abilText.text = "1. 보안관을 도와 무법자를 모두 사살하십시오.";
                    break;
                case jType.Outlaw:
                    Debug.Log("당신은 무법자입니다.");
                    jobUIAnimator.SetTrigger("Outlaw");
                    temp = "OUTLAW\n무 법 자";

                    jobImage1.sprite = outlaw1;
                    jobImage2.sprite = outlaw2;
                    jobImage3.sprite = outlaw3;

                    abilImage.sprite = outlaw4;
                    abilText.text = "1. 무법자들과 함께 보안관을 사살하십시오.";
                    break;
                case jType.Renegade:
                    Debug.Log("당신은 배신자입니다.");
                    jobUIAnimator.SetTrigger("Renegade");
                    temp = "RENEGADE\n배 신 자";

                    jobImage1.sprite = renegade1;
                    jobImage2.sprite = renegade2;
                    jobImage3.sprite = renegade3;

                    abilImage.sprite = renegade4;
                    abilText.text = "1. 보안관에겐 부관처럼 무법자에겐 친구처럼 보이십시오.";
                    break;
            }
            MineUI.Instance.questTitle.text = abilText.text.Substring(2);

            return temp;
        }

        // 능력 관련 텍스트
        public string AblityText()
        {
            // 능력 UI 애니메이션 
            string temp = "";
            Debug.Log(playerManager.abilityType);
            switch (playerManager.abilityType)
            {
                case aType.BangMissed:
                    temp = "\n2. 뱅과 빗나감이 같은 능력이 됩니다.";
                    break;
                case aType.DrinkBottle:
                    temp = "\n2. 당신옆에 항상 술통이 있습니다.";
                    break;
                case aType.HumanVolcanic:
                    temp = "\n2. 뱅을 마구 쏠 수 있습니다.";
                    break;
                case aType.OnehpOnecard:
                    temp = "\n2. 체력이 달았다면 카드를 얻습니다.";
                    break;
                case aType.ThreeCard:
                    temp = "\n2. 카드를 뽑을 때 3장을 보고 2장을 가져옵니다.";
                    break;
                case aType.TwocardOnecard:
                    temp = "\n2. 카드 펼치기를 할 때 2장을 뽑고 한장을 선택할 수 있습니다.";
                    break;
                case aType.TwocardOnehp:
                    temp = "\n2. 카드 2장을 버리고 체력을 얻습니다.";
                    break;
            }
            return temp;
        }

        // 방 나가기 임시 구현
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
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
        // 사운드 설정
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

        // 버튼 동작
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
        // 슬라이더 동작

        void SliderValueChangeSound(float volume)
        {
            Data.SoundOpt = soundOpt = Mathf.RoundToInt(volume);
            UpdateSoundOpt();
        }

        // 내부 텍스트 값( 또는 슬라이더 값도 포함) 
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

        #region Photon Callback

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.Log("플레이어가 입장했습니다 : " + other.NickName);
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("당신이 마스터 클라이언트(방장)입니다.");

                //LoadArena();
            }
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.Log("플레이어가 나갔습니다 : " + other.NickName);
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("마스터클라이언트가 바뀌었습니다. :  {0}", PhotonNetwork.IsMasterClient);

                //LoadArena();
            }
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