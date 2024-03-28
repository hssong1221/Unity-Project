using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Photon.Pun;


namespace com.ThreeCS.McCree
{
    // 방장 masterplayer가 해야하는 동작을 정의해 놓은 곳
    public class MasterPlayerManager : Controller
    {
        // 방장이 해야하는것
        public static Action masterPlayerAction;

        // gamemanager playerList
        GameObject[] pList;

        void Start()
        {
            pList = GameManager.Instance.playerList;
            CheckMaster();
        }
        public void CheckMaster()
        {
            if(photonView.IsMine && PhotonNetwork.IsMasterClient)
            {
                masterPlayerAction += SetPlayerJob;
                masterPlayerAction += SetPlayerAbility;
                masterPlayerAction += SetPlayerHP;
                masterPlayerAction += IntroAniPlay;
            }
        }

        public void SetPlayerJob()
        {
            // (인구수에 맞게 하는 거 추가하기)
            //List<int> jobList = new List<int>() { 1, 2, 3, 4, 5, 6, 7 };
            List<int> jobList = null;
            if (pList.Length == 3)
                jobList = new List<int>() { 2, 4, 7 }; // 3인 룰
            else if (pList.Length == 4)
                jobList = new List<int>() { 1, 4, 5, 7 }; // 4인 룰
            else if (pList.Length == 5)
                jobList = new List<int>() { 1, 2, 4, 5, 7 }; // 5인 룰
            else if (pList.Length == 6)
                jobList = new List<int>() { 1, 2, 4, 5, 6, 7 }; // 6인 룰
            else if (pList.Length == 7)
                jobList = new List<int>() { 1, 2, 3, 4, 5, 6, 7 }; // 7인 룰

            // 실험용
            if (pList.Length == 1)
                jobList = new List<int>() { 1 }; // 7인 룰
            // 실험용
            if (pList.Length == 2)
                jobList = new List<int>() { 1,4 }; // 7인 룰


            jobList = CommonFunction.ShuffleList(jobList);
            //Debug.Log("잡리스트" + jobList[0] + " " + jobList[1] + " " + jobList[2]);

            // 직업을 나눠주고 동기화 시킴
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Debug.Log("잡 리스트 번호 : " + jobList[i]);
                pList[i].GetComponent<PhotonView>().RPC("JobSelect", RpcTarget.All, jobList[i]);
            }
        }

        public void SetPlayerAbility()
        {
            // 능력 갯수에 맞게 해야함
            List<int> abilityList = new List<int>() { 1, 2, 3, 4, 5, 6, 7 };

            abilityList = CommonFunction.ShuffleList(abilityList);
            //Debug.Log("어빌리스트" + abilityList[0] + " " + abilityList[1] + " " + abilityList[2]);

            // 능력을 나눠주고 동기화 시킴
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Debug.Log("어빌 리스트 번호 : " + abilityList[i]);
                pList[i].GetComponent<PhotonView>().RPC("AbilitySelect", RpcTarget.All, abilityList[i]);
            }
        }

        public void SetPlayerHP()
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Debug.Log("체력 동기화 : " + pList[i]);
                pList[i].GetComponent<PhotonView>().RPC("HPSelect", RpcTarget.All, pList[i].GetComponent<PlayerManager>().playerType);
            }
        }

        public void IntroAniPlay()
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                pList[i].GetComponent<PhotonView>().RPC("AnimStart", RpcTarget.All);
            }
        }

        #region Pun RPC

        [PunRPC]
        public void JobSelect(int num) // 내 직업 동기화 
        {
            switch (num)
            {
                case 1:
                    playerManager.playerType = GameManager.jType.Sheriff;
                    break;
                case 2:
                case 3:
                    playerManager.playerType = GameManager.jType.Vice;
                    break;
                case 4:
                case 5:
                case 6:
                    playerManager.playerType = GameManager.jType.Outlaw;
                    break;
                case 7:
                    playerManager.playerType = GameManager.jType.Renegade;
                    break;
            }
        }

        [PunRPC]
        public void AbilitySelect(int num) // 내 능력 동기화 
        {
            switch (num)
            {
                case 1:
                    playerManager.abilityType = GameManager.aType.BangMissed;
                    break;
                case 2:
                    playerManager.abilityType = GameManager.aType.DrinkBottle;
                    break;
                case 3:
                    playerManager.abilityType = GameManager.aType.HumanVolcanic;
                    break;
                case 4:
                    playerManager.abilityType = GameManager.aType.OnehpOnecard;
                    break;
                case 5:
                    playerManager.abilityType = GameManager.aType.ThreeCard;
                    break;
                case 6:
                    playerManager.abilityType = GameManager.aType.TwocardOnecard;
                    break;
                case 7:
                    playerManager.abilityType = GameManager.aType.TwocardOnehp;
                    break;
            }
        }

        [PunRPC]
        public void HPSelect(GameManager.jType type) // 내 최대체력 동기화 
        {
            switch (type)
            {
                case GameManager.jType.Sheriff:
                    playerInfo.hp = 5;
                    playerInfo.maxHp = 5;
                    break;
                case GameManager.jType.Vice:
                case GameManager.jType.Outlaw:
                case GameManager.jType.Renegade:
                    playerInfo.hp = 4;
                    playerInfo.maxHp = 4;
                    break;
            }
            playerInfo.Show_Hp();
        }

        // 게임 시작 애니메이션 플레이
        [PunRPC]
        public void AnimStart()
        {
            if (photonView.IsMine)
            {
                IntroAniUI.introAction();
                //StartCoroutine(GameManager.Instance.GameStart());
                StartCoroutine(GameManager.Instance.GameStart());
            }
        }
        #endregion
    }
}
