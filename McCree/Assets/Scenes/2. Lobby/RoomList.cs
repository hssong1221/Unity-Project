using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class RoomList : MonoBehaviour
    {
        #region Varibale Fields


        [HideInInspector]
        public Text roomText;  // 방 요약 정보 
        [HideInInspector]
        public Button roomBtn;  // 방 클릭시 조인 버튼

        private RoomInfo _roomInfo; // 방 정보
        public RoomInfo myRoomInfo  // 방 정보 set / get
        {
            get
            {
                return _roomInfo;
            }
            set
            {
                _roomInfo = value;
                roomText.text = $"{_roomInfo.Name} ({_roomInfo.PlayerCount}/{_roomInfo.MaxPlayers})";
            } // 각 고유 방만의 정보
        }
        #endregion


        private void Awake() // 하위 요소 넣기
        {
            roomBtn = transform.GetComponent<Button>();
            roomText = transform.Find("RoomInfo").GetComponent<Text>();
            roomBtn.onClick.AddListener(Join_Btn);
        }

        #region Function Fields
        public void SetRoomInfo(RoomInfo roomInfo)
        {
            myRoomInfo = roomInfo;
            roomText.text = roomInfo.MaxPlayers + ", " + roomInfo.Name;
        }

        //public void Create_Btn()
        //{
        //    if (string.IsNullOrEmpty(roomName.text))
        //        return;
        //    PhotonNetwork.JoinRoom(RoomInfo.Name);
        //}

        public void Join_Btn()
        {
            string roomName = this.myRoomInfo.Name;
            PhotonNetwork.JoinRoom(roomName);
            PunCallbacks.statusText.text = "방 입장 중...";
            PunCallbacks.statusUI.SetActive(true);
        }
        #endregion
    }


}
