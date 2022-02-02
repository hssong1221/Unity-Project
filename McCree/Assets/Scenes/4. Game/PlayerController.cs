using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class PlayerController : MonoBehaviourPunCallbacks
    {
        protected GameObject character;
        private GameObject cameraArm;
        private Camera mainCamera;

        protected Rigidbody rigidBody;
        protected CapsuleCollider capsuleCollider; // 캐릭터 캡슐
        protected BoxCollider groundCollider;         // isground체크용
        protected Animator animator;

        private Vector3 Offset;

        protected void Awake()
        {
            character = GameObject.FindWithTag("Player");
            cameraArm = GameObject.FindWithTag("CameraArm");
            mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

            animator = character.GetComponent<Animator>();
            capsuleCollider = character.GetComponent<CapsuleCollider>();
            //groundCollider = character.GetComponent<BoxCollider>();
            rigidBody = character.GetComponent<Rigidbody>();
        }
        void Start()
        {
            Debug.Log(animator);
            cameraArm.transform.rotation = Quaternion.Euler(45, 0, 0);
            Offset = new Vector3(0, 4, -4);
        }

        // Update is called once per frame
        void Update()
        {
            Camera_Move();
        }


        protected void Camera_Move()
        {
            cameraArm.transform.position = character.transform.position + Offset;
        }
    }
}
