using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class Lobby : MonoBehaviourPunCallbacks
    {
        #region Private Fields

        [SerializeField]    // 동시 접속자 수 
        private Text curPlayerText;     

        [SerializeField]
        private Transform content;
        
        [SerializeField]   // 룸 리스트 하나의 Prefab
        private GameObject roomListing;
        //private RoomList roomListing;

        private Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>();

        private List<RoomList> listings = new List<RoomList>();

        #endregion

        #region MonoBehaviour CallBacks

        private void Update()
        {
            //Debug.Log(roomDict.Count + "   " + PhotonNetwork.CountOfRooms);
        }
        void FixedUpdate()
        {
            CheckPlayerCount();
        }

        #endregion

        #region MonoBehaviourPunCallbacks Callbacks
        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            Debug.Log("로비 참가 성공");
            SceneManager.LoadScene("Lobby");
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log("방 참가 성공");
            SceneManager.LoadScene("Room");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            Debug.Log("연결 끊어짐");
        }

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            Debug.Log("방 만들기 성공");
            SceneManager.LoadScene("Room");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);
            Debug.Log("방 만들기 실패");
        }

        // 룸에 관련된 사항이 바뀔때 호출되는 함수 
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            //Debug.Log("?");
            //foreach (RoomInfo info in roomList)
            //{
            //    // 없어진 룸 리스트에서 없앨 때
            //    if (info.RemovedFromList)
            //    {
            //        int index = listings.FindIndex(x => x.myRoomInfo.Name == info.Name);
            //        Debug.Log(index);
            //        if (index != -1)
            //        {
            //            Destroy(listings[index].gameObject);
            //            listings.RemoveAt(index);
            //        }
            //    }
            //    else // 생성된 룸 리스트에 더할 때
            //    {
            //        Debug.Log("???");
            //        RoomList listing = Instantiate(roomListing, content);
            //        if (listing != null)
            //        {
            //            listing.SetRoomInfo(info);
            //            listings.Add(listing);
            //            Debug.Log(listing.myRoomInfo);
            //        }
            //    }
            //}
            //base.OnRoomListUpdate(roomList);

            GameObject tempRoom = null;
            foreach (var room in roomList)
            {
                // 룸이 삭제된 경우
                if (room.RemovedFromList == true)
                {
                    roomDict.TryGetValue(room.Name, out tempRoom);
                    Destroy(tempRoom);
                    roomDict.Remove(room.Name);
                }
                // 룸 정보가 갱신된경우
                else
                {
                    // 룸이 하나도 없을 경우
                    if (roomDict.ContainsKey(room.Name) == false)
                    {
                        GameObject _room = Instantiate(roomListing, content);
                        _room.GetComponent<RoomList>().myRoomInfo = room;
                        roomDict.Add(room.Name, _room);
                    }
                    // 룸 정보를 갱신하는 이유
                    else
                    {
                        roomDict.TryGetValue(room.Name, out tempRoom);
                        tempRoom.GetComponent<RoomList>().myRoomInfo = room;
                    }
                }
            }

        }

        #endregion

        #region Public Methods

        // 동시 접속자 수 표현
        public void CheckPlayerCount()
        {
            int connectPlayer = PhotonNetwork.CountOfPlayers;
            //Debug.Log("현재 동시접속자수 " + connectPlayer);
            curPlayerText.text = "현재 접속자 수 : " + connectPlayer.ToString();
        }
        #endregion

    }


}
