using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class PlayerManager : Controller
    {
        #region Variable Fields

        // 화면 상에 보이는 로컬 플레이어들
        public static GameObject LocalPlayerInstance;

        public GameObject[] players;

        public bool isAiming;
        public bool isDeath;

        // 플레이어 정보 타입
        public enum jType { Sheriff, Vice, Outlaw, Renegade } // 직업 타입
        public enum aType { HumanVolcanic, BangMissed, ThreeCard, OnehpOnecard, TwocardOnehp,  TwocardOnecard, DrinkBottle }
        [Header("플레이어 정보")]
        public jType playerType;
        public aType abilityType;

        [Header("이동 관련")]
        [HideInInspector]
        public float rotateSpeedMovement = 0.1f;  // 캐릭터 회전하는 속도
        [HideInInspector]
        public float rotateVelocity;

        [Header("카메라 오프셋")]
        protected Vector3 offset;

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

            //플레이어 오브젝트가 전부 담김
            players = GameObject.FindGameObjectsWithTag("Player");
            Debug.Log(players.Length);

            // 포톤뷰에 의한 내 플레이어만
            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject; // gameObject는 이 컴포넌트가 붙어있는 게임오브젝트 즉 플레이어를 의미

                // 카메라 x 45도, offset
                Camera.main.transform.rotation = Quaternion.Euler(45, 0, 0);
                offset = new Vector3(0.0f, 5.0f, -5f);
            }
            DontDestroyOnLoad(gameObject);
        }


        void Start()
        {
            //SceneManager.sceneLoaded += OnSceneLoaded;

            // 방장이 직업을 섞어서 나눠줌
            if (PhotonNetwork.IsMasterClient)
            {
                JobandAbility(); //플레이어 직업과 능력 분배
            }
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

            Camera.main.transform.position = character.transform.position + offset;
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
            }

        }

        #endregion


        #region private Methods

        // 플레이어 직업과 능력 분배
        public void JobandAbility()
        {
            // (인구수에 맞게 하는 거 추가하기)
            List<int> jobList = new List<int>() { 1, 2, 3, 4, 5, 6, 7 };

            // 능력 갯수에 맞게 해야함
            List<int> abilityList = new List<int>() { 1, 2, 3, 4, 5, 6, 7 };

            jobList = ShuffleList(jobList);
            abilityList = ShuffleList(abilityList);

            // 직업을 나눠주고 동기화 시킴
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                players[i].GetComponent<PhotonView>().RPC("JobSelect", RpcTarget.All, jobList[i]);
            }

            // 능력을 나눠주고 동기화 시킴
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                players[i].GetComponent<PhotonView>().RPC("AbilitySelect", RpcTarget.All, abilityList[i]);
            }

        }

        // 플레이어 직업 동기화
        [PunRPC]
        public void JobSelect(int num)
        {
            switch (num)
            {
                case 1:
                    playerType = jType.Sheriff;
                    break;
                case 2:
                case 3:
                    playerType = jType.Vice;
                    break;
                case 4:
                case 5:
                case 6:
                    playerType = jType.Outlaw;
                    break;
                case 7:
                    playerType = jType.Renegade;
                    break;
            }

        }

        // 플레이어 능력 동기화
        [PunRPC]
        public void AbilitySelect(int num)
        {
            switch (num)
            {
                case 1:
                    abilityType = aType.BangMissed;
                    break;
                case 2:
                    abilityType = aType.DrinkBottle;
                    break;
                case 3:
                    abilityType = aType.HumanVolcanic;
                    break;
                case 4:
                    abilityType = aType.OnehpOnecard;
                    break;
                case 5:
                    abilityType = aType.ThreeCard;
                    break;
                case 6:
                    abilityType = aType.TwocardOnecard;
                    break;
                case 7:
                    abilityType = aType.TwocardOnehp;
                    break;
            }

        }

        // 리스트 셔플, 자바에는 그냥 있는데 씨썁에는 없다ㅋㅋ
        private List<T> ShuffleList<T>(List<T> list)
        {
            int random1, random2;
            T temp;

            for (int i = 0; i < list.Count; ++i)
            {
                random1 = Random.Range(0, list.Count);
                random2 = Random.Range(0, list.Count);

                temp = list[random1];
                list[random1] = list[random2];
                list[random2] = temp;
            }
            return list;
        }

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

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Floor")))
                {
                    // 이동
                    agent.SetDestination(hit.point);
                    playerAutoMove.targetedEnemy = null;
                    agent.stoppingDistance = 0;

                    // 회전
                    Quaternion rotationToLookAt = Quaternion.LookRotation(hit.point - transform.position);
                    float rotationY = Mathf.SmoothDamp(transform.eulerAngles.y,
                        rotationToLookAt.eulerAngles.y,
                        ref rotateVelocity,
                        rotateSpeedMovement * (Time.deltaTime * 5));
                    transform.eulerAngles = new Vector3(0, rotationY, 0);
                }
            }
            float speed = agent.velocity.magnitude / agent.speed;
            animator.SetFloat("Speed", speed);
            //Debug.Log(speed);
        }

        #endregion
    }
}

