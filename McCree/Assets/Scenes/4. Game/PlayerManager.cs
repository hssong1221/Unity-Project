using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class PlayerManager : PlayerController
    {
        #region Public Fields

        // 화면 상에 보이는 로컬 플레이어들
        public static GameObject LocalPlayerInstance;

        //TEMP
        public float moveSpeed = 10.0f;

        private Vector3 movePos = Vector3.zero;
        private Vector3 moveDir = Vector3.zero;

        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
            base.Awake();  // PlayerController Awake 함수 그대로 사용

            // 포톤뷰에 의한 내 플레이어만
            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject; // gameObject는 이 컴포넌트가 붙어있는 게임오브젝트 즉 플레이어를 의미
            }

            DontDestroyOnLoad(gameObject);

        }


        void Start()
        {
            //SceneManager.sceneLoaded += OnSceneLoaded;
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
        }

        void FixedUpdate()
        {
            if (photonView.IsMine)
                Move();
        }

        #endregion


        #region private Methods

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

            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if(Physics.Raycast(ray, out RaycastHit raycastHit))
                {
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

