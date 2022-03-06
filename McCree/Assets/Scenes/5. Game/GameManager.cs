using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;
using Newtonsoft.Json;

namespace com.ThreeCS.McCree
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Variable Fields

        // 게임 매니저 어디서든 사용가능?
        static public GameManager Instance;

        // 내 카메라
        private CameraWork cameraWork;

        // 플레이어 리스트
        public GameObject[] playerList;

        // 플레이어 프리팹
        [SerializeField]
        private GameObject playerPrefab;

        // 자기 자신(자기 포톤 뷰) 찾아서 Player관련 저장
        private PhotonView photonView;
        private PlayerManager playerManager;
        private PlayerInfo playerInfo;

        // 마스터만 사용하는 (전체) 카드 셋 
        private CardSet cardSet; // 마스터만 줄꺼임

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
        public GameObject[] maps;

        #endregion


        #region MonoBehaviour CallBacks

        void Awake()
        {
            // 어디서든 쓸 수 있게 인스턴스화
            Instance = this;

            jobUIAnimator = jobPanel.GetComponent<Animator>();
            abilUIAnimator = abilPanel.GetComponent<Animator>();

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

            if (playerPrefab == null)
            {
                Debug.Log("플레이어 프리팹이 생성되지 못했습니다. 게임매니저에서 확인하세요.");
            }
            else
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    Map();

                    StartCoroutine(SpawnPlayer()); // PhotonNetwork.Instantiate

                    cameraWork = GetComponent<CameraWork>(); // 본인 카메라 가져오기
                }
            }
            StartCoroutine(WaitAllPlayers()); // 다른 플레이어 기다리기
        }

        #endregion

        #region Coroutine

        IEnumerator SpawnPlayer()
        {
            Debug.Log("로컬플레이어를 생성합니다.");

            float ran1 = Random.Range(-2, 2);
            float ran2 = Random.Range(-2, 2);
            // 로컬 플레이어를 스폰합니다. 동기화도 됨
            PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(ran1, 2f, ran2), Quaternion.identity, 0);

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
                
                // 임시 카드셋
                cardSet = gameObject.AddComponent<CardSet>();

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
            Debug.Log("잡리스트" + jobList[0] + " " + jobList[1] + " " + jobList[2]);
            abilityList = CommonFunction.ShuffleList(abilityList);
            Debug.Log("어빌리스트" + abilityList[0] + " " + abilityList[1] + " " + abilityList[2]);


            // 직업을 나눠주고 동기화 시킴
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Debug.Log("잡 리스트 번호 : " + jobList[i]);
                playerList[i].GetComponent<PhotonView>().RPC("JobSelect", RpcTarget.All, jobList[i]);
            }

            // 능력을 나눠주고 동기화 시킴
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Debug.Log("잡 리스트 번호 : " + abilityList[i]);
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

            // 사람들이 텍스트를 읽을 시간 부여
            //yield return new WaitForSeconds(12f);
            abilPanel.SetActive(false);
        }

        IEnumerator Cards()
        {
            cardSet = gameObject.AddComponent<CardSet>(); // 마스터 클라이언트만 카드 셋 정보 가지고있는다

            yield return new WaitForEndOfFrame(); // 기다리지않으면 cardSet의 Start가 돌아가지않는다

            //for (int i = 0; i < cardSet.cardList.Count; i++) // 전체 카드 보기
            //    Debug.Log(i+"번째: " + cardSet.cardList[i].ability.ToString());


            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                // 해당 플레이어의 PhotonView
                PhotonView player = playerList[i].GetComponent<PhotonView>();

                // 해당플레이어의 최대 체력
                int MHp = player.GetComponent<PlayerInfo>().maxHp;

                // 해당플레이어가 가져갈 카드 이름
                Card.cType[] startCards = new Card.cType[MHp];

                // 해당 플레이어의 체력 수 만큼 카드 뽑음
                for (int j = 0; j < MHp; j++)
                {
                    startCards[j] = cardSet.cardList[0].ability;
                    cardSet.cardList.RemoveAt(0);
                }

                //Debug.Log("뽑은것:");
                //for (int k = 0; k < startCards.Length; k++)
                //{
                //    Debug.Log(startCards[k]);
                //}

                // photon sibal string int array 기본적인 내용밖에 전송불가능하지만
                // json으로 직렬화 시키면 다른 타입도 photon으로 전송가능 
                var json = JsonConvert.SerializeObject(startCards);

                playerList[i].GetComponent<PhotonView>().RPC("GiveCards", RpcTarget.All, json);
            }

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
                    abilText.text = "1. 부관을 찾고 무법자를 모두 사살하십시오.\n";
                    break;
                case jType.Vice:
                    Debug.Log("당신은 부관입니다.");
                    jobUIAnimator.SetTrigger("Deputy");
                    temp = "DEPUTY\n 부  관";

                    jobImage1.sprite = deputy1;
                    jobImage2.sprite = deputy2;
                    jobImage3.sprite = deputy3;

                    abilImage.sprite = deputy4;
                    abilText.text = "1. 보안관을 도와 무법자를 모두 사살하십시오.\n";
                    break;
                case jType.Outlaw:
                    Debug.Log("당신은 무법자입니다.");
                    jobUIAnimator.SetTrigger("Outlaw");
                    temp = "OUTLAW\n무 법 자";

                    jobImage1.sprite = outlaw1;
                    jobImage2.sprite = outlaw2;
                    jobImage3.sprite = outlaw3;

                    abilImage.sprite = outlaw4;
                    abilText.text = "1. 무법자들과 함께 보안관을 사살하십시오.\n";
                    break;
                case jType.Renegade:
                    Debug.Log("당신은 배신자입니다.");
                    jobUIAnimator.SetTrigger("Renegade");
                    temp = "RENEGADE\n배 신 자";

                    jobImage1.sprite = renegade1;
                    jobImage2.sprite = renegade2;
                    jobImage3.sprite = renegade3;

                    abilImage.sprite = renegade4;
                    abilText.text = "1. 보안관에겐 부관처럼 무법자에겐 친구처럼 보이십시오. 하지만 마지막에 남는 사람은 당신 혼자여야 합니다.\n";
                    break;
            }

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
                    temp = "2. 뱅과 빗나감이 같은 능력이 됩니다.";
                    break;
                case aType.DrinkBottle:
                    temp = "2. 당신옆에 항상 술통이 있습니다.";
                    break;
                case aType.HumanVolcanic:
                    temp = "2. 뱅을 마구 쏠 수 있습니다.";
                    break;
                case aType.OnehpOnecard:
                    temp = "2. 체력이 달았다면 카드를 얻습니다.";
                    break;
                case aType.ThreeCard:
                    temp = "2. 카드를 뽑을 때 3장을 보고 2장을 가져옵니다.";
                    break;
                case aType.TwocardOnecard:
                    temp = "2. 카드 펼치기를 할 때 2장을 뽑고 한장을 선택할 수 있습니다.";
                    break;
                case aType.TwocardOnehp:
                    temp = "2. 카드 2장을 버리고 체력을 얻습니다.";
                    break;
            }
            return temp;
        }

        // 방 나가기 임시 구현
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        /*void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("마스터클라이언트가 아니면 씬, 레벨을 로드할수 없습니다.");
            }

            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);

            PhotonNetwork.LoadLevel("Game");
        }*/

        public CardSet Get_CardSet()
        {
            return (cardSet);
        }

        // 맵 생성 임시 구현
        public void Map()
        {
            for(int i = 0; i < 8; i++)
            {
                GameObject temp = Instantiate(maps[0], points[i]);

            }
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