using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class PlayerManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields

        // 화면 상에 보이는 로컬 플레이어들
        public static GameObject LocalPlayerInstance;

        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
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
            if(photonView.IsMine)
                Move();
        }


        #endregion

        #region private Methods
        // 플레이어 이동 임시구현

        void Move()
        {
            float hAxis = Input.GetAxis("Horizontal");
            float vAxis = Input.GetAxis("Vertical");
            Vector3 moveVec = new Vector3(hAxis, 0, vAxis).normalized;

            transform.position += moveVec * 5 * Time.deltaTime; 
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

