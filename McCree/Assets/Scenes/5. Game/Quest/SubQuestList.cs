using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace com.ThreeCS.McCree
{
    public class SubQuestList : MonoBehaviour
    {
        private Quest_Obj _questObj;

        [HideInInspector]
        public GameObject obj;
        [HideInInspector]
        public Button objBtn;
        public Text questTitle;


        public Quest_Obj questObj
        {
            get { return _questObj; }

            set
            {
                _questObj = value;

                questTitle.text = _questObj.quest.questTitle;
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
            MineUI.Instance.questNpcName.text = questObj.quest.npcName;
            MineUI.Instance.questNpcImg.sprite = questObj.quest.npcImg;
            MineUI.Instance.questTitle2.text = questObj.quest.questTitle;
            MineUI.Instance.questContent.text = questObj.questContent_progress;


            MineUI.Instance.questDetail.SetActive(true);
            MineUI.Instance.isquestDetailopen = true;
        }

    }
}
