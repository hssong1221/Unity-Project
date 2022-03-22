using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ThreeCS.McCree
{
    public class Quest_Obj : MonoBehaviour
    {
        [SerializeField]
        protected Quest _quest;

        public virtual Quest quest
        {
            get { return _quest; }
        }

        protected int curCount; // 몇개 주웟는지

        public enum qType
        {
            Start,
            Progress,
            Complete
        }


        protected List<string> _npcChatList; // 현재 텍스트
        public List<string> npcChatList
        {
            get { return _npcChatList; }
            set { _npcChatList = value; }
        }

        [HideInInspector]
        public string questTitle_progress;
        [HideInInspector]
        public string questContent_progress;


        protected qType QuestState; // 현재 퀘스트 상태

        public virtual qType qState
        {
            get { return QuestState; }
            set
            {
                QuestState = value;

                if (QuestState == qType.Start)
                    _npcChatList = quest.npcChatList_start;

                else if (QuestState == qType.Progress)
                    _npcChatList = quest.npcChatList_progress;

                else if (QuestState == qType.Complete)
                    _npcChatList = quest.npcChatList_complete;
            }
        }

        protected void Awake()
        {
            _npcChatList = quest.npcChatList_start;
        }
    }
}