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

        [Header("카드 뽑혀서 나오는 위치랑 덱 위치")]
        public Transform pos_CardSpwan;
        public Transform pos_CardParent;
        public Transform pos_CardLeft;
        public Transform pos_CardRight;

        [Header("잡화점카드 위치 ")]
        public Transform pos_StoreCardParent;
        public Transform pos_StoreLeft;
        public Transform pos_StoreRight;

        [Header("다음턴 버튼")]
        public GameObject NextButton;

        [Header("뱅 맞았다는 UI")]
        public GameObject targetedPanel;
        public Text ttext;

        [Header("사거리 부족 알림 UI")]
        public GameObject distancePanel;
        public Text distanceText;

        [Header("감옥 알림 UI")]
        public GameObject jailPanel;
        public Text jailText;

        [Header("다이너마이트 알림 UI")]
        public GameObject dynamitePanel;
        public Text dynamiteText;

        [Header("술통 알림 UI")]
        public GameObject barrelPanel;
        public Text barrelText;

        [Header("마우스 입력 블락 패널")]
        public GameObject blockingPanel;
        public GameObject cardblockingPanel;


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


        // 카드 UI 정렬 함수
        public void CardAlignment()
        {
            List<Preset> originMyCards = new List<Preset>();
            originMyCards = RoundAlignment(pos_CardLeft, pos_CardRight, playerInfo.mycards.Count, 0.5f, Vector3.one * 1.0f); // playerinfo를 본인 거로 확정 지어야 할 수도 있음 현재는 안해도 돌아가긴함

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


        // 캣벌로우 시 타겟 대상 카드 갯수 만큼 화면에 빈카드 생성후 하나 클릭하면 원상복귀
        public void CatbalouCards(int num)
        {
            for (int i = 0; i < num; i++)
            {
                // 빈카드 생성
                playerManager.cardObject.cardContent = Card.cType.Blank;
                playerManager.cardObject.matchImg();
                playerManager.cardObject.storeIdx = i; // idx가 필요해서 있는거 가져다 쓰기

                // 카드를 뽑은게 본인이면 UI에 카드를 생성하고 본인 playerinfo에 카드 리스트에 저장
                var card = Instantiate(playerManager.cardObject, pos_CardSpwan.position, Quaternion.identity, pos_StoreCardParent); // 상점위치에다 소환
                var card2 = card.GetComponent<Card>();
                GameManager.Instance.storecardList.Add(card2); // 상점 리스트 활용

                ui.StoreCardAlignment();    // 잡화점 카드 정리 함수 활용
            }
        }

        public void CatbalouCardUIDel() // 캣벌루 사용자 화면 정상화
        {
            // 캣벌로우 하려고 스토어 리스트 채운거 정리
            GameManager.Instance.storecardList.Clear();

            // 화면에 보여주려고 생성한 카드 오브젝트 삭제
            GameObject[] obj = GameObject.FindGameObjectsWithTag("Card");
            foreach (GameObject card in obj)
            {
                if(card.GetComponent<Card>().storeIdx < 10)
                    Destroy(card.gameObject);
            }
        }

        public void CatTargetCardDel(int num)  // 캣벌로우 타겟의 카드 하나 랜덤 삭제
        {
            System.Random rand = new System.Random();
            int n = rand.Next(0, num + 1);  // 내 카드 갯수

            Debug.Log("카드 갯수 : " + num);

            int i = 0;
            foreach(Card card in playerInfo.mycards)
            {
                card.idx = i;
                i++;
            }

            // 전체 리스트 맨 뒤에 다시 추가
            photonView.RPC("CardDeckSync", RpcTarget.All, playerInfo.mycards[n].cardContent);

            // 카드 리스트에서 정보 삭제
            playerInfo.mycards.RemoveAt(n);

            // 카드 정렬
            CardAlignment();

            // 실제 오브젝트 destroy
            GameObject[] c = GameObject.FindGameObjectsWithTag("Card");
            foreach(GameObject card in c)
            {
                if(card.GetComponent<Card>().idx == n)
                {
                    Destroy(card.gameObject);
                    break;
                }
            }
        }

    }
}