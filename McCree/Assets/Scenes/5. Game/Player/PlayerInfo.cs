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
        private static PlayerInfo pInstance;

        public static PlayerInfo Instance
        {
            get { return pInstance; }
        }

        private int _hp;

        public int hp
        {
            get { return _hp;  }
            set
            {
                int prehp = _hp;
                
                _hp = value;

                if (prehp - 1 == _hp) // hp가 1깎였을때 캐릭터 UI 총알 이미지 바꿈
                {
                    Image tempImg = ui.hpImgs[playerInfo.hp].GetComponent<Image>();
                    tempImg.sprite = ui.emptyBullet;

                    if (photonView.IsMine) // MineUI 체력 이미지 바꿈
                    {
                        Image tempImg2 = MineUI.Instance.mineUIhpImgs[playerInfo.hp].GetComponent<Image>();
                        tempImg2.sprite = MineUI.Instance.emptyHealth;
                    }
                }
                else if (prehp + 1 == _hp) // hp가 1업했을때 
                {
                    Image tempImg = ui.hpImgs[playerInfo.hp].GetComponent<Image>();
                    tempImg.sprite = ui.fullBullet;

                    if (photonView.IsMine) // MineUI 체력 이미지 바꿈
                    {
                        Image tempImg2 = MineUI.Instance.mineUIhpImgs[playerInfo.hp].GetComponent<Image>();
                        tempImg2.sprite = MineUI.Instance.fullHealth;
                    }
                }
                
                Debug.Log(hp + "  " + _hp);
            }
        }
        public int maxHp;

        public List<ItemList> myItemList;
        // 0번째 Bang
        // 1번째 Avoid
        // 2번째 Heal

        [Header("내가 지금 가지고 있는 카드리스트")]
        public List<Card> mycards = new List<Card>();

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
                    playerManager.EquipedNone = false;
                    playerManager.EquipedPistol = true;
                    playerManager.EquipedRifle = false;
                    //animSync.SendPlayAnimationEvent(photonView.ViewID, "Pistol", "Trigger");

                    _equipedWeapon.gameObject.transform.SetParent(pistolPos);
                    _equipedWeapon.transform.localPosition = new Vector3(0f, 0f, 0f);
                    _equipedWeapon.transform.localRotation = Quaternion.identity;

                    riflePos.gameObject.SetActive(false);
                    pistolPos.gameObject.SetActive(true);
                }
                else if (_equipedWeapon.wepaon.kind == Weapon.iType.Rifle)
                {
                    playerManager.EquipedNone = false;
                    playerManager.EquipedPistol = false;
                    playerManager.EquipedRifle = true;
                    //animSync.SendPlayAnimationEvent(photonView.ViewID, "Rifle", "Trigger");

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
            pInstance = this;

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

            StartCoroutine(CurrentHP());
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

        IEnumerator CurrentHP()
        {
            // 너무 빨리 측정하면 hp 동기화 하기 전이라 사망처리됨 
            yield return new WaitForSeconds(5f);
            while (true)
            {
                //Debug.Log("현재 체력 : " + hp);
                if (hp <= 0)
                {
                    isDeath = true;
                    break;
                }

                yield return new WaitForSeconds(1f);
            }
            yield return null;
        }
    }
}