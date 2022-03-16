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

        [Header("상호작용 창 [F키] (Interaction UI")]
        public GameObject interactionPanel;
        [HideInInspector]
        public RectTransform interactionRect;
        public Text interactionText;

        [Header("NPC 대화 UI")]
        public GameObject chatPanel;
        public Text npcName;
        public Text npcChat;
        public Image npcImg;
        public GameObject npcbtns;
        public Button acceptBtn;
        public Button rejectBtn;

        [Header("Mission UI (RightTop)")]
        public GameObject rightTop;
        public Button mainQuestBtn;
        public Text questTitle;
        public Transform subQuestPanel;
        public GameObject subQuestObj;

        [Header("Quest Dtail UI 퀘스트 상세보기 창")]
        public GameObject questDetail;
        public Image questNpcImg;
        public Text questNpcName;
        public Text questTitle2;
        public Text questContent;
        public Button questcloseBtn;
        public bool isquestDetailopen;


        void Awake()
        {
            // 어디서든 쓸 수 있게 인스턴스화
            Instance = this;

            interactionRect = interactionPanel.GetComponent<RectTransform>();
            questcloseBtn.onClick.AddListener(Close_Quest_Detail_Panel);

            isquestDetailopen = false;
        }

        public void FindMinePv(GameObject player)
        {
            photonView = player.GetComponent<PhotonView>();
            playerManager = player.GetComponent<PlayerManager>();
            playerInfo = player.GetComponent<PlayerInfo>();
        }

        public void Close_Quest_Detail_Panel()
        {
            if (isquestDetailopen)
            {
                questDetail.SetActive(false);
                isquestDetailopen = false;
            }
        }

    }
}