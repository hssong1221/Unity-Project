using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace com.ThreeCS.McCree
{
    public class NPC : MonoBehaviour
    {
        [SerializeField]
        private Quest_Obj _questObj;

        public Quest_Obj questObj
        {
            get { return _questObj; }
        }

        protected bool _isComplete;
        public bool isComplete
        {
            get { return _isComplete; }
            set { _isComplete = value; }
        }
    }
}
