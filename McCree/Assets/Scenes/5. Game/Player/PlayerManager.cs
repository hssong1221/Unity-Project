using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using Newtonsoft.Json;

namespace com.ThreeCS.McCree
{
    public class PlayerManager : Controller
    {
        #region Variable Fields

        // 화면 상에 보이는 로컬 플레이어
        public static GameObject LocalPlayerInstance;

       
        [SerializeField]
        private bool _canBehave;

        public bool isDeath;

        public bool isSit = false; // 의자에 앉는 동작 관련

        // 동작관련 플래그 true여야 동작 중 이동이 불가능함
        public bool canBehave;

        [Header("본인이 앉은 의자랑 게임 판")]
        public GameObject mygamePlate;
        public GameObject myChair;

        private bool isInventoryOpen;

        // 플레이어 정보 타입
        [Header("플레이어 정보")]
        public GameManager.jType playerType;
        public GameManager.aType abilityType;

        [Header("이동 관련")]
        [HideInInspector]
        public float rotateSpeedMovement = 0.1f;  // 캐릭터 회전하는 속도
        [HideInInspector]
        public float rotateVelocity;

        [Header("카메라 오프셋")]
        protected Vector3 offset;

        [SerializeField]
        protected Transform _objectTransPos;

        public Transform objectTransPos
        {
            get { return _objectTransPos; }
        }

        public GameObject bulletAttackedPos;

        protected bool isCharacterPlayer;
        public float maxAttackDistance;

        private IEnumerator coroutine;

        // 대충 길찾기 Ai
        private NavMeshAgent _agent;

        public NavMeshAgent agent
        {
            get { return _agent; }
        }

        // float  agent.speed 최대 이동 속도
        // float  agent.angularSpeed 최대 회전 속도
        // int    agent.acceleration 최대 가속
        // float  agent.stoppingDistance 목표 위치 가까워졌을시 정지
        // bool   agent.Auto Braking 목적지에 다다를때 속도를 줄이는지


        // 캐릭터 키보드 움직임 구현
        public float h;
        public float v;

        Vector3 moveVec;
        Vector3 moveDir;
        Vector3 lookForward;
        Vector3 lookRight;

        float moveSpeed = 5f; // 캐릭터 이동 속도

        [Header("카드 프리팹")]
        public Card cardObject;

        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
            base.Awake();
            _agent = gameObject.GetComponent<NavMeshAgent>();

            MineUI.Instance.inventoryBtn.onClick.AddListener(Inventory); // 아이템 이미지
            MineUI.Instance.mainQuestBtn.onClick.AddListener(Inventory); // 메인 목표 

            

            // 포톤뷰에 의한 내 플레이어만
            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject; // gameObject는 이 컴포넌트가 붙어있는 게임오브젝트 즉 플레이어를 의미

                // 카메라 x 45도, offset
                /*Camera.main.transform.rotation = Quaternion.Euler(45, 0, 0);
                offset = new Vector3(0.0f, 5.0f, -5f);*/
            }
            DontDestroyOnLoad(gameObject);

        }

        void Start()
        {
            photonView.RPC("PlayerListSync", RpcTarget.All); // 플레이어 리스트 동기화

            canBehave = true;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            //SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void Update()
        {
            //if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            //{
            //    return;
            //}
            if (photonView.IsMine) // 자기자신 캐릭터만 제어해야함
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {   // 인벤토리 
                    Inventory();
                }
            }
        }

        void FixedUpdate() // move
        {
            if (photonView.IsMine && !isSit)
            {
                h = Input.GetAxis("Horizontal");
                v = Input.GetAxis("Vertical");

                // 모바일 조이패드 (포톤뷰 때문에 조이컨트롤을 여기에 선언하면 이벤트 트리거로 내부에 값이 안 넣어진다.)
                if (GameManager.Instance.isControl)
                {
                    if (GameManager.Instance.joyControl[0]) { h = -1f; v = 1f; }
                    if (GameManager.Instance.joyControl[1]) { h = 0; v = 1f; }
                    if (GameManager.Instance.joyControl[2]) { h = 1f; v = 1f; }
                    if (GameManager.Instance.joyControl[3]) { h = -1f; v = 0; }
                    if (GameManager.Instance.joyControl[4]) { h = 0; v = 0; }
                    if (GameManager.Instance.joyControl[5]) { h = 1f; v = 0; }
                    if (GameManager.Instance.joyControl[6]) { h = -1f; v = -1f; }
                    if (GameManager.Instance.joyControl[7]) { h = 0; v = -1f; }
                    if (GameManager.Instance.joyControl[8]) { h = 1f; v = -1f; }
                }
                else
                {
                    for (int i = 0; i < 9; i++){
                        GameManager.Instance.joyControl[i] = false;
                    }
                }
                
            

                if (!canBehave || PunChat.Instance.usingInput) // 일단 임시로 inputfield사용중일때 캐릭터 움직임 금지
                {   // 플레이어가 동작중일때 0 0 넣어줘서 못 움직이게 만듬
                    h = 0;
                    v = 0;
                }

                moveVec = new Vector3(h, 0f, v);

                lookForward = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z).normalized;
                lookRight = new Vector3(Camera.main.transform.right.x, 0f, Camera.main.transform.right.z).normalized;

                moveDir = lookForward * moveVec.z + lookRight * moveVec.x;

                if (moveDir.magnitude >= 0.001) // 이동 후 플레이어 회전 방지
                {
                    agent.SetDestination(transform.position);
                    transform.rotation = Quaternion.LookRotation(moveDir);
                }

                // Rigidbody의 MovePosition 메소드로 캐릭터 이동
                // transfrom.position을 사용해도 되지만 얇은 벽등을 통과할 문제등이 생길 수 있다.
                // 객체의 충돌을 유지하면서 이동하기 위해 MovePosition을 사용 했다.
                rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * moveDir);

                animator.SetFloat("Speed", moveDir.magnitude); 
            }
        }
        #endregion

        #region  Methods
        // 의자에 앉고 일어나기
        public void Sit(Transform other, MeshRenderer mr, string chairName)
        {
            // 앉은 플레이어 상태 동기화
            if(isSit == false)
            {
                photonView.RPC("SitnumSync", RpcTarget.All);
                photonView.RPC("ChairSync", RpcTarget.All, chairName);
            }

            isSit = true;
            //Debug.Log("sit이 작동됨");
            animator.SetBool("isSit", true);
            // 네비게이션 메쉬가 꺼져있어야 순간이동이 가능. 그리고 덜덜 떨림 방지
            agent.enabled = false;
            

            // 의자에 앉으면 의자 하이라이트 꺼짐
            Material mat = mr.material;
            mat.SetColor("_EmissionColor", Color.black);

            
            // 의자에 앉는 모습으로 순간이동
            transform.position = other.position;
            transform.rotation = other.rotation;
            // 앉는 위치 보정
            transform.position += new Vector3(0, 0.1f, 0);
        }
        public void StandUp(Transform other)
        {
            // 플레이어 상태 동기화
            if(isSit == true)
            {
                photonView.RPC("StandnumSync", RpcTarget.All);
            }

            // 네비메쉬 다시 켜주기
            agent.enabled = true;
            animator.SetBool("isSit", false);
            isSit = false;
        }

        void Inventory()
        {
            if (!isInventoryOpen)
            {
                //MineUI.Instance.title_Item.text = "";
                //MineUI.Instance.explain_Item.text = "";
                MineUI.Instance.statusPanel.SetActive(true);
                isInventoryOpen = true;
            }
            else
            {
                MineUI.Instance.statusPanel.SetActive(false);
                isInventoryOpen = false;
            }
        }   // 안씀   
        #endregion

        #region Pun RPC

        [PunRPC]
        public void PlayerListSync()
        {
            //플레이어 오브젝트가 전부 담김
            GameManager.Instance.playerList = GameObject.FindGameObjectsWithTag("Player");
            Debug.Log("플레이어 수(만큼 나올 듯) : " + GameManager.Instance.playerList.Length);
            GameStartManager.setPlayerNumAction();
        }

        [PunRPC]
        public void SitnumSync()
        {
            isSit = true;
        }

        [PunRPC]
        public void StandnumSync()
        {
            isSit = false;
        }

        // 본인이 앉은 의자 동기화
        [PunRPC]
        public void ChairSync(string cname)
        {
            // 의자 태그 찾아서 본인거 찾기
            GameObject[] chair = GameObject.FindGameObjectsWithTag("chair");
            foreach (GameObject c in chair)
            {
                if (c.name.Equals(cname))
                {
                    myChair = c;
                    mygamePlate = c.GetComponent<ChairManager>().gamePlate;
                    mygamePlate.gameObject.SetActive(true);
                }
            }
        }

        [PunRPC]
        public void GiveCardSet(string jsonData, int state) // 카드덱을 나눠주기(카드 순서 동기화)
        {
            if(state == 0)
            {
                //GameManager.Instance.cardSet = gameObject.AddComponent<CardSet>();

                // json으로 넘어온거 다시 풀어놓기
                Card.cType[] initialDeck = JsonConvert.DeserializeObject<Card.cType[]>(jsonData);

                // 모든 사람 카드리스트에 카드 섞은거 넣기
                for (int i = 0; i < initialDeck.Length; i++)
                {
                    Card card = new Card();
                    card.cardContent = initialDeck[i];

                    GameManager.Instance.cardList.Add(card);
                }
            }
            else
            {
                // json으로 넘어온거 다시 풀어놓기
                Card.cType[] deck = JsonConvert.DeserializeObject<Card.cType[]>(jsonData);

                // 중간 셔플이므로 원래 있던거 삭제 후 추가
                GameManager.Instance.cardList.Clear();
                Debug.Log("셔플 끝");
                for (int i = 0; i < deck.Length; i++)
                {
                    Card card = new Card();
                    card.cardContent = deck[i];
                    Debug.Log(deck[i]);

                    GameManager.Instance.cardList.Add(card);
                }
            }
            
        }

        [PunRPC]
        public void GiveCards(int num) // 카드 나눠주기(카드 덱을 동기화)
        {
            Debug.Log("num: " + num);
            for (int i = 0; i < num; i++)
            {
                // 리스트 맨 앞에서 뽑은 카드
                Card drawCard = GameManager.Instance.cardList[0];

                // 카드 타입과 그림 매칭
                cardObject.cardContent = drawCard.cardContent;
                cardObject.matchImg();
                cardObject.storeIdx = 1000;

                // 카드를 뽑은게 본인이면 UI에 카드를 생성하고 본인 playerinfo에 카드 리스트에 저장
                if (photonView.IsMine) // 내 개인 UI에 내껏만 추가 
                {
                    var card = Instantiate(cardObject, MineUI.Instance.pos_CardSpwan.position, Quaternion.identity, MineUI.Instance.pos_CardParent);
                    var card2 = card.GetComponent<Card>();
                    playerInfo.mycards.Add(card2);

                    MineUI.Instance.CardAlignment();
                }
                // 남의 카드는 보이지 않고 정보만 playerinfo에 저장해놓음
                /*else
                {
                    playerInfo.mycards.Add(cardObject); 
                }*/

                // 덱에서 뽑힌 카드는 사라짐
                GameManager.Instance.cardList.RemoveAt(0);
            }

        }

        #endregion
    }

}

