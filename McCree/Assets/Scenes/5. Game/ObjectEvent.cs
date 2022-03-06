using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

using Photon.Pun;

using DG.Tweening;

namespace com.ThreeCS.McCree
{
    public class ObjectEvent : MonoBehaviour
    {
        private void Update()
        {
            transform.Rotate(Vector3.up * 40.0f * Time.deltaTime);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                int num = 1; // 아이템 부딪히면 나눠줄 카드 개수

                if (other.GetComponent<PhotonView>().IsMine)
                    other.GetComponent<PhotonView>().RPC("GiveCards", RpcTarget.All, num , transform.position);
            }
        }
    }
}