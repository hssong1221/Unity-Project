using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    // UI는 전체 플레이어에게 공통적으로 보여줘야하는 UI를 다룹니다 (MineUI와 다름)
    public class UI : Controller
    {
        #region Variable Field

        [Header("카드관련 UI")]
        public Card cardObject;

        [Header("체력관련 UI")]
        public Canvas hpCanvas;
        public GameObject[] hpImgs;

        public Sprite fullBullet;
        public Sprite emptyBullet;
        public Text nickName; // 닉네임

        public Text cardNumText;

        public IEnumerator coroutine;

        public Vector3 hpOffset;
        Vector3 bangOffset;
        Vector3 itemOffset;
        Vector3 progressOffset;


        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
            base.Awake();  // Controller Awake 함수 그대로 사용

            hpOffset = new Vector3(0, 2.0f, 0);
            bangOffset = new Vector3(0, 3.0f, 0);
            itemOffset = new Vector3(0f, 1.0f, 0f);
            progressOffset = new Vector3(0, 2.5f, 0f);
        }


        private void Start()
        {
            if (photonView.IsMine)
            {
                nickName.text = PhotonNetwork.LocalPlayer.NickName;
            }
            else
            {
                nickName.text = GetComponent<PhotonView>().Owner.NickName;
            }
        }

        void Update()
        {
            // 플레이어 머리 위 UI 각도 고정
            hpCanvas.transform.LookAt(hpCanvas.transform.position + Camera.main.transform.forward);
            // 각자 캐릭터 머리 위 UI 위치 고정
            hpCanvas.transform.position = character.transform.position + hpOffset;
        }


        [PunRPC]
        public void GiveStoreCard(int num)
        {
            Debug.Log("잡화점 진입");
            for(int i = 0; i < num; i++)
            {
                // 리스트 맨 앞에서 뽑은 카드
                Card drawCard = GameManager.Instance.cardList[0];

                cardObject.cardContent = drawCard.cardContent;
                cardObject.matchImg();
                cardObject.storeIdx = i;

                var card = Instantiate(cardObject, MineUI.Instance.pos_CardSpwan.position, Quaternion.identity, MineUI.Instance.pos_StoreCardParent);
                var card2 = card.GetComponent<Card>();
                GameManager.Instance.storecardList.Add(card2);
                GameManager.Instance.cardList.RemoveAt(0);
            }
            // 화면 가운데 정렬
            StoreCardAlignment();
        }

        public void StoreCardAlignment()
        {
            List<Preset> originMyCards = new List<Preset>();
            originMyCards = LineAlignment(MineUI.Instance.pos_StoreLeft, MineUI.Instance.pos_StoreRight, GameManager.Instance.storecardList.Count, Vector3.one * 1.0f);

            for (int i = 0; i < GameManager.Instance.storecardList.Count; i++)
            {
                var targetCard = GameManager.Instance.storecardList[i];

                targetCard.originPRS = originMyCards[i];
                targetCard.MoveTransform(targetCard.originPRS, true, 0.9f);
            }
        }

        List<Preset> LineAlignment(Transform leftTr, Transform rightTr, int objCount, Vector3 scale)
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

                results.Add(new Preset(targetPos, targetRot, scale));
            }
            return results;
        }

        // 잡화점 리스트에서 내 리스트로 카드 복제
        public void StoreToMy(Card card)
        {
            cardObject.cardContent = card.cardContent;
            cardObject.matchImg();

            var temp = Instantiate(cardObject, card.transform.position, Quaternion.identity, MineUI.Instance.pos_CardParent);
            var temp2 = temp.GetComponent<Card>();
            playerInfo.mycards.Add(temp2);

            MineUI.Instance.CardAlignment();

            // 스토어 카드였기 때문에 storeidx가 남아있는데 본인 덱 다시 초기화
            foreach(Card c in playerInfo.mycards)
                c.storeIdx = 1000;
        }

        #endregion
    }
}