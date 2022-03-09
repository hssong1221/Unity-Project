using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace com.ThreeCS.McCree
{
    public class ItemSet : MonoBehaviour
    {
        public List<Item.iType> itemSet;

        Item.iType picked;

        int itemCount;

        private void Awake()
        {
            itemCount = System.Enum.GetValues(typeof(Item.iType)).Length;
        }


        public Item.iType Pick_Item()
        {
            int random = Random.Range(1, 11); // 1 ~ 10

            switch (random)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    picked = Item.iType.Bang;
                    break;
                case 5:
                case 6:
                case 7:
                    picked = Item.iType.Avoid;
                    break;
                case 8:
                case 9:
                case 10:
                    picked = Item.iType.Heal;
                    break;
            }
            return picked;
        }
    }
}
