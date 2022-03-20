using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace com.ThreeCS.McCree
{
    [CreateAssetMenu(fileName = "Pick Up Stick", menuName = "Quests/Pick Up Stick")]
    public class Pick_Up_Stick : Quest_PickUp
    {


        protected void OnEnable() // 부모꺼 물려받으면서 값 초기화
        {
            base.OnEnable();

            questContent = "바쁜 보안관 대신 마을 주변에있는 나뭇가지들을 치우자";
            questContent_Copy = questContent;
            questContent = questContent_Copy + questProgress;
        }
    }
}

