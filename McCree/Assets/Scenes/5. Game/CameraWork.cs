using System.Collections;
using System.Collections.Generic;
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

        #endregion

        #region MonoBehaviour CallBacks

        void Start()
        {
            Cam = GetComponent<CinemachineVirtualCamera>();
            ccam = GetComponentInParent<CinemachineClearShot>();
            player = null;
        }


        void FixedUpdate()
        {
            // 처음 진입
            if(player == null)
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

                // 카메라 마다 기능 부여
                if (player != null)
                {
                    // 메인 카메라
                    if(Cam.name == "CM vcam1")
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
                }
            }

            Debug.Log("현재 승리 상태 : " + GameManager.Instance.isVitory);


            if (GameManager.Instance.isVitory)
            {
                ccam.ChildCameras[2].gameObject.SetActive(true);
                ccam.ChildCameras[0].gameObject.SetActive(false);
                ccam.ChildCameras[1].gameObject.SetActive(false);
            }
        }

        #endregion
    }

}