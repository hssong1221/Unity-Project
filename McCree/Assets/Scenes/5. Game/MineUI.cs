using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class MineUI : MonoBehaviour
    {
        //public Image weaponImg;
        //public Text weaponName;

        public PlayerInfo playerInfo;

        [Header("개인 (나한테만 보이는) UI")]
        public GameObject[] cardImgs;
        public GameObject[] cardBorders;



        public void Show_Start_Cards()
        {
            for (int i = 0; i < cardImgs.Length; i++)
            {
                if (i < playerInfo.mycards.Count)
                {
                    cardImgs[i].GetComponent<Image>().sprite = playerInfo.mycards[i].cardImg.sprite;
                    cardImgs[i].SetActive(true);
                }
                else
                    cardBorders[i].SetActive(false);
            }
        }
    }
}