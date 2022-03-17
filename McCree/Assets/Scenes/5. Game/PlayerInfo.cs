using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class PlayerInfo : Controller
    {
        public int hp;  // 기본 5
        public int maxHp;
        public int damage = 1;

        public List<ItemList> myItemList;
        // 0번째 Bang
        // 1번째 Avoid
        // 2번째 Heal

        public List<Quest> myQuestList;


        public Transform content;

        public bool isDeath;

        void Awake()
        {
            base.Awake();  // Controller Awake 함수 그대로 사용
            isDeath = false;

            MineUI.Instance.statusPanel.gameObject.SetActive(true);
            content = GameObject.FindGameObjectWithTag("Content").transform;

            myItemList = new List<ItemList>();

            myQuestList = new List<Quest>();

            for (int i=0; i<content.childCount; i++)
            {
                myItemList.Add(content.GetChild(i).GetComponent<ItemList>());
            }
            MineUI.Instance.statusPanel.gameObject.SetActive(false);
        }

        public void Show_Hp()
        {
            for (int i = 0; i < ui.hpImgs.Length; i++)
            {
                if (i < maxHp)
                {
                    ui.hpImgs[i].SetActive(true);

                    if (photonView.IsMine)
                        MineUI.Instance.mineUIhpImgs[i].SetActive(true);
                }
                else
                {
                    ui.hpImgs[i].SetActive(false);

                    if (photonView.IsMine)
                        MineUI.Instance.mineUIhpImgs[i].SetActive(false);
                }
            }
        }
    }
}