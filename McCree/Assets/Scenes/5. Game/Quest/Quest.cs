using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ThreeCS.McCree
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Quests/Quest")]
    public class Quest : ScriptableObject
    {
        [SerializeField]
        protected Item.iType Reward; // 퀘스트 보상

        public Item.iType reward
        {
            get { return Reward; }
        }

        [Header("담길 대화")]
        public List<string> npcChatList_start = new List<string>();
        public List<string> npcChatList_progress = new List<string>();
        public List<string> npcChatList_complete = new List<string>();

        [Header("NPC 정보")]
        public Sprite npcImg;
        public string npcName;

        [Header("퀘스트 내용")]
        public string questTitle;
        public string questContent;


    }
}