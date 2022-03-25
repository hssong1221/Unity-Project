using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ThreeCS.McCree
{
    [CreateAssetMenu(fileName = "Quest_Transport", menuName = "Quests/Quest_Transport")]
    public class Quest_Transport : Quest_Interface_PT
    {
        public List<Transform> start_InstantePos; // 시작 위치
        public List<Transform> dest_InstantePos; // 목적 위치

        public int endCount; // 옮겨야 하는 개수
    }
}