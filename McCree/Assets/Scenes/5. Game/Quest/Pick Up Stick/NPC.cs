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


        private bool Complete;

        public bool isComplete
        {
            get { return Complete; }
            set
            {
                Complete = value;
            }
        }

        private void Awake()
        {
            npcName = quest.npcName;
            npcImg = quest.npcImg;

            isComplete = false;
            quest.npcChatList = quest.npcChatList_start;
        }
    }
}
