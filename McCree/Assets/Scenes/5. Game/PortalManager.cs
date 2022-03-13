using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Cinemachine;

using UnityEngine.AI;

using Photon.Pun;


namespace com.ThreeCS.McCree
{
    public class PortalManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields

        // 포탈 이용 안내 텍스트
        public GameObject ui;
        
        // 포탈 사용자 매니저
        private PlayerManager playerManager;

        // 시네머신 카메라
        public CinemachineClearShot cam;

        // 플레이어 네비메쉬에이전트
        protected NavMeshAgent nav;

        [Header("포탈 사용시 갈 곳")]
        public GameObject target;
        #endregion


        #region Trigger Methods

        // 포탈에 플레이어가 닿앗을 때
        void OnTriggerStay(Collider other)
        {
            if(other.gameObject.tag == "Player")
            {
                ui.SetActive(true);
                playerManager = other.GetComponent<PlayerManager>();
                nav = other.GetComponent<NavMeshAgent>();

                Debug.Log("플레이어 진입!!!");

                switch (gameObject.name)
                {
                    case "SaloonIN":
                        Debug.Log("주점 내부");
                        cam.ChildCameras[1].gameObject.SetActive(true);
                        cam.ChildCameras[0].gameObject.SetActive(false);
                        break;
                    case "SaloonOUT":
                        Debug.Log("주점 외부");
                        cam.ChildCameras[0].gameObject.SetActive(true);
                        cam.ChildCameras[1].gameObject.SetActive(false);
                        break;
                    default:
                        Debug.Log("일반 포탈");
                        if (Input.GetKey(KeyCode.E))
                        {
                            nav.enabled = false;
                            playerManager.Move(target.transform);
                            Invoke("NavOFF", 0.1f);
                        }
                        break;

                }

                
            }
        }

        // 포탈에서 플레이어가 벗어났을 때
        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                Debug.Log("플레이어 나감!!!");
                ui.SetActive(false);
            }
        }

        #endregion

        #region
        // 트랜스폼 순간이동을 위해서 잠깐 네비메쉬를 꺼야함
        void NavOFF()
        {
            nav.enabled = true;
        }
        #endregion


    }
}
