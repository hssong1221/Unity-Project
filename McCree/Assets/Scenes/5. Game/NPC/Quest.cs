using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ThreeCS.McCree
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Quests/Quest")]
    public class Quest : ScriptableObject
    {
        [Header("담길 대화")]
        public List<string> npcChatList = new List<string>();

        [Header("NPC 정보")]
        public Sprite npcImg;
        public string npcName;

        [Header("퀘스트 내용")]
        public string questTitle;
        public string questContent;
           
    }
}