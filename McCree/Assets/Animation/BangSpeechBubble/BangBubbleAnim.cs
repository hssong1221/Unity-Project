using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.ThreeCS.McCree
{
    public class BangBubbleAnim : MonoBehaviour
    {
        public Image bangBubble;
        public void End_Anim()
        {
            bangBubble.enabled = false;
        }
    }
}
