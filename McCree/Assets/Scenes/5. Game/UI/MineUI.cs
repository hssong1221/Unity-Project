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
        #region 개인UI패널 변수

        // 로컬 플레이어의 UI 어디서든 사용가능 (로컬 한정)  
        private static MineUI pInstance;

        public static MineUI Instance
        {
            get { return pInstance; }
        }

        private PhotonView photonView;
        private PlayerManager playerManager;
        private PlayerInfo playerInfo;
        private UI ui;



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
        public Transform logPanel;

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
        public Transform worldQuestPanel;
        public Transform subQuestPanel;
        public GameObject questObj;

        [Header("Quest Dtail UI 퀘스트 상세보기 창")]
        public GameObject questDetail;
        public Image questNpcImg;
        public Text questNpcName;
        public Text questTitle2;
        public Text questContent;
        public Button questcloseBtn;
        public bool isquestDetailopen;


        [HideInInspector]
        public float range_x;
        [HideInInspector]
        public float range_y;

        // -------------------- 카드 기능 부활 중
        public Transform pos_CardSpwan;
        public Transform pos_CardParent;
        public Transform pos_CardLeft;
        public Transform pos_CardRight;
        // -------------------------------------------------------

        public GameObject NextButton;

        #endregion


        void Awake()
        {
            // 어디서든 쓸 수 있게 인스턴스화
            pInstance = this;

            interactionRect = interactionPanel.GetComponent<RectTransform>();
            questcloseBtn.onClick.AddListener(Close_Quest_Detail_Panel);
            isquestDetailopen = false;
        }

        public void FindMinePv(GameObject player)
        {
            photonView = player.GetComponent<PhotonView>();
            playerManager = player.GetComponent<PlayerManager>();
            playerInfo = player.GetComponent<PlayerInfo>();
            ui = player.GetComponent<UI>();
        }

        public void Close_Quest_Detail_Panel()
        {
            if (isquestDetailopen)
            {
                questDetail.SetActive(false);
                isquestDetailopen = false;
            }
        }


        // ----------------------------------------카드 기능 부활중 -----------------------

        public void CardAlignment()
        {

            List<Preset> originMyCards = new List<Preset>();
            originMyCards = RoundAlignment(pos_CardLeft, pos_CardRight, playerInfo.mycards.Count, 0.5f, Vector3.one * 1.0f);

            for (int i = 0; i < playerInfo.mycards.Count; i++)
            {
                var targetCard = playerInfo.mycards[i];

                targetCard.originPRS = originMyCards[i];
                targetCard.MoveTransform(targetCard.originPRS, true, 0.9f);
            }

        }

        List<Preset> RoundAlignment(Transform leftTr, Transform rightTr, int objCount, float height, Vector3 scale)
        {
            float[] objLerps = new float[objCount];
            List<Preset> results = new List<Preset>(objCount);

            switch (objCount)
            {
                case 1: objLerps = new float[] { 0.5f }; break;
                case 2: objLerps = new float[] { 0.27f, 0.73f }; break;
                case 3: objLerps = new float[] { 0.1f, 0.5f, 0.9f }; break;
                default:
                    float interval = 1f / (objCount - 1);
                    for (int i = 0; i < objCount; i++)
                        objLerps[i] = interval * i;
                    break;
            }

            for (int i = 0; i < objCount; i++)
            {
                var targetPos = Vector3.Lerp(leftTr.position, rightTr.position, objLerps[i]);
                var targetRot = Quaternion.identity;
                if (objCount >= 4)
                {
                    float curve = Mathf.Sqrt(Mathf.Pow(height, 2) - Mathf.Pow(objLerps[i] - 0.5f, 2));
                    curve = height >= 0 ? curve : -curve;
                    targetPos.y += curve;
                    targetRot = Quaternion.Slerp(leftTr.rotation, rightTr.rotation, objLerps[i]);
                }
                results.Add(new Preset(targetPos, targetRot, scale));
            }
            return results;
        }
        // ----------------------------------------------------------------------------------

    }
}