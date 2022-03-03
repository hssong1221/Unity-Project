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
            transform.Rotate(Vector3.up * 30.0f * Time.deltaTime);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                int num = 1;

                Card.cType[] startCards = new Card.cType[num];

                CardSet restCard = GameManager.Instance.Get_CardSet();

                for (int i = 0; i < num; i++) // Are you Serious?
                {
                    startCards[i] = restCard.cardList[i].ability;
                    restCard.cardList.RemoveAt(i);
                }
                
                var json = JsonConvert.SerializeObject(startCards);
               
                
                other.GetComponent<PhotonView>().RPC("GiveCards", RpcTarget.All, json, transform.position);

            }
        }
    }
}