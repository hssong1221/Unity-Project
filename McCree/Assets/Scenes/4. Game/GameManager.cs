using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;


namespace com.ThreeCS.McCree
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields
        // 어디서든 사용 가능
        static public GameManager Instance;

        public PlayerManager playerManager;

        [Header("직업 관련 UI")]
        public GameObject jobPanel;
        public RectTransform jobBoard;
        public Text jobText;
        public Image jobImage;
        private Animator jobUIAnimator;



        [Header("고유 능력 관련 UI")]
        public GameObject abilPanel;
        public RectTransform abilBoard;
        public Text abilText;
        public float uiSpeed;  // UI 넘어가는 속도

        [Header("직업 일러스트")]
        public Sprite sheriff;  // 보안관 일러스트
        public Sprite aide;     // 부관   일러스트
        public Sprite outlaw;   // 무법자 일러스트
        public Sprite traitor;  // 배신자 일러스트
        

        #endregion


        #region Private Fields

        // 플레이어 프리팹
        [SerializeField]
        private GameObject playerPrefab;

        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
            // 어디서든 쓸 수 있게 인스턴스화
            Instance = this;

            jobUIAnimator = jobPanel.GetComponent<Animator>();

            // 접속 못하면 초기화면으로 쫓아냄
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene("Launcher");
                return;
            }
        }

        void Start()
        {
            if (playerPrefab == null)
            {
                Debug.Log("플레이어 프리팹이 생성되지 못했습니다. 게임매니저에서 확인하세요.");
            }
            else
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    // 플레이어 스폰 구현(추후에 스폰 장소를 따로 구현할 예정)
                    SpawnPlayer();

                    // 게임을 시작하면 나오는 UI
                    StartCoroutine(GameStart());
                }
            }
        }

        void Update()
        {

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

        #region Public Methods

        IEnumerator GameStart()
        {
            // 플레이어가 생성되고 그 안에 있는 플레이어 매니저를 가져와야함, 그 안에 정보들이 있음
            playerManager = GameObject.FindWithTag("Player").GetComponent<PlayerManager>();
            //ui = GameObject.FindWithTag("Player").GetComponent<UI>();

            Debug.Log(playerManager);
            //Debug.Log(ui);

            yield return new WaitForEndOfFrame();

            jobText.text = JobText();
            jobUIAnimator.SetTrigger("Start");

            //float t = 0;
            //while (t < uiSpeed)
            //{
            //    t += 1 * Time.deltaTime;
            //    jobBoard.anchoredPosition = Vector3.Lerp(Vector3.up * 1000, Vector3.zero, t);
            //    Debug.Log("--------직업 텍스트 출력--------");

            //    // 플레이어 타입에 맞는 대사 출력


            //    yield return null;
            //}

            //// 직업 설명이 내려오고 나서 읽을 시간을 줘야함
            //yield return new WaitForSeconds(5f);


            //t = 0;
            //while (t < uiSpeed)
            //{
            //    t += 1 * Time.deltaTime;
            //    jobBoard.anchoredPosition = Vector3.Lerp(Vector3.zero, Vector3.down * 1000, t);
            //    yield return null;
            //}

            //yield return new WaitForSeconds(1f);

            ////-------------------------------------------------------------------

            //t = 0;
            //while (t < uiSpeed)
            //{
            //    t += 1 * Time.deltaTime;
            //    abilBoard.anchoredPosition = Vector3.Lerp(Vector3.up * 1000, Vector3.zero, t);
            //    Debug.Log("--------능력 텍스트 출력--------");

            //    // 플레이어 타입에 맞는 대사 출력
            //    abilText.text = AblityText();

            //    yield return null;
            //}

            //// 능력 설명이 내려오고 나서 읽을 시간을 줘야함
            //yield return new WaitForSeconds(5f);

            //t = 0;
            //while (t < uiSpeed)
            //{
            //    t += 1 * Time.deltaTime;
            //    abilBoard.anchoredPosition = Vector3.Lerp(Vector3.zero, Vector3.down * 1000, t);
            //    yield return null;
            //}


            //yield return new WaitForSeconds(1f);

            //jobPanel.SetActive(false);
            //abilPanel.SetActive(false);


        }

        // 직업 관련 텍스트
        public string JobText()
        {
            string temp = "";
            Debug.Log(playerManager.playerType);
            switch (playerManager.playerType)
            {
                case PlayerManager.jType.Sheriff:
                    Debug.Log("당신은 보안관입니다.");
                    temp = "보안관 입니다. 부관을 찾고 무법자를 전부 제거하십시오.";
                    jobImage.sprite = sheriff;
                    break;
                case PlayerManager.jType.Vice:
                    Debug.Log("당신은 부관입니다.");
                    temp = "부관 입니다. 보안관을 도와 무법자를 전부 제거하십시오.";
                    jobImage.sprite = aide;
                    break;
                case PlayerManager.jType.Outlaw:
                    Debug.Log("당신은 무법자입니다.");
                    temp = "무법자 입니다. 다른 무법자와 함께 보안관을 살해하십시오.";
                    jobImage.sprite = outlaw;
                    break;
                case PlayerManager.jType.Renegade:
                    Debug.Log("당신은 배신자입니다.");
                    temp = "배신자 입니다. 당신은 보안관에겐 부관처럼 무법자에겐 친구처럼 보이십시오. 하지만 마지막에 살아남는건 당신 혼자이어야합니다.";
                    jobImage.sprite = traitor;
                    break;
            }

            return temp;
        }

        // 능력 관련 텍스트
        public string AblityText()
        {
            string temp = "";
            Debug.Log(playerManager.abilityType);
            switch (playerManager.abilityType)
            {
                case PlayerManager.aType.BangMissed:
                    temp = "뱅과 빗나감이 같은 능력이 됩니다.";
                    break;
                case PlayerManager.aType.DrinkBottle:
                    temp = "당신옆에 항상 술통이 있습니다.";
                    break;
                case PlayerManager.aType.HumanVolcanic:
                    temp = "뱅을 마구 쏠 수 있습니다.";
                    break;
                case PlayerManager.aType.OnehpOnecard:
                    temp = "체력이 달았다면 카드를 얻습니다.";
                    break;
                case PlayerManager.aType.ThreeCard:
                    temp = "카드를 뽑을 때 3장을 보고 2장을 가져옵니다.";
                    break;
                case PlayerManager.aType.TwocardOnecard:
                    temp = "카드 펼치기를 할 때 2장을 뽑고 한장을 선택할 수 있습니다.";
                    break;
                case PlayerManager.aType.TwocardOnehp:
                    temp = "카드 2장을 버리고 체력을 얻습니다.";
                    break;
            }

            return temp;
        }


        // 플레이어 생성
        public void SpawnPlayer()
        {
            Debug.Log("로컬플레이어를 생성합니다.");
            int ran1 = Random.Range(-10, 10);
            int ran2 = Random.Range(-10, 10);
            // 로컬 플레이어를 스폰합니다. 동기화도 됨
            PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(ran1, 5f, ran2), Quaternion.identity, 0);
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

        #endregion

        #region

        

        #endregion


    }
}