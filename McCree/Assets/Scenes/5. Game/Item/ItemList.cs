using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.ThreeCS.McCree
{
    public class ItemList : MonoBehaviour
    {
        public Item item; // 아이템
        private int Count; // 아이템 개수

        [HideInInspector]
        private GameObject itemListObj;

        [SerializeField]
        private Image itemImg; // 사진 이미지
        [SerializeField]
        private Text nameText; // 이름 텍스트
        [SerializeField]
        private Text CountText; // 개수 텍스트



        private void Awake()
        {
            itemListObj = this.gameObject;
            itemImg.sprite = item.itemImg;
            nameText.text = item.itemName;
            Count = 0;

            itemListObj.GetComponent<Button>().onClick.AddListener(Detail_item);

            itemListObj.SetActive(false);
        }

        public int itemCount
        {
            get { return Count; }
            set
            {
                Count = value;
                CountText.text = Count.ToString();

                if (item.ability == Item.iType.Bang)
                    MineUI.Instance.bangCount.text = CountText.text;
                else if (item.ability == Item.iType.Avoid)
                    MineUI.Instance.avoidCount.text = CountText.text;

                if (Count <= 0)
                    itemListObj.SetActive(false);
                else
                    itemListObj.SetActive(true);
            }
        }

        public Text countText
        {
            get { return CountText; }
        }

        public void Detail_item()
        {
            MineUI.Instance.title_Item.text = item.itemName;
            MineUI.Instance.explain_Item.text = item.itemExplain;
        }
    }
}
