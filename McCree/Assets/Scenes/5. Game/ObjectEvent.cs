using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

using Photon.Pun;


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

                for (int i = 0; i < num; i++)
                {
                    Item.iType pickItem = GameManager.Instance.entireItemSet.Pick_Item();

                    var json = JsonConvert.SerializeObject(pickItem);

                    if (other.GetComponent<PhotonView>().IsMine)
                    {
                        other.GetComponent<PhotonView>().RPC("GiveItems", RpcTarget.All, json);
                    }
                }
            }
        }
    }
}