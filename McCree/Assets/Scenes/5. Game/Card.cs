using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Photon.Pun;
using DG.Tweening;

namespace com.ThreeCS.McCree
{
    public class Card : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler, IPointerUpHandler
    {
        #region variable

        [Header("카드 그림")]
        public Sprite bangImg;
        public Sprite avoidImg;
        public Sprite beerImg;
        public Sprite indianImg;
        public Sprite MGImg;
        public Sprite stagecoachImg;
        public Sprite wellsfargoImg;
        public Sprite saloonImg;
        public Sprite generalstoreImg;
        public Sprite coltsaaImg;
        public Sprite russianRevolverImg;
        public Sprite navyRevolverImg;
        public Sprite carbineImg;
        public Sprite winchesterImg;
        public Sprite scopeImg;
        public Sprite mustangImg;
        public Sprite barrelImg;
        public Sprite dynamiteImg;
        public Sprite jailImg;
        public Sprite catbalouImg;
        public Sprite panicImg;
        public Sprite duelImg;
        public Sprite blank;

        [Header("카드 테두리, 내용")]
        public Image cardInImg;
        public Text cardText;
        public GameObject cardBorder;

        Transform cardPos;
        public Preset originPRS;
        //public float cpos_x;
        //public float cpos_y;
        //public float cpos_z;

        public enum cType
        {
            Bang,
            Avoid,
            Beer,
            Indian,
            MachineGun,
            StageCoach,
            WellsFargo,
            Saloon,
            GeneralStore,
            Colt,
            Russian,
            Navy,
            Carbine,
            Winchester,
            Scope,
            Mustang,
            Barrel,
            Dynamite,
            Jail,
            Catbalou,
            Panic,
            Duel,
            Blank
        }

        public cType cardContent;

        [Header("이동할 카드UI object")]
        public Transform targetUI;
        protected Vector2 startPnt;
        protected Vector2 moveBegin;
        protected Vector2 moveoffset;

        // 카드를 사용한다 안한다
        public bool useCard = false;
        // 카드를 삭제한다
        public bool delCard = false;

        // mycards 에서의 카드 인덱스
        public int idx = 0;

        // storeList에서의 인덱스(잡화점때 삭제 카드 인식에 필요)
        public int storeIdx;

        [Header("카드 사용 판정 패널")]
        public UseCardPanelUI ucpui;
        protected GameObject usecardPanel;

        [Header("카드 삭제 판정 패널")]
        public DelCardPanelUI dcpui;
        protected GameObject delcardPanel;

        [Header("장착 카드 ")]
        public bool equipCard = false;

        // 카드를 들고 있는 플레이어를 의미함(본인)
        protected GameObject player;

        #endregion

        void Awake()
        {
            usecardPanel = GameObject.Find("UseCardPanel");
            ucpui = usecardPanel.GetComponent<UseCardPanelUI>();

            delcardPanel = GameObject.Find("DelCardPanel");
            dcpui = delcardPanel.GetComponent<DelCardPanelUI>();

            // 본인 카드 덱을 찾으려면 본인이 누군지 알아야함
            player = GameManager.Instance.CallMyPlayer();
        }

        public void posValue(Vector3 myCardPos)
        {
            cardPos.position = new Vector3(myCardPos.x, myCardPos.y, myCardPos.z);
            //cpos_x = myCardPos.position.x;
            //cpos_y = myCardPos.position.y;
            //cpos_z = myCardPos.position.z;
        }

        // 카드 이미지와 타입을 맞추는 함수(글자도)
        public void matchImg()
        {
            cardText.fontSize = 90;
            equipCard = false;
            if (this.cardContent == cType.Bang)
            {
                cardInImg.sprite = bangImg;
                cardText.text = "BANG";
            }
            else if (this.cardContent == cType.Avoid)
            {
                cardInImg.sprite = avoidImg;
                cardText.text = "DODGE";
            }
            else if (this.cardContent == cType.Beer)
            {
                cardInImg.sprite = beerImg;
                cardText.text = "BEER";
            }
            else if (this.cardContent == cType.Indian)
            {
                cardInImg.sprite = indianImg;
                cardText.text = "INDIAN";
            }
            else if (this.cardContent == cType.MachineGun)
            {
                cardInImg.sprite = MGImg;
                cardText.text = "GATLING";
            }
            else if (this.cardContent == cType.StageCoach)
            {
                cardInImg.sprite = stagecoachImg;
                cardText.text = "STAGECOACH";
                cardText.fontSize = 63;
            }
            else if (this.cardContent == cType.WellsFargo)
            {
                cardInImg.sprite = wellsfargoImg;
                cardText.text = "WELLS FARGO";
                cardText.fontSize = 58;
            }
            else if (this.cardContent == cType.Saloon)
            {
                cardInImg.sprite = saloonImg;
                cardText.text = "SALOON";
            }
            else if (this.cardContent == cType.GeneralStore)
            {
                cardInImg.sprite = generalstoreImg;
                cardText.text = "STORE";
            }
            else if (this.cardContent == cType.Colt)
            {
                cardInImg.sprite = coltsaaImg;
                cardText.text = "COLT";
            }
            else if (this.cardContent == cType.Russian)
            {
                cardInImg.sprite = russianRevolverImg;
                cardText.text = "RUSSIAN";
                equipCard = true;
            }
            else if (this.cardContent == cType.Navy)
            {
                cardInImg.sprite = navyRevolverImg;
                cardText.text = "NAVY";
                equipCard = true;
            }
            else if (this.cardContent == cType.Carbine)
            {
                cardInImg.sprite = carbineImg;
                cardText.text = "CARBINE";
                equipCard = true;
            }
            else if (this.cardContent == cType.Winchester)
            {
                cardInImg.sprite = winchesterImg;
                cardText.text = "WINCHESTER";
                cardText.fontSize = 63;
                equipCard = true;
            }
            else if (this.cardContent == cType.Scope)
            {
                cardInImg.sprite = scopeImg;
                cardText.text = "SCOPE";
                equipCard = true;
            }
            else if (this.cardContent == cType.Mustang)
            {
                cardInImg.sprite = mustangImg;
                cardText.text = "MUSTANG";
                equipCard = true;
            }
            else if (this.cardContent == cType.Barrel)
            {
                cardInImg.sprite = barrelImg;
                cardText.text = "BARREL";
                equipCard = true;
            }
            else if (this.cardContent == cType.Dynamite)
            {
                cardInImg.sprite = dynamiteImg;
                cardText.text = "DYNAMITE";
                cardText.fontSize = 68;
                equipCard = true;
            }
            else if (this.cardContent == cType.Jail)
            {
                cardInImg.sprite = jailImg;
                cardText.text = "JAIL";
                equipCard = true;
            }
            else if (this.cardContent == cType.Catbalou)
            {
                cardInImg.sprite = catbalouImg;
                cardText.text = "CAT BALOU";
                cardText.fontSize = 65;
            }
            else if (this.cardContent == cType.Panic)
            {
                cardInImg.sprite = panicImg;
                cardText.text = "PANIC!";
            }
            else if (this.cardContent == cType.Duel)
            {
                cardInImg.sprite = duelImg;
                cardText.text = "DUEL";
            }
            else if (this.cardContent == cType.Blank)
            {
                cardInImg.sprite = blank;
                cardText.text = "?";
            }
        }

        // 뽑은 카드는 지정된 위치로 가게하는 함수
        public void MoveTransform (Preset prs, bool useDotween, float dotweenTime = 0)
        {
            if (useDotween)
            {
                Debug.Log("do tween");
                transform.DOMove(prs.pos, dotweenTime);
                transform.DORotateQuaternion(prs.rot, dotweenTime);
                transform.DOScale(prs.scale, dotweenTime);
            }
            else
            {
                transform.position = prs.pos;
                transform.rotation = prs.rot;
                transform.localScale = prs.scale;
            }
        }


        // 카드 선택시 판정 패널 보였다 안보였다 하는 거
        public void PanelOnOFF(int num)
        {
            if(num == 0) // 투명
            {
                Color color = usecardPanel.GetComponent<Image>().color;
                color.a = 0f;
                usecardPanel.GetComponent<Image>().color = color;

                Color color_d = delcardPanel.GetComponent<Image>().color;
                color_d.a = 0f;
                delcardPanel.GetComponent<Image>().color = color_d;
            }
            else if(num == 1) // 불투명
            {
                Color color = usecardPanel.GetComponent<Image>().color;
                color.a = 0.4f;
                usecardPanel.GetComponent<Image>().color = color;

                Color color_d = delcardPanel.GetComponent<Image>().color;
                color_d.a = 0.4f;
                delcardPanel.GetComponent<Image>().color = color_d;
            }
        }


        #region 카드 드래그 기능
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("현재카드 : " + targetUI.GetComponent<Card>().cardContent);

            startPnt = targetUI.position;
            moveBegin = eventData.position;
            // 누르면 커져서 잘 보이게 만듬
            transform.DOScale(transform.localScale * 1.5f, 0.1f);

            if (GameManager.Instance.myTurn == true)
            {
                PanelOnOFF(1);
                useCard = false;

                // 현재 카드 사용 중에는 다른거 취급 못하게 함 
                if (GameManager.Instance.isCard == false)
                {
                    // 패널에 정보 넘기기
                    ucpui.TargetMatch(targetUI);
                    dcpui.TargetMatch(targetUI);
                }
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            //Debug.Log("마우스 뗴기");
            PanelOnOFF(0);
            Vector3 vec = new Vector3(1, 1, 1);
            transform.DOScale(vec, 0.1f);

            // 잡화점이 켜져있을 때
            if (player.GetComponent<PlayerInfo>().isStore == true)
            {
                // storelist에 속한 index 찾기
                var item = targetUI.GetComponent<Card>();
                int idx = GameManager.Instance.storecardList.FindIndex(a => a == item);

                Debug.Log("storlist : " + idx);

                // 스토어 리스트에서 본인 리스트로 카드 복제
                player.GetComponent<UI>().StoreToMy(item);

                // 스토어리스트에 정보 삭제
                player.GetComponent<PhotonView>().RPC("StoreListSync", RpcTarget.All, idx);

                // 내 상태 변경(잡화점 끝)
                player.GetComponent<PhotonView>().RPC("StoreSync", RpcTarget.All, 1);


                // 스토어 리스트에 카드 오브젝트 destroy(모든 사람)
                foreach (GameObject player in GameManager.Instance.turnList)
                {
                    player.GetComponent<PhotonView>().RPC("StoreCardDel", RpcTarget.All, storeIdx);
                }

                //Destroy(targetUI.gameObject);
            }
            if (player.GetComponent<PlayerInfo>().useCat)
            {
                // (눈속임) 어떤 카드를 클릭하든 랜덤으로 상대편 카드 하나 삭제
                Debug.Log("catcard 작동중 ");
                player.GetComponent<PlayerInfo>().useCat = false;
                MineUI.Instance.CatbalouCardUIDel();
            }
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (GameManager.Instance.myTurn == true)
            {
                if (player.GetComponent<PlayerInfo>().isTarget == 3)
                {
                    // 타겟이여서 회피카드를 내야할 때
                    if (player.GetComponent<PlayerInfo>().targetedBang)
                    {
                        if (targetUI.GetComponent<Card>().cardContent != cType.Avoid)
                            return;
                    }
                    // 타겟이여서 반격카드를 내야할 때
                    else if (player.GetComponent<PlayerInfo>().targetedIndian)
                    {
                        if (targetUI.GetComponent<Card>().cardContent != cType.Bang)
                            return;
                    }
                }

                // 일반적인 상황
                moveoffset = eventData.position - moveBegin;
                targetUI.position = startPnt + moveoffset;
            }
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            // 카드 사용 위치에 올려놔서 카드를 사용함
            if (useCard) 
            {
                if (player.GetComponent<PlayerInfo>().isTarget == 3)
                {
                    // 타겟이여서 회피카드를 내야할 때
                    if (player.GetComponent<PlayerInfo>().targetedBang)
                    {
                        if (targetUI.GetComponent<Card>().cardContent != cType.Avoid)
                            return;
                    }
                    // 타겟이여서 반격카드를 내야할 때
                    else if (player.GetComponent<PlayerInfo>().targetedIndian)
                    {
                        if (targetUI.GetComponent<Card>().cardContent != cType.Bang)
                            return;
                    }
                }

                GameManager.Instance.isCard = true;
                StartCoroutine("CardUse");
            }
            // 카드 삭제 위치에 올려놔서 카드를 삭제함
            else if(delCard)
            {
                GameManager.Instance.isCard = true;
                StartCoroutine("CardUse");
            }
            // 카드를 사용하지 않고 다시 덱으로 
            else
            {
                targetUI.position = startPnt;
            }
        }

        #endregion

        #region 카드 사용 후 동작
        IEnumerator CardUse()  
        {
            // 현재 카드가 내 카드리스트에서 몇번쨰인지
            idx = 0;
            for (int i = 0; i < player.GetComponent<PlayerInfo>().mycards.Count; i++)
            {
                if (useCard)
                {
                    if (player.GetComponent<PlayerInfo>().mycards[i].useCard == true)
                        idx = i;
                }
                else if(delCard)
                {
                    if (player.GetComponent<PlayerInfo>().mycards[i].delCard == true)
                        idx = i;
                }
            }

            if (useCard)
                StartCoroutine("CardUse1");
            else if (delCard)
                StartCoroutine("CardUse2");

            yield return new WaitForEndOfFrame();
        }

        // 카드 사용 애니메이션
        IEnumerator CardUse1()  
        {
            //카드 사용시 지정 위치로 이동
            float dtime = 0.5f;
            
            int width = Screen.width;
            int height = Screen.height;
            Vector3 vec = new Vector3(width / 2, height / 2, 0);
            this.transform.DOMove(vec, dtime);
            yield return new WaitForSeconds(1f);

            StartCoroutine("CardUse2");
        }

        // 본인 덱에서 카드 삭제 및 전체 카드더미 맨뒤에 다시 추가 그리고 카드더미 상태 동기화
        IEnumerator CardUse2()
        {
            // 카드 사용 후 
            if (useCard)
            {
                if(equipCard) // 장착 카드일 때
                    GameManager.Instance.AfterCardUse(cardContent, 0, true);
                else // 소비 카드 일 때
                    GameManager.Instance.AfterCardUse(cardContent, 0, false);
            }
            // 카드 삭제 후 
            else if (delCard)
                GameManager.Instance.AfterCardUse(cardContent, 1, false);

            // 내 리스트에서 사용한 카드 삭제(장착 카드 제외)
            player.GetComponent<PlayerInfo>().mycards.RemoveAt(idx);

            // 카드 재정렬
            MineUI.Instance.CardAlignment();

            // 잠시 대기
            yield return new WaitForSeconds(1f);

            // 다른 카드 선택 가능하게 풀어줌
            GameManager.Instance.isCard = false;

            StartCoroutine("CardUse3", 0);

            yield return new WaitForEndOfFrame();
        }

        IEnumerator CardUse3()
        {
            // 카드 사용 후 파괴 지시
            ucpui.Des();
            yield return null;
        }
    }
    #endregion


}