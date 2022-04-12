using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
// 캐릭터 스크립트 실험실
// 이 스크립트는 나중에 따로 구현해야하는 기능이기때문에 하나씩 지워나가야 함

namespace com.ThreeCS.McCree
{
    public class TempLaboratory : Controller
    {

        public float force;


        void Awake()
        {
            base.Awake();  // Controller Awake 함수 그대로 사용
        }


        void Update()
        {
            // 임시 피격 모션  U
            if (Input.GetKeyDown(KeyCode.U))
            {
                rb.AddForce((-transform.forward).normalized * force, ForceMode.Impulse);
                animator.SetTrigger("Banged");
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                if (playerManager.objectTransPos.childCount != 0)
                {
                    int photonID = playerManager.objectTransPos.GetChild(0).GetComponent<PhotonView>().ViewID;

                    GameObject interactObj = PhotonView.Find(photonID).gameObject;

                    interactObj.transform.SetParent(null);

                    interactObj.GetComponent<LiftItem>().isLifting = false; // 이 아이템을 떨군 상태


                    playerManager.isLifting = false;
                }
            }

        }
    }
}