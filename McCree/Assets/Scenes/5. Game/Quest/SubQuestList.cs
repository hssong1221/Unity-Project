using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace com.ThreeCS.McCree
{
    public class SubQuestList : MonoBehaviour
    {
        private Quest _quest;

        [HideInInspector]
        public GameObject obj;
        [HideInInspector]
        public Button objBtn;
        public Text questTitle;


        public Quest quest
        {
            get { return _quest; }

            set
            {
                _quest = value;

                questTitle.text = _quest.questTitle;
            }
        }


        void Awake()
        {
            obj = gameObject;
            objBtn = obj.GetComponent<Button>();
            objBtn.onClick.AddListener(Quest_Detail);
        }

        void Quest_Detail()
        {
            MineUI.Instance.questNpcName.text = quest.npcName;
            MineUI.Instance.questNpcImg.sprite = quest.npcImg;
            MineUI.Instance.questTitle2.text = quest.questTitle;
            MineUI.Instance.questContent.text = quest.questContent;

            MineUI.Instance.questDetail.SetActive(true);
            MineUI.Instance.isquestDetailopen = true;
        }

    }
}
