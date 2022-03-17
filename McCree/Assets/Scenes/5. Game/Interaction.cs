using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class Interaction : Controller
    {

        float range_x;
        float range_y;

        private IEnumerator coroutine;



        void Awake()
        {
            base.Awake();

            MineUI.Instance.rejectBtn.onClick.AddListener(Close_NPC_Chat);
        }

        // interaction UI

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "NPC" && photonView.IsMine)
            {
                range_x = Random.Range(-200, 200);
                range_y = Random.Range(-150, 150);
                MineUI.Instance.interactionRect.anchoredPosition = new Vector2(range_x, range_y);
                MineUI.Instance.interactionPanel.SetActive(true);
                MineUI.Instance.interactionText.text = "대화 하기";
                // F 상호작용 랜덤 위치

                coroutine = returnchatList(0, other);
                StartCoroutine(coroutine);
                // 트리거된 상태에서 F누르면 대화창 뜰수있도록 코루틴함수 실행
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "NPC" && photonView.IsMine)
            {
                if (coroutine != null)
                    StopCoroutine(coroutine);

                MineUI.Instance.interactionPanel.SetActive(false);
                MineUI.Instance.chatPanel.SetActive(false);
                playerManager.isInteraction = false;
            }
        }

        IEnumerator returnchatList(int num, Collider other)
        {
            while (true)
            {
                if (Input.GetButtonDown("Interaction") && !other.GetComponent<NPC>().isAccept)
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
                            MineUI.Instance.npcbtns.SetActive(true);

                            MineUI.Instance.acceptBtn.onClick.RemoveAllListeners();

                            MineUI.Instance.acceptBtn.onClick.AddListener(
                                delegate (){
                                    Accept_NPC_Chat(other.GetComponent<NPC>());
                                });
                        }
                    }
                }
                else if (Input.GetButtonDown("Interaction") && other.GetComponent<NPC>().isAccept)
                {
                    playerManager.return_itemNoticeText("<color=#FF3939>" + "이미 수락한 퀘스트 입니다!" + "</color>");
                }

                yield return null;
            }
        }

        void Accept_NPC_Chat(NPC npc)
        {
            if (photonView.IsMine)
            {
                GameObject subquestObj = Instantiate(MineUI.Instance.subQuestObj, MineUI.Instance.subQuestPanel);
                subquestObj.GetComponent<SubQuestList>().quest = npc.quest;

                playerInfo.myQuestList.Add(subquestObj.GetComponent<SubQuestList>().quest);

                npc.isAccept = true;

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
    }
}