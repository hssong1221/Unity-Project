using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ThreeCS.McCree
{
    [CreateAssetMenu(fileName = "Quest_PickUp", menuName = "Quests/Quest_Interface_PT")]
    public class Quest_Interface_PT : Quest
    {
        public GameObject bringGameObj; // 스폰되는 아이템
    }
}