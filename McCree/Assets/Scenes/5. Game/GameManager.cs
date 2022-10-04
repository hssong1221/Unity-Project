using System;
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


        [Header("게임 종료 관련 UI")]
        public GameObject vicPanel;
        public GameObject backPlane;

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

        [Header("카드 개수")]
        [SerializeField]
        private int bang_c;
        [SerializeField]
        private int avoid_c;
        [SerializeField]
        private int beer_c;
        

        // 턴 관련 변수들
        [HideInInspector]
        public bool nextSignal = false; // 턴을 다음사람에게 넘기라는 변수
        [HideInInspector]   
        public int tidx;                // 현재 턴을 가지고 있는 사람의 turnList index
        [HideInInspector]
        public bool myTurn = false;    // 내 턴이면 true

        public GameObject usecardPanel; // 카드 사용 판정 패널

        // ----------------------------------- 카드 컨텐츠 구현 -------------------------------
        [HideInInspector]
        public bool isCard;             // 현재 선택한 카드가 있다는 의미
        // 뱅 상태인지 아닌지
        [HideInInspector]
        public bool isBang = false;
        // 뱅 상태에서 클릭을 했는지 안했는지
        [HideInInspector]
        public bool bangClick = false;


        // sendAvoid에 들어가는 파라미터
        // 0 : 공격자가 회피를 받았을 때
        // 1 : 공격자가 그냥 맞기를 받았을 때
        // 2 : 타겟이 0 or 1 행동 한 후에 자신의 상태를 변경할 때
        [HideInInspector]
        public int avoidFlag = 0;

        // 회피카드가 아닌 그냥 맞기 버튼의 상태
        [HideInInspector]
        public bool avoidBtnFlag = false;

        // avoid카드를 냈으면 안맞고 안 내면 맞고
        [HideInInspector]
        public bool willDamage = false;

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

            // 뱅 카드 사용 중
            if (isBang && Input.GetMouseButtonDown(0))
            {
                bangClick = true;
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

            //StartCoroutine(EndGame()); // 게임 종료 조건을 판단

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

        // --------------------------------카드 기능 부활 중------------------------------------
        IEnumerator GiveCardSet()
        {
            // 임시 카드셋
            //cardSet = gameObject.AddComponent<CardSet>();

            // 초기 카드 세팅
            Card.cType[] initialDeck = new Card.cType[
                bang_c + avoid_c + beer_c
            ];

            int k = 0;
            for (int i = 0; i < bang_c; i++, k++)
                initialDeck[k] = Card.cType.Bang;
            for (int i = 0; i < avoid_c; i++, k++)
                initialDeck[k] = Card.cType.Avoid;
            for (int i = 0; i < beer_c; i++, k++)
                initialDeck[k] = Card.cType.Beer;

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
            for (int i = 0; i < initialDeck.Length; i++)
            {
                Debug.Log("card: " + initialDeck[i]);
            }

            // rpc가 리스트를 넘길수 없으므로 json으로 바꿔서 보냄
            var json = JsonConvert.SerializeObject(initialDeck);

            player1.GetComponent<PhotonView>().RPC("GiveCardSet", RpcTarget.All, json);

            yield return new WaitForEndOfFrame();
        }

        IEnumerator JobandAbility()
        {
            yield return new WaitForEndOfFrame();

            // (인구수에 맞게 하는 거 추가하기)
            //List<int> jobList = new List<int>() { 1, 2, 3, 4, 5, 6, 7 }; ----- 실험을 위해 보안관 확률을 높였음-----
            List<int> jobList = new List<int>() { 1, 2 };


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
                    if (player.GetComponent<PlayerManager>().playerType == jType.Sheriff)
                    {
                        bangBtn.gameObject.SetActive(true);
                    }
                }
            }
        }

        
        IEnumerator GameLoop1() // 게임 시작후 덱 나눠주기 
        {
            // 카드 초기화를 위해 켜야함
            usecardPanel.SetActive(true);

            // 보안관을 시작으로 순서 정하기 
            // 보안관 앉은 위치찾기
            int sheriffIdx = 0;
            for (int i = 0; i < sitList.Length; i++)
            {
                if (sitList[i].name == "tempsit")
                    continue;

                if (sitList[i].GetComponent<PlayerManager>().playerType == jType.Sheriff)
                {
                    sheriffIdx = i;
                    //Debug.Log("she idx : " + sheriffIdx);
                    break;
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
                    turnList[i].GetComponent<PhotonView>().RPC("GiveCards", RpcTarget.AllViaServer, 5, transform.position);
                }
                yield return new WaitForEndOfFrame();
            }

            // 시점을 1인칭으로 바꿈
            cam.ChildCameras[2].gameObject.SetActive(true);
            cam.ChildCameras[1].gameObject.SetActive(false);

            StartCoroutine("GameLoop2");
            yield return new WaitForSeconds(1f);
        }

        IEnumerator GameLoop2() // 턴이 돌아가는 곳
        { 
            tidx = 0;
            nextSignal = false;

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
                }
                // 본인턴이 아니라면 반복문 통과 못하고 대기중
                else
                {
                    Debug.Log("자기 턴 아님");

                    // 뱅 카드에 의해 타겟팅이 되었을 때
                    if(playerInfo.isTarget == 1)
                    {
                        myTurn = true;
                        Debug.Log("뱅 맞는 중 : " + playerInfo.isTarget);
                        StartCoroutine("Avoid");
                        playerInfo.isTarget = 2;
                    }

                    yield return new WaitForSeconds(0.1f);
                    continue;
                }

                yield return new WaitForEndOfFrame();

                // 본인턴에 카드 2장 뽑으면서 시작
                turnList[tidx].GetComponent<PhotonView>().RPC("GiveCards", RpcTarget.AllViaServer, 1, transform.position);

                while (true)
                {
                    if (nextSignal)
                    {
                        Debug.Log("턴 넘김");
                        turnList[tidx].GetComponent<PhotonView>().RPC("TurnIndexPlus", RpcTarget.All);
                        nextSignal = false;
                        myTurn = false;
                        break;
                    }
                    // -------------------------------------------  본인 턴 때 실행할 것들 여기에 쓰면 됨-------------------------------------------------------

                    Debug.Log("턴 소요 중~~~~~~~~");
                    yield return new WaitForSeconds(0.1f);
                    // ------------------------------------------------------------------------------------------------------------------------------------------
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

            while (!isVictory)
            {
                outlawNum = 0;
                renegadeNum = 0;
                viceNum = 0;

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
            }
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

            SpawnWinner();

            switch (winner)
            {
                case "sherrif":
                    Debug.Log("보안관과 부관 승리!");
                    break;
                case "outlaw":
                    Debug.Log("무법자 승리!");
                    break;
                case "renegade":
                    Debug.Log("배신자 승리");
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

        // --------------------------- 앉아있는 인원 체크 --------------------------------
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

        // --------------------------- 앉아있는 인원 체크 --------------------------------

        public void NextBtnClick()
        {
            //Debug.Log("턴 버튼 누름!");
            nextSignal = true;
            MineUI.Instance.NextButton.SetActive(false);
        }

        public void BangBtnClick()
        {
            foreach (GameObject player in playerList)
            {
                // 모든 사람의 turnlist에 정보 저장을 위함
                player.GetComponent<PhotonView>().RPC("StartUIOff", RpcTarget.All);
                // 모든 사람의 gameloop 진입을 위함
                player.GetComponent<PhotonView>().RPC("Gameloop", RpcTarget.All);
            }
        }

        public void GLStart()
        {
            StartCoroutine("GameLoop1");
        }

        // 본인 카드덱 찾게 도와줌
        public GameObject CallMyPlayer()
        {
            return player1;
        }


        // 본인이 뱅 타겟되었다는 UI on
        public void TargetedPanelOn()
        {
            //if(player1.GetComponent<PlayerInfo>().isTarget == true)
            if (playerInfo.isTarget == 1)
            {
                MineUI.Instance.targetedPanel.SetActive(true);
            }
        }

        // 카드 사용한 거 다시 카드 셋으로 넣는 기능
        public void AfterCardUse(Card.cType content)
        {
            Debug.Log("use card content : " + content);


            // 카드 종류에 따라 실행이 달라진다
            string t = content.ToString();
            switch (t)
            {
                case "Bang":
                    StartCoroutine("Bang");
                    break;
                case "Avoid":
                    playerInfo.sendAvoid = true;
                    break;
                case "Beer":
                    StartCoroutine("Beer");
                    break;
                default:
                    break;
            }
            
            // 카드더미 동기화 시켜주기 - DataSync로
            player1.GetComponent<PhotonView>().RPC("CardDeckSync", RpcTarget.All, content);
        }   


        //----------------------------- 카드 기능 구현 중 --------------------------------
        

        IEnumerator Bang()
        {
            isBang = true;
            Material mat;
            GameObject temp = null;

            while (true)
            {
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
                    else if(temp != null)
                    {
                        mat = temp.transform.GetComponentInChildren<SkinnedMeshRenderer>().material;
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", Color.black);
                    }

                    if (go.CompareTag("Player") && bangClick)
                    {
                        Debug.Log("플레이어 선택 : " + go);

                        // 맞았으니까 하이라이트 꺼야함
                        mat = hit.transform.GetComponentInChildren<SkinnedMeshRenderer>().material;
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", Color.black);

                        // 타겟 선언 및 상대편 화면에 UI 띄움
                        go.GetComponent<PhotonView>().RPC("BangTargeted", RpcTarget.All);

                        // 나의 waitAvoid 상태를 전체에게 동기화
                        playerInfo.waitAvoid = true;
                        photonView.RPC("WaitAvoid", RpcTarget.All);

                        // 상대방의 avoid 기다림
                        while (playerInfo.waitAvoid == true)
                        {
                            Debug.Log("상대방의 회피를 기다리는 중 ");
                            
                            yield return new WaitForSeconds(0.1f);
                        }

                        Debug.Log("상대방의 회피를 기다리는 상태를 빠져나옴 ");

                        // avoid 하면 그냥 넘어가고 못하면 hp 달게함
                        if (willDamage == false)
                            break;
                        else if(willDamage == true)
                        {
                            go.GetComponent<PlayerInfo>().hp--;
                            int targethp = go.GetComponent<PlayerInfo>().hp;
                            go.GetComponent<PhotonView>().RPC("SyncHp", RpcTarget.All, targethp);
                            break;
                        }
                    }
                }
                yield return new WaitForEndOfFrame();
            }
             
            // 뱅 관련 플래그 초기화
            bangClick = false;
            isBang = false;
            yield return new WaitForEndOfFrame();
        }

        IEnumerator Avoid()
        {
            avoidFlag = 0;
            

            // 회피카드를 낼 건지 그냥 맞을 건지 선택 대기
            while (playerInfo.sendAvoid == false)
            {
                Debug.Log("회피 카드 내기 대기중");

                // 그냥 맞기를 선택함
                if (avoidBtnFlag == true)
                {
                    avoidFlag = 1;
                    break;
                }

                yield return new WaitForSeconds(0.1f);
            }

            if(avoidFlag == 0)
                Debug.Log("회피 카드 내서 빠져나옴");
            else
                Debug.Log("맞고 빠져나옴");

            foreach (GameObject player in turnList)
            {
                // 공격자의 회피 대기 상태를 바꿔야함
                if (player.GetComponent<PlayerInfo>().waitAvoid == true)
                {
                    Debug.Log("상대편 waitavoid 상태 변경중");
                    player.GetComponent<PhotonView>().RPC("SendAvoid", RpcTarget.All, avoidFlag);
                }
                    
            }

            myTurn = false;
            // 타겟 상태에서 벗어남
            playerInfo.isTarget = 0;
            photonView.RPC("SendAvoid", RpcTarget.All, 2);

            // 타겟 화면 UI 끔
            MineUI.Instance.targetedPanel.SetActive(false);

            // 회피관련 플래그 초기화
            avoidBtnFlag = false;
            playerInfo.sendAvoid = false;
            yield return new WaitForEndOfFrame();
        }

        public void DamageBtn()
        {
            playerInfo.sendAvoid = false;
            avoidBtnFlag = true;
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