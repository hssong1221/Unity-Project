using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.ThreeCS.McCree
{
    [CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
    public class Item : ScriptableObject
    {
        [Header("UI 아이템 이미지")]
        public Sprite itemImg;

        //[Header("SpriteRenderer 아이템 이미지")]
        //public SpriteRenderer spriteImg;

        public string itemName;
        public string itemExplain;
        public iType ability;


        public enum iType
        {
            Bang,
            Avoid,
            Heal
        }
    }





}
