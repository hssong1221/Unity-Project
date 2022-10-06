using System.Collections;
using System;

using UnityEngine;
using Cinemachine;

using Photon.Pun;

namespace com.ThreeCS.McCree
{
    public class CameraWork : MonoBehaviour
    {
        #region private Fields

        // 가상 카메라
        private CinemachineVirtualCamera Cam;

        private CinemachineClearShot ccam;
        #endregion

        #region Public Fields

        public Transform target;
        public GameObject player;

        public Vector3 vec;

        // 마우스 입력값
        protected float mouseX;
        protected float mouseY;
        protected float mouseLeftClick = 0f;

        #endregion

        #region MonoBehaviour CallBacks

        void Start()
        {
            Cam = GetComponent<CinemachineVirtualCamera>();
            ccam = GetComponentInParent<CinemachineClearShot>();
            player = null;

            StartCoroutine("CamWork");
        }

        void Update()
        {
            if (Input.GetMouseButton(1))
                mouseLeftClick = 1f;
            else if (Input.GetMouseButtonUp(1))
                mouseLeftClick = 0f;

            try
            {
                // 화면은 돌리려면 마우스 우클릭을 해야 돌아감
                mouseX = Input.GetAxis("Mouse X");
                mouseY = Input.GetAxis("Mouse Y");
                
                Cam.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InputAxisValue = mouseY * mouseLeftClick;
                Cam.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InputAxisValue = mouseX * mouseLeftClick;
            }
            catch(Exception e)
            {
                e.ToString();
                //Debug.Log(e);
            }
        }

        #endregion

        #region methods

        public void PortalIn()
        {
            ccam.ChildCameras[1].gameObject.SetActive(true);
            ccam.ChildCameras[0].gameObject.SetActive(false);
        }

        IEnumerator CamWork()
        {
            while (player == null)
            {
                // 플레이어 리스트에서 내 거 찾아서 가상 카메라 붙이기
                foreach (GameObject player in GameManager.Instance.playerList)
                {
                    if (player.GetComponent<PhotonView>().IsMine)
                    {
                        this.player = player;
                        break;
                    }
                }
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForEndOfFrame();

            // 카메라 마다 기능 부여
            while (player != null)
            {
                // 각도
                vec = player.transform.localEulerAngles;

                // 메인 카메라
                if (Cam.name == "CM vcam1")
                {
                    target = player.transform;
                    Cam.Follow = target;
                    Cam.LookAt = target;

                }
                // 주점 CCTV
                else if (Cam.name == "CM Saloon")
                {
                    target = player.transform;
                    Cam.LookAt = target;
                }
                // 게임진행 1인칭 
                else if (Cam.name == "CM Game")
                {
                    target = player.transform;
                    Cam.Follow = target;
                    Cam.LookAt = target;

                    // 앉으면 1인칭이되고 고개 돌릴수 있는 각도 제한
                    if (0 <= vec.y && vec.y <= 180)
                    {
                        Cam.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MinValue = vec.y + 20;
                        Cam.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxValue = vec.y + 170;
                    }
                    else
                    {
                        Cam.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MinValue = (vec.y - 180) - 160;
                        Cam.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxValue = (vec.y - 180) - 10;
                    }

                }
                yield return new WaitForEndOfFrame();

                //Debug.Log("현재 승리 상태 : " + GameManager.Instance.isVictory);
                if (GameManager.Instance.isVictory)
                {
                    ccam.ChildCameras[3].gameObject.SetActive(true);
                    ccam.ChildCameras[0].gameObject.SetActive(false);
                    ccam.ChildCameras[1].gameObject.SetActive(false);
                    ccam.ChildCameras[2].gameObject.SetActive(false);
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
        #endregion
    }

}