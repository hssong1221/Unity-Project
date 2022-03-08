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

        #endregion

        #region Public Fields

        public Transform target;
        public GameObject player;

        #endregion

        #region MonoBehaviour CallBacks

        void Start()
        {
            Cam = GetComponent<CinemachineVirtualCamera>();
            player = null;
        }

        void Update()
        {
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

                if (player != null)
                {
                    if(Cam.name == "CM vcam1")
                    {
                        target = player.transform;
                        Cam.Follow = target;
                        Cam.LookAt = target;
                    }
                    else if (Cam.name == "CM vcam2")
                    {
                        target = player.transform;
                        Cam.Follow = target;
                        Cam.LookAt = target;
                    }
                    else if (Cam.name == "CM vcam3")
                    {
                        target = player.transform;
                        Cam.Follow = target;
                        Cam.LookAt = target;
                    }
                    else if(Cam.name == "CM vcam4")
                    {
                        target = player.transform;
                        Cam.LookAt = target;
                    }
                    
                }
            }
        }

        #endregion

    }

}