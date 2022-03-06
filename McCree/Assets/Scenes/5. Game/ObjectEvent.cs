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

                // 콜라이더를 포톤으로 못넘겨서 플레이어 넘버를 넘겨줌

                PhotonView pv = other.GetComponent<PhotonView>(); // 부딪힌 플레이어 PhotonView
                Vector3 pos = transform.position; // 아이템 위치
                int pvNum = pv.OwnerActorNr;      // 부딪힌 플레이어 넘버


                if (pv.IsMine)
                {
                    // 마스터한테만 보낸다 (카드셋에서 카드 뽑기)
                    other.GetComponent<PhotonView>().RPC("Draw_Card", RpcTarget.MasterClient, num, pvNum, pos);
                }
            }
        }
    }
}