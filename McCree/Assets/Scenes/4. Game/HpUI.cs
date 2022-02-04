using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class HpUI : Controller
    {

        public int hp;
        public int maxHp;

        private Transform imgPos;

        public GameObject[] hpImgs;
        
        public Sprite fullBullet;
        public Sprite emptyBullet;

        Vector3 offset;

        Image healthImg;


        void Awake()
        {
            base.Awake();  // PlayerController Awake 함수 그대로 사용

            if (photonView.IsMine)
            {
                imgPos = transform.Find("HpImgs");
                for (int i=0; i < hpImgs.Length; i++)
                {
                    //if (i < hp)
                    //    hpImgs[i].GetComponent<Sprite>() = fullBullet;
                    //else
                    //    hpImgs[i].sprite = emptyBullet;

                    if (i < maxHp)
                        hpImgs[i].SetActive(true);
                    else
                        hpImgs[i].SetActive(false);
                }
            }
            offset = new Vector3(0, 2.0f, 0);
        }

        [PunRPC]
        void TestDamaged()
        {
            Image tempImg = hpImgs[hp - 1].GetComponent<Image>();
            tempImg.sprite = emptyBullet;
            hp--;

            Debug.Log("hp: " + hp);
        }

        void TestDamagedStart()
        {
            //photonView.RPC("TestDamaged", RpcTarget.Others);
            TestDamaged();
        }

        private void Start()
        {
            Debug.Log("pv: " + photonView);
            InvokeRepeating("TestDamagedStart", 3f, 1f);
        }

        void Update()
        {
            transform.LookAt(transform.position + Camera.main.transform.forward);

            if (photonView.IsMine)
                imgPos.position = character.transform.position + offset;
        }
    }
}