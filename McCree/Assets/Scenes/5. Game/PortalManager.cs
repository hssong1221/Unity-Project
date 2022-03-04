using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;


namespace com.ThreeCS.McCree
{
    public class PortalManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields

        // 포탈 이용 안내 텍스트
        public GameObject ui;
        
        // 포탈 사용자 매니저
        public PlayerManager playerManager;

        [Header("포탈 사용시 갈 곳")]
        public GameObject target;
        #endregion



        #region Trigger Methods

        // 포탈에 플레이어가 닿앗을 때
        void OnTriggerStay(Collider other)
        {
            if(other.gameObject.tag == "Player")
            {
                ui.SetActive(true);
                playerManager = other.GetComponent<PlayerManager>();

                Debug.Log("플레이어 진입!!!");
                if (Input.GetKey(KeyCode.E))
                {
                    playerManager.Move(target.transform);
                }
            }
        }

        // 포탈에서 플레이어가 벗어났을 때
        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                Debug.Log("플레이어 나감!!!");
                ui.SetActive(false);
            }
        }

        #endregion



    }
}