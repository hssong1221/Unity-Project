using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

namespace com.ThreeCS.McCree
{
    public class Interaction : Controller
    {

        private IEnumerator _coroutine;
        public IEnumerator coroutine
        {
            get { return _coroutine; }
            set
            {
                _coroutine = value;
            }
        }

        void Awake()
        {
            base.Awake();
            MineUI.Instance.rejectBtn.onClick.AddListener(Close_NPC_Chat);
        }


        private void OnTriggerEnter(Collider other)
        {
            if (photonView.IsMine)
            {
                if (other.tag == "NPC")
                {
                    if (!other.GetComponent<NPC>().isComplete)
                    {
                        MineUI.Instance.range_x = Random.Range(-150, 150);
                        MineUI.Instance.range_y = Random.Range(-100, 100);
                        MineUI.Instance.interactionRect.anchoredPosition = new Vector2(MineUI.Instance.range_x, MineUI.Instance.range_y);
                        MineUI.Instance.interactionPanel.SetActive(true);
                        MineUI.Instance.interactionText.text = "대화 하기";
                        // F 상호작용 랜덤 위치

                        if (coroutine != null)
                            StopCoroutine(coroutine);
                        coroutine = returnchatList(0, other);
                        StartCoroutine(coroutine);
                        // 트리거된 상태에서 F누르면 대화창 뜰수있도록 코루틴함수 실행
                    }
                }
                else if (other.gameObject.layer == LayerMask.NameToLayer("QuestItem"))
                {
                    foreach (SubQuestList subQuestObj in playerInfo.myQuestList)
                    {
                        Quest_Interface_PT_Obj pickQuest = (Quest_Interface_PT_Obj) subQuestObj.questObj;
                        // 줍거나 운반가능한 아이템일때 Quest_Interface_PT

                        // 내가 가지고있는 퀘스트 중 줍기퀘스트 아이템이있을때 
                        if (other.name == pickQuest.quest.bringGameObj.name)
                        {
                            MineUI.Instance.range_x = Random.Range(-100, 100);
                            MineUI.Instance.range_y = Random.Range(-75, 75);
                            MineUI.Instance.interactionRect.anchoredPosition = new Vector2(MineUI.Instance.range_x, MineUI.Instance.range_y);
                            MineUI.Instance.interactionPanel.SetActive(true);
                            MineUI.Instance.interactionText.text = "줍기";
                            // F 상호작용 랜덤 위치

                            if (coroutine != null)
                                StopCoroutine(coroutine);
                            coroutine = QuestItemInteraction(other);
                            StartCoroutine(coroutine);
                            // 트리거된 상태에서 F누르면 줍는 애니메이션 실행될수있도록
                            break;
                        }
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {

            if (photonView.IsMine)
            {
                if (other.tag == "NPC")
                {
                    MineUI.Instance.interactionPanel.SetActive(false);
                    MineUI.Instance.chatPanel.SetActive(false);
                    playerManager.isInteraction = false;
                }
                else if (other.gameObject.layer == LayerMask.NameToLayer("QuestItem"))
                {
                    MineUI.Instance.interactionPanel.SetActive(false);
                }

                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
            }
        }


        #region npc대화 관련
        IEnumerator returnchatList(int num, Collider other)
        {
            while (true)
            {
                if (Input.GetButtonDown("Interaction"))
                {
                    MineUI.Instance.interactionPanel.SetActive(false);

                    NPC npc = other.GetComponent<NPC>();
                    if (!MineUI.Instance.chatPanel.activeSelf)
                    {   // chatPanel창 안켜져있으면 변수 저장
                        playerManager.isInteraction = true;

                        MineUI.Instance.npcName.text = npc.questObj.quest.npcName;
                        MineUI.Instance.npcImg.sprite = npc.questObj.quest.npcImg;

                        
                        MineUI.Instance.npcChat.text = npc.questObj.npcChatList[num++];
                        // 첫번째 대화

                        MineUI.Instance.npcbtns.SetActive(false);
                        MineUI.Instance.chatPanel.SetActive(true);
                    }
                    else
                    {
                        // chatPanel창 켜져있으면 다음꺼 보여주고
                        if (num < npc.questObj.npcChatList.Count)
                        {
                            MineUI.Instance.npcChat.text = npc.questObj.npcChatList[num++]; ;
                        }
                        else // 끝이면 비활성화
                        {
                            if (npc.questObj.qState == Quest_Obj.qType.Start)
                            {  // 퀘스트 받을때만 수락/거절 버튼
                                MineUI.Instance.npcbtns.SetActive(true);

                                MineUI.Instance.acceptBtn.onClick.RemoveAllListeners();
                                
                                MineUI.Instance.acceptBtn.onClick.AddListener( // 수락할때 해당 퀘스트로 다시 이어주기위해 AddListner
                                    delegate ()
                                    {
                                        Accept_NPC_Chat(npc);
                                    });
                            }
                            else if (npc.questObj.qState == Quest_Obj.qType.Complete)
                            {
                                // 보상
                                foreach (ItemList itemList in playerInfo.myItemList)
                                {
                                    if (itemList.item.ability == npc.questObj.quest.reward)
                                    {
                                        itemList.itemCount++;
                                        break;
                                    }
                                }

                                // 완료한 퀘스트 목록에서 삭제
                                foreach (SubQuestList subQuest in playerInfo.myQuestList)
                                {
                                    if (subQuest.questObj == npc.questObj)
                                    {
                                        Destroy(subQuest.gameObject);
                                        break;
                                    }
                                }

                                npc.isComplete = true;

                                Close_NPC_Chat();

                                photonView.RPC("QuestComplete", RpcTarget.All, npc.questObj.quest.questTitle);
                            }

                            else
                            {   // 나머지 대화 끝나면 닫아줌
                                Close_NPC_Chat();
                            }
                            
                        }
                    }
                }

                yield return null;
            }
        }


        void Accept_NPC_Chat(NPC npc)
        {
            if (photonView.IsMine)
            {
                
                // 퀘스트 진행중으로 상태 바꿈
                npc.questObj.qState = Quest_Obj.qType.Progress;
                npc.questObj.npcChatList = npc.questObj.quest.npcChatList_progress;

                // 퀘스트 생성해서 오른쪽 상단 목표에 붙여줌
                GameObject subquestObj;

                if (npc.questObj.qrange == Quest_Obj.oType.World) 
                {
                    subquestObj = Instantiate(MineUI.Instance.questObj, MineUI.Instance.worldQuestPanel);
                    photonView.RPC("QuestLog", RpcTarget.All, npc.questObj.quest.questTitle); // 퀘스트 알림
                    RaiseEventManager.Instance.Add_World_Quest(npc); // 월드 퀘스트라면 전체에게 퀘스트 추가
                }
                else // 서브 퀘스트면 나한테만 추가
                {   
                    subquestObj = Instantiate(MineUI.Instance.questObj, MineUI.Instance.subQuestPanel);
                }

                subquestObj.GetComponent<SubQuestList>().questObj = npc.questObj;
                subquestObj.GetComponent<SubQuestList>().questTitle.text = npc.questObj.questTitle_progress;

                // 내 퀘스트리스트에 붙임
                playerInfo.myQuestList.Add(subquestObj.GetComponent<SubQuestList>());

                // contentsizefillter가 적용이 안되는 오류때메 재배치하는 함수
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)MineUI.Instance.worldQuestPanel);

                // npc대화 끔
                Close_NPC_Chat();
            }

            photonView.RPC("GetQuest", RpcTarget.All, npc.questObj.quest.questTitle);
        }

        void Close_NPC_Chat()
        {
            if (photonView.IsMine)
            {
                if (coroutine != null)
                    StopCoroutine(coroutine);
                MineUI.Instance.interactionPanel.SetActive(false);
                MineUI.Instance.chatPanel.SetActive(false);
                playerManager.isInteraction = false;
            }
        }
        #endregion

        #region 퀘스트 아이템 관련
        IEnumerator QuestItemInteraction(Collider other)
        {
            while (true)
            {
                Debug.Log("화제의 코루틴");
                if (Input.GetButtonDown("Interaction"))
                {
                    ui.InterAction(5, other.gameObject);
                    MineUI.Instance.interactionPanel.SetActive(false);
                }

                yield return null;
            }
        }

        public void Direct_StopCoroutine() // 코루틴은 해당 파일에서 꺼야함
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
        #endregion
    }
}