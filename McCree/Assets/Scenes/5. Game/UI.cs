using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class UI : Controller
    {
        #region Variable Field

        
        [Header("체력관련 UI")]
        public Canvas hpCanvas;

        public GameObject[] hpImgs;

        public Sprite fullBullet;
        public Sprite emptyBullet;
        public Text nickName; // 닉네임
        



        [Header("뱅 준비 사거리 표시 관련 UI")]
        public Canvas attackRangeCanvas;  // 공격 사거리 캔버스
        public Image indicatorRangeCircle;// 공격 사거리 이미지
        public int attackRange; // 기본 공격 사거리 일단 1로 설정해놓은상태



        [Header("Bang! 말풍선 관련 UI")]
        public Canvas bangCanvas;         // 뱅 말풍선 캔버스
        public Image bangGifImg;          // 뱅 말풍선 gif 이미지

        Vector3 hpOffset;
        Vector3 bangOffset;


        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
            base.Awake();  // Controller Awake 함수 그대로 사용

            hpOffset = new Vector3(0, 2.0f, 0);
            bangOffset = new Vector3(0, 3.0f, 0);

            if (photonView.IsMine)
            {
                nickName.text = PhotonNetwork.LocalPlayer.NickName;
            }
            else
            {
                nickName.text = GetComponent<PhotonView>().Owner.NickName;
            }
        }


        private void Start()
        {
            // 기본사거리
            attackRange = 1;
            // 기본 Indicator Range img 크기
            indicatorRangeCircle.rectTransform.localScale = new Vector3(attackRange, attackRange, 0);
            // 공격범위 UI 꺼주기
            indicatorRangeCircle.GetComponent<Image>().enabled = false;
            // 뱅 말풍선 UI 꺼주기
            bangGifImg.GetComponent<Image>().enabled = false;
    }

        void Update()
        {
            // 플레이어 머리 위 UI 각도 고정
            hpCanvas.transform.LookAt(hpCanvas.transform.position + Camera.main.transform.forward);
            bangCanvas.transform.LookAt(bangCanvas.transform.position + Camera.main.transform.forward);

            if (photonView.IsMine)
            {
                // 각자 캐릭터 머리 위 UI 위치 고정
                hpCanvas.transform.position = character.transform.position + hpOffset;
                bangCanvas.transform.position = character.transform.position + bangOffset;

                if (playerInfo.isDeath && playerInfo.hp <= 0)
                    GameManager.Instance.LeaveRoom();
            }
            else
                return;

            
           

            
        }

        #endregion

        #region Methods
        
        

        #endregion


    }


}