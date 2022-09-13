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

        private bool _isAiming;
        public bool isAiming // 조준 중 , 조준 끝
        {
            get { return _isAiming; }
            set
            {
                _isAiming = value;

                if (_isAiming == true)
                {
                    animSync.SendPlayAnimationEvent(photonView.ViewID, "IsAiming", "Bool", _isAiming);
                    moveSpeed = 2.5f;
                    ui.indicatorRangeCircle.enabled = true;
                }
                else
                {
                    animSync.SendPlayAnimationEvent(photonView.ViewID, "IsAiming", "Bool", _isAiming);
                    moveSpeed = 5.0f;
                    ui.indicatorRangeCircle.enabled = false;
                }
            }
        }

        [SerializeField]
        private bool _isBanging;
        [SerializeField]
        private bool _isBangeding;
        [SerializeField]
        private bool _isInteraction;
        [SerializeField]
        private bool _isLifting;
        [SerializeField]
        private bool _isPicking;
        [SerializeField]
        private bool _canBehave;

        public bool isBanging
        {
            get { return _isBanging; }
            set
            {
                _isBanging = value;

                if (_isBanging == true)
                    canBehave = false;
            }
        }
        
        public bool isBangeding
        {
            get { return _isBangeding; }
            set
            {
                _isBangeding = value;

                if (_isBangeding == true)
                    canBehave = false;
            }
        }
        public bool isInteraction
        {
            get { return _isInteraction; }
            set
            {
                _isInteraction = value;

                if (_isInteraction == true)
                    canBehave = false;
            }
        }

        public bool isPicking
        {
            get { return _isPicking; }
            set
            {
                _isPicking = value;

                if (_isPicking == true)
                {
                    animSync.SendPlayAnimationEvent(photonView.ViewID, "IsPicking", "Bool", _isPicking);
                }
                else
                {
                    animSync.SendPlayAnimationEvent(photonView.ViewID, "IsPicking", "Bool", _isPicking);
                }
            }
        }

        public bool isLifting // 들고있는 중 , 들고있지않은 중
        {
            get { return _isLifting; }
            set
            {
                _isLifting = value;

                if (_isLifting == true)
                {
                    animSync.SendPlayAnimationEvent(photonView.ViewID, "IsLifting", "Bool", _isLifting);
                    moveSpeed = 2.0f;
                }
                else
                {
                    animSync.SendPlayAnimationEvent(photonView.ViewID, "IsLifting", "Bool", _isLifting);
                    moveSpeed = 5.0f;
                }
            }
        }



        public bool isDeath;

        public bool isSit = false; // 의자에 앉는 동작 관련

        // 동작관련 플래그 true여야 동작 중 이동이 불가능함
        public bool canBehave;


        // 장착무기상태--------------------------------------------------------------------------------
        private bool _EquipedNone;
        public bool EquipedNone
        {
            get { return _EquipedNone; }
            set
            {
                _EquipedNone = value;

                animSync.SendPlayAnimationEvent(photonView.ViewID, "EquipedNone", "Bool", _EquipedNone);
            }
        }

        private bool _EquipedPistol;
        public bool EquipedPistol
        {
            get { return _EquipedPistol; }
            set
            {
                _EquipedPistol = value;

                animSync.SendPlayAnimationEvent(photonView.ViewID, "EquipedPistol", "Bool", _EquipedPistol);
            }
        }

        private bool _EquipedRifle;
        public bool EquipedRifle
        {
            get { return _EquipedRifle; }
            set
            {
                _EquipedRifle = value;

                animSync.SendPlayAnimationEvent(photonView.ViewID, "EquipedRifle", "Bool", _EquipedRifle);
            }
        }
        // ----------------------------------------------------------------------------------------


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
        float h;
        float v;

        Vector3 moveVec;
        Vector3 moveDir;
        Vector3 lookForward;
        Vector3 lookRight;

        float moveSpeed = 5f; // 캐릭터 이동 속도

        // --------------------------- 카드 기능 부활 중 -----------------------
        public Card cardObject;
        //-----------------------------------------------


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

            EquipedNone = true;
            EquipedPistol = false;
            EquipedRifle = false;
        }

        [PunRPC]
        public void PlayerListSync()
        {
            //플레이어 오브젝트가 전부 담김
            GameManager.Instance.playerList = GameObject.FindGameObjectsWithTag("Player");
            Debug.Log("플레이어 수(만큼 나올 듯) : " + GameManager.Instance.playerList.Length);
        }


        public override void OnDisable()
        {
            base.OnDisable();
            //SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void Update()
        {
            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }


            if (photonView.IsMine) // 자기자신 캐릭터만 제어해야함
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {   // 인벤토리 
                    Inventory();
                }

                if (canBehave && !isLifting && !PunChat.Instance.usingInput)
                {   // 아무입력 못받게 
                    if (!EquipedNone && Input.GetButtonDown("LockOn"))
                    {
                        AttackRange(); // 뱅 준비
                    }

                    if (isAiming && Input.GetButtonDown("Attack"))
                    {
                        Bang();
                    }
                    //Camera.main.transform.position = character.transform.position + offset;
                    if (Input.GetKeyDown("1")) // 시점 임의로 변경, 추후에 아이템먹으면 시점변경
                        ui.attackRange = 1;
                    if (Input.GetKeyDown("2"))
                        ui.attackRange = 2;
                    if (Input.GetKeyDown("3"))
                        ui.attackRange = 3;

                    if (Input.GetKeyDown("4"))
                    {
                        animSync.Temp_Weapon_None(photonView.ViewID);
                    }
                    if (Input.GetKeyDown("5"))
                    {
                        animSync.Temp_Weapon_Pistol(photonView.ViewID);
                    }
                    if (Input.GetKeyDown("6"))
                    {
                        animSync.Temp_Weapon_Rifle(photonView.ViewID);
                    }


                    ui.indicatorRangeCircle.rectTransform.localScale = new Vector3(ui.attackRange, ui.attackRange, 0);
                    // Range_Indicator 이미지의 크기 변경 
                }
            }
        }

        private void FixedUpdate() // move
        {
            if (photonView.IsMine && !isSit)
            {
                h = Input.GetAxis("Horizontal");
                v = Input.GetAxis("Vertical");

                if (!isBanging && !isBangeding && !isInteraction)
                {
                    canBehave = true;
                }


                if (!canBehave || PunChat.Instance.usingInput) // 일단 임시로 inputfield사용중일때 캐릭터 움직임 금지
                {   // 플레이어가 동작중일때 0 0 넣어줘서 못 움직이게 만듬
                    h = 0;
                    v = 0;
                }

                if (isPicking == true && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0))
                {   // 상호작용중에 움직이면 
                    ui.CanCel_Animation();
                }

                moveVec = new Vector3(h, 0f, v);

                lookForward = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z).normalized;
                lookRight = new Vector3(Camera.main.transform.right.x, 0f, Camera.main.transform.right.z).normalized;

                moveDir = lookForward * moveVec.z + lookRight * moveVec.x;

                if (moveDir.magnitude >= 0.001) // 이동 후 플레이어 회전 방지
                {
                    agent.SetDestination(transform.position);
                    playerAutoMove.targetedEnemy = null;
                    transform.rotation = Quaternion.LookRotation(moveDir);
                }

                // Rigidbody의 MovePosition 메소드로 캐릭터 이동
                // transfrom.position을 사용해도 되지만 얇은 벽등을 통과할 문제등이 생길 수 있다.
                // 객체의 충돌을 유지하면서 이동하기 위해 MovePosition을 사용 했다.
                rb.MovePosition(rb.position + moveDir * Time.fixedDeltaTime * moveSpeed);

                if (playerAutoMove.targetedEnemy == null)
                {
                    animator.SetFloat("Speed", moveDir.magnitude); // speed는 raiseonevent하니까 이상함
                }
                else // 조준중일때는 agent Speed넣어줘야 애니메이션 작동
                {
                    animator.SetFloat("Speed", agent.velocity.magnitude / agent.speed);
                }
            }
            //else
            //{
            //    rb.position = Vector3.MoveTowards(rb.position, networkPosition, Time.fixedDeltaTime * 5.0f);
            //    rb.rotation = Quaternion.RotateTowards(rb.rotation, networkRotation, Time.fixedDeltaTime * 1000.0f);
            //    animator.SetFloat("Speed", mSmag);
            //}
        }

        //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        //{
            //if (stream.IsWriting)
            //{
            //    stream.SendNext(rb.position);
            //    stream.SendNext(rb.rotation);
            //    stream.SendNext(rb.velocity);

            //    stream.SendNext(mSmag);
            //}
            //else
            //{
            //    networkPosition = (Vector3)stream.ReceiveNext();
            //    networkRotation = (Quaternion)stream.ReceiveNext();
            //    rb.velocity = (Vector3)stream.ReceiveNext();

            //    mSmag = (float)stream.ReceiveNext();

            //    float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTimestamp));
            //    networkPosition += rb.velocity * lag;
            //}
        //}

        #endregion

        #region  Methods

        // 뱅 준비 (공격 사거리 표시)
        void AttackRange()
        {
            if (!isAiming)
            {
                if (ui.attackRange == 1)
                {
                    maxAttackDistance = 5;
                }
                else if (ui.attackRange == 2)
                {
                    maxAttackDistance = 10;
                }
                else if (ui.attackRange == 3)
                {
                    maxAttackDistance = 15;
                }

                isAiming = true;
            }
            else
            {
                isAiming = false;
            }
        }

        // 뱅!
        void Bang()
        {

            RaycastHit[] hits;
            hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition));

            for (int i=0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];

                if (hit.collider.gameObject != character && hit.collider.gameObject.tag == "Player")
                { // 클릭한 오브젝트가 자기 자신이 아닌 다른 플레이어 일때

                    // 클릭한 물체의 위치와 내 위치의 거리 
                    float distance = Vector3.Distance(hit.collider.transform.position, transform.position);

                    if (distance <= maxAttackDistance)
                        Debug.Log("캐릭터 선택 닿음  " + "거리: " + distance);
                    else
                        Debug.Log("캐릭터 선택 그러나 닿지않음   " + "거리: " + distance);

                    playerAutoMove.targetedEnemy = hit.collider.gameObject;
                    return;
                }
                else
                {
                    playerAutoMove.targetedEnemy = null;
                }
            }


            //if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
            //{

            //    Debug.Log("레이캐스트");
            //    Debug.Log(hit.collider.name + "  " + (hit.collider.gameObject != character && hit.collider.gameObject.tag == "Player"));
            //    if (hit.collider.gameObject != character && hit.collider.gameObject.tag == "Player")
            //    { // 클릭한 오브젝트가 자기 자신이 아닌 다른 플레이어 일때

            //        // 클릭한 물체의 위치와 내 위치의 거리 
            //        float distance = Vector3.Distance(hit.collider.transform.position, transform.position);


            //        if (distance <= maxAttackDistance)
            //            Debug.Log("캐릭터 선택 닿음  " + "거리: " + distance);
            //        else
            //            Debug.Log("캐릭터 선택 그러나 닿지않음   " + "거리: " + distance);

            //        playerAutoMove.targetedEnemy = hit.collider.gameObject;
            //        Debug.Log("타겟 설정");
            //    }
            //    else
            //    {
            //        playerAutoMove.targetedEnemy = null;
            //    }
            //}

        }


        // 포탈에 상호작용 했을 때 동작
        public void Move(Transform target)
        {
            transform.position = target.position;
            playerAutoMove.targetedEnemy = null;

            /*
            // 이동
            agent.SetDestination(target.position);
            playerAutoMove.targetedEnemy = null;
            agent.stoppingDistance = 0;


            float speed = agent.velocity.magnitude / agent.speed;
            animator.SetFloat("Speed", speed);*/
        }

        // 의자에 앉고 일어나기
        public void Sit(Transform other, MeshRenderer mr)
        {
            // 앉은 플레이어 상태 동기화
            photonView.RPC("SitnumSync", RpcTarget.All); 

            isSit = true;
            Debug.Log("sit이 작동됨");
            animator.SetBool("isSit", true);
            // 네비게이션 메쉬가 꺼져있어야 순간이동이 가능. 그리고 덜덜 떨림 방지
            agent.enabled = false;
            

            // 의자에 앉으면 의자 하이라이트 꺼짐
            Material mat = mr.material;
            mat.SetColor("_EmissionColor", Color.black);

            // 의자에 앉는 모습으로 순간이동
            transform.position = other.position;
            playerAutoMove.targetedEnemy = null;
            transform.rotation = other.rotation;
            // 앉는 위치 보정
            transform.position += new Vector3(0, 0.1f, 0);

        }
        public void StandUp(Transform other)
        {
            // 플레이어 상태 동기화
            photonView.RPC("StandnumSync", RpcTarget.All);
            // 네비메쉬 다시 켜주기
            agent.enabled = true;
            animator.SetBool("isSit", false);
            isSit = false;
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
        }
        #endregion


        #region GameMaster 로 부터 받은 정보 로컬과 동기화

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
                    playerInfo.hp = 4;
                    playerInfo.maxHp = 4;
                    break;
                case 7:
                    playerManager.playerType = GameManager.jType.Renegade;
                    playerInfo.hp = 4;
                    playerInfo.maxHp = 4;
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
        public void GiveItems(string jsonData, int photonViewId)
        {
            Item.iType pickedItem = JsonConvert.DeserializeObject<Item.iType>(jsonData);


            if (photonView.IsMine)
            {
                foreach (ItemList itemList in playerInfo.myItemList)
                {
                    if (itemList.item.ability == pickedItem)
                    {
                        itemList.itemCount++;
                        break;
                    }
                }
            }
            Character_Notice_Text("<color=#000000>" + pickedItem.ToString() + " 을 흭득하였습니다!" + "</color>");
        }

        // -----------------------카드 기능 부활 중------------------------
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
            Debug.Log("num: " + num);
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

        // ---------------------------------------------------------------

       





















        // ------------------------------------ 퀘스트는 나중에 싹다 없애버릴 것 -------------------------------------------------------

        [PunRPC]
        public void GetQuest(string questTitle)
        {
            Character_Notice_Text("<color=#000000>" + questTitle + " 퀘스트를 수락하였습니다!" + "</color>");
        }

        [PunRPC]
        public void QuestComplete(string questTitle)
        {
            Character_Notice_Text("<color=#FF3E6E>" + questTitle + " 퀘스트를 완료하였습니다!" + "</color>");
        }

        public void Character_Notice_Text(string sentence)
        {
            if (coroutine != null) // 이미 텍스트 나온게있다면 4초 기다리던 코루틴 중지시키고 
                StopCoroutine(coroutine); // 새로운 코루틴 스타트 해서 다시 4초기다림
            coroutine = Character_Notice_Text_Coroutine(sentence);
            StartCoroutine(coroutine);
        }

        public IEnumerator Character_Notice_Text_Coroutine(string sentence)
        {
            ui.itemNotice.enabled = true;
            ui.itemNotice.text = sentence;

            yield return new WaitForSeconds(4.0f);

            ui.itemNotice.enabled = false;

            yield return null;
        }

        // Avoid 보류
        [PunRPC]
        void Damaged(Vector3 lookat)
        {
            //Debug.Log("회피 수: " + playerInfo.myItemList[1].itemCount);

            //if (playerInfo.myItemList[1].itemCount > 0) // 회피 있으면
            //{
            //    playerInfo.myItemList[1].itemCount -= 1;
            //    Debug.Log("로그 떠야함");
            //    Character_Notice_Text("<color=#FF8000>" + "회피!" + "</color>");
            //    Avoid_Trigger();
            //    return; // 회피있으면 avoid로그 출력하고 함수 종료
            //}
            //else
            //{
            //    // 회피없으면 날라가는 함수 실행 

            if (playerManager.objectTransPos.childCount != 0)
            {  // 무언가 들고있다면 떨군다.
                int photonID = playerManager.objectTransPos.GetChild(0).GetComponent<PhotonView>().ViewID;

                GameObject interactObj = PhotonView.Find(photonID).gameObject;
                interactObj.transform.SetParent(null);
                interactObj.GetComponent<LiftItem>().isLifting = false; // 이 아이템을 떨군 상태

                playerManager.isLifting = false;
            }

            transform.rotation = Quaternion.LookRotation(-lookat);
            //rb.AddForce((lookat).normalized * tempLaboratory.force, ForceMode.Impulse);
            playerInfo.hp -= 1;
            animator.SetTrigger("Banged");
            //}
        }


        [PunRPC]
        public void PickUp_Transform_Item(int photonID)
        {
            GameObject interactObj = PhotonView.Find(photonID).gameObject;


            interactObj.transform.SetParent(playerManager.objectTransPos);
            interactObj.GetComponent<LiftItem>().isLifting = true; // 이 아이템은 누가 들어올린 상태

            if (photonView.IsMine)
            {
                playerManager.isLifting = true;
            }
        }

        [PunRPC]
        void BangLog(string shooterNick, string targetNick)
        {
            GameObject bangLogObj = ObjectPool.Instance.GetObject(1); //오브젝트 풀에서 가져오기
            bangLogObj.transform.SetParent(MineUI.Instance.logPanel);
            bangLogObj.GetComponent<BangLogObj>().shooterNick.text = shooterNick;
            bangLogObj.GetComponent<BangLogObj>().targetNick.text = targetNick;
            bangLogObj.GetComponent<Animator>().Play("LogStart");
        }

        //void Avoid_Trigger()
        //{
        //    GameObject avoidLogObj = ObjectPool.Instance.GetObject(3); //오브젝트 풀에서 가져오기
        //    avoidLogObj.transform.SetParent(MineUI.Instance.logPanel);
        //    avoidLogObj.GetComponent<AvoidLogObj>().nick1.text = PhotonNetwork.LocalPlayer.NickName;
        //    avoidLogObj.GetComponent<Animator>().Play("LogStart");
        //}

        [PunRPC]
        public void QuestLog(string questTitle)
        {
            // 퀘스트 공지 창
            GameObject bangLogObj = ObjectPool.Instance.GetObject(2); //오브젝트 풀에서 가져오기
            bangLogObj.transform.SetParent(MineUI.Instance.logPanel);
            bangLogObj.GetComponent<QuestLogObj>().noticeText.text = "\'"+questTitle+"\' 월드 퀘스트가 활성화 되었습니다.";
            bangLogObj.GetComponent<Animator>().Play("LogStart");
        }


        #endregion
    }

}

