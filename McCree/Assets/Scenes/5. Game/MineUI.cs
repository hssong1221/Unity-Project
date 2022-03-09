using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class MineUI : MonoBehaviour
    {
        // 로컬 플레이어의 UI 어디서든 사용가능 (로컬 한정)  
        static public MineUI Instance;

        private PhotonView photonView;
        private PlayerManager playerManager;
        private PlayerInfo playerInfo;


        [Header("인벤토리 창 (StatusUI")]
        public GameObject statusPanel;


        void Awake()
        {
            // 어디서든 쓸 수 있게 인스턴스화
            Instance = this;
        }

        public void FindMinePv(GameObject player)
        {
            photonView = player.GetComponent<PhotonView>();
            playerManager = player.GetComponent<PlayerManager>();
            playerInfo = player.GetComponent<PlayerInfo>();
        }
    }
}