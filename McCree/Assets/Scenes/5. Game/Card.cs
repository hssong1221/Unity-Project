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


        cType content;
        [HideInInspector]
        public Image contentImg;

        public Card(cType content)
        {
            ability = content;

            Debug.Log(bangImg);

            if (content == cType.Bang)
            {
                cardImg.sprite = bangImg;
                Debug.Log(cardImg);
            }
            else if (content == cType.Avoid)
            {
                cardImg.sprite = avoidImg;
                Debug.Log(cardImg);
            }
            else if (content == cType.Heal)
            {
                cardImg.sprite = healImg;
                Debug.Log(cardImg);
            }

        }

        public cType ability
        {
            get => content;
            set => content = value;
        }

        public Image cardImg
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

    }
}