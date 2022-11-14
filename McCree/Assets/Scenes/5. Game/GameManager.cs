using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.Audio;

using Photon.Pun;
using Photon.Realtime;

using Cinemachine;
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
        public CameraWork cameraWork;
        public CinemachineClearShot cam;
         
        // 플레이어 리스트
        public GameObject[] playerList;
        // 본인 
        [HideInInspector]
        public GameObject player1;

        [Header("의자에 앉은 플레이어 위치 표시")]
        public GameObject[] sitList = new GameObject[7];
        [Header(" 자리에 비교를 위한 임시 오브젝트 생성")]
        public GameObject tempsit;
        [Header("플레이어 턴 표시기")]
        public List<GameObject> turnList = new List<GameObject>();

        // 자기 자신(자기 포톤 뷰) 찾아서 Player관련 저장
        private PhotonView photonView;
        private PlayerManager playerManager;
        private PlayerInfo playerInfo;
        private UI ui;

        // 게임 배경 음악
        private GameObject BGMLobby;
        private AudioSource bgm;

        // 게임 종료 조건(각 직업의 숫자 - 보안관은 죽으면 바로 무법자 승리)
        private int outlawNum;
        private int renegadeNum;
        private int viceNum;

        [Header("게임 승리 변수")]
        public bool isVictory = false;


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

        [Header("게임 시작 관련 UI(의자 주변)")]
        public Canvas startCanvas;
        public GameObject startPanel;
        public Text pnumText; // 플레이어 앉은 숫자
        public int sitNum = 0; // 앉아 있는 숫자
        public Button bangBtn;


        [Header("게임 승리 패배 관련 UI")]
        public GameObject vicPanel;
        public GameObject vicText;  // 승리
        public GameObject defText;  // 패배
        public GameObject backPlane;    // 게임종료시 켜지는 배경 판때기

        [Header("게임 종료후 출력 될 위치와 플레이어 오브젝트")]
        public GameObject[] pnt;
        public GameObject[] player;


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


        // 한 게임에서 사용할 전체 아이템 세트
        public ItemSet entireItemSet;

        private Button setButton;


        //[HideInInspector] // <- 카드 리스트 직접 확인하려면 삭제
        public List<Card> cardList = new List<Card>();
        public List<Card> storecardList = new List<Card>();

        #region 카드 종류

        [Header("카드 개수")]
        [SerializeField]
        private int bang_c;
        [SerializeField]
        private int avoid_c;
        [SerializeField]
        private int beer_c;
        [SerializeField]
        private int machinegun_c;
        [SerializeField]
        private int indian_c;
        [SerializeField]
        private int stagecoach_c;
        [SerializeField]
        private int wellsfargo_c;
        [SerializeField]
        private int saloon_c;
        [SerializeField]
        private int generalstore_c;
        [SerializeField]
        private int russian_c;
        [SerializeField]
        private int navy_c;
        [SerializeField]
        private int carbine_c;
        [SerializeField]
        private int winchester_c;
        [SerializeField]
        private int scope_c;
        [SerializeField]
        private int mustang_c;
        [SerializeField]
        private int barrel_c;
        [SerializeField]
        private int dynamite_c;
        [SerializeField]
        private int jail_c;
        [SerializeField]
        private int carbalou_c;
        [SerializeField]
        private int panic_c;
        [SerializeField]
        private int duel_c;

        #endregion

        // 턴 관련 변수들
        [HideInInspector]
        public bool nextSignal = false; // 턴을 다음사람에게 넘기라는 변수
        [HideInInspector]   
        public int tidx;                // 현재 턴을 가지고 있는 사람의 turnList index

        public bool myTurn = false;    // 내 턴이면 true

        public bool duelTurn = false; // 결투시 켜짐 특수하게 턴이 돌아가게함
        public int duelIdx;          // 결투를 사용한 사람의 idx를 저장했다가 끝날 때 턴다시 줘야함

        // ----------------------------------- 카드 컨텐츠 구현 -------------------------------

        public GameObject usecardPanel; // 카드 사용 판정 패널

        public GameObject delcardPanel; // 카드 삭제 판정 패널
        public Text delcardText;

        [HideInInspector]
        public bool isCard;             // 현재 선택한 카드가 있다는 의미
        // 뱅 상태인지 아닌지
        [HideInInspector]
        public bool isBang = false;
        // 뱅 상태에서 클릭을 했는지 안했는지
        [HideInInspector]
        public bool bangClick = false;

        // 타겟과의 거리측정 결과
        public int targetDistance;

        // sendAvoid에 들어가는 파라미터
        // 0 : 공격자에게 뱅을 맞고 회피나 맞기를 했다고 알림
        // 1 : 공격자에게 기관총을 맞고 ~
        // 2 : 타겟이 0번 행동 한 후에 자신의 상태를 변경할 때
        [HideInInspector]
        public int avoidFlag = 0;

        // 회피카드가 아닌 그냥 맞기 버튼의 상태
        [HideInInspector]
        public bool avoidBtnFlag = false;

        // 죽은 사람 숫자
        public int deadman;

        // 잡화점 주인
        public bool storeMaster = false;

        // 결투를 시작 사람
        public bool duelMaster = false;

        // 셔플 인덱스 - 카드를 어느정도 사용하면 다시 섞어줌
        public int shuffleIdx;
        

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

            // 게임 들어오면 환경설정 버튼 숨기기 (임시)
            setButton = GameObject.Find("SetButton").GetComponent<Button>();
            setButton.gameObject.SetActive(false);

            // ------------------------  마우스가 화면 밖으로 나가지 못하게 함---------------
            //Cursor.lockState = CursorLockMode.Confined;

            isCard = false;

            // 셔플 인덱스 초기화
            shuffleIdx = 0;

            // 접속 못하면 초기화면으로 쫓아냄
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene("Launcher");
                return;
            }
        }

        void Start()
        {
            PhotonNetwork.IsMessageQueueRunning = true;

            //자리 리스트 초기화
            for (int i = 0; i < 7; i++)
                sitList[i] = tempsit;

            if (PlayerManager.LocalPlayerInstance == null)
            {
                StartCoroutine(InstantiateResource());

                cameraWork = GetComponent<CameraWork>(); // 본인 카메라 가져오기
            }
            StartCoroutine(WaitAllPlayers()); // 다른 플레이어 기다리기
        }

        private void Update()
        {
            // 인원 체크 UI가 계속 정면을 보게 만듬
            startCanvas.transform.LookAt(startCanvas.transform.position + Camera.main.transform.forward);

            // 뱅 카드 사용 중(상대방을 눌러야하는 카드에는 다 적용된다.)
            if (isBang && Input.GetMouseButtonDown(0))
                bangClick = true;
            else if (isBang && Input.GetMouseButtonUp(0)) 
                bangClick = false;

            // 사용한 카드가 점점 많아지면 다시 섞어줘야함
            // ----------------------------------------------------- shuffle 인덱스 
            if(shuffleIdx >= 50)
            {
                shuffleIdx = 0;
                if(PhotonNetwork.IsMasterClient && photonView.IsMine)
                    StartCoroutine("ShuffleCard");
            }
        }

        #endregion

        #region Coroutine

        // 캐릭터 생성
        IEnumerator InstantiateResource()
        {
            yield return new WaitForEndOfFrame();

            StartCoroutine(SpawnPlayer());
            StartCoroutine(SpawnNpc());

            //StartCoroutine(SpawnMap());
        }

        // 맵 생성(기능 삭제 - 기능 부활 시 맵 리스트랑 스폰포인트 생성해야함)
        /*IEnumerator SpawnMap()
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
                    PhotonNetwork.Instantiate("World/" + maps[i].name, points[i].position + maps[i].transform.position , maps[i].transform.rotation , 0);
                }
                yield return new WaitForEndOfFrame();
            }
            
        }*/

        IEnumerator SpawnPlayer()
        {
            RaiseEventManager.Instance.Spwan_Player();
            yield return new WaitForEndOfFrame();
        }

        IEnumerator SpawnNpc()
        {
            RaiseEventManager.Instance.Spawn_NPC();
            yield return null;
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
                    player1 = player;

                    ui = player.GetComponent<UI>();

                    MineUI.Instance.FindMinePv(player);
                    RaiseEventManager.Instance.FindMinePv(player);
                    break;
                }
            }

            yield return new WaitForEndOfFrame();
        }

        IEnumerator WaitAllPlayers()
        {
            // 플레이어들이 모두 들어오면 밑에 실행 가능
            while (PhotonNetwork.PlayerList.Length != playerList.Length)
            {
                Debug.Log("PM 방 총원 : " + PhotonNetwork.PlayerList.Length);
                Debug.Log("PM 현재 로딩된 인원 수 : " + playerList.Length);
                yield return null;
            }

            // 의자에 전체 인원 수 적용
            pnumText.text = sitNum + " / " + playerList.Length;

            StartCoroutine(FindMinePv());  // 자기 자신의 PhotonView, 관련 스크립트 찾기

            StartCoroutine(EndGame()); // 게임 종료 조건을 판단

            if (PhotonNetwork.IsMasterClient && photonView.IsMine)
            {

                // 직업이랑 능력을 나누어 준다.
                StartCoroutine(JobandAbility());

                StartCoroutine(AnimPlay());

                // 카드 나눠주는것
                //StartCoroutine(Cards());

                // ------------------------------------카드 기능 부활 중------------------------------------
                StartCoroutine(GiveCardSet());

            }
        }

        IEnumerator JobandAbility()
        {
            yield return new WaitForEndOfFrame();

            // (인구수에 맞게 하는 거 추가하기)
            //List<int> jobList = new List<int>() { 1, 2, 3, 4, 5, 6, 7 };
            List<int> jobList = new List<int>() { 2 ,4, 7 }; // 3인 룰


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
                playerList[i].GetComponent<PhotonView>().RPC("SyncHp", RpcTarget.All, -10);
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


        // 카드 덱 초기화 기능
        IEnumerator GiveCardSet()
        {
            // 임시 카드셋
            //cardSet = gameObject.AddComponent<CardSet>();

            // 초기 카드 세팅
            Card.cType[] initialDeck = new Card.cType[
                bang_c + avoid_c + beer_c + machinegun_c + indian_c + stagecoach_c + wellsfargo_c + saloon_c
                + generalstore_c + russian_c + navy_c + carbine_c + winchester_c + scope_c + mustang_c + barrel_c
                + dynamite_c + jail_c + carbalou_c + panic_c + duel_c
            ];

            int k = 0;
            for (int i = 0; i < bang_c; i++, k++)
                initialDeck[k] = Card.cType.Bang;
            for (int i = 0; i < avoid_c; i++, k++)
                initialDeck[k] = Card.cType.Avoid;
            for (int i = 0; i < beer_c; i++, k++)
                initialDeck[k] = Card.cType.Beer;
            for (int i = 0; i < machinegun_c; i++, k++)
                initialDeck[k] = Card.cType.MachineGun;
            for (int i = 0; i < indian_c; i++, k++)
                initialDeck[k] = Card.cType.Indian;
            for (int i = 0; i < stagecoach_c; i++, k++)
                initialDeck[k] = Card.cType.StageCoach;
            for (int i = 0; i < wellsfargo_c; i++, k++)
                initialDeck[k] = Card.cType.WellsFargo;
            for (int i = 0; i < saloon_c; i++, k++)
                initialDeck[k] = Card.cType.Saloon;
            for (int i = 0; i < generalstore_c; i++, k++)
                initialDeck[k] = Card.cType.GeneralStore;
            for (int i = 0; i < russian_c; i++, k++)
                initialDeck[k] = Card.cType.Russian;
            for (int i = 0; i < navy_c; i++, k++)
                initialDeck[k] = Card.cType.Navy;
            for (int i = 0; i < carbine_c; i++, k++)
                initialDeck[k] = Card.cType.Carbine;
            for (int i = 0; i < winchester_c; i++, k++)
                initialDeck[k] = Card.cType.Winchester;
            for (int i = 0; i < scope_c; i++, k++)
                initialDeck[k] = Card.cType.Scope;
            for (int i = 0; i < mustang_c; i++, k++)
                initialDeck[k] = Card.cType.Mustang;
            for (int i = 0; i < barrel_c; i++, k++)
                initialDeck[k] = Card.cType.Barrel;
            for (int i = 0; i < dynamite_c; i++, k++)
                initialDeck[k] = Card.cType.Dynamite;
            for (int i = 0; i < jail_c; i++, k++)
                initialDeck[k] = Card.cType.Jail;
            for (int i = 0; i < carbalou_c; i++, k++)
                initialDeck[k] = Card.cType.Catbalou;
            for (int i = 0; i < panic_c; i++, k++)
                initialDeck[k] = Card.cType.Panic;
            for (int i = 0; i < duel_c; i++, k++)
                initialDeck[k] = Card.cType.Duel;

            // 섞기
            int random1;
            int random2;
            Card.cType temp;
            for (int i = 0; i < initialDeck.Length; i++)
            {
                random1 = UnityEngine.Random.Range(0, initialDeck.Length);
                random2 = UnityEngine.Random.Range(0, initialDeck.Length);

                temp = initialDeck[random1];
                initialDeck[random1] = initialDeck[random2];
                initialDeck[random2] = temp;
            }

            //카드 섞인거 확인
            /*for (int i = 0; i < initialDeck.Length; i++)
            {
                Debug.Log("card: " + initialDeck[i]);
            }*/

            // rpc가 리스트를 넘길수 없으므로 json으로 바꿔서 보냄
            var json = JsonConvert.SerializeObject(initialDeck);

            player1.GetComponent<PhotonView>().RPC("GiveCardSet", RpcTarget.All, json, 0);

            yield return new WaitForEndOfFrame();
        }

        IEnumerator ShuffleCard()
        {
            // 임시 리스트
            List<Card.cType> temp = new List<Card.cType>();

            // 카드 리스트 에서 타입만 임시리스트로
            Debug.Log("shuffle start");
            foreach (Card c in cardList)
            {
                temp.Add(c.cardContent);
                Debug.Log(c.cardContent);
            }
            // 셔플
            temp = CommonFunction.ShuffleList(temp);
            var json = JsonConvert.SerializeObject(temp);

            player1.GetComponent<PhotonView>().RPC("GiveCardSet", RpcTarget.All, json, 1);

            yield return null;
        }


        // 게임 동작 시작
        public IEnumerator GameStart()
        {
            // 직업 선택 텍스트랑 애니메이션 재생

            yield return new WaitForEndOfFrame();
            jobPanel.SetActive(true);
            jobText.text = JobText();

            //yield return new WaitForSeconds(6f);
            jobPanel.SetActive(false);
            abilPanel.SetActive(true);

            //yield return new WaitForSeconds(0.5f);
            //abilUIAnimator.SetTrigger("Abil");
            abilText.text += AblityText();
            abilText.text += "\n3. 당신의 능력을 잘 활용하십시오";


            // Status 창 (인벤토리창 동기화)
            StatusUI.Instance.job_Name.text = jobText.text;
            StatusUI.Instance.job_Explain.text = abilText.text;


            // ------------------------------------------------ 사람들이 텍스트를 읽을 시간 부여(나중에 다시 활성화) ----------------------------
            //yield return new WaitForSeconds(12f);
            abilPanel.SetActive(false);
            MineUI.Instance.leftTopPanel.SetActive(true);
            MineUI.Instance.rightBottomPanel.SetActive(true);
            MineUI.Instance.rightTop.SetActive(true);

            // 전부 테이블에 앉으면 시작 준비 끝
            bangBtn.gameObject.SetActive(false);
            bool checkflag = true;
            while (checkflag)
            {
                for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                {
                    if (playerList[i].GetComponent<PlayerManager>().isSit == false)
                    {
                        checkflag = true;
                        break;
                    }
                    checkflag = false;
                }
                yield return null;
            }

            Debug.Log("시작 가능!");
            foreach (GameObject player in playerList)
            {
                if (player.GetComponent<PhotonView>().IsMine)
                {
                    if(playerList.Length == 3 )
                    {
                        if (player.GetComponent<PlayerManager>().playerType == jType.Vice)
                        {
                            bangBtn.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        if (player.GetComponent<PlayerManager>().playerType == jType.Sheriff)
                        {
                            bangBtn.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }

        
        IEnumerator GameLoop1() // 게임 시작후 덱 나눠주기 
        {
            // 카드 초기화를 위해 켜야함
            usecardPanel.SetActive(true);
            delcardPanel.SetActive(true);
            delcardText.gameObject.SetActive(false);

            // 보안관을 시작으로 순서 정하기 
            // 보안관 앉은 위치찾기
            int sheriffIdx = 0;
            for (int i = 0; i < sitList.Length; i++)
            {
                if (sitList[i].name == "tempsit")
                    continue;

                if(playerList.Length == 3 )
                {
                    if (sitList[i].GetComponent<PlayerManager>().playerType == jType.Vice)
                    {
                        sheriffIdx = i;
                        //Debug.Log("she idx : " + sheriffIdx);
                        break;
                    }
                }
                else
                {
                    if (sitList[i].GetComponent<PlayerManager>().playerType == jType.Sheriff)
                    {
                        sheriffIdx = i;
                        //Debug.Log("she idx : " + sheriffIdx);
                        break;
                    }
                }
                
            }
            // 보안관부터 시작하므로 시계방향으로 쭉 재정렬
            for (int k = 0; k < 7; k++)
            {
                // 모든 플레이어가 앉으면 끝
                if (turnList.Count == playerList.Length)
                    break;
                // 리스트 끝까지 가면 다시 앞으로 돌려
                if (sheriffIdx == 7)
                    sheriffIdx = 0;
                // 의자에 앉은 놈이 없으면 통과
                if (sitList[sheriffIdx].name == "tempsit")
                {
                    sheriffIdx++;
                    continue;
                }
                // 보안관부터 차례대로 저장
                turnList.Add(sitList[sheriffIdx++]);
                // turnlist에 턴 순서대로 플레이어들이 들어가 있음
            }

            yield return new WaitForEndOfFrame();

            // 카드 나눠주기 중복 방지를 위해 마스터클라이언트 혼자만 작동(1번만 해야하는 동작임)
            if (PhotonNetwork.IsMasterClient)
            {
                // 맨 처음에 5장씩 뿌림
                for (int i = 0; i < turnList.Count; i++)
                {
                    // 동시에
                    turnList[i].GetComponent<PhotonView>().RPC("GiveCards", RpcTarget.AllViaServer, 5);
                }
                yield return new WaitForEndOfFrame();
            }

            // 시점을 1인칭으로 바꿈
            cam.ChildCameras[2].gameObject.SetActive(true);
            cam.ChildCameras[1].gameObject.SetActive(false);

            StartCoroutine("GameLoop2");
            yield return new WaitForSeconds(1f);
        }
        
        public void GLStart() // gameloop1 실행 명령
        {
            StartCoroutine("GameLoop1");
        }

        IEnumerator GameLoop2() // 턴이 돌아가는 곳
        { 
            tidx = 0;
            nextSignal = false;

            // 시작 전 플레이어들의 카드 리스트를 동기화 시킴
            photonView.RPC("MyCardSync", RpcTarget.All);

            //playerinfo에서 머리위 카드 숫자 보여줌
            photonView.RPC("CardNumView", RpcTarget.All, 5);

            // 턴 진행
            Debug.Log("현재 턴 진행 진입");
            while (!isVictory)
            {
                if (tidx == turnList.Count)
                    tidx = 0;

                // 본인이 턴리스트 순서와 같아야 본인 턴 (중복 rpc 방지위해 본인 것만)
                if (player1.GetComponent<PhotonView>().ViewID == turnList[tidx].GetComponent<PhotonView>().ViewID && player1.GetComponent<PhotonView>().IsMine)
                {
                    turnList[tidx].GetComponent<PhotonView>().RPC("MyTurn", RpcTarget.All, tidx);
                    myTurn = true;

                    // 만약 사망 했을 경우 그냥 넘어감
                    if (playerInfo.isDeath)
                    {
                        turnList[tidx].GetComponent<PhotonView>().RPC("TurnIndexPlus", RpcTarget.All);
                        MineUI.Instance.NextButton.SetActive(false);
                        nextSignal = false;
                        myTurn = false;
                        continue;
                    }
                    // 살아있으니까 때린사람 초기화
                    photonView.RPC("AttackerIdxSync", RpcTarget.All);
                }
                // 본인턴이 아니라면 반복문 통과 못하고 대기중
                else
                {
                    Debug.Log("자기 턴 아님");

                    // 뱅(기관총) 카드에 의해 타겟팅이 되었을 때
                    if (playerInfo.isTarget == 1)
                    {
                        myTurn = true;
                        Debug.Log("뱅(기관총) 맞는 중 : " + playerInfo.isTarget);
                        playerInfo.isTarget = 3;
                        playerInfo.targetedBang = true;
                        StartCoroutine("Avoid", 0);
                    }
                    // 인디언 카드에 타겟팅 됨
                    else if(playerInfo.isTarget == 2)
                    {
                        myTurn = true;
                        Debug.Log("인디언 맞는 중 : " + playerInfo.isTarget);
                        playerInfo.isTarget = 3;
                        playerInfo.targetedIndian = true;
                        StartCoroutine("Avoid", 1);
                    }

                    // 캣벌로우 타겟팅 됨
                    if (playerInfo.isCat)
                    {
                        photonView.RPC("CardNumSync", RpcTarget.All, playerInfo.mycards.Count);
                        playerInfo.isCat = false;
                    }
                    // 강탈 타겟팅 됨
                    if(playerInfo.isPanic)
                    {
                        photonView.RPC("CardNumSync", RpcTarget.All, playerInfo.mycards.Count);
                        playerInfo.isPanic = false;
                    }

                    // 결투 대기 중
                    if (playerInfo.isDuel)
                    {
                        MineUI.Instance.duelPanel.SetActive(true);
                        MineUI.Instance.duelText.text = "결투 중... ( 상대편의 대응을 기다리는 중.. )";
                        MineUI.Instance.surrenderBtn.gameObject.SetActive(false);
                    }

                    //playerinfo에서 머리위 카드 숫자 보여줌 - 해결을 못하면 여기에 배치 할 듯
                    photonView.RPC("CardNumView", RpcTarget.All, playerInfo.mycardNumView);
                    yield return new WaitForSeconds(0.3f);

                    continue;
                }

                yield return new WaitForEndOfFrame();

                // 카드 드로우 하기 전 행동
                // 결투 중 확인
                if (duelTurn)
                {
                    // 본인이 결투중인거 아니면 넘어감
                    if (!playerInfo.isDuel)
                    {
                        turnList[tidx].GetComponent<PhotonView>().RPC("TurnIndexPlus", RpcTarget.All);
                        MineUI.Instance.NextButton.SetActive(false);
                        nextSignal = false;
                        myTurn = false;
                        continue;
                    }
                    // 본인이 결투중이면 항복 패널이 뜸
                    else
                    {
                        // 결투 패널 on
                        MineUI.Instance.duelPanel.SetActive(true);
                        MineUI.Instance.duelText.text = "결투 중... (BANG 으로 반격하세요)";
                        MineUI.Instance.surrenderBtn.gameObject.SetActive(true);
                        // 다음 턴 버튼 안보이게 함
                        MineUI.Instance.NextButton.SetActive(false);
                    }
                }

                // 감옥과 다이너마이트는 잡화점턴과 결투턴에는 작동 하면 안됌
                if(!playerInfo.isStore && !playerInfo.isDuel)
                {
                    // 감옥 확인
                    System.Random rand = new System.Random();
                    if (playerInfo.isJail)
                    {
                        int a = rand.Next(1, 5);
                        // 25% 확률로 탈출 가능 75% 확률로 턴 바로 종료
                        if (a != 1)
                        {
                            MineUI.Instance.jailText.text = "탈옥 실패";
                            JailPanelOnOff(0);
                            turnList[tidx].GetComponent<PhotonView>().RPC("TurnIndexPlus", RpcTarget.All);
                            MineUI.Instance.NextButton.SetActive(false);
                            nextSignal = false;
                            myTurn = false;
                            yield return new WaitForSeconds(1.2f);
                            photonView.RPC("JailSync", RpcTarget.All, 1);
                            JailPanelOnOff(1);
                            continue;
                        }
                        else // 탈옥
                        {
                            MineUI.Instance.jailText.text = "탈옥 성공";
                            JailPanelOnOff(0);
                            yield return new WaitForSeconds(1.2f);
                            photonView.RPC("JailSync", RpcTarget.All, 1);
                            JailPanelOnOff(1);
                        }

                        // 감옥은 다시 덱에 추가
                        photonView.RPC("CardDeckSync", RpcTarget.All, Card.cType.Jail);
                    }

                    // 다이너 마이트 확인
                    if (playerInfo.isDynamite)
                    {
                        int b = rand.Next(1, 3);
                        //12.5% 확률로 터짐 데미지는 3
                        if (b == 1) // 터짐
                        {
                            MineUI.Instance.dynamiteText.text = "터짐";
                            DynamitePanelOnOff(0);
                            // hp 변수 설계상 이렇게 해야함
                            for (int i = 0; i < 3; i++)
                            {
                                playerInfo.hp--;
                                photonView.RPC("SyncHp", RpcTarget.All, playerInfo.hp);
                            }
                            photonView.RPC("DynamiteSync", RpcTarget.All, 1);
                            yield return new WaitForSeconds(1.2f);
                            // 터졌으니까 다시 덱에 추가
                            photonView.RPC("CardDeckSync", RpcTarget.All, Card.cType.Dynamite);
                            DynamitePanelOnOff(1);
                        }
                        // 안터지면 다음 사람한테 붙음
                        else
                        {
                            MineUI.Instance.dynamiteText.text = "다음사람한테 던짐";
                            DynamitePanelOnOff(0);
                            photonView.RPC("DynamiteSync", RpcTarget.All, 1);
                            yield return new WaitForSeconds(1.2f);
                            DynamitePanelOnOff(1);
                            // 다음 사람한테 다이너 마이트 적용
                            if (tidx + 1 == turnList.Count)
                                turnList[0].GetComponent<PhotonView>().RPC("DynamiteSync", RpcTarget.All, 0);
                            else
                                turnList[tidx + 1].GetComponent<PhotonView>().RPC("DynamiteSync", RpcTarget.All, 0);
                        }
                    }
                }
                

                // ------ 카드 드로우 -------
                if(playerInfo.isStore == false && storeMaster == false && playerInfo.isDuel == false && duelMaster == false)
                {
                    // 일반 턴
                    // 본인턴에 카드 2장 뽑으면서 시작
                    turnList[tidx].GetComponent<PhotonView>().RPC("GiveCards", RpcTarget.AllViaServer, 2);
                }
                else if(playerInfo.isStore == true)
                {
                    // 잡화점 턴은 카드 안뽑음
                    // 잡화점 본인 턴이 오면 블록 패널 꺼줘서 클릭가능 
                    MineUI.Instance.blockingPanel.SetActive(false);
                }
                else if (storeMaster)
                {
                    // 잡화점을 사용한 사람이 한바퀴돌고 다시 돌아왔을 때 카드 안뽑음
                    storeMaster = false;
                }
                else if(playerInfo.isDuel == false && duelMaster == true)
                {
                    // 결투턴이 끝남
                    duelMaster = false;
                    MineUI.Instance.NextButton.SetActive(true);
                }

                // 본인턴에 본인 몸이 빛남
                photonView.RPC("TurnColor", RpcTarget.All, tidx, 1);

                while (true)
                {
                    if (nextSignal)
                    {
                        Debug.Log("턴 넘김");
                        photonView.RPC("TurnColor", RpcTarget.All, tidx, 0);

                        turnList[tidx].GetComponent<PhotonView>().RPC("TurnIndexPlus", RpcTarget.All);
                        nextSignal = false;
                        myTurn = false;
                        if (!storeMaster)
                            playerInfo.usedBang = false;

                        
                        break;
                    }
                    // 본인 턴 때 실행 중
                    Debug.Log("턴 소요 중~~~~~~~~");

                    //playerinfo에서 머리위 카드 숫자 보여줌 - 해결을 못하면 여기에 배치 할 듯
                    photonView.RPC("CardNumView", RpcTarget.All, playerInfo.mycardNumView);

                    yield return new WaitForSeconds(0.3f);
                }
                yield return null;
            }

            //------------ 게임이 끝남 -------------

            yield return new WaitForEndOfFrame();
        }
       

        // 게임 종료 조건 만족하는지 확인함 
        IEnumerator EndGame()
        {
            // 너무 빨리 측정하면 hp 동기화 전이라 전부 사망처리됨
            yield return new WaitForSeconds(10f);
            Debug.Log("게임 종료 조건 측정 시작");

            // 게임 인원수마다 게임 종료 조건이 다름
            switch (turnList.Count)
            {
                case 3:
                    outlawNum = 1;
                    viceNum = 1;
                    renegadeNum = 1;
                    break;
                default:
                    break;
            }

            // 3인 룰 : 부관 -> 배신자 -> 무법자 -> 부관 을 제거해야함 본인이 제거 못하면 최후의 1인이 남아야함
            while (!isVictory)
            {
                // 현재 남은 인원수
                int left = turnList.Count;
                foreach (GameObject player in playerList)
                {
                    // 현재 플레이어를 죽인사람 인덱스
                    int kidx = player.GetComponent<PlayerInfo>().attackerIdx;

                    // 부관 사망
                    if (player.GetComponent<PlayerManager>().playerType == jType.Vice && player.GetComponent<PlayerInfo>().isDeath)
                    {
                        left--;
                        // 부관이 무법자에게 당하면 무법자 승
                        if (turnList[kidx].GetComponent<PlayerManager>().playerType == jType.Outlaw)
                        {
                            Victory("outlaw");
                            isVictory = true;
                        }
                    }
                    // 배신자 사망
                    if (player.GetComponent<PlayerManager>().playerType == jType.Renegade && player.GetComponent<PlayerInfo>().isDeath)
                    {
                        left--;
                        // 배신자가 부관에게 당하면 부관 승
                        if (turnList[kidx].GetComponent<PlayerManager>().playerType == jType.Vice)
                        {
                            Victory("sherrif");
                            isVictory = true;
                        }
                    }
                    // 무법자 사망
                    if (player.GetComponent<PlayerManager>().playerType == jType.Outlaw && player.GetComponent<PlayerInfo>().isDeath)
                    {
                        left--;
                        // 무법자가 배신자에게 당하면 배신자 승
                        if (turnList[kidx].GetComponent<PlayerManager>().playerType == jType.Renegade)
                        {
                            Victory("renegade");
                            isVictory = true;
                        }
                    }
                }

                // 최후의 1인이 되면 승리
                if (left == 1)
                {
                    foreach (GameObject player in turnList)
                    {
                        // 살아잇으면 승리
                        if(player.GetComponent<PlayerInfo>().isDeath == false)
                        {
                            isVictory = true;
                            if (player.GetComponent<PlayerManager>().playerType == jType.Renegade)
                                Victory("renegade");
                            else if (player.GetComponent<PlayerManager>().playerType == jType.Outlaw)
                                Victory("outlaw");
                            else if (player.GetComponent<PlayerManager>().playerType == jType.Vice)
                                Victory("vice");
                        }
                    }
                }
                yield return new WaitForSeconds(1f);
            }

            // 4-7인 룰
            /*while (!isVictory)
            {
                foreach (GameObject player in playerList)
                {
                    
                    if(player.GetComponent<PlayerManager>().playerType == jType.Sheriff && player.GetComponent<PlayerInfo>().isDeath)
                    {
                        // 보안관 사망시 무법자 승리
                        Victory("outlaw");
                        isVictory = true;
                    }
                    else if(player.GetComponent<PlayerManager>().playerType == jType.Outlaw)
                    {
                        //현재 남아있는 무법자 수
                        if (!player.GetComponent<PlayerInfo>().isDeath)
                        {
                            outlawNum++;
                        }
                        Debug.Log("현재 무법자 수 : " + outlawNum);
                    }
                    else if(player.GetComponent<PlayerManager>().playerType == jType.Renegade)
                    {
                        // 현재 남아있는 배신자 수
                        if (!player.GetComponent<PlayerInfo>().isDeath)
                        {
                            renegadeNum++;
                        }
                        Debug.Log("현재 배신자 수 : " + renegadeNum);

                    }
                    else if(player.GetComponent<PlayerManager>().playerType == jType.Vice)
                    {
                        // 현재 남아있는 부관 수
                        if (!player.GetComponent<PlayerInfo>().isDeath)
                        {
                            viceNum++;
                        }
                        Debug.Log("현재 부관 수 : " + viceNum);

                    }

                    //Debug.Log("플레이어 : " + player.GetComponent<PlayerManager>().playerType + "    플레이어 hp : " + player.GetComponent<PlayerInfo>().hp);
                }

                // 부관과 무법자가 모두 사망해서 보안관과 1대1일이 되면 배신자 승리
                if (viceNum == 0 && outlawNum == 0)
                {
                    Victory("renegade");
                    isVictory = true;
                }

                // 무법자 배신자 모두 사망하면 보안관 승리
                if (outlawNum == 0 && renegadeNum == 0)
                {
                    Victory("sherrif");
                    isVictory = true;
                }

                yield return new WaitForSeconds(1f);

            
            }*/

            yield return null;
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

        // 게임 종료 관련 
        public void Victory(string winner)
        {
            vicPanel.SetActive(true);
            backPlane.SetActive(true);

            // 카드 UI 버튼 UI싹다 off
            MineUI.Instance.pos_CardParent.gameObject.SetActive(false);
            MineUI.Instance.NextButton.SetActive(false);

            SpawnWinner();

            // 본인 직업따라 승리 패배 결정

            jType temp = playerManager.playerType;
            switch (winner)
            {
                case "sherrif":
                    if (temp == jType.Sheriff || temp == jType.Vice)
                        vicText.gameObject.SetActive(true);
                    else
                        defText.gameObject.SetActive(true);
                    Debug.Log("보안관과 부관 승리!");
                    break;
                case "outlaw":
                    if (temp == jType.Outlaw)
                        vicText.gameObject.SetActive(true);
                    else
                        defText.gameObject.SetActive(true);
                    Debug.Log("무법자 승리!");
                    break;
                case "renegade":
                    if (temp == jType.Renegade)
                        vicText.gameObject.SetActive(true);
                    else
                        defText.gameObject.SetActive(true);
                    Debug.Log("배신자 승리");
                    break;
                case "vice":    // 3인룰 에서만 등장
                    if (temp == jType.Vice)
                        vicText.gameObject.SetActive(true);
                    else
                        defText.gameObject.SetActive(true);
                    Debug.Log("부관 승리");
                    break;
                default:
                    Debug.Log("어딘가에서 조건 빠진게 생김");
                    break;

            }

        }

        // 마지막에 전체 인원 생존 확인 (살아있으면 캐릭터 죽으면 무덤)
        public void SpawnWinner()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                for (int i = 0; i < playerList.Length; i++)
                {
                    PhotonNetwork.Instantiate(player[0].name , pnt[i].transform.position, pnt[i].transform.rotation, 0);
                }
            }
        }

        // 앉아있는 또는 서있는 인원 체크 
        public void NumCheckSit()
        {
            int max = 0;
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (max < playerList[i].GetComponent<Interaction>().sitNum)
                    max = playerList[i].GetComponent<Interaction>().sitNum;
            }
            sitNum = max;

            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                playerList[i].GetComponent<PhotonView>().RPC("NumCheck", RpcTarget.All, sitNum);
            }
        }
        public void NumCheckStand()
        {
            int min = 8;
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if(playerList[i].GetComponent<Interaction>().sitNum < min)
                    min = playerList[i].GetComponent<Interaction>().sitNum; 
            }
            sitNum = min;

            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                playerList[i].GetComponent<PhotonView>().RPC("NumCheck", RpcTarget.All, sitNum);
            }
        }
        
        // 게임 시작하기 위한 버튼
        public void BangBtnClick()
        {
            foreach (GameObject player in playerList)
            {
                // 모든 사람의 turnlist에 정보 저장을 위함
                player.GetComponent<PhotonView>().RPC("StartUIOff", RpcTarget.All);
                // 모든 사람의 gameloop 진입을 위함
                player.GetComponent<PhotonView>().RPC("Gameloop", RpcTarget.All);

                // 머리위 HP 닉네임 위치 보정
                player.GetComponent<PhotonView>().RPC("UIMatch", RpcTarget.All);
            }
        }

        // 게임 중 턴 넘기는 버튼
        public void NextBtnClick(string state = "default" ) 
        {
            //Debug.Log("턴 버튼 누름!");
            // 본인 카드가 본인 hp보다 많으면 못넘어감
            if(playerInfo.mycards.Count > playerInfo.hp)
            {
                // 잡화점 턴에는 카드 수 무시
                if (state.Equals("store"))
                {
                    nextSignal = true;
                    MineUI.Instance.NextButton.SetActive(false);
                }
                else
                    StartCoroutine(Alert(0));
            }
            else
            {
                nextSignal = true;
                MineUI.Instance.NextButton.SetActive(false);
            }

            // 임시로 해놓은 거
            //nextSignal = true;
            //MineUI.Instance.NextButton.SetActive(false);
        }

        // -------------------------------- 알림 패널 관련 0-------------------
        public void alertOrder(int num)
        {
            StartCoroutine(Alert(num));
        }

        IEnumerator Alert(int state)
        {
            if(state == 0) // hp 보다 카드가 많은데 nextbutton을 누름
            {
                MineUI.Instance.alertPanel.SetActive(true);
                MineUI.Instance.alertText.text = "본인 카드 수가 본인 HP보다 많을 수 없습니다.";
                yield return new WaitForSeconds(1.5f);
                MineUI.Instance.alertPanel.SetActive(false);
            }
            else if(state == 1)
            {
                MineUI.Instance.alertPanel.SetActive(true);
                MineUI.Instance.alertText.text = "Bang은 한턴에 한번만 쓸수 있습니다.";
                yield return new WaitForSeconds(1.5f);
                MineUI.Instance.alertPanel.SetActive(false);
            }
        }
        
        // 게임 중 회피를 못할 때 그냥 맞는 버튼
        public void DamageBtn()
        {
            playerInfo.sendAvoid = false;
            playerInfo.sendBang = false;
            avoidBtnFlag = true;
        }

        // 결투 중 뱅을 못 내거나 그냥 항복하는 버튼
        public void SurrenderBtn()
        {
            Debug.Log("항복");
            // 체력 -1
            playerInfo.hp--;
            photonView.RPC("SyncHp", RpcTarget.All, playerInfo.hp);

            // duelTurn을 끝내고 각 player의 isduel 상태를 false로
            // tidx = duelMaster 포함 되어있음 원래 순서로 돌아가는 코드 
            photonView.RPC("DuelTurn", RpcTarget.All, 1);
            photonView.RPC("DuelSync", RpcTarget.All, 1);

            // 본인이 항복하면 턴 넘기기 버튼이 사라짐 그래서 다시 켜줌
            if (duelIdx == tidx)
                MineUI.Instance.NextButton.SetActive(true);
        }

       

        // 본인 카드덱 찾게 도와줌
        public GameObject CallMyPlayer()
        {
            return player1;
        }

        // 본인과 타겟사이의 거리를 측정함
        public int MeasureDistance(int targetIdx)
        {
            int result;
            result = Math.Abs(tidx - targetIdx);
            if (result > turnList.Count / 2)
                result = turnList.Count - result;

            return result;
        }

        // 본인이 뱅 타겟되었다는 UI on
        public void TargetedPanelOn(string msg = "")
        {
            //if(player1.GetComponent<PlayerInfo>().isTarget == true)
            if (playerInfo.isTarget == 1 || playerInfo.isTarget == 2)
            {
                MineUI.Instance.targetedPanel.SetActive(true);
                MineUI.Instance.ttext.text = msg;
            }
        }

        // 사거리 부족 UI ONOFF
        public void DistancePanelOnOff(int state)
        {
            if (state == 0)
                MineUI.Instance.distancePanel.SetActive(true);
            else
                MineUI.Instance.distancePanel.SetActive(false);
        }

        // 감옥 UI ONOFF
        public void JailPanelOnOff(int state)
        {
            if (state == 0)
                MineUI.Instance.jailPanel.SetActive(true);
            else
                MineUI.Instance.jailPanel.SetActive(false);
        }

        // 다이너마이트 UI ONOFF
        public void DynamitePanelOnOff(int state)
        {
            if (state == 0)
                MineUI.Instance.dynamitePanel.SetActive(true);
            else
                MineUI.Instance.dynamitePanel.SetActive(false);
        }

        // 술통 UI ONOFF
        public void BarrelPanelOnOff(int state)
        {
            if (state == 0)
                MineUI.Instance.barrelPanel.SetActive(true);
            else
                MineUI.Instance.barrelPanel.SetActive(false);
        }

        // string을 enum형태로 바꿔줌 
        public Card.cType StringtoEnum(string s)
        {
            return (Card.cType)Enum.Parse(typeof(Card.cType), s);
        }

        // 카드 사용한 거 다시 카드 셋으로 넣는 기능
        public void AfterCardUse(Card.cType content, int state, bool equip) 
        {
            //playerinfo에서 머리위 카드 숫자 보여줌
            photonView.RPC("CardNumView", RpcTarget.All, playerInfo.mycardNumView - 1);
            //Debug.Log("use card content : " + content);
            // 0: 카드사용 1: 카드 삭제 2: 인디언 사용
            // 카드 삭제일 때
            if (state == 1)
            {
                // 카드더미 동기화 시켜주기 - DataSync로
                player1.GetComponent<PhotonView>().RPC("CardDeckSync", RpcTarget.All, content);
                return;
            }

            // 현재 사용 무기 임시 등록
            Card.cType temp = StringtoEnum(playerInfo.wName);

            // 카드 사용 일 때
            // 카드 종류에 따라 실행이 달라진다
            string t = content.ToString();
            switch (t)
            {
                case "Bang":
                    // 인디언 회피 
                    if (playerInfo.isTarget == 3)
                        playerInfo.sendBang = true;
                    // 공격
                    else
                        StartCoroutine("Bang");

                    break;
                case "Avoid":
                    playerInfo.sendAvoid = true;
                    break;
                case "Beer":
                    StartCoroutine("Beer");
                    break;
                case "Indian":
                    StartCoroutine("Indian");
                    break;
                case "MachineGun":
                    StartCoroutine("MachineGun");
                    break;
                case "StageCoach":
                    StartCoroutine("StageCoach");
                    break;
                case "WellsFargo":
                    StartCoroutine("WellsFargo");
                    break;
                case "Saloon":
                    StartCoroutine("Saloon");
                    break;
                case "GeneralStore":
                    StartCoroutine("GeneralStore");
                    break;
                case "Russian":
                    // 무기 사거리 증가
                    playerInfo.maximumRange = 2;
                    if (playerInfo.isWeapon) // 현재 무기가 기본 무기가 아니면
                        photonView.RPC("CardDeckSync", RpcTarget.All, temp);
                    photonView.RPC("WeaponSync", RpcTarget.All, 2);
                    break;
                case "Navy":
                    playerInfo.maximumRange = 3;
                    if (playerInfo.isWeapon) // 현재 무기가 기본 무기가 아니면
                        photonView.RPC("CardDeckSync", RpcTarget.All, temp);
                    photonView.RPC("WeaponSync", RpcTarget.All, 3);
                    break;
                case "Carbine":
                    playerInfo.maximumRange = 4;
                    if (playerInfo.isWeapon) // 현재 무기가 기본 무기가 아니면
                        photonView.RPC("CardDeckSync", RpcTarget.All, temp);
                    photonView.RPC("WeaponSync", RpcTarget.All, 4);
                    break;
                case "Winchester":
                    playerInfo.maximumRange = 5;
                    if (playerInfo.isWeapon) // 현재 무기가 기본 무기가 아니면
                        photonView.RPC("CardDeckSync", RpcTarget.All, temp);
                    photonView.RPC("WeaponSync", RpcTarget.All, 5);
                    break;
                case "Scope":
                    photonView.RPC("ScopeSync", RpcTarget.All, 0);
                    break;
                case "Mustang":
                    photonView.RPC("MustangSync", RpcTarget.All, 0);
                    break;
                case "Barrel":
                    photonView.RPC("BarrelSync", RpcTarget.All, 0);
                    break;
                case "Dynamite":
                    StartCoroutine("Dynamite");
                    break;
                case "Jail":
                    StartCoroutine("Jail");
                    break;
                case "Catbalou":
                    StartCoroutine("Catbalou");
                    break;
                case "Panic":
                    StartCoroutine("Panic");
                    break;
                case "Duel":
                    StartCoroutine("Duel");
                    break;
                default:
                    break;
            }
            
            // 카드더미 동기화 시켜주기 - DataSync로
            if(!equip) // 장착카드 아니면 사용 후 바로 덱에 추가
                photonView.RPC("CardDeckSync", RpcTarget.All, content);
        }

        #endregion

        // 카드  만드는 중
        // 하이라이트 부분이 계속 반복 되므로 따로 함수로 빼는 것을 고려 해보기
        
        #region 완성된 카드 기능 

        IEnumerator Bang()
        {
            isBang = true;
            Material mat;
            GameObject temp = null;

            while (true)
            {
                Debug.Log("Bang 동작 중");
                // 빔 
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;


                if (Physics.Raycast(ray, out hit))
                {
                    //Debug.Log("마우스에 닿음 : " + hit.transform.gameObject);
                    GameObject go = hit.transform.gameObject;

                    // 마우스 닿은 캐릭터 하이라이트 (플레이어에서 마우스 벗어나면 다시 꺼지게 변경해야함)
                    if (go.CompareTag("Player"))
                    {
                        mat = hit.transform.GetComponentInChildren<SkinnedMeshRenderer>().material;
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", Color.red * 0.5f);
                        temp = go;
                    }
                    else if (temp != null)
                    {
                        mat = temp.transform.GetComponentInChildren<SkinnedMeshRenderer>().material;
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", Color.black);
                    }

                    if (go.CompareTag("Player") && bangClick)
                    {
                        Debug.Log("플레이어 선택 : " + go);

                        // 클릭했으니까 하이라이트 꺼야함
                        mat = hit.transform.GetComponentInChildren<SkinnedMeshRenderer>().material;
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", Color.black);

                        // 타겟과 내 사이 거리를 구함
                        int targetIdx = turnList.FindIndex(a => a == go);
                        targetDistance = MeasureDistance(targetIdx);
                        // 타겟의 야생마가 있으면 거리 +1
                        if (go.GetComponent<PlayerInfo>().isMustang)
                            targetDistance++;
                        //Debug.Log("Target Dis : " + targetDistance);

                        // 측정 거리 > 본인 최대 사거리 (뱅 불가)
                        if (targetDistance > playerInfo.maximumRange)
                        {
                            //Debug.Log("뱅 불가함");
                            DistancePanelOnOff(0);
                            yield return new WaitForSeconds(1f);
                            DistancePanelOnOff(1);
                        }
                        else
                        {
                            // 타겟 선언 및 상대편 화면에 UI 띄움
                            go.GetComponent<PhotonView>().RPC("BangTargeted", RpcTarget.All, 1, tidx);

                            // 나의 waitAvoid 상태를 전체에게 동기화
                            playerInfo.waitAvoid = true;
                            photonView.RPC("WaitAvoid", RpcTarget.All, 0);

                            // 상대방의 avoid 기다림
                            while (playerInfo.waitAvoid == true)
                            {
                                Debug.Log("상대방의 회피를 기다리는 중 ");

                                // 투명 패널 켜서 기다리는 동안 입력 막기
                                MineUI.Instance.blockingPanel.SetActive(true);
                                yield return new WaitForSeconds(0.1f);
                            }
                            Debug.Log("상대방의 회피를 기다리는 상태를 빠져나옴 ");
                            MineUI.Instance.blockingPanel.SetActive(false);

                            // 끝나면 루프 종료
                            break;
                        }
                    }
                }
                yield return new WaitForEndOfFrame();
            }
            // 뱅 관련 플래그 초기화
            bangClick = false;
            isBang = false;

            playerInfo.usedBang = true;

            yield return new WaitForEndOfFrame();
        }

        IEnumerator MachineGun()
        {
            photonView.RPC("MgSync", RpcTarget.All, 0);

            deadman = 0;
            // 타겟 선언 및 상대편 화면에 UI 띄움
            foreach (GameObject player in playerList)
            {
                // 본인 제외 전체 공격
                if (player.GetComponent<PhotonView>().ViewID != player1.GetComponent<PhotonView>().ViewID)
                {
                    if (!player.GetComponent<PlayerInfo>().isDeath)
                        player.GetComponent<PhotonView>().RPC("BangTargeted", RpcTarget.All, 1, tidx);
                    else
                        deadman++;
                }
            }

            // 나의 waitAvoids 상태를 전체에게 동기화
            photonView.RPC("WaitAvoid", RpcTarget.All, 1);

            // 상대방의 avoid 갯수
            while (playerInfo.waitAvoids < (playerList.Length - (1 + deadman)))
            {
                Debug.Log(playerInfo.waitAvoids);
                Debug.Log("상대방의 회피를 기다리는 중 ");
                // 투명 패널 켜서 기다리는 동안 입력 막기
                MineUI.Instance.blockingPanel.SetActive(true);
                yield return new WaitForSeconds(0.1f);
            }
            Debug.Log("상대방의 회피를 기다리는 상태를 빠져나옴 ");
            MineUI.Instance.blockingPanel.SetActive(false);

            photonView.RPC("MgSync", RpcTarget.All, 1);

            // 기관총 관련 플래그 초기화
            playerInfo.waitAvoids = -1;
            deadman = 0;

            yield return new WaitForEndOfFrame();
        }

        IEnumerator Indian()
        {
            photonView.RPC("IndianSync", RpcTarget.All, 0);
            deadman = 0;

            // 타겟 선언 및 상대편 화면에 UI 띄움
            foreach (GameObject player in playerList)
            {
                // 본인 제외 전체 공격
                if (player.GetComponent<PhotonView>().ViewID != player1.GetComponent<PhotonView>().ViewID)
                {
                    if (!player.GetComponent<PlayerInfo>().isDeath)
                        player.GetComponent<PhotonView>().RPC("BangTargeted", RpcTarget.All, 2, tidx);
                    else
                        deadman++;
                }
            }

            // 나의 waitBangs 상태를 전체에게 동기화(함수 돌려쓰기)
            photonView.RPC("WaitAvoid", RpcTarget.All, 2);

            // 상대방의 bang 갯수
            while (playerInfo.waitBangs < (playerList.Length - (1 + deadman)))
            {
                Debug.Log("상대방의 반격을 기다리는 중 ");

                // 투명 패널 켜서 기다리는 동안 입력 막기
                MineUI.Instance.blockingPanel.SetActive(true);
                yield return new WaitForSeconds(0.1f);
            }
            Debug.Log("상대방의 반격을 기다리는 상태를 빠져나옴 ");
            MineUI.Instance.blockingPanel.SetActive(false);

            photonView.RPC("IndianSync", RpcTarget.All, 1);

            // 인디언 관련 플래그 초기화
            playerInfo.waitBangs = -1;
            deadman = 0;
            yield return new WaitForEndOfFrame();
        }

        IEnumerator Avoid(int state)
        {
            // 0 : bang MG Avoid
            // 1 : indian Avoid

            avoidFlag = 0;
            bool barrelTurn = true; // 회피중 술통 기회는 한번 뿐이어야함

            if(state == 0)
            {
                System.Random rand = new System.Random();
                // 회피카드를 낼 건지 그냥 맞을 건지 선택 대기
                while (playerInfo.sendAvoid == false)
                {
                    Debug.Log("회피 카드 내기 대기중");
                    // 술통을 장착하고 1회 회피 기회 부여
                    if (playerInfo.isBarrel && barrelTurn)
                    {
                        barrelTurn = false;
                        // 25% 확률로 뱅 무효
                        int a = rand.Next(1, 5);
                        if (a == 1)
                        {
                            MineUI.Instance.barrelText.text = "술통에 맞음";
                            BarrelPanelOnOff(0);
                            yield return new WaitForSeconds(1.2f);
                            BarrelPanelOnOff(1);
                            break;
                        }
                    }

                    // 그냥 맞기를 선택함
                    if (avoidBtnFlag == true)
                    {
                        avoidFlag = 1;
                        break;
                    }

                    yield return new WaitForSeconds(0.1f);
                }

                // 상황에 따른 HP 상태
                if (avoidFlag == 0)
                {
                    Debug.Log("회피 카드 내서 빠져나옴");
                }
                else if (avoidFlag == 1)
                {
                    playerInfo.hp--;
                    photonView.RPC("SyncHp", RpcTarget.All, playerInfo.hp);
                    Debug.Log("맞고 빠져나옴");
                }

                foreach (GameObject player in turnList)
                {
                    // 공격자의 회피 대기 상태를 바꿔야함
                    // 기관총
                    if (player.GetComponent<PlayerInfo>().isMG == true)
                    {
                        Debug.Log("상대편 waitavoids 상태 변경중");
                        player.GetComponent<PhotonView>().RPC("SendAvoid", RpcTarget.All, 1);
                        break;
                    }
                    // 뱅
                    if (player.GetComponent<PlayerInfo>().waitAvoid == true)
                    {
                        Debug.Log("상대편 waitavoid 상태 변경중");
                        player.GetComponent<PhotonView>().RPC("SendAvoid", RpcTarget.All, 0);
                        break;
                    }
                }
            }
            else if(state == 1) // indian
            {
                // 회피카드를 낼 건지 그냥 맞을 건지 선택 대기
                while (playerInfo.sendBang == false)
                {
                    Debug.Log("뱅(반격) 카드 내기 대기중");

                    // 그냥 맞기를 선택함
                    if (avoidBtnFlag == true)
                    {
                        avoidFlag = 1;
                        break;
                    }
                    yield return new WaitForSeconds(0.1f);
                }

                // 상황에 따른 HP 상태
                if (avoidFlag == 0)
                {
                    Debug.Log("반격 카드 내서 빠져나옴");
                }
                else if (avoidFlag == 1)
                {
                    playerInfo.hp--;
                    photonView.RPC("SyncHp", RpcTarget.All, playerInfo.hp);
                    Debug.Log("맞고 빠져나옴");
                }

                foreach (GameObject player in turnList)
                {
                    // 공격자의 회피 대기 상태를 바꿔야함
                    // 인디언
                    if (player.GetComponent<PlayerInfo>().isIndian == true)
                    {
                        Debug.Log("상대편 waitBangs 상태 변경중");
                        player.GetComponent<PhotonView>().RPC("SendAvoid", RpcTarget.All, 2);
                        break;
                    }
                }

            }

            myTurn = false;
            // 타겟 상태에서 벗어남
            playerInfo.isTarget = 0;
            photonView.RPC("SendAvoid", RpcTarget.All, 3);

            // 타겟 화면 UI 끔
            MineUI.Instance.targetedPanel.SetActive(false);

            // 회피관련 플래그 초기화
            avoidBtnFlag = false;
            playerInfo.sendAvoid = false;
            playerInfo.sendBang = false;
            playerInfo.targetedBang = false;
            playerInfo.targetedIndian = false;

            yield return new WaitForEndOfFrame();
        }

        IEnumerator Beer()
        {
            int temp = 0;
            if (playerInfo.hp == playerInfo.maxHp)
                temp = playerInfo.hp;
            else if (playerInfo.hp < playerInfo.maxHp)
                temp = ++playerInfo.hp;

            photonView.RPC("SyncHp", RpcTarget.All, temp);
            yield return new WaitForEndOfFrame();
        }

        IEnumerator Saloon()
        {
            // 모든 사람 hp1 회복
            foreach (GameObject player in turnList)
            {
                // 죽은사람 건너뛰기
                if (player.GetComponent<PlayerInfo>().isDeath)
                    continue;

                int temp = 0;
                if (player.GetComponent<PlayerInfo>().hp == player.GetComponent<PlayerInfo>().maxHp)
                    temp = player.GetComponent<PlayerInfo>().hp;
                else if (player.GetComponent<PlayerInfo>().hp < player.GetComponent<PlayerInfo>().maxHp)
                    temp = ++player.GetComponent<PlayerInfo>().hp;
                player.GetComponent<PhotonView>().RPC("SyncHp", RpcTarget.All, temp);
            }
            yield return new WaitForEndOfFrame();
        }

        IEnumerator StageCoach()
        {
            turnList[tidx].GetComponent<PhotonView>().RPC("GiveCards", RpcTarget.AllViaServer, 2);
            yield return null;
        }

        IEnumerator WellsFargo()
        {
            turnList[tidx].GetComponent<PhotonView>().RPC("GiveCards", RpcTarget.AllViaServer, 3);
            yield return null;
        }
        
        IEnumerator GeneralStore()
        {
            // 죽은 사람 체크
            deadman = 0;
            foreach(GameObject player in turnList)
            {
                if (player.GetComponent<PlayerInfo>().isDeath)
                    deadman++;
            }
            // 카드를 인원수에 맞게 중앙에 펼침
            photonView.GetComponent<PhotonView>().RPC("GiveStoreCard", RpcTarget.All, (turnList.Count - deadman));

            // 모든 사람의 isStore true로 만듬
            photonView.GetComponent<PhotonView>().RPC("StoreSync", RpcTarget.All, 0);

            storeMaster = true;
            deadman = 0;

            yield return null;
        }

        IEnumerator Jail()
        {
            isBang = true;          // 뱅에서 쓰던거 재활용(필요한 기능이 같아서)
            Material mat;
            GameObject temp = null;

            while (true)
            {
                Debug.Log("jail 동작 중");
                // 빔 
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    //Debug.Log("마우스에 닿음 : " + hit.transform.gameObject);
                    GameObject go = hit.transform.gameObject;

                    // 마우스 닿은 캐릭터 하이라이트 (플레이어에서 마우스 벗어나면 다시 꺼지게 변경해야함)
                    if (go.CompareTag("Player"))
                    {
                        mat = hit.transform.GetComponentInChildren<SkinnedMeshRenderer>().material;
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", Color.red * 0.5f);
                        temp = go;
                    }
                    else if (temp != null)
                    {
                        mat = temp.transform.GetComponentInChildren<SkinnedMeshRenderer>().material;
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", Color.black);
                    }

                    if (go.CompareTag("Player") && bangClick)
                    {
                        Debug.Log("플레이어 선택 : " + go);

                        // 클릭했으니까 하이라이트 꺼야함
                        mat = hit.transform.GetComponentInChildren<SkinnedMeshRenderer>().material;
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", Color.black);

                        // 감옥 동기화
                        go.GetComponent<PhotonView>().RPC("JailSync", RpcTarget.All, 0);

                        break;
                    }
                }
                yield return new WaitForEndOfFrame();
            }
            // 뱅에서 쓰던거 재활용
            bangClick = false;
            isBang = false;

            yield return new WaitForEndOfFrame();
        }

        IEnumerator Dynamite()
        {
            isBang = true;          // 뱅에서 쓰던거 재활용(필요한 기능이 같아서)
            Material mat;
            GameObject temp = null;

            while (true)
            {
                Debug.Log("dynamite 동작 중");
                // 빔 
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    //Debug.Log("마우스에 닿음 : " + hit.transform.gameObject);
                    GameObject go = hit.transform.gameObject;

                    // 마우스 닿은 캐릭터 하이라이트 (플레이어에서 마우스 벗어나면 다시 꺼지게 변경해야함)
                    if (go.CompareTag("Player"))
                    {
                        mat = hit.transform.GetComponentInChildren<SkinnedMeshRenderer>().material;
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", Color.red * 0.5f);
                        temp = go;
                    }
                    else if (temp != null)
                    {
                        mat = temp.transform.GetComponentInChildren<SkinnedMeshRenderer>().material;
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", Color.black);
                    }

                    if (go.CompareTag("Player") && bangClick)
                    {
                        Debug.Log("플레이어 선택 : " + go);

                        // 클릭했으니까 하이라이트 꺼야함
                        mat = hit.transform.GetComponentInChildren<SkinnedMeshRenderer>().material;
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", Color.black);

                        // 다이너마이트 동기화
                        go.GetComponent<PhotonView>().RPC("DynamiteSync", RpcTarget.All, 0);

                        break;
                    }
                }
                yield return new WaitForEndOfFrame();
            }
            // 뱅에서 쓰던거 재활용
            bangClick = false;
            isBang = false;

            yield return new WaitForEndOfFrame();
        }

        IEnumerator Catbalou()
        {
            isBang = true;          // 뱅에서 쓰던거 재활용(필요한 기능이 같아서)
            Material mat;
            GameObject temp = null;

            while (true)
            {
                Debug.Log("캣벌로우 동작 중");
                // 빔 
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    //Debug.Log("마우스에 닿음 : " + hit.transform.gameObject);
                    GameObject go = hit.transform.gameObject;

                    // 마우스 닿은 오브젝트 하이라이트 
                    if (go.CompareTag("Player") || go.CompareTag("Item") || go.CompareTag("Weapon"))
                    {
                        if (go.CompareTag("Player"))
                            mat = hit.transform.GetComponentInChildren<SkinnedMeshRenderer>().material;
                        else
                        {
                            if (go.name.Equals("Mustang"))
                                mat = hit.transform.GetChild(0).GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>().material;
                            else
                                mat = hit.transform.GetComponent<MeshRenderer>().material;
                        }

                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", Color.red * 0.5f);
                        temp = go;
                    }
                    else if (temp != null)
                    {
                        if (temp.CompareTag("Player"))
                            mat = temp.transform.GetComponentInChildren<SkinnedMeshRenderer>().material;
                        else if (temp.CompareTag("Item") || temp.CompareTag("Weapon"))
                        {
                            // 야생마는 구조가 다름
                            if (temp.name.Equals("Mustang"))
                                mat = temp.transform.GetChild(0).GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>().material;
                            else
                                mat = temp.transform.GetComponent<MeshRenderer>().material;
                        }
                        else
                            mat = null;

                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", Color.black);
                    }

                    // 1. 캐릭터 덱에 카드 한장 없애기
                    if (go.CompareTag("Player") && bangClick)
                    {
                        Debug.Log("플레이어 선택 : " + go);
                        playerInfo.useCat = true;

                        // 클릭했으니까 하이라이트 꺼야함
                        mat = hit.transform.GetComponentInChildren<SkinnedMeshRenderer>().material;
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", Color.black);

                        // 상대편 클릭하고 상대편 덱의 갯수를 가져옴
                        go.GetComponent<PhotonView>().RPC("CatSync", RpcTarget.All, 0);

                        //gameloop2에서 cardNumSync하는거 기다림
                        yield return new WaitForSeconds(1.2f);

                        // 빈 카드를 덱 갯수 만큼 생성함 (상대편 카드를 뽑는 것처럼 눈속임 )
                        MineUI.Instance.CatbalouCards(go.GetComponent<PlayerInfo>().mycardNum);

                        // 카드 뽑기할때 방해되서 잠시 끔
                        delcardPanel.gameObject.SetActive(false);
                        MineUI.Instance.cardblockingPanel.SetActive(true);

                        while (playerInfo.useCat)
                        {
                            // 선택 대기
                            Debug.Log("캣벌로우 선택 대기");
                            yield return null;
                        }
                        // 다시 켬
                        delcardPanel.gameObject.SetActive(true);
                        MineUI.Instance.cardblockingPanel.SetActive(false);

                        // 캣 벌로우에 의해 타겟 카드 중 랜덤으로 하나 삭제
                        go.GetPhotonView().RPC("CatbalouDel", RpcTarget.All, go.GetComponent<PlayerInfo>().mycardNum);

                        // 캣 벌로우 변수 초기화
                        go.GetComponent<PhotonView>().RPC("CatSync", RpcTarget.All, 1);

                        break;
                    }
                    // 2. 캐릭터 아이템 중 하나 없애기
                    else if (go.CompareTag("Item") && bangClick)
                    {
                        Debug.Log("아이템 선택 : " + go);
                        playerInfo.useCat = true;
                        // 클릭했으니까 하이라이트 꺼야함
                        if (go.name.Equals("Mustang"))
                        {
                            mat = hit.transform.GetChild(0).GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>().material;
                            mat.EnableKeyword("_EMISSION");
                            mat.SetColor("_EmissionColor", Color.black);
                        }
                        else
                        {
                            mat = hit.transform.GetComponent<MeshRenderer>().material;
                            mat.EnableKeyword("_EMISSION");
                            mat.SetColor("_EmissionColor", Color.black);
                        }

                        // 아이템 클릭하면 부모는 게임플레이트고 그거의 부모가 의자(0 - 6 플레이어 idx)
                        GameObject tmp = go.transform.parent.gameObject;
                        GameObject chair = tmp.transform.parent.gameObject;

                        foreach (GameObject itemMaster in turnList)
                        {
                            // 아이템의 주인 찾음
                            if (itemMaster.GetComponent<PlayerManager>().myChair.name.Equals(chair.name))
                            {
                                // 아이템 장착 상태 해제
                                string t = go.name + "Sync";
                                itemMaster.GetComponent<PhotonView>().RPC(t, RpcTarget.All, 1);
                            }
                        }

                        // 아이템 이름이랑 열거형 타입이랑 같기 때문에 이름가지고 enum타입 만들기
                        Card.cType type = StringtoEnum(go.name);

                        // 장착 해제 된 아이템을 카드로 바꿔서 전체 덱 뒤에 다시 추가
                        photonView.RPC("CardDeckSync", RpcTarget.All, type);

                        break;
                    }
                    //3. 무기 초기화
                    else if (go.CompareTag("Weapon") && bangClick)
                    {
                        Debug.Log("무기 선택 : " + go);
                        playerInfo.useCat = true;
                        // 클릭했으니까 하이라이트 꺼야함
                        mat = hit.transform.GetComponent<MeshRenderer>().material;
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", Color.black);

                        // 아이템 클릭하면 부모는 게임플레이트고 그거의 부모가 의자(0 - 6 플레이어 idx)
                        GameObject tmp = go.transform.parent.gameObject;
                        GameObject chair = tmp.transform.parent.gameObject;

                        foreach (GameObject itemMaster in turnList)
                        {
                            // 무기 주인 찾음 무기는 없어지면서 자동으로 기본무기 colt로 바뀜
                            if (itemMaster.GetComponent<PlayerManager>().myChair.name.Equals(chair.name))
                               itemMaster.GetComponent<PhotonView>().RPC("WeaponSync", RpcTarget.All, 1);
                        }
                        // 무기 이름이랑 열거형 타입이랑 같기 때문에 이름가지고 enum타입 만들기
                        Card.cType type = StringtoEnum(go.name);

                        // 장착 해제 된 아이템을 카드로 바꿔서 전체 덱 뒤에 다시 추가
                        photonView.RPC("CardDeckSync", RpcTarget.All, type);

                        break;
                    }
                }
                yield return new WaitForEndOfFrame();
            }
            // 뱅에서 쓰던거 재활용
            bangClick = false;
            isBang = false;

            yield return new WaitForEndOfFrame();
        }

        IEnumerator Panic()
        {
            isBang = true;          // 뱅에서 쓰던거 재활용(필요한 기능이 같아서)
            Material mat;
            GameObject temp = null;

            while (true)
            {
                Debug.Log("강탈 동작 중");
                // 빔 
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    //Debug.Log("마우스에 닿음 : " + hit.transform.gameObject);
                    GameObject go = hit.transform.gameObject;

                    // 강탈은 장착 카드를 훔치지 않기로 했다.
                    // 마우스 닿은 오브젝트 하이라이트 
                    if (go.CompareTag("Player"))
                    {
                        mat = hit.transform.GetComponentInChildren<SkinnedMeshRenderer>().material;
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", Color.red * 0.5f);
                        temp = go;
                    }
                    else if (temp != null)
                    {
                        mat = temp.transform.GetComponentInChildren<SkinnedMeshRenderer>().material;
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", Color.black);
                    }


                    if (go.CompareTag("Player") && bangClick)
                    {
                        // 타겟과의 거리가 1일 때만 선택 가능
                        Debug.Log("플레이어 선택 : " + go);

                        // 타겟과 내 사이 거리를 구함
                        int targetIdx = turnList.FindIndex(a => a == go);
                        targetDistance = MeasureDistance(targetIdx);
                        // 타겟의 야생마가 있으면 거리 +1
                        if (go.GetComponent<PlayerInfo>().isMustang)
                            targetDistance++;
                        // 내가 스코프를 가지고 있으면 거리 -1
                        if (playerInfo.isScope)
                            targetDistance--;

                        // 타겟과의 거리가 1
                        if (targetDistance <= 1)
                        {
                            playerInfo.usePanic = true;

                            // 클릭했으니까 하이라이트 꺼야함
                            mat = hit.transform.GetComponentInChildren<SkinnedMeshRenderer>().material;
                            mat.EnableKeyword("_EMISSION");
                            mat.SetColor("_EmissionColor", Color.black);

                            // 상대편 클릭하고 상대편 덱의 갯수를 가져옴
                            go.GetComponent<PhotonView>().RPC("PanicSync", RpcTarget.All, 0);

                            //gameloop2에서 cardNumSync하는거 기다림
                            yield return new WaitForSeconds(1.2f);

                            // 빈 카드를 덱 갯수 만큼 생성함 - 캣벌로우 함수 가져다 씀 
                            MineUI.Instance.CatbalouCards(go.GetComponent<PlayerInfo>().mycardNum);

                            // 카드 뽑기할때 방해되서 잠시 끔
                            delcardPanel.gameObject.SetActive(false);
                            MineUI.Instance.cardblockingPanel.SetActive(true);
                            while (playerInfo.usePanic)
                            {
                                // 선택 대기
                                Debug.Log("강탈 선택 대기");
                                yield return null;
                            }
                            // 다시 켬
                            delcardPanel.gameObject.SetActive(true);
                            MineUI.Instance.cardblockingPanel.SetActive(false);


                            // 기다림을 위해 패닉플래그 다시 켜줌
                            go.GetPhotonView().RPC("PanicSync", RpcTarget.All, 0);

                            // 강탈에 의해 타겟 카드 중 랜덤으로 하나 삭제 
                            go.GetPhotonView().RPC("PanicDel", RpcTarget.All, go.GetComponent<PlayerInfo>().mycardNum);

                            // 타겟 카드가 덱 제일 앞으로 가니까 카드 하나 뽑으면 강탈 완성

                            // 카드가 덱에 추가 되는 시간 기다림
                            while (go.GetComponent<PlayerInfo>().isPanic)
                            {
                                Debug.Log("카드 맨 앞 추가 기다림");
                                yield return null;
                            }

                            // 카드 뽑기
                            photonView.RPC("GiveCards", RpcTarget.AllViaServer, 1);

                            break;
                        }
                        else
                        {
                            // 타겟과 멀어서 강탈 불가
                            DistancePanelOnOff(0);
                            yield return new WaitForSeconds(1f);
                            DistancePanelOnOff(1);
                        }
                    }
                }
                yield return new WaitForEndOfFrame();
            }
            // 뱅에서 쓰던거 재활용
            bangClick = false;
            isBang = false;

            yield return new WaitForEndOfFrame();
        }

        IEnumerator Duel()
        {
            isBang = true;          // 뱅에서 쓰던거 재활용(필요한 기능이 같아서)
            Material mat;
            GameObject temp = null;

            while (true)
            {
                Debug.Log("결투 동작 중");
                // 빔 
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    //Debug.Log("마우스에 닿음 : " + hit.transform.gameObject);
                    GameObject go = hit.transform.gameObject;

                    // 마우스 닿은 오브젝트 하이라이트 
                    if (go.CompareTag("Player"))
                    {
                        mat = hit.transform.GetComponentInChildren<SkinnedMeshRenderer>().material;
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", Color.red * 0.5f);
                        temp = go;
                    }
                    else if (temp != null)
                    {
                        mat = temp.transform.GetComponentInChildren<SkinnedMeshRenderer>().material;
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", Color.black);
                    }

                    // 결투 상대 선택
                    if (go.CompareTag("Player") && bangClick)
                    {
                        Debug.Log("플레이어 선택 : " + go);
                        // 클릭했으니까 하이라이트 꺼야함
                        mat = hit.transform.GetComponentInChildren<SkinnedMeshRenderer>().material;
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", Color.black);

                        // GM의 duelTurn을 켜줌
                        photonView.RPC("DuelTurn", RpcTarget.All, 0);

                        // 본인과 상대편의 playerinfo의 isduel 켜줌
                        photonView.RPC("DuelSync", RpcTarget.All, 0, -1);
                        go.GetPhotonView().RPC("DuelSync", RpcTarget.All, 0, tidx);

                        // 내가 결투 카드를 내면 상대편부터 결투 턴이 시작 됨
                        // 턴을 넘겨버림
                        nextSignal = true;
                        break;
                    }
                }
                yield return new WaitForEndOfFrame();
            }
            // 뱅에서 쓰던거 재활용
            bangClick = false;
            isBang = false;

            yield return new WaitForEndOfFrame();
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
}