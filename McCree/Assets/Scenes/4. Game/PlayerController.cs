using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using UnityStandardAssets.Utility;

namespace com.ThreeCS.McCree
{
    public class PlayerController : MonoBehaviourPunCallbacks
    {
        public static GameObject LocalPlayerInstance;

        protected GameObject character; // Character객체 (상속가능) 

        protected Vector3 Offset;

        protected void Awake()
        {
            character = GameObject.FindWithTag("Player");
            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject; // gameObject는 이 컴포넌트가 붙어있는 게임오브젝트 즉 플레이어를 의미
                Camera.main.GetComponent<SmoothFollow>().target = character.transform;
                // 플레이어 마다 카메라를 설정하려면 유니티 기본 에셋 SmoothFollow를 임포트하여 써야한다고 한다. 왜인진 모름
            }
            Camera.main.transform.rotation = Quaternion.Euler(45, 0, 0);
            Offset = new Vector3(0, 4.0f, -4.0f);
            // 또한 카메라에 붙어있는 SmoothFollow.cs의 값과 연동되는데 마음대로 잘 안된다. 

        }
        private void Start()
        {
            //DontDestroyOnLoad(gameObject);
        }

        // Update is called once per frame
        private void Update()
        {
            Camera_Move();

            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }
        }


        private void Camera_Move()
        {
            Camera.main.transform.position = character.transform.position + Offset;
        }
    }
}
