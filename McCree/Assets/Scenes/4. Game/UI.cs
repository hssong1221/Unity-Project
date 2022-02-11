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
        [Header("게임 시작 관련 UI")]
        public GameObject jobPanel;
        public RectTransform jobBoard;
        public Text jobText;
        public float uiSpeed;
        
        [Header("체력관련 UI")]
        public Canvas hpCanvas;
        public int hp;
        public int maxHp;
        public int damage = 1;

        public GameObject[] hpImgs;

        public Sprite fullBullet;
        public Sprite emptyBullet;


        [Header("뱅 준비 사거리 표시 관련 UI")]
        public Canvas attackRangeCanvas;  // 공격 사거리 캔버스
        public Image indicatorRangeCircle;// 공격 사거리 이미지
        public int attackRange; // 기본 공격 사거리 일단 1로 설정해놓은상태



        [Header("Bang! 말풍선 관련 UI")]
        public Canvas bangCanvas;         // 뱅 말풍선 캔버스
        public Image bangSpeechBubble;    // 뱅 말풍선 이미지
        public Text bangText;             // 뱅 말풍선 텍스트

        Vector3 offset;


        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
            base.Awake();  // Controller Awake 함수 그대로 사용

            for (int i = 0; i < hpImgs.Length; i++)
            {

                if (i < maxHp)
                    hpImgs[i].SetActive(true);
                else
                    hpImgs[i].SetActive(false);

            }
            offset = new Vector3(0, 2.0f, 0);

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
            bangSpeechBubble.GetComponent<Image>().enabled = false;
            bangText.enabled = false;

        }

        void Update()
        {
            if (photonView.IsMine)
            {
                if (this.hp <= 0)
                    GameManager.Instance.LeaveRoom();
            }
            else
                return;

            // 플레이어 머리 위 UI 각도 고정
            hpCanvas.transform.LookAt(hpCanvas.transform.position + Camera.main.transform.forward);
            bangCanvas.transform.LookAt(bangCanvas.transform.position + Camera.main.transform.forward);
            // 머리 위 UI 위치 고정
            hpCanvas.transform.position = character.transform.position + offset;
            bangCanvas.transform.position = character.transform.position + offset;

           
        }

        #endregion

        #region Methods
        
        

        #endregion


    }


}