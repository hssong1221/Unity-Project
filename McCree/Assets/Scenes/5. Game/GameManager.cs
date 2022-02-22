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
            //*****************꼼수로 수정한 부분(나중에 더 좋은 방법 찾으면 무조건 바꿔야함)*********************
            // 플레이어 정보 생길 때까지 잠시 대기  
            yield return new WaitForSeconds(2f);
            //*****************꼼수로 수정한 부분(나중에 더 좋은 방법 찾으면 무조건 바꿔야함)*********************

            Debug.Log("게임 ui 시작!!");

            // 플레이어가 생성되고 그 안에 있는 플레이어 매니저를 가져와야함, 그 안에 정보들이 있음
            GameObject[] temp;
            temp = GameObject.FindGameObjectsWithTag("Player");
            
            foreach (GameObject item in temp)
            {
                if (item.GetComponent<PhotonView>().IsMine)
                {
                    playerManager = item.GetComponent<PlayerManager>();
                    break;
                }
            }

            while (PhotonNetwork.PlayerList.Length != playerManager.players.Length)
            {
                Debug.Log("GM 방 총원 : " + PhotonNetwork.PlayerList.Length);
                Debug.Log("GM 현재 로딩된 인원 수 : " + playerManager.players.Length);
                yield return null;
            }

            jobPanel.SetActive(true);

            yield return new WaitForEndOfFrame();

            // 직업 선택 텍스트랑 애니메이션 재생
            jobText.text = JobText();
            yield return new WaitForSeconds(6f);

            jobPanel.SetActive(false);

            abilPanel.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            abilUIAnimator.SetTrigger("Abil");
            abilText.text += AblityText();
            abilText.text += "\n3. 당신의 능력을 잘 활용하십시오";
            

            yield return new WaitForSeconds(12f);

            abilPanel.SetActive(false);
        }

        // 직업 관련 텍스트
        public string JobText()
        {
            string temp = "";
            Debug.Log(playerManager.playerType);
            switch (playerManager.playerType)
            {
                // 플레이어 타입마다 다른 그림 부여 (애니메이션도 다 다르게 하려 햇는데 일단 하나로 함)
                case PlayerManager.jType.Sheriff:
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
                case PlayerManager.jType.Vice:
                    Debug.Log("당신은 부관입니다.");
                    jobUIAnimator.SetTrigger("Deputy");
                    temp = "DEPUTY\n 부  관";
                    
                    jobImage1.sprite = deputy1;
                    jobImage2.sprite = deputy2;
                    jobImage3.sprite = deputy3;

                    abilImage.sprite = deputy4;
                    abilText.text = "1. 보안관을 도와 무법자를 모두 사살하십시오.\n";
                    break;
                case PlayerManager.jType.Outlaw:
                    Debug.Log("당신은 무법자입니다.");
                    jobUIAnimator.SetTrigger("Outlaw");
                    temp = "OUTLAW\n무 법 자";

                    jobImage1.sprite = outlaw1;
                    jobImage2.sprite = outlaw2;
                    jobImage3.sprite = outlaw3;

                    abilImage.sprite = outlaw4;
                    abilText.text = "1. 무법자들과 함께 보안관을 사살하십시오.\n";
                    break;
                case PlayerManager.jType.Renegade:
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
                case PlayerManager.aType.BangMissed:
                    temp = "2. 뱅과 빗나감이 같은 능력이 됩니다.";
                    break;
                case PlayerManager.aType.DrinkBottle:
                    temp = "2. 당신옆에 항상 술통이 있습니다.";
                    break;
                case PlayerManager.aType.HumanVolcanic:
                    temp = "2. 뱅을 마구 쏠 수 있습니다.";
                    break;
                case PlayerManager.aType.OnehpOnecard:
                    temp = "2. 체력이 달았다면 카드를 얻습니다.";
                    break;
                case PlayerManager.aType.ThreeCard:
                    temp = "2. 카드를 뽑을 때 3장을 보고 2장을 가져옵니다.";
                    break;
                case PlayerManager.aType.TwocardOnecard:
                    temp = "2. 카드 펼치기를 할 때 2장을 뽑고 한장을 선택할 수 있습니다.";
                    break;
                case PlayerManager.aType.TwocardOnehp:
                    temp = "2. 카드 2장을 버리고 체력을 얻습니다.";
                    break;
            }

            return temp;
        }


        // 플레이어 생성
        public void SpawnPlayer()
        {
            Debug.Log("로컬플레이어를 생성합니다.");

            float ran1 = Random.Range(-5, 5);
            float ran2 = Random.Range(-5, 5);
            // 로컬 플레이어를 스폰합니다. 동기화도 됨
            PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(ran1, 2f, ran2), Quaternion.identity, 0);
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

    }
}