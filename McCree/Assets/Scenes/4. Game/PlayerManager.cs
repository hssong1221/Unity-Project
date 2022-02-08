using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class PlayerManager : Controller
    {
        #region Variable Fields

        // 화면 상에 보이는 로컬 플레이어들
        public static GameObject LocalPlayerInstance;

        //TEMP
        public float moveSpeed = 5.0f;

        private Vector3 movePos = Vector3.zero;
        //private Vector3 moveDir = Vector3.zero;


        [Header("AttackRange")]
        Vector3 position;
        public Image targetCircle;        // 공격 사거리 이미지
        public Image indicatorRangeCircle;// 목표 범위 이미지
        public Canvas attackRangeCanvas;  // 공격 사거리 캔버스
        public Canvas targetRangeCanvas;  // 목표 쪽 캔버스
        public bool isAiming;
        private Vector3 posUp;
        public int attackRange; // 기본 공격 사거리 일단 1로 설정해놓은상태

        //public float maxAttackDistance; 

        public CameraWork cameraWork;

        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
            base.Awake();

            // 포톤뷰에 의한 내 플레이어만
            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject; // gameObject는 이 컴포넌트가 붙어있는 게임오브젝트 즉 플레이어를 의미
            }
            DontDestroyOnLoad(gameObject);
            cameraWork = GetComponent<CameraWork>();
        }


        void Start()
        {
            CameraWork _cameraWork = gameObject.GetComponent<CameraWork>();

            if (_cameraWork != null)
            {
                if (photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> CameraWork Component on player Prefab.", this);
            }
            //SceneManager.sceneLoaded += OnSceneLoaded;

            attackRange = 1;
            indicatorRangeCircle.rectTransform.localScale = new Vector3(attackRange, attackRange, 0);
            // 공격범위 UI 꺼주기
            targetCircle.GetComponent<Image>().enabled = false;
            indicatorRangeCircle.GetComponent<Image>().enabled = false;
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

            // 공격범위 관련
            AttackRange();

            if (Input.GetKeyDown("a")) // 시점 임의로 변경, 추후에 아이템먹으면 시점변경
                attackRange = 1;
            if (Input.GetKeyDown("s"))
                attackRange = 2;
            if (Input.GetKeyDown("d"))
                attackRange = 3;
            indicatorRangeCircle.rectTransform.localScale = new Vector3(attackRange, attackRange, 0);

            /* 타켓 ui 캔버스 위치 설정

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject != this.gameObject) // 자기자신제외
                {
                    posUp = new Vector3(hit.point.x, 10f, hit.point.z); // ??
                    position = hit.point; // 목표지점
                }
            }

            var hitPosDir = (hit.point - transform.position).normalized;
            float distance = Vector3.Distance(hit.point, transform.position);
            distance = Mathf.Min(distance, maxAttackDistance); // 범위 

            var newHitPos = transform.position + hitPosDir * distance;
            targetRangeCanvas.transform.position = (newHitPos);


            Debug.Log(transform.position + "   " + hit.point + "  " + distance);

            */

        }

        void FixedUpdate()
        {
            if (isDeath == true)
                enabled = false;


            if (photonView.IsMine)
                Move();
        }

        #endregion


        #region private Methods

        void AttackRange()
        {
            if (Input.GetButtonDown("LockOn"))
            {
                if (!isAiming)
                {
                    if (attackRange == 1)
                    {
                        cameraWork.distance = 7;
                        cameraWork.height = 6.3f;
                    }
                    else if (attackRange == 2)
                    {
                        cameraWork.distance = 14;
                        cameraWork.height = 12.6f;
                    }
                    else if (attackRange == 3)
                    {
                        cameraWork.distance = 21;
                        cameraWork.height = 18.9f;
                    }


                    isAiming = true;
                    animator.SetBool("IsAiming", isAiming);
                    moveSpeed = 2.5f;
                    indicatorRangeCircle.GetComponent<Image>().enabled = true;
                    //targetCircle.GetComponent<Image>().enabled = true;
                }
                else
                {
                    isAiming = false;
                    animator.SetBool("IsAiming", isAiming);
                    moveSpeed = 5.0f;
                    cameraWork.distance = 5;
                    cameraWork.height = 4.5f;
                    indicatorRangeCircle.GetComponent<Image>().enabled = false;
                    //targetCircle.GetComponent<Image>().enabled = false;
                }
            }

        }







        // 플레이어 이동 임시구현
        void Move()
        {
            // 임시로 만든 키보드 입력
            /*float hAxis = Input.GetAxis("Horizontal");
            float vAxis = Input.GetAxis("Vertical");
            Vector3 moveVec = new Vector3(hAxis, 0, vAxis).normalized;
            

            transform.position += moveVec * 5 * Time.deltaTime;*/


            Vector3 Old_Position;
            Vector3 Cur_Position;

            Old_Position = transform.position;

            if (Input.GetButton("Move"))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if(Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Floor")))
                {   // 무한으로 쏴야 카메라가 멀리있어도 땅 클릭 가능
                    movePos = raycastHit.point;
                }
                Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green);
            }
            if(movePos != Vector3.zero)
            {
                Vector3 dir = movePos - transform.position;

                float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(0, angle, 0);
                transform.position = Vector3.MoveTowards(transform.position, movePos, moveSpeed * Time.deltaTime);
            }

            float dis = Vector3.Distance(transform.position, movePos);
            if (dis <= 0.3f)
            {
                movePos = Vector3.zero;
            }

            Cur_Position = transform.position;
            animator.SetFloat("Speed", Vector3.Distance(Old_Position, Cur_Position) * 100f);
            //Debug.Log(Vector3.Distance(Old_Position, Cur_Position) * 100f);
        }


        /*void OnSceneLoaded(Scene scene, LoadSceneMode loadingMode)
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        }

        void CalledOnLevelWasLoaded(int level)
        {
            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }
        }*/



        #endregion
    }
}

