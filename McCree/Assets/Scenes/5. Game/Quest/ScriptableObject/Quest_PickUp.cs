using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ThreeCS.McCree
{
    [CreateAssetMenu(fileName = "Quest_PickUp", menuName = "Quests/Quest_PickUp")]
    public class Quest_PickUp : Quest
    {
        public GameObject bringGameObj; // 스폰되는 아이템

        public List<Transform> instantePos; // 스폰 위치 리스트

        public int endCount; // 주워야하는 개수

    }
}