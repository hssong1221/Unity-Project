using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    }
}