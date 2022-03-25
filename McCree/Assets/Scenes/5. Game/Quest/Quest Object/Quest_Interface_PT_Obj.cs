using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ThreeCS.McCree
{
    public class Quest_Interface_PT_Obj : Quest_Obj
    {
        public new Quest_Interface_PT quest
        {
            get { return (Quest_Interface_PT)_quest; }
        }

        protected void Awake()
        {
            base.Awake();
        }
    }
}