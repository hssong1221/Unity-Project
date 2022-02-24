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
    }
}