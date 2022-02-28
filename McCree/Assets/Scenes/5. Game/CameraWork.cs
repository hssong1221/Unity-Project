using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace com.ThreeCS.McCree
{
    public class CameraWork : MonoBehaviour
    {
        // 가상 카메라
        private CinemachineVirtualCamera Cam1;
        public Transform target;
        public GameObject player;

        void Start()
        {
            Debug.Log("-------------------카메라 스크립트 작동!!!");
            Cam1 = GetComponent<CinemachineVirtualCamera>();
            player = null;
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
                    target = player.transform;
                    Cam1.Follow = target;
                    Cam1.LookAt = target;
                }
            }
        }

    }

}