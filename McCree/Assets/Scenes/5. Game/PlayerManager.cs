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

        public bool isAiming;
        public bool isDeath;

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


        
        public Card cardObject;
        public Transform pos;

        protected bool isCharacterPlayer;
        public float maxAttackDistance;
        // 기본상태                                        offset Vector3(0.0f, 5.0f, -5.0f)

        // attackRange가 1일때는  maxAttackDistance  5     offset Vector3(0.0f, 7.0f, -7.0f)
        // indicatorRangeCircle.rectTransform.localScale  Vector3(1, 1, 0)

        // attackRange가 2일때는  maxAttackDistance  10    offset Vector3(0.0f, 14.0f, -14.0f)
        // indicatorRangeCircle.rectTransform.localScale  Vector3(2, 2, 0)

        // attackRange가 3일때는  maxAttackDistance  15    offset Vector3(0.0f, 21.0f, -21.0f)
        // indicatorRangeCircle.rectTransform.localScale  Vector3(3, 3, 0)


        // attackRange는 총 사거리 
        // maxAttackDistance는 Range_Indicator img 의 반지름의 길이 (정확하지 않고 추측해서 값대입)
        // indicatorRangeCircle.rectTransform.localScale는 Range_Indicator img의 크기



        // 에이전트는 내비메시를 이용하여 게임 월드에 대해 추론하고
        // 서로 또는 기타 움직이는 장애물을 피할 방법을 이해하고 있습니다
        // 대충 길찾기 Ai
        [HideInInspector]
        public NavMeshAgent agent;
        // float  agent.speed 최대 이동 속도
        // float  agent.angularSpeed 최대 회전 속도
        // int    agent.acceleration 최대 가속
        // float  agent.stoppingDistance 목표 위치 가까워졌을시 정지
        // bool   agent.Auto Braking 목적지에 다다를때 속도를 줄이는지

        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
            base.Awake();
            agent = gameObject.GetComponent<NavMeshAgent>();
            pos = GameObject.FindWithTag("CardsPos").transform;

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
        }

        [PunRPC]
        public void PlayerListSync()
        {
            //플레이어 오브젝트가 전부 담김
            GameManager.Instance.playerList = GameObject.FindGameObjectsWithTag("Player");
            Debug.Log("플레이어 수(많큼 나올 듯) : " + GameManager.Instance.playerList.Length);
        }


        public override void OnDisable()
        {
            base.OnDisable();
            //SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void Update()
        {
            
            // 게임 내에서 밖으로 나오는거 임시 구현
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.Instance.LeaveRoom();
            }

            if(photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }

            if (photonView.IsMine)
            {
                AttackRange(); // 뱅 준비
                if (isAiming)
                    Bang();
            }

            //Camera.main.transform.position = character.transform.position + offset;
            if (Input.GetKeyDown("a")) // 시점 임의로 변경, 추후에 아이템먹으면 시점변경
                ui.attackRange = 1;
            if (Input.GetKeyDown("s"))
                ui.attackRange = 2;
            if (Input.GetKeyDown("d"))
                ui.attackRange = 3;
            ui.indicatorRangeCircle.rectTransform.localScale = new Vector3(ui.attackRange, ui.attackRange, 0);
            // Range_Indicator 이미지의 크기 변경 

        }

        void FixedUpdate()
        {
            //if (isDeath == true)
            //    enabled = false;

            if (photonView.IsMine)
            {
                Move();        // 이동

                //물리적 가속도를 0으로 만들면 충돌했을때에 떨림이나
                //오브젝트가 밀리는 현상이 발생하지 않게된다고함
                //rb.velocity = Vector3.zero;
                //rb.angularVelocity = Vector3.zero;
            }

        }

        #endregion


        #region private Methods

        // 뱅 준비 (공격 사거리 표시)
        void AttackRange()
        {
            if (Input.GetButtonDown("LockOn"))
            {
                if (!isAiming)
                {
                    if (ui.attackRange == 1)
                    {
                        offset = new Vector3(0.0f, 7.0f, -7.0f);
                        maxAttackDistance = 5;
                    }
                    else if (ui.attackRange == 2)
                    {
                        offset = new Vector3(0.0f, 14.0f, -14.0f);
                        maxAttackDistance = 10;
                    }
                    else if (ui.attackRange == 3)
                    { 
                        offset = new Vector3(0.0f, 21.0f, -21.0f);
                        maxAttackDistance = 15;
                    }

                    isAiming = true;
                    animator.SetBool("IsAiming", isAiming);
                    agent.speed = 2.5f;
                    ui.indicatorRangeCircle.GetComponent<Image>().enabled = true;
                }
                else
                {
                    isAiming = false;
                    animator.SetBool("IsAiming", isAiming);
                    agent.speed = 5.0f;
                    offset = new Vector3(0.0f, 5.0f, -5.0f);
                    ui.indicatorRangeCircle.GetComponent<Image>().enabled = false;
                }
            }

        }
        
        // 뱅!
        void Bang()
        {
            if (Input.GetButtonDown("Attack"))
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
                {
                    if (hit.collider.gameObject != character && hit.collider.gameObject.tag == "Player")
                    { // 클릭한 오브젝트가 자기 자신이 아닌 다른 플레이어 일때

                        // 클릭한 물체의 위치와 내 위치의 거리 
                        float distance = Vector3.Distance(hit.collider.transform.position, transform.position);


                        if (distance <= maxAttackDistance)
                            Debug.Log("캐릭터 선택 닿음  " + "거리: "+ distance);
                        else
                            Debug.Log("캐릭터 선택 그러나 닿지않음   " + "거리: " + distance);

                        playerAutoMove.targetedEnemy = hit.collider.gameObject;
                    }
                    else
                    {
                        playerAutoMove.targetedEnemy = null;
                    }
                }
            }
        }


        // 플레이어 이동
        void Move()
        {
            if (Input.GetButton("Move"))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, (1 << LayerMask.NameToLayer("Floor")) + (1 << LayerMask.NameToLayer("Building")) ))
                {
                    // 이동
                    agent.SetDestination(hit.point);
                    playerAutoMove.targetedEnemy = null;
                    agent.stoppingDistance = 0;

                    // 회전이 딱히 필요없음
                    //Quaternion rotationToLookAt = Quaternion.LookRotation(hit.point - transform.position);
                    //float rotationY = Mathf.SmoothDamp(transform.eulerAngles.y,
                    //    rotationToLookAt.eulerAngles.y,
                    //    ref rotateVelocity,
                    //    rotateSpeedMovement * (Time.deltaTime * 5));
                    //transform.eulerAngles = new Vector3(0, rotationY, 0);

                }
            }
            float speed = agent.velocity.magnitude / agent.speed;
            animator.SetFloat("Speed", speed);
            
        }

        public void Move(Transform target)
        {
            // 이동
            agent.SetDestination(target.position);
            playerAutoMove.targetedEnemy = null;
            agent.stoppingDistance = 0;

            
            float speed = agent.velocity.magnitude / agent.speed;
            animator.SetFloat("Speed", speed);
        }


        #endregion


        #region 마스터로 부터 받은 정보 로컬과 동기화

        [PunRPC]
        public void JobSelect(int num) // 내 직업 동기화 
        {
            switch (num)
            {
                case 1:
                    playerManager.playerType = GameManager.jType.Sheriff;
                    playerInfo.hp = 5;
                    playerInfo.maxHp = 5;
                    break;
                case 2:
                case 3:
                    playerManager.playerType = GameManager.jType.Vice;
                    playerInfo.hp = 4;
                    playerInfo.maxHp = 4;
                    break;
                case 4:
                case 5:
                case 6:
                    playerManager.playerType = GameManager.jType.Outlaw;
                    playerInfo.hp = 3;
                    playerInfo.maxHp = 3;
                    break;
                case 7:
                    playerManager.playerType = GameManager.jType.Renegade;
                    playerInfo.hp = 2;
                    playerInfo.maxHp = 2;
                    break;
            }
        }

        [PunRPC]
        public void AbilitySelect(int num) // 내 능력 동기화 
        {
            switch (num)
            {
                case 1:
                    playerManager.abilityType = GameManager.aType.BangMissed;
                    break;
                case 2:
                    playerManager.abilityType = GameManager.aType.DrinkBottle;
                    break;
                case 3:
                    playerManager.abilityType = GameManager.aType.HumanVolcanic;
                    break;
                case 4:
                    playerManager.abilityType = GameManager.aType.OnehpOnecard;
                    break;
                case 5:
                    playerManager.abilityType = GameManager.aType.ThreeCard;
                    break;
                case 6:
                    playerManager.abilityType = GameManager.aType.TwocardOnecard;
                    break;
                case 7:
                    playerManager.abilityType = GameManager.aType.TwocardOnehp;
                    break;
            }
        }

        [PunRPC] 
        public void SyncHp() // 내 체력 동기화
        {
            playerInfo.Show_Hp();
        }

        [PunRPC]
        public void AnimStart() // 애니메이션 플레이
        {
            if (photonView.IsMine)
            { 
                StartCoroutine(GameManager.Instance.GameStart());
            }
        }

        [PunRPC]
        public void GiveCardSet(string jsonData)
        {
            GameManager.Instance.cardSet = gameObject.AddComponent<CardSet>();

            Card.cType[] startCards = JsonConvert.DeserializeObject<Card.cType[]>(jsonData);

            for (int i = 0; i < startCards.Length; i++)
            {
                Card card = new Card(startCards[i]);
                GameManager.Instance.cardSet.cardList.Add(card);
            }
        }


        [PunRPC]
        public void GiveCards(int num, Vector3 objPos) // 카드 나눠주기
        {
            Debug.Log("num: "+num);
            for (int i = 0; i < num; i++)
            {
                Card DrawCard = GameManager.Instance.cardSet.cardList[0];

                cardObject.ability = DrawCard.ability;
                cardObject.matchImg();


                if (photonView.IsMine) // 내 개인 UI에 내껏만 추가 
                {
                    var card = Instantiate(cardObject, MineUI.Instance.pos_CardSpwan.position, Quaternion.identity, MineUI.Instance.pos_CardParent);
                    var card2 = card.GetComponent<Card>();
                    playerInfo.mycards.Add(card2);

                    MineUI.Instance.CardAlignment();
                }
                else
                {
                    playerInfo.mycards.Add(cardObject); // 내가 가지고있는 카드셋 mycards에 추가 
                }

                GameManager.Instance.cardSet.cardList.RemoveAt(0);
            }
        }

        

        #endregion
    }

}

