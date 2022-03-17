using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace com.ThreeCS.McCree
{
    public class NPC : MonoBehaviour
    {
        public Quest quest;
        private string npcName;
        private Sprite npcImg;

        [HideInInspector]
        public bool isAccept;
        private void Awake()
        {
            isAccept = false;
            npcName = quest.npcName;
            npcImg = quest.npcImg;
        }
    }
}
