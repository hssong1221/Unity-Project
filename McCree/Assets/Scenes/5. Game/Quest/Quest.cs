using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ThreeCS.McCree
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Quests/Quest")]
    public class Quest : ScriptableObject
    {
        public enum qType
        {
            Start,
            Progress,
            Complete
        }

        [SerializeField]
        protected Item.iType Reward; // 퀘스트 보상

        public Item.iType reward
        {
            get { return Reward; }
        }

        [SerializeField]
        protected qType QuestState;

        public virtual qType qState
        {
            get { return QuestState; }
            set
            {
                QuestState = value;

                if (QuestState == qType.Start)
                    npcChatList = npcChatList_start;

                else if (QuestState == qType.Progress)
                    npcChatList = npcChatList_progress;

                else if (QuestState == qType.Complete)
                    npcChatList = npcChatList_complete;
            }

        }

        [Header("담길 대화")]
        public List<string> npcChatList;
        public List<string> npcChatList_start = new List<string>();
        public List<string> npcChatList_progress = new List<string>();
        public List<string> npcChatList_complete = new List<string>();

        [Header("NPC 정보")]
        public Sprite npcImg;
        public string npcName;

        [Header("퀘스트 내용")]
        public string questTitle;
        public string questTitle_Copy;
        public string questContent;
        public string questContent_Copy;

        protected void Awake() { } // ScriptableObject은 Awake는 실행되지않음


        protected void OnEnable() // state초기화, 현재 대화 초기화
        {
            qState = qType.Start;
            npcChatList = new List<string>();
        }
    }
}