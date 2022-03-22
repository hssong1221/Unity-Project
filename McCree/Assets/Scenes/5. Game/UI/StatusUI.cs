using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace com.ThreeCS.McCree
{
    public class StatusUI : MonoBehaviour
    {
        private static StatusUI pInstance;

        public static StatusUI Instance
        {
            get { return pInstance; }
        }

        [Header("아이템")]
        public Text item_Explain;
        public GameObject itemList;
        public Transform content;

        [Header("직업")]
        public Text job_Name;
        public Text job_Explain;

        [Header("장착 총")]
        public Image weapon_Img;
        public Text weapon_Name;
        public Text weapon_Range;

        [Header("능력")]
        public Text Ability_Name;
        public Text Ability_Explain;


        void Awake()
        {
            // 어디서든 쓸 수 있게 인스턴스화
            pInstance = this;
        }
    }
}