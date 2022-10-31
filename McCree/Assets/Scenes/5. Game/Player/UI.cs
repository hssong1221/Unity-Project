using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
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

        [Header("뱅 준비 사거리 표시 관련 UI")]
        public Canvas attackRangeCanvas;  // 공격 사거리 캔버스
        public Image indicatorRangeCircle;// 공격 사거리 이미지
        public int attackRange; // 기본 공격 사거리 일단 1로 설정해놓은상태

        [Header("Bang! 말풍선 관련 UI")]
        public Canvas bangCanvas;         // 뱅 말풍선 캔버스
        public Image bangGifImg;          // 뱅 말풍선 gif 이미지


        [Header("아이템 관련 UI")]
        public Canvas itemCanvas;   // 아이템 알림 캔버스
        public Text itemNotice;     // 아이템 알림 텍스트


        [Header("ProgressBar 관련 UI")]
        public Canvas progressCanvas;
        public Text progressText;
        public Text progressPercent;
        public Image progressBar;
        float currentValue;

        public IEnumerator coroutine;

        Vector3 hpOffset;
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
            // 기본사거리
            attackRange = 1;
            // 기본 Indicator Range img 크기
            indicatorRangeCircle.rectTransform.localScale = new Vector3(attackRange, attackRange, 0);
            // 공격범위 UI 꺼주기
            indicatorRangeCircle.enabled = false;
            // 뱅 말풍선 UI 꺼주기
            bangGifImg.enabled = false;
            // 아이템 공지 UI 꺼주기
            itemNotice.enabled = false;

            // 프로그레스바 UI 꺼주기
            progressText.enabled = false;
            progressBar.enabled = false;
            progressPercent.enabled = false;

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
            bangCanvas.transform.LookAt(bangCanvas.transform.position + Camera.main.transform.forward);
            itemCanvas.transform.LookAt(itemCanvas.transform.position + Camera.main.transform.forward);
            progressCanvas.transform.LookAt(progressCanvas.transform.position + Camera.main.transform.forward);

            if (photonView.IsMine)
            {
                // 각자 캐릭터 머리 위 UI 위치 고정
                hpCanvas.transform.position = character.transform.position + hpOffset;
                bangCanvas.transform.position = character.transform.position + bangOffset;
                itemCanvas.transform.position = character.transform.position + itemOffset;
                progressCanvas.transform.position = character.transform.position + progressOffset;

            }
            else
                return;
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

                StoreCardAlignment();

                GameManager.Instance.cardList.RemoveAt(0);
            }
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
        }








        // ----------- 삭제 예정
        public void InterAction(float time, GameObject interactObj)
        {
            if (interactObj.tag == "QItem_PickUp")
            {
                progressText.text = interactObj.name+" 치우는 중...";
                animSync.SendPlayAnimationEvent(photonView.ViewID, "Pick", "Trigger");
            }

            else if (interactObj.tag == "QItem_TransPort")
            {
                progressText.text = interactObj.name + " 들어 올리는 중...";
                animSync.SendPlayAnimationEvent(photonView.ViewID, "Lift", "Trigger");
            }
                
            playerManager.isPicking = true;
            LoadingProgreeCircle(time, interactObj);
        }

        public void LoadingProgreeCircle(float time, GameObject interactObj)
        {
            progressText.enabled = true;
            progressBar.enabled = true;
            progressPercent.enabled = true;

            currentValue = 0;
            if (coroutine != null)
                StopCoroutine(coroutine);
            coroutine = LoadingProgreeCircleCoroutine(time, interactObj);
            StartCoroutine(coroutine);
        }

        IEnumerator LoadingProgreeCircleCoroutine(float time, GameObject interactObj)
        {
            while (playerManager.isPicking)
            {
                currentValue += 100 * Time.deltaTime / time; // 아마 초단위 맞을듯?
                progressPercent.text = ((int)currentValue).ToString() + "%";
                progressBar.fillAmount = currentValue / 100;
                MineUI.Instance.interactionPanel.SetActive(false);

                if (interactObj.GetComponent<LiftItem>() != null)
                {
                    if (interactObj.GetComponent<LiftItem>().isLifting)
                    {
                        Off_ProgressUI();
                        yield break;
                    }
                }

                if (progressBar.fillAmount > 0.99)
                {
                    foreach (SubQuestList subQuestObj in playerInfo.myQuestList)
                    {
                        Quest_Interface_PT_Obj ptQuest = (Quest_Interface_PT_Obj)subQuestObj.questObj;

                        if (interactObj.name == ptQuest.quest.bringGameObj.name)
                        {
                            if (interactObj.tag == "QItem_PickUp")
                            {
                                Quest_PickUp_Obj pickQuest = (Quest_PickUp_Obj)ptQuest;

                                pickQuest.count++; // 퀘스트 아이템 개수 증가
                                MineUI.Instance.interactionPanel.SetActive(false);

                                // 증가한 아이템 개수로 텍스트 바꿔줌
                                subQuestObj.questTitle.text = pickQuest.questTitle_progress;

                                Debug.Log("줍기 성공!");
                                Destroy(interactObj); // 애니메이션 끝나면 오브젝트 파괴
                            }
                            else if (interactObj.tag == "QItem_TransPort")
                            {
                                Quest_Transport_Obj pickQuest = (Quest_Transport_Obj)ptQuest;

                                if (pickQuest.qrange == Quest_Obj.oType.World) // 월드 퀘스트 아이템일때
                                {
                                    //Debug.Log(interactObj.GetComponent<PhotonView>().ViewID);
                                    photonView.RPC("PickUp_Transform_Item", RpcTarget.All, interactObj.GetComponent<PhotonView>().ViewID);
                                }
                                else // 개인 퀘스트 아이템 일때
                                {
                                    interactObj.transform.SetParent(playerManager.objectTransPos);
                                    interactObj.GetComponent<ParticleSystem>().Stop();
                                    interactObj.GetComponent<ParticleSystem>().Clear();
                                    // position 오브젝트의 위치를 항상 월드의 원점을 기준으로 월드 공간상에 선언한다.
                                    // localPosition 부모의 위치 기준으로 설정한다
                                    interactObj.transform.localPosition = new Vector3(0f, 0f, 0f);
                                    interactObj.transform.localRotation = Quaternion.identity;
                                    interactObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

                                    Debug.Log("들어올리기 성공!");
                                    playerManager.isLifting = true;
                                }
                            }
                            break;
                        }
                    }
                    Off_ProgressUI();
                    //interactObj.SetActive(false);

                    
                }
                yield return null;
            }
        }

        public void Off_ProgressUI() // 원 진행도, interaction 코루틴 스탑
        {
            playerManager.isPicking = false;
            if (coroutine != null) 
                StopCoroutine(coroutine);
            interaction.StopCoroutine_Direct(interaction.coroutine_Interact);
            progressText.enabled = false;
            progressBar.enabled = false;
            progressPercent.enabled = false;
        }

        public void CanCel_Animation()
        {
            MineUI.Instance.interactionPanel.SetActive(true);

            playerManager.isPicking = false;
            if (coroutine != null) // 원 진행도 코루틴만 스탑, 다시 주울수있도록 interaction코루틴은 끄지않음
                StopCoroutine(coroutine);
            progressText.enabled = false;
            progressBar.enabled = false;
            progressPercent.enabled = false;
        }
        #endregion



    }


}