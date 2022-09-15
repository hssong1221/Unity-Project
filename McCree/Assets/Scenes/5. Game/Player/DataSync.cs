using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

using Photon.Pun;
// 이 스크립트는 게임매니저에서 들어오는 data를 전체에게 적용시키기 위한 
// 즉 RPC 전용 스크립트입니다.
// 또는 적용하기 전에 해보는 실험용

namespace com.ThreeCS.McCree
{
    public class DataSync : Controller
    {
        public bool nextSignal = false;


        // 보안관이 뱅 버튼 눌렀을 때 모든 사람의 인구수ui가 꺼져야함
        [PunRPC]
        public void StartUIOff()
        {
            GameManager.Instance.startPanel.SetActive(false);
        }

        [PunRPC]
        public void Gameloop()
        {
            // 본인만 진입시켜서 중복을 막음
            if (photonView.IsMine)
                GameManager.Instance.GLStart();
        }

        [PunRPC]
        public void MyTurn(int idx)
        {
            if (photonView.IsMine)
            {
                Debug.Log("현재 순서 : " + idx);
                MineUI.Instance.TempButton.SetActive(true);
            }
            else
            {
                MineUI.Instance.TempButton.SetActive(false);
            }
        }
        [PunRPC]
        public void TurnIndexPlus()
        {
            GameManager.Instance.tidx++;
        }

       
    }
}