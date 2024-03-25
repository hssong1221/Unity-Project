using System.Collections;
using System.Collections.Generic;
using System;
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
        #region public variable

            // 잡화점 카드 동시삭제를 위해 필요
            public GameObject StoreCard;

            // 내 카드덱 동기화하려고 
            public List<Card> mycards = new List<Card>();

            
        #endregion

        // 게임 시작 애니메이션 플레이
        [PunRPC]
        public void AnimStart()
        {
            if (photonView.IsMine)
            {
                StartCoroutine(GameManager.Instance.GameStart());
            }
        }


        #region 앉은 후 게임 시작하면서 해야하는 것들

        
            // 보안관이 뱅 버튼 눌렀을 때 모든 사람의 인구수ui가 꺼져야함
            [PunRPC]
            public void StartUIOff()
            {
                GameStartManager.startUIOffAction();
            }

            // 게임 루프에 진입 명령
            [PunRPC]
            public void GameLoop()
            {
            // 본인만 진입시켜서 중복을 막음
            if (photonView.IsMine)
                GameManager.Instance.GLStart();
            }

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

            //UI 조정
            [PunRPC]
            public void UIMatch()
            {
                if(GameManager.Instance.playerList.Length == 3)
                {
                    ui.hpOffset = new Vector3(0, 1.6f, 0);
                    string temp = "(" + playerManager.playerType.ToString() + ")";
                    ui.nickName.text += temp;
                }
                else
                {
                    ui.hpOffset = new Vector3(0, 1.6f, 0);
                    if (playerManager.playerType.ToString().Equals("Sheriff"))
                    {
                        string temp = "(" + playerManager.playerType.ToString() + ")";
                        ui.nickName.text += temp;
                    }
                }
            }
        #endregion


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

            // 현재 턴이 누군지 알리면서 직접 이동(결투용)
            [PunRPC]
            public void TurnIndexMove(int idx)
            {
                GameManager.Instance.tidx = idx;
            }

            // 본인턴에 몸이 빛나서 다른사람에게도 자기 턴이라는 것을 알리기
            [PunRPC]
            public void TurnColor(int idx, int state)
        {
            Material mat;
            if (state == 0) //color OFF
            {
                mat = GameManager.Instance.turnList[idx].transform.GetComponentInChildren<SkinnedMeshRenderer>().material;
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", Color.black * 0.5f);
            }
            else if(state == 1) // color ON
            {
                mat = GameManager.Instance.turnList[idx].transform.GetComponentInChildren<SkinnedMeshRenderer>().material;
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", Color.blue * 0.5f);
            }
        }

            // 턴 지나고 살아있으면 공격자 인덱스 저장한거 삭제하기
            [PunRPC]
            public void AttackerIdxSync()
            {
                playerInfo.attackerIdx = -1;
            }

        #endregion


        #region 덱에 카드를 추가 하는 부분

            [PunRPC]
            public void MyCardSync()
            {
                mycards = playerInfo.mycards;
            }

            // 사용한 카드를 카드더미에 넣고 전체에게 동기화
            [PunRPC]
            public void CardDeckSync(Card.cType content)
            {
                //Debug.Log("datsync : " + content);
                Card card = new Card();
                card.cardContent = content;
                GameManager.Instance.cardList.Add(card);
                // 사용한 카드 +1
                GameManager.Instance.shuffleIdx++;
            }
            // 사용한 카드를 카드더미 제일 앞에 넣고 전체에게 동기화
            [PunRPC]
            public void CardDeckFrontSync(Card.cType content)
            {
                //Debug.Log("datsync : " + content);
                Card card = new Card();
                card.cardContent = content;
                GameManager.Instance.cardList.Insert(0, card);
            }

        #endregion

        #region 카드 기능 구현할 때 필요한 data 동기화 하는 부분 

        // 뱅 카드의 타겟이 되었을 때 (공격카드 포함)
        [PunRPC]
        public void BangTargeted(int state, int attackerIdx)
        {
            // state
            // 1 : bang, MG
            // 2 : indian
            playerInfo.isTarget = state;
            // 공격자인덱스를 타겟에 저장
            playerInfo.attackerIdx = attackerIdx;
            // 혹시나 잠겨있는 블록 패널 해제
            MineUI.Instance.cardblockingPanel.SetActive(false);
            MineUI.Instance.blockingPanel.SetActive(false);

            if (state == 1)
                GameManager.Instance.TargetedPanelOn("공격 당하는 중! 회피하세요. (Dodge 필요)");
            else
                GameManager.Instance.TargetedPanelOn("인디언이 공격하는 중! 반격하세요. (Bang 필요)");
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

        [PunRPC]
        public void StoreSync(int state)
        {
            // 0 : true 바꿈
            if(state == 0)
            {
                foreach (GameObject player in GameManager.Instance.turnList)
                {
                    player.GetComponent<PlayerInfo>().isStore = true;
                    MineUI.Instance.cardblockingPanel.SetActive(true);
                    // 본인 제외 전부 화면 클릭 막음
                    if(!photonView.IsMine)
                        MineUI.Instance.blockingPanel.SetActive(true);
                }
            }
            // 1: 한명 씩 선택하고 false로 돌아옴
            else if(state == 1)
            {
                //카드 하나 먹고 자동으로 다음사람으로 넘어감 그렇게 한바퀴 돌고 다시 돌아옴
                if (photonView.IsMine)
                    GameManager.Instance.NextBtnClick("store");

                playerInfo.isStore = false;
            }
        }

        [PunRPC]
        public void StoreListSync(int idx)
        {
            GameManager.Instance.storecardList.RemoveAt(idx);
        }

        [PunRPC]
        public void StoreCardDel(int idx)
        {
            Debug.Log("idx : " + idx);
            GameObject[] obj = GameObject.FindGameObjectsWithTag("Card");
            foreach(GameObject card in obj)
            {
                Debug.Log("card.GetComponent<Card>().storeIdx : " + card.GetComponent<Card>().storeIdx);
                if(card.GetComponent<Card>().storeIdx == idx)
                {
                    Destroy(card.gameObject);
                    break;
                }
            }
        }


        [PunRPC]
        public void ScopeSync(int state)
        {
            if(state == 0)
            {
                playerInfo.isScope = true;
                GameObject scope = playerManager.mygamePlate.transform.Find("Scope").gameObject;
                scope.SetActive(true);
            }
            else
            {
                playerInfo.isScope = false;
                GameObject scope = playerManager.mygamePlate.transform.Find("Scope").gameObject;
                scope.SetActive(false);
            }
            
        }

        [PunRPC]
        public void MustangSync(int state)
        {
            if(state == 0)
            {
                playerInfo.isMustang = true;
                GameObject mustang = playerManager.mygamePlate.transform.Find("Mustang").gameObject;
                mustang.SetActive(true);
            }
            else
            {
                playerInfo.isMustang = false;
                GameObject mustang = playerManager.mygamePlate.transform.Find("Mustang").gameObject;
                mustang.SetActive(false);
            }
        }

        [PunRPC]
        public void BarrelSync(int state)
        {
            if(state == 0)
            {
                playerInfo.isBarrel = true;
                GameObject barrel = playerManager.mygamePlate.transform.Find("Barrel").gameObject;
                barrel.SetActive(true);
            }
            else
            {
                playerInfo.isBarrel = false;
                GameObject barrel = playerManager.mygamePlate.transform.Find("Barrel").gameObject;
                barrel.SetActive(false);
            }
        }

        [PunRPC]
        public void DynamiteSync(int state)
        {
            if(state == 0) // 다이너 마이트 장착
            {
                // 다이너마이트 주인 저장 최초 한번만
                if(GameManager.Instance.dynamiteIdx == -1)
                    GameManager.Instance.dynamiteIdx = GameManager.Instance.tidx;
                playerInfo.isDynamite = true;
                GameObject dynamite = playerManager.mygamePlate.transform.Find("Dynamite").gameObject;
                dynamite.SetActive(true);
            }
            else if(state == 1) // 다음 사람한테 넘겨줌
            {
                playerInfo.isDynamite = false;
                GameObject dynamite = playerManager.mygamePlate.transform.Find("Dynamite").gameObject;
                dynamite.SetActive(false);
            }
            else if (state == 2) // 터짐
            {
                // 터지면 공격자 저장
                playerInfo.attackerIdx = GameManager.Instance.dynamiteIdx;
                playerInfo.isDynamite = false;
                GameObject dynamite = playerManager.mygamePlate.transform.Find("Dynamite").gameObject;
                dynamite.SetActive(false);
                GameManager.Instance.dynamiteIdx = -1;
            }
        }

        [PunRPC]
        public void JailSync(int state)
        {
            if(state == 0) // 감옥 수감
            {
                playerInfo.isJail = true;
                GameObject jail = playerManager.mygamePlate.transform.Find("Jail").gameObject;
                jail.SetActive(true);
            }
            else // 탈옥 또는 기간이 끝남
            {
                playerInfo.isJail = false;
                GameObject jail = playerManager.mygamePlate.transform.Find("Jail").gameObject;
                jail.SetActive(false);
            }
        }

        [PunRPC]
        public void CatSync(int state)
        {
            if (state == 0)
                playerInfo.isCat = true;
            else
                playerInfo.isCat = false;
        }

        [PunRPC]
        public void CardNumSync(int num)   // 본인 손 덱 갯수
        {
            playerInfo.mycardNum = num;
        }

        [PunRPC]
        public void CatbalouDel(int num)
        {
            if(photonView.IsMine)
                MineUI.Instance.CatTargetCardDel(num);
        }

        [PunRPC]
        public void PanicSync(int state)
        {
            if (state == 0)
                playerInfo.isPanic = true;
            else
                playerInfo.isPanic = false;
        }

        [PunRPC]
        public void PanicDel(int num)
        {
            if (photonView.IsMine)
                MineUI.Instance.PanicTargetCardDel(num);
        }

        [PunRPC]
        public void DuelTurn(int state)
        {
            if (state == 0)
            {
                GameManager.Instance.duelTurn = true;
                GameManager.Instance.duelIdx = GameManager.Instance.tidx;
            }
            else
            {
                GameManager.Instance.duelTurn = false;
                GameManager.Instance.tidx = GameManager.Instance.duelIdx;
            }
        }

        [PunRPC]
        public void DuelSync(int state, int atkIdx)
        {
            if (state == 0)
            {
                playerInfo.isDuel = true;
                playerInfo.attackerIdx = atkIdx;
            }
            else
            {
                // 결투 끝낼 때는 각자 하기 어려워서 통채로 초기화
                foreach(GameObject player in GameManager.Instance.turnList)
                {
                    player.GetComponent<PlayerInfo>().isDuel = false;
                    MineUI.Instance.duelPanel.SetActive(false);
                }
            } 
                
        }

        [PunRPC]
        public void WeaponSync(int state)
        {
            GameObject colt = playerManager.mygamePlate.transform.Find("Colt").gameObject;
            GameObject russian = playerManager.mygamePlate.transform.Find("Russian").gameObject;
            GameObject navy = playerManager.mygamePlate.transform.Find("Navy").gameObject;
            GameObject carbine = playerManager.mygamePlate.transform.Find("Carbine").gameObject;
            GameObject winchester = playerManager.mygamePlate.transform.Find("Winchester").gameObject;

            GameObject[] weapon = new GameObject[5];
            weapon[0] = colt;
            weapon[1] = russian;
            weapon[2] = navy;
            weapon[3] = carbine;
            weapon[4] = winchester;

            // 기본 무기 확인
            if (state == 1)
            {
                playerInfo.isWeapon = false;
                if (photonView.IsMine)
                {
                    MineUI.Instance.wIMG.sprite = MineUI.Instance.wImg1;
                    MineUI.Instance.wName.text = "COLT";
                    MineUI.Instance.wRange.text = "1";
                }
            }
            else
                playerInfo.isWeapon = true;

            // 일단 다 끔
            foreach (GameObject w in weapon)
                w.SetActive(false);

            // 상황에 맞는 무기를 켜줌
            switch (state) {
                case 1: //colt
                    colt.SetActive(true);
                    playerInfo.weaponRange = 0;
                    playerInfo.wName = "Colt";
                    break;
                case 2: //russian
                    russian.SetActive(true);
                    playerInfo.weaponRange = 1;
                    playerInfo.wName = "Russian";
                    break;
                case 3: // navy
                    navy.SetActive(true);
                    playerInfo.weaponRange = 2;
                    playerInfo.wName = "Navy";
                    break;
                case 4: //carbine
                    carbine.SetActive(true);
                    playerInfo.weaponRange = 3;
                    playerInfo.wName = "Carbine";
                    break;
                case 5: // winchester
                    winchester.SetActive(true);
                    playerInfo.weaponRange = 4;
                    playerInfo.wName = "Winchester";
                    break;
                default:
                    break;
            }
        }

        #endregion

        [PunRPC]
        public void AlertInfo(string state, string atk, string target)
        {
            Debug.Log(state + " atk : " + atk + " target : " + target);

            if(state.Equals("6"))
                GameManager.Instance.alertOrder(6);

            if (state.Equals("Bang"))
                GameManager.Instance.alertOrder(10, atk, target);
            else if(state.Equals("MG"))
                GameManager.Instance.alertOrder(11, atk);
            else if (state.Equals("Indian"))
                GameManager.Instance.alertOrder(12, atk);
            else if (state.Equals("Beer"))
                GameManager.Instance.alertOrder(13, atk);
            else if (state.Equals("Saloon"))
                GameManager.Instance.alertOrder(14, atk);
            else if (state.Equals("Stage"))
                GameManager.Instance.alertOrder(15, atk);
            else if (state.Equals("Wells"))
                GameManager.Instance.alertOrder(16, atk);
            else if (state.Equals("Store"))
                GameManager.Instance.alertOrder(17, atk);
            else if (state.Equals("Jail"))
                GameManager.Instance.alertOrder(18, atk, target);
            else if (state.Equals("JailEsc"))
                GameManager.Instance.alertOrder(181, atk);
            else if (state.Equals("JailFail"))
                GameManager.Instance.alertOrder(182, atk);
            else if (state.Equals("Dyn"))
                GameManager.Instance.alertOrder(19, atk, target);
            else if (state.Equals("DynNext"))
                GameManager.Instance.alertOrder(191, atk);
            else if (state.Equals("DynBoom"))
                GameManager.Instance.alertOrder(192, atk);
            else if (state.Equals("Cat"))
                GameManager.Instance.alertOrder(20, atk, target);
            else if (state.Equals("Panic"))
                GameManager.Instance.alertOrder(21, atk, target);
            else if (state.Equals("Duel"))
                GameManager.Instance.alertOrder(22, atk, target);
            else if (state.Equals("Scope"))
                GameManager.Instance.alertOrder(23, atk);
            else if (state.Equals("Mustang"))
                GameManager.Instance.alertOrder(24, atk);
            else if (state.Equals("Barrel"))
                GameManager.Instance.alertOrder(25, atk);
            else if (state.Equals("Weapon"))
            {
                if(target.Equals("r"))
                    GameManager.Instance.alertOrder(30, atk);
                else if (target.Equals("n"))
                    GameManager.Instance.alertOrder(31, atk);
                else if (target.Equals("c"))
                    GameManager.Instance.alertOrder(32, atk);
                else if (target.Equals("w"))
                    GameManager.Instance.alertOrder(33, atk);
            }
            else if (state.Equals("Avoid1"))
                GameManager.Instance.alertOrder(40, atk);
            else if (state.Equals("Avoid2"))
                GameManager.Instance.alertOrder(41, atk);

            else if (state.Equals("SDie"))
                GameManager.Instance.alertOrder(50, atk);
            else if (state.Equals("VDie"))
                GameManager.Instance.alertOrder(51, atk);
            else if (state.Equals("ODie"))
                GameManager.Instance.alertOrder(52, atk);
            else if (state.Equals("RDie"))
                GameManager.Instance.alertOrder(53, atk);

        }
    }
}
