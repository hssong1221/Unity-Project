using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class PlayerList : MonoBehaviour
    {
        #region Varibale Fields

        private Text playerNickName;

        #endregion

        private Player _player; // 플레이어 정보
        public Player myPlayer  // 플레이어 정보 set / get
        {
            get
            {
                return _player;
            }
            set
            {
                _player = value;
                playerNickName.text = $"{_player.NickName}";
            }

        }
        public void Awake()
        {
            playerNickName = transform.Find("PlayerInfo").GetComponent<Text>();
        }

        public void SetPlayerInfo(Player player)
        {
            myPlayer = player;
            playerNickName.text = player.NickName;
        }


    }
}