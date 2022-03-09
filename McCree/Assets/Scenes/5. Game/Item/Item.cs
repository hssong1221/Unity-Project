using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.ThreeCS.McCree
{
    public class Item : MonoBehaviour
    {
        [Header("아이템 이미지")]
        public Sprite bangImg;
        public Sprite avoidImg;
        public Sprite healImg;

        [SerializeField]
        private iType Ability;
        [SerializeField]
        private SpriteRenderer Item2DObj;
        [SerializeField]
        private Sprite ItemImg;


        //private void Awake()
        //{
        //    ItemImg = GetComponent<SpriteRenderer>();
        //}

        public enum iType
        {
            Bang,
            Avoid,
            Heal
        }


        public iType ability
        {
            get { return Ability; }
            set
            {
                Ability = value;
                if (Ability == iType.Bang)
                {
                    ItemImg = bangImg;
                    Item2DObj.sprite = bangImg;
                }
                else if (Ability == iType.Avoid)
                {
                    ItemImg = avoidImg;
                    Item2DObj.sprite = avoidImg;
                }
                else if (Ability == iType.Heal)
                {
                    ItemImg = healImg;
                    Item2DObj.sprite = healImg;
                }
            }
        }

        public Sprite itemImg
        {
            get { return ItemImg; }
        }

        public SpriteRenderer item2DObj
        {
            get { return Item2DObj; }
        }

        public Item(iType content)
        {
            ability = content;
        }

  
    }
}
