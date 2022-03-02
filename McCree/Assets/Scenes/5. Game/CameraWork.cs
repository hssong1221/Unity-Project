using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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
            Debug.Log("-------------------카메라 스크립트 작동!!!");
            Cam = GetComponent<CinemachineVirtualCamera>();
            player = null;
            Debug.Log(Cam.name);
        }

        // 일단 자기 플레이가 가장 위에 생성된다고 가정하고 만듬
        void Update()
        {
            if(player == null)
            {
                player = GameObject.FindWithTag("Player");
                Debug.Log(player);

                if (player != null)
                {
                    if(Cam.name == "CM vcam1")
                    {
                        target = player.transform;
                        Cam.Follow = target;
                        Cam.LookAt = target;
                    }
                    else if(Cam.name == "CM vcam2")
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