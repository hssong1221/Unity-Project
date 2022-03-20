using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class Interaction : Controller
    {

        public IEnumerator coroutine;

        void Awake()
        {
            base.Awake();

            MineUI.Instance.rejectBtn.onClick.AddListener(Close_NPC_Chat);
        }

        // interaction UI

        private void OnTriggerEnter(Collider other)
        {
            if (photonView.IsMine)
            {
                if (other.tag == "NPC")
                {
                    MineUI.Instance.range_x = Random.Range(-200, 200);
                    MineUI.Instance.range_y = Random.Range(-150, 150);
                    MineUI.Instance.interactionRect.anchoredPosition = new Vector2(MineUI.Instance.range_x, MineUI.Instance.range_y);
                    MineUI.Instance.interactionPanel.SetActive(true);
                    MineUI.Instance.interactionText.text = "대화 하기";
                    // F 상호작용 랜덤 위치

                    coroutine = returnchatList(0, other);
                    StartCoroutine(coroutine);
                    // 트리거된 상태에서 F누르면 대화창 뜰수있도록 코루틴함수 실행
                }

                else if (other.tag == "QuestItem") // 줍는 퀘스트
                {
                    foreach (Quest quest in playerInfo.myQuestList)
                    {
                        Quest_PickUp pickQuest = (Quest_PickUp)quest;

                        // 이름으로 비교하긴했는데 게임 오브젝트 클론된거라 비교가안됨
                        if (other.name.Substring(0, other.name.Length - 7) == pickQuest.bringGameObj.name)
                        //if (System.Object.ReferenceEquals(tquest.bringGameObj.gameObject, gameObject))
                        {
                            MineUI.Instance.range_x = Random.Range(-100, 100);
                            MineUI.Instance.range_y = Random.Range(-75, 75);
                            MineUI.Instance.interactionRect.anchoredPosition = new Vector2(MineUI.Instance.range_x, MineUI.Instance.range_y);
                            MineUI.Instance.interactionPanel.SetActive(true);
                            MineUI.Instance.interactionText.text = "줍기";
                            // F 상호작용 랜덤 위치

                            coroutine = QuestItemInteraction(other);
                            StartCoroutine(coroutine);
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
                else if (other.tag == "QuestItem")
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
                    if (!MineUI.Instance.chatPanel.activeSelf)
                    {   // chatPanel창 안켜져있으면 변수 저장
                        playerManager.isInteraction = true;

                        MineUI.Instance.npcName.text = other.GetComponent<NPC>().quest.npcName;
                        MineUI.Instance.npcImg.sprite = other.GetComponent<NPC>().quest.npcImg;

                        MineUI.Instance.npcChat.text = other.GetComponent<NPC>().quest.npcChatList[num++];
                        // 첫번째 대화

                        MineUI.Instance.npcbtns.SetActive(false);
                        MineUI.Instance.chatPanel.SetActive(true);
                    }
                    else
                    {
                        // chatPanel창 켜져있으면 다음꺼 보여주고
                        if (num < other.GetComponent<NPC>().quest.npcChatList.Count)
                        {
                            MineUI.Instance.npcChat.text = other.GetComponent<NPC>().quest.npcChatList[num++]; ;
                        }
                        else // 끝이면 비활성화
                        {
                            Debug.Log(other.GetComponent<NPC>().quest.qState);

                            if (other.GetComponent<NPC>().quest.qState == Quest.qType.Start)
                            {  // 퀘스트 받을때만 수락/거절 버튼
                                MineUI.Instance.npcbtns.SetActive(true);

                                MineUI.Instance.acceptBtn.onClick.RemoveAllListeners();

                                MineUI.Instance.acceptBtn.onClick.AddListener(
                                    delegate ()
                                    {
                                        Accept_NPC_Chat(other.GetComponent<NPC>());
                                    });
                            }
                            else if (other.GetComponent<NPC>().quest.qState == Quest.qType.Complete)
                            {
                                foreach (ItemList itemList in playerInfo.myItemList)
                                {
                                    if (itemList.item.ability == other.GetComponent<NPC>().quest.reward)
                                    {
                                        itemList.itemCount++;
                                        break;
                                    }
                                }

                                Close_NPC_Chat();
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
                npc.quest.qState = Quest.qType.Progress;
                npc.quest.npcChatList = npc.quest.npcChatList_progress;

                Debug.Log(npc.quest + "  " + npc.quest.qState + " ");


                // 퀘스트 진행중으로 상태 바꿈
                GameObject subquestObj = Instantiate(MineUI.Instance.subQuestObj, MineUI.Instance.subQuestPanel);
                subquestObj.GetComponent<SubQuestList>().quest = npc.quest;

                playerInfo.myQuestList.Add(subquestObj.GetComponent<SubQuestList>().quest);



                Close_NPC_Chat();
            }

            photonView.RPC("GetQuest", RpcTarget.All, npc.quest.questTitle);
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
                if (Input.GetButtonDown("Interaction"))
                {
                    ui.progressText.text = "나무 치우는 중...";
                    ui.PickInterAction(5, other.gameObject);
                    MineUI.Instance.interactionPanel.SetActive(false);
                }

                yield return null;
            }
        }

        public void Direct_StopCoroutine()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
        #endregion
    }
}