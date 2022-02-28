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


        public List<Card> mycards = new List<Card>();

        public bool isDeath;

        void Awake()
        {
            base.Awake();  // Controller Awake 함수 그대로 사용
            isDeath = false;
        }

        public void Show_Hp()
        {
            for (int i = 0; i < ui.hpImgs.Length; i++)
            {
                if (i < maxHp)
                    ui.hpImgs[i].SetActive(true);
                else
                    ui.hpImgs[i].SetActive(false);
            }
        }

        //public void Show_Cards()
        //{
        //    for (int i = 0; i < mineUI.cards.Length; i++)
        //    {
        //        ui.hpImgs[i].SetActive(true);
        //    }
        //    // 최대 체력 만큼 카드 가지고있어야하는 코드 구현해야함
        //}
    }
}