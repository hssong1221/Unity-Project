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

        public List<SubQuestList> myQuestList;


        protected Transform content;



        public Transform pistolPos;
        //public Transform shotgunPos;
        public Transform riflePos;

        [SerializeField]
        protected Weapon_Obj _equipedWeapon;
        public Weapon_Obj equipedWeapon
        { 
            get { return _equipedWeapon; }
            set
            {

                _equipedWeapon = value;

                if(_equipedWeapon.wepaon.kind == Weapon.iType.Pistol)
                {
                    animator.SetTrigger("Pistol");

                    _equipedWeapon.gameObject.transform.SetParent(pistolPos);
                    _equipedWeapon.transform.localPosition = new Vector3(0f, 0f, 0f);
                    _equipedWeapon.transform.localRotation = Quaternion.identity;

                    riflePos.gameObject.SetActive(false);
                    pistolPos.gameObject.SetActive(true);
                }
                else if (_equipedWeapon.wepaon.kind == Weapon.iType.Rifle)
                {
                    animator.SetTrigger("Rifle");

                    _equipedWeapon.gameObject.transform.SetParent(riflePos);
                    _equipedWeapon.transform.localPosition = new Vector3(0f, 0f, 0f);
                    _equipedWeapon.transform.localRotation = Quaternion.identity;

                    pistolPos.gameObject.SetActive(false);
                    riflePos.gameObject.SetActive(true);
                }
            }
        }

        public bool isDeath;

        void Awake()
        {
            base.Awake();  // Controller Awake 함수 그대로 사용
            isDeath = false;

            MineUI.Instance.statusPanel.gameObject.SetActive(true);
            content = GameObject.FindGameObjectWithTag("Content").transform;

            myItemList = new List<ItemList>();

            myQuestList = new List<SubQuestList>();

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