using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

using Photon.Pun;
// 이 스크립트는 다른 게임 오브젝트(게임매니저, 카드)에서 들어오는 data를 전체에게 적용시키기 위한 
// 즉 RPC 전용 스크립트입니다.
// 또는 적용하기 전에 해보는 실험용

namespace com.ThreeCS.McCree
{
    public class DataSync : Controller
    {
        // 보안관이 뱅 버튼 눌렀을 때 모든 사람의 인구수ui가 꺼져야함
        [PunRPC]
        public void StartUIOff()
        {
            GameManager.Instance.startPanel.SetActive(false);
        }

        // 게임 루프에 진입 명령
        [PunRPC]
        public void Gameloop()
        {
            // 본인만 진입시켜서 중복을 막음
            if (photonView.IsMine)
                GameManager.Instance.GLStart();
        }

        // 본인 턴에 작동해서 턴 종료버튼이 본인에게만 보임
        [PunRPC]
        public void MyTurn(int idx)
        {
            if (photonView.IsMine)//(혹시 몰라서 추가)
            {
                Debug.Log("현재 순서 : " + idx);
                MineUI.Instance.NextButton.SetActive(true);
            }
        }

        // 현재 턴이 누군지 전체에게 알림
        [PunRPC]
        public void TurnIndexPlus()
        {
            GameManager.Instance.tidx++;
        }

        // 사용한 카드를 카드더미에 넣고 전체에게 동기화
        [PunRPC]
        public void CardDeckSync(Card.cType content)
        {
            Debug.Log("datsync : " + content);
            Card card = new Card();
            card.cardContent = content;
            GameManager.Instance.cardList.Add(card);
            GameManager.Instance.temp = true;
        }
       
    }
}