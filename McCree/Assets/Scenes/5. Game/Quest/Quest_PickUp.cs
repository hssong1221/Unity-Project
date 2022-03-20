using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ThreeCS.McCree
{
    [CreateAssetMenu(fileName = "Quest_PickUp", menuName = "Quests/Quest_PickUp")]
    public class Quest_PickUp : Quest
    {
        public bool isBring;

        public GameObject bringGameObj; // 스폰되는 아이템

        [SerializeField]
        protected List<Transform> instantePos; // 스폰 위치

        [Header("진행상황")]
        public string questProgress;

        // 나중에 protected로 바꾸기
        protected int curCount; // 몇개 주웟는지
        protected int endCount; // 주워야하는 개수

        public int count
        {
            get { return curCount; }
            set
            {
                curCount = value;

                questProgress = "\n(" + curCount + " / " + endCount + ")";

                questContent = questContent_Copy + questProgress;

                if (curCount == endCount)
                {
                    isBring = true;
                    qState = qType.Complete;
                }
            }

        }

        public override qType qState
        {
            get { return QuestState; }
            set
            {
                QuestState = value;

                if (QuestState == qType.Start)
                    npcChatList = npcChatList_start;

                else if (QuestState == qType.Progress)
                {
                    npcChatList = npcChatList_progress;
                    foreach (Transform pos in instantePos)
                    {
                        Instantiate(bringGameObj, pos.position, Quaternion.identity);
                    }
                }

                else if (QuestState == qType.Complete)
                    npcChatList = npcChatList_complete;
            }

        }

        protected void OnEnable() // 부모꺼 물려받으면서 값 초기화
        {
            base.OnEnable();

            isBring = false;
            curCount = 0;
            questProgress = "\n(" + curCount + " / " + endCount + ")";
        }

    }
}