using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using DG.Tweening;

namespace com.ThreeCS.McCree
{
    public class Card : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler, IPointerUpHandler
    {
        #region variable

        [Header("카드 그림")]
        public Sprite bangImg;
        public Sprite avoidImg;
        public Sprite healImg;

        [Header("카드 테두리, 내용")]
        public Image cardInImg;
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
            Heal
        }

        public cType cardContent;

        [Header("이동할 카드UI object")]
        public Transform targetUI;
        protected Vector2 startPnt;
        protected Vector2 moveBegin;
        protected Vector2 moveoffset;

        // 카드를 사용한다 안한다
        public bool useCard;

        // 카드 사용 판정 패널
        public UseCardPanelUI ucpui;
        protected GameObject usecardPanel;

        #endregion

        void Awake()
        {
            usecardPanel = GameObject.Find("UseCardPanel");
            ucpui = usecardPanel.GetComponent<UseCardPanelUI>();
        }

        public void posValue(Vector3 myCardPos)
        {
            cardPos.position = new Vector3(myCardPos.x, myCardPos.y, myCardPos.z);
            //cpos_x = myCardPos.position.x;
            //cpos_y = myCardPos.position.y;
            //cpos_z = myCardPos.position.z;
        }

        // 카드 이미지와 타입을 맞추는 함수
        public void matchImg()
        {
            if (this.cardContent == cType.Bang)
            {
                cardInImg.sprite = bangImg;
            }
            else if (this.cardContent == cType.Avoid)
            {
                cardInImg.sprite = avoidImg;
            }
            else if (this.cardContent == cType.Heal)
            {
                cardInImg.sprite = healImg;
            }
        }

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

        // 벡터로 이동하는 거고 화면 중앙으로 부드럽게 이동하는 것을 만들면 될거 같음 
        // 중앙 프리셋을 만들던지 그렇게 하고
        // 두트윈 사용법을 좀 배워야 하고
        public void MoveCenter()
        {
            float dtime = 0f;
            Vector3 temp = new Vector3(0, 0, 0);
            this.transform.DOMove(temp, dtime);
        }


        // 카드 드래그 기능 
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            startPnt = targetUI.position;
            moveBegin = eventData.position;
            Debug.Log("현재카드 : " + targetUI.GetComponent<Card>().cardContent);
            GameManager.Instance.usecardPanel.SetActive(true);
            useCard = false;

            // 사용 패널에 정보 넘기기
            ucpui.TargetMatch(targetUI);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("마우스 뗴기");
            GameManager.Instance.usecardPanel.SetActive(false);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            moveoffset = eventData.position - moveBegin;
            targetUI.position = startPnt + moveoffset;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("드래그 뗴기" + useCard);

            // 카드 사용 위치에 올려놔서 카드를 사용함
            if (useCard == true) 
            {
                Debug.Log("카드 사용함");
                MoveCenter();
            }
            // 카드를 사용하지 않고 다시 덱으로 
            else if (useCard == false)
            {
                targetUI.position = startPnt;
                GameManager.Instance.usecardPanel.SetActive(false);
            }
            
        }

        public void CardUse()
        {

        }
    }
}