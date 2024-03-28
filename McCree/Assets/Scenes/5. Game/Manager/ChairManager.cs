using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace com.ThreeCS.McCree
{
    public class ChairManager : MonoBehaviour
    {
        public bool isPlayer = false;  // 의자에 닿은 사람이 있을 때

        public GameObject gamePlate;

        // 앉은 사람 있으면 게임 시작할 때 게임 판 켜줌
        public void GamePlateOn()
        {
            gamePlate.SetActive(true);
        }
        public void GamePlateOff()
        {
            gamePlate.SetActive(false);
        }
    }
}
