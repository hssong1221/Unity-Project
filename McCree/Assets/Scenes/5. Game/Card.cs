using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace com.ThreeCS.McCree
{
    public class Card : MonoBehaviour  // 카드 하나하나의 내용
    {

        [Header("카드 그림")]
        public Sprite bangImg;
        public Sprite avoidImg;
        public Sprite healImg;

        [Header("카드 테두리, 내용")]
        public Image cardInImg;
        public GameObject cardBorder;

        cType content;
        Sprite contentImg;

        Transform cardPos;
        public PRS originPRS;
        //public float cpos_x;
        //public float cpos_y;
        //public float cpos_z;



        public Card(cType content)
        {
            ability = content;
        }

        public cType ability
        {
            get => content;
            set => content = value;
        }

        public Sprite cardImg
        {
            get => contentImg;
            set => contentImg = value;
        }

        public enum cType
        {
            Bang,
            Avoid,
            Heal
        }


        public void posValue(Vector3 myCardPos)
        {
            cardPos.position = new Vector3(myCardPos.x, myCardPos.y, myCardPos.z);
            //cpos_x = myCardPos.position.x;
            //cpos_y = myCardPos.position.y;
            //cpos_z = myCardPos.position.z;
        }

        public void matchImg()
        {
            if (this.content == cType.Bang)
            {
                this.cardImg = bangImg;
            }
            else if (this.content == cType.Avoid)
            {
                this.cardImg = avoidImg;
            }
            else if (this.content == cType.Heal)
            {
                this.cardImg = healImg;
            }
            cardInImg.sprite = this.cardImg;
        }

        public void MoveTransform (PRS prs, bool useDotween, float dotweenTime = 0)
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

    }
}