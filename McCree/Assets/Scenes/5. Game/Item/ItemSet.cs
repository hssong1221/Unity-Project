using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace com.ThreeCS.McCree
{
    public class ItemSet : MonoBehaviour
    {
        public Dictionary<Item.iType, int> itemSet;

        Item.iType picked;
        public Item.iType Pick_Item()
        {
            int random = Random.Range(1, 101); // 1 ~ 100

            if (1 <= random && random <= 50)
                picked = Item.iType.Bang;
            else if (51 <= random && random <= 75)
                picked = Item.iType.Avoid;
            else if (76 <= random && random <= 100)
                picked = Item.iType.Heal;

            return picked;
        }
    }
}
