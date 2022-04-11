using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
namespace com.ThreeCS.McCree
{
    public class Bullet : MonoBehaviour
    {
        public float speed;

        private GameObject _target;
        public GameObject target
        {
            get { return _target; }
            set
            {
                _target = value;
            }
        }


        private void FixedUpdate()
        {
            // 유도탄 처럼 따라가기
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponent<Collider>() == target.transform.parent.GetComponent<Collider>())
            {
                enabled = false;
                ObjectPool.Instance.ReturnObject(gameObject, 4);

                if (collision.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    Vector3 bulletFoward = target.transform.position - transform.position;
                    collision.gameObject.GetComponent<PhotonView>().RPC("Damaged", RpcTarget.All, bulletFoward);
                }
            }
        }
    }
}