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
                Debug.Log("이건뭐?");

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


        public bool isBanging;
        public bool isDeath;
        public bool isBangeding;
        public bool isInteraction;

        private bool _isPicking;
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

        private bool _isLifting;
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


            EquipedNone = true;
            EquipedPistol = false;
            EquipedRifle = false;
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
            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }


            //if (!PunChat.Instance.usingInput) // 일단 임시로 inputfield사용중일때 캐릭터 모든 입력금지
            //{                                 // 대화창 활성화하면 캐릭터 제자리 걸음함 
            if (photonView.IsMine)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    Inventory();
                }

                if (!isBanging && !isBangeding && !isInteraction && !isLifting) // 아무코토 못함
                {
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
            //}
            //else
            //{
            //    animator.SetFloat("Speed", 0f); // 최후의 보루 
            //}

        }

        private void FixedUpdate() // move
        {
            if (photonView.IsMine)
            {
                if (!isBanging && !isBangeding && !isInteraction) // 아무코토 못함
                {
                    if (isPicking == true && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0))
                    {   // 상호작용중에 움직이면 
                        ui.CanCel_Animation();
                    }

                    h = Input.GetAxis("Horizontal");
                    v = Input.GetAxis("Vertical");

                    moveVec = new Vector3(h, 0f, v);

                    lookForward = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z).normalized;
                    lookRight = new Vector3(Camera.main.transform.right.x, 0f, Camera.main.transform.right.z).normalized;

                    moveDir = lookForward * moveVec.z + lookRight * moveVec.x;

                    if (moveDir.magnitude >= 0.001)
                    {
                        agent.SetDestination(transform.position);
                        playerAutoMove.targetedEnemy = null;

                        transform.rotation = Quaternion.LookRotation(moveDir);
                        // Rigidbody의 MovePosition 메소드로 캐릭터 이동
                        // transfrom.position을 사용해도 되지만 얇은 벽등을 통과할 문제등이 생길 수 있다.
                        // 객체의 충돌을 유지하면서 이동하기 위해 MovePosition을 사용 했다.
                        rb.MovePosition(rb.position + moveDir * Time.fixedDeltaTime * moveSpeed);
                        animator.SetFloat("Speed", moveDir.magnitude); // speed는 raiseonevent하니까 이상함

                        //animSync.SendPlayAnimationEvent(photonView.ViewID, "Speed", "Float", moveDir.magnitude);
                    }
                }
            }
        }
        #endregion

        #region private Methods

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
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
            {
                Debug.Log("레이캐스트");
                if (hit.collider.gameObject != character && hit.collider.gameObject.tag == "Player")
                { // 클릭한 오브젝트가 자기 자신이 아닌 다른 플레이어 일때

                    // 클릭한 물체의 위치와 내 위치의 거리 
                    float distance = Vector3.Distance(hit.collider.transform.position, transform.position);


                    if (distance <= maxAttackDistance)
                        Debug.Log("캐릭터 선택 닿음  " + "거리: " + distance);
                    else
                        Debug.Log("캐릭터 선택 그러나 닿지않음   " + "거리: " + distance);

                    playerAutoMove.targetedEnemy = hit.collider.gameObject;
                    Debug.Log("타겟 설정");
                }
                else
                {
                    playerAutoMove.targetedEnemy = null;
                }
            }
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
        public void GiveItems(string jsonData)
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
        void Damaged()
        {
            //    Debug.Log("회피 수: " + playerInfo.myItemList[1].itemCount);
            //    if (playerInfo.myItemList[1].itemCount > 0) // 회피 있으면
            //    {
            //        playerInfo.myItemList[1].itemCount -= 1;
            //        Debug.Log("로그 떠야함");
            //        Character_Notice_Text("<color=#FF8000>" + "회피!" + "</color>");
            //        Avoid_Trigger();
            //        return; // 회피있으면 avoid로그 출력하고 함수 종료
            //    }
            //    else
            //    {
            //        // 회피없으면 날라가는 함수 실행 
                
                rb.AddForce(new Vector3(50.0f, 10.0f, 50.0f), ForceMode.Impulse);

                playerInfo.hp -= playerInfo.damage;

                animator.SetTrigger("Banged");
        }


        [PunRPC]
        public void PickUp_Transform_Item(int photonID)
        {
            GameObject interactObj = PhotonView.Find(photonID).gameObject;

            interactObj.transform.SetParent(playerManager.objectTransPos);
            interactObj.GetComponent<ParticleSystem>().Stop();
            interactObj.GetComponent<ParticleSystem>().Clear();
            // position 오브젝트의 위치를 항상 월드의 원점을 기준으로 월드 공간상에 선언한다.
            // localPosition 부모의 위치 기준으로 설정한다
            interactObj.transform.localPosition = new Vector3(0f, 0f, 0f);
            interactObj.transform.localRotation = Quaternion.identity;
            interactObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

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

