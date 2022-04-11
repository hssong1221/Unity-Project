using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ThreeCS.McCree
{
    public class QuestListAnim : MonoBehaviour
    {
        void Start()
        {
            GetComponent<Animator>().Play("QuestListStart");
        }

        public void Destroy_Object_End_Anim()
        {
            GetComponent<Animator>().Play("QuestListEnd");
        }

        public void Real_Destroy()
        {
            Destroy(gameObject);
        }


    }
}
