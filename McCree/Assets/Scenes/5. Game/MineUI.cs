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


        public Text bangCount;
        public Text avoidCount;

        public GameObject[] mineUIhpImgs;

        public Sprite fullHealth;
        public Sprite emptyHealth;

        [Header("인벤토리 창 (StatusUI")]
        public GameObject statusPanel;
        public Text title_Item;
        public Text explain_Item;

        [Header("체력, 디버프 창 (leftTop")]
        public GameObject leftTopPanel;

        [Header("캐릭터 장착무기, 뱅, 회피 개수 창 (RightBottom")]
        public GameObject rightBottomPanel;
        public Button inventoryBtn;




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