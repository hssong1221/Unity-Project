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

        // 게임 종료 조건(각 직업의 숫자 - 보안관은 죽으면 바로 무법자 승리)
        private int outlawNum;
        private int renegadeNum;
        private int viceNum;

        [Header("게임 승리 변수")]
        public bool isVitory = false;
        

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
        public Text pnumText; // 플레이어 앉은 숫자
        public int sitNum = 0; // 앉아 있는 숫자


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

            if (PlayerManager.LocalPlayerInstance == null)
            {
                StartCoroutine(InstantiateResource()); 

                cameraWork = GetComponent<CameraWork>(); // 본인 카메라 가져오기
            }
            StartCoroutine(WaitAllPlayers()); // 다른 플레이어 기다리기

        }

        private void Update()
        {
            // UI가 계속 정면을 보게 만듬
            startCanvas.transform.LookAt(startCanvas.transform.position + Camera.main.transform.forward);
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


            // ------------------------------------------------ 사람들이 텍스트를 읽을 시간 부여(나중에 다시 활성화) ----------------------------
            //yield return new WaitForSeconds(12f);
            abilPanel.SetActive(false);
            MineUI.Instance.leftTopPanel.SetActive(true);
            MineUI.Instance.rightBottomPanel.SetActive(true);
            MineUI.Instance.rightTop.SetActive(true);
        }

        // 게임 종료 조건 만족하는지 확인함 
        IEnumerator EndGame()
        {
            // 너무 빨리 측정하면 hp 동기화 전이라 전부 사망처리됨
            yield return new WaitForSeconds(5f);

            while (!isVitory)
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
                        isVitory = true;
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
                    isVitory = true;
                }

                // 무법자 배신자 모두 사망하면 보안관 승리
                if (outlawNum == 0 && renegadeNum == 0)
                {
                    Victory("sherrif");
                    isVitory = true;
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