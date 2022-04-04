using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ThreeCS.McCree
{
    public class Bullet : MonoBehaviour
    {
        public float speed;

        private Rigidbody rb;

        private GameObject _target;
        public GameObject target
        {
            get { return _target; }
            set
            {
                _target = value;
            }
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            // 유도탄 처럼 따라가기
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                Debug.Log("닿음1");
            }
        }
    }
}