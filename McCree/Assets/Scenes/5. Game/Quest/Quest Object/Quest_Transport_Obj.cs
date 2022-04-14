using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
namespace com.ThreeCS.McCree
{
    public class Quest_Transport_Obj : Quest_Interface_PT_Obj
    {
        public new Quest_Transport quest
        {
            get { return (Quest_Transport)_quest; }
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


                    if (qrange == oType.Local)
                    {
                        foreach (Transform pos in quest.start_InstantePos)
                        {
                            GameObject clonedObj = Instantiate(quest.bringGameObj, pos.position, Quaternion.identity);
                            clonedObj.name = quest.bringGameObj.name;
                        }
                    }
                    else if (qrange == oType.World)
                    {
                        foreach (Transform pos in quest.start_InstantePos)
                        {
                            object[] data = new object[]
                            {
                                pos.position, Quaternion.identity
                            };
                            Debug.Log("asds");
                            RaiseEventManager.Instance.Pipe_Instantiate(data);
                        }
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