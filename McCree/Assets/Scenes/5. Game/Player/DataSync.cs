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
        public void SyncHp(int hp)
        {
            if (hp == -10)
                playerInfo.Show_Hp();
            else
            {
                playerInfo.hp = hp;
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
        public void BangTargeted(int state)
        {
            // state
            // 1 : bang, MG
            // 2 : indian
            playerInfo.isTarget = state;
            GameManager.Instance.TargetedPanelOn();
        }

        // 뱅 카드를 내고 회피를 기다리는 상태라는 것을 알림
        [PunRPC]
        public void WaitAvoid(int state)
        {
            if (state == 0)  // 일반 뱅
                playerInfo.waitAvoid = true;
            else if (state == 1)    // 머신건 뱅
                playerInfo.waitAvoids = 0;
            else if (state == 2)    // 인디언 뱅
                playerInfo.waitBangs = 0;
                
        }

        // 뱅 카드 타겟일 때 회피 했다는 것을 알림
        [PunRPC]
        public void SendAvoid(int state)
        {
            // 0 : 공격자에게 뱅을 맞고 회피나 맞기를 했다고 알림
            // 1 : 공격자에게 기관총을 맞고 ~
            // 2 : 공격자에게 인디언을 맞고 ~
            // 3 : 타겟이 0번 행동 한 후에 자신의 상태를 변경할 때
            if (state == 0 )
            {
                // 공격자의 상태를 변경하는 것
                playerInfo.waitAvoid = false;
            }
            else if(state == 1)
            {
                // 공격자의 상태를 변경하는 것
                playerInfo.waitAvoids += 1;
            }
            else if (state == 2)
            {
                // 공격자의 상태를 변경하는 것
                playerInfo.waitBangs += 1;
            }
            else if (state == 3)
            {
                // 본인 상태를 변경해서 알리는 것
                playerInfo.isTarget = 0;
            }
        }

        [PunRPC]
        public void MgSync(int state)
        {
            // 기관총 사용중
            if (state == 0)
                playerInfo.isMG = true;
            // 사용 끝
            else
            {
                playerInfo.isMG = false;
                playerInfo.waitAvoids = -1;
            }
        }

        [PunRPC]
        public void IndianSync(int state)
        {
            // 인디언 사용중
            if (state == 0)
                playerInfo.isIndian = true;
            else
            {
                playerInfo.isIndian = false;
                playerInfo.waitBangs = -1;
            }
        }
    }
}