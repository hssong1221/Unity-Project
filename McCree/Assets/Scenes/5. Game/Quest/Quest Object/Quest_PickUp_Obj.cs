using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ThreeCS.McCree
{
    public class Quest_PickUp_Obj : Quest_Interface_PT_Obj
    {
        public new Quest_PickUp quest
        {
            get { return (Quest_PickUp)_quest; }
        }

        [Header("진행상황")]
        protected string questProgress;


        public int count
        {
            get { return curCount; }
            set
            {
                curCount = value;

                questProgress = "(" + curCount + " / " + quest.endCount + ")";


                questTitle_progress = questProgress + " " + quest.questTitle;
                questContent_progress = quest.questContent + "\n" + questProgress;

                if (curCount == quest.endCount)
                {
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
                    npcChatList = quest.npcChatList_start;

                else if (QuestState == qType.Progress)
                {
                    npcChatList = quest.npcChatList_progress;
                    foreach (Transform pos in quest.instantePos)
                    {
                        GameObject clonedObj = Instantiate(quest.bringGameObj, pos.position, Quaternion.identity);
                        clonedObj.name = quest.bringGameObj.name;
                    }
                }

                else if (QuestState == qType.Complete)
                    npcChatList = quest.npcChatList_complete;
            }

        }

        protected void Awake()
        {
            base.Awake();

            count = 0;
            questProgress = "(" + curCount + " / " + quest.endCount + ")";
            questTitle_progress = questProgress + " " + quest.questTitle;
            questContent_progress = quest.questContent + "\n" + questProgress;
        }

    }
}
