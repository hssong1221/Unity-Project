using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace com.ThreeCS.McCree 
{
    public class GameStartManager : MonoBehaviour
    {
        [Header("게임 시작 관련 UI(의자 주변)")]
        public Canvas startCanvas;
        public GameObject startPanel;
        public Text playerNumText; // 플레이어 앉은 숫자
        public static int playerSitNum;   // 앉아 있는 숫자
        public Button bangBtn;

        public static Action startUIOffAction;
        public static Action bangBtnOnAction;
        public static Action setPlayerNumAction;

        void Start()
        {
            playerSitNum = 0;

            startPanel.SetActive(true);
            bangBtn.onClick.AddListener(BangBtnClick);
            bangBtn.gameObject.SetActive(false);

            startUIOffAction = () => { startPanelOff(); };
            bangBtnOnAction = () => { bangBtnOn(); };
            setPlayerNumAction = () => { SetPlayerNumText(); };

            // 임시로 켜놓은 거니까 무조건 없애야함
            bangBtnOn();
        }

        void Update()
        {
            // 인원 체크 UI가 계속 정면을 보게 만듬
            startCanvas.transform.LookAt(startCanvas.transform.position + Camera.main.transform.forward);
        }

        // 게임 시작하기 위한 버튼
        public void BangBtnClick()
        {
            MasterPlayerManager.masterPlayerAction();

            foreach (GameObject player in GameManager.Instance.playerList)
            {
                // 모든 사람의 turnlist에 정보 저장을 위함
                player.GetComponent<PhotonView>().RPC("StartUIOff", RpcTarget.All);
                // 모든 사람의 gameloop 진입을 위함
                player.GetComponent<PhotonView>().RPC("GameLoop", RpcTarget.All);
                // 머리위 HP 닉네임 위치 보정
                player.GetComponent<PhotonView>().RPC("UIMatch", RpcTarget.All);
            }
        }

        public void startPanelOff()
        {
            startPanel.SetActive(false);
        }

        public void bangBtnOn()
        {
            bangBtn.gameObject.SetActive(true);
        }

        public void SetPlayerNumText()
        {
            // 앉은 인원 / 전체 인원
            playerNumText.text = playerSitNum + " / " + GameManager.Instance.playerList.Length;
        }
    }
}


