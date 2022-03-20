using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace com.ThreeCS.McCree
{
    public class NPC : MonoBehaviour
    {
        //[HideInInspector]
        public Quest quest;
        private string npcName;
        private Sprite npcImg;

        private void Awake()
        {
            npcName = quest.npcName;
            npcImg = quest.npcImg;

            quest.npcChatList = quest.npcChatList_start;
        }
    }
}
