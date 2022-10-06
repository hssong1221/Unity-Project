using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

using Photon.Pun;
// 이 스크립트는 다른 게임 오브젝트(게임매니저, 카드 등)에서 들어오는 data를 전체에게 적용시키기 위한 
// 즉 RPC 전용 스크립트입니다.
// 또는 적용하기 전에 해보는 실험용

namespace com.ThreeCS.McCree
{
    public class DataSync : Controller
    {
        // 게임 시작 애니메이션 플레이
        [PunRPC]    
        public void AnimStart() 
        {
            if (photonView.IsMine)
            {
                StartCoroutine(GameManager.Instance.GameStart());
            }
        }

        #region 앉은 후 게임 시작

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

        #endregion

        // 내 체력 동기화
        [PunRPC]
        public void SyncHp(int myhp)
        {
            if (myhp == -10)
                playerInfo.Show_Hp();
            else
            {
                playerInfo.hp = myhp;
                playerInfo.Show_Hp();
            }

        }

        #region 턴 관련
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

        #endregion

        // 사용한 카드를 카드더미에 넣고 전체에게 동기화
        [PunRPC]
        public void CardDeckSync(Card.cType content)
        {
            //Debug.Log("datsync : " + content);
            Card card = new Card();
            card.cardContent = content;
            GameManager.Instance.cardList.Add(card);
        }

        // 뱅 카드의 타겟이 되었을 때
        [PunRPC]
        public void BangTargeted()
        {
            playerInfo.isTarget = 1;
            GameManager.Instance.TargetedPanelOn();
        }

        // 뱅 카드를 내고 회피를 기다리는 상태라는 것을 알림
        [PunRPC]
        public void WaitAvoid()
        {
            playerInfo.waitAvoid = true;
        }

        // 뱅 카드 타겟일 때 회피 했다는 것을 알림
        [PunRPC]
        public void SendAvoid(int ccase)
        {
            // 0 : 공격자가 회피를 받았을 때
            // 1 : 공격자가 그냥 맞기를 받았을 때
            // 2 : 타겟이 0 or 1 행동 한 후에 자신의 상태를 변경할 때
            if (ccase == 0)
            {
                // 공격자의 상태를 변경하는 것
                playerInfo.waitAvoid = false;
                GameManager.Instance.willDamage = false;
            }
            else if(ccase == 1)
            {
                playerInfo.waitAvoid = false;
                GameManager.Instance.willDamage = true;

            }
            else if (ccase == 2)
            {
                // 본인 상태를 변경해서 알리는 것
                playerInfo.isTarget = 0;
            }
        }
    }
}