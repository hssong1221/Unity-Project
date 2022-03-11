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

        [Header("아이템 Prefab")]
        public Item itemObject;

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


        // 캐릭터 키보드 움직임 구현
        float h;
        float v;
        Vector3 Old_Position;
        Vector3 Cur_Position;

        Vector3 moveVec;
        Vector3 moveDir;
        Vector3 lookForward;
        Vector3 lookRight;



        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
            base.Awake();
            agent = gameObject.GetComponent<NavMeshAgent>();

            MineUI.Instance.inventoryBtn.onClick.AddListener(Inventory);

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

            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }


            if (!PunChat.Instance.usingInput) // 일단 임시로 inputfield사용중일때 캐릭터 모든 입력금지
            {                                 // 대화창 활성화하면 캐릭터 제자리 걸음함 
                if (photonView.IsMine)
                {

                    // 키네마틱 리지드 바디라서 픽스드 업데이트에 할 필요가 없음 
                    Move();

                    if (Input.GetButtonDown("LockOn"))
                    {
                        AttackRange(); // 뱅 준비
                    }
                    if (Input.GetKeyDown(KeyCode.Tab))
                    {
                        Inventory();
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
                    ui.indicatorRangeCircle.rectTransform.localScale = new Vector3(ui.attackRange, ui.attackRange, 0);
                    // Range_Indicator 이미지의 크기 변경 
                }
            }
            else
            {
                animator.SetFloat("Speed", 0f); // 최후의 보루 
            }

        }

        /*void FixedUpdate()
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

        }*/

        #endregion


        #region private Methods

        // 뱅 준비 (공격 사거리 표시)
        void AttackRange()
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
                ui.indicatorRangeCircle.enabled = true;
            }
            else
            {
                isAiming = false;
                animator.SetBool("IsAiming", isAiming);
                agent.speed = 5.0f;
                offset = new Vector3(0.0f, 5.0f, -5.0f);
                ui.indicatorRangeCircle.enabled = false;
            }
        }

        // 뱅!
        void Bang()
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject != character && hit.collider.gameObject.tag == "Player")
                { // 클릭한 오브젝트가 자기 자신이 아닌 다른 플레이어 일때

                    // 클릭한 물체의 위치와 내 위치의 거리 
                    float distance = Vector3.Distance(hit.collider.transform.position, transform.position);


                    if (distance <= maxAttackDistance)
                        Debug.Log("캐릭터 선택 닿음  " + "거리: " + distance);
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


        // 플레이어 이동
        void Move()
        {
            // 키보드로 움직임 임시 구현

            playerAutoMove.targetedEnemy = null; 

            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
            moveVec = new Vector3(h, 0, v);

            Old_Position = transform.position;

            lookForward = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z).normalized;
            lookRight = new Vector3(Camera.main.transform.right.x, 0f, Camera.main.transform.right.z).normalized;

            moveDir = lookForward * moveVec.z + lookRight * moveVec.x;

            // 회전 다시 돌아오는것을 막기위해
            if (!(h == 0 && v == 0))
            {
                transform.forward = moveDir;
            }

            transform.position += 5f * Time.deltaTime * moveDir;

            Cur_Position = transform.position;
            animator.SetFloat("Speed", Vector3.Distance(Old_Position, Cur_Position) * 100f);

            /*if (Input.GetButton("Move"))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, (1 << LayerMask.NameToLayer("Floor")) + (1 << LayerMask.NameToLayer("Building"))))
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
                float speed = agent.velocity.magnitude / agent.speed;
                animator.SetFloat("Speed", speed);
            }*/
        }

        // 포탈에 상호작용 했을 때 동작
        public void Move(Transform target)
        {
            // 이동
            agent.SetDestination(target.position);
            playerAutoMove.targetedEnemy = null;
            agent.stoppingDistance = 0;


            float speed = agent.velocity.magnitude / agent.speed;
            animator.SetFloat("Speed", speed);
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
            Debug.Log(playerManager);
            Debug.Log(playerManager.playerType);
            Debug.Log(GameManager.jType.Sheriff);
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
        public IEnumerator GiveItems(string jsonData)
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
            return_itemNoticeText("<color=#000000>" + pickedItem.ToString() + " 을 흭득하였습니다!" + "</color>");

            yield return new WaitForEndOfFrame();
        }
        
        public void return_itemNoticeText(string sentece)
        {
            ui.itemNotice.enabled = true;
            ui.itemNotice.text = sentece;
            //ui.itemNotice1.GetComponent<Animator>().Play("ItemNoticeText");
            Invoke("TurnOffItemText", 2.0f);
        }

        void TurnOffItemText()
        {
            ui.itemNotice.enabled = false;
        }


        #endregion
    }

}

