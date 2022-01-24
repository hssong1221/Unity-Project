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

        [SerializeField]    // ���� ������ �� 
        private Text curPlayerText;     

        [SerializeField]    // �� ����� �޴�
        private GameObject createRoomMenu;

        [SerializeField]    // �� ����� �޴� �ȿ� �Է�ĭ
        private InputField roomName;


        #endregion


        #region MonoBehaviour CallBacks

        void FixedUpdate()
        {
            CheckPlayerCount();
        }

        #endregion

        #region MonoBehaviourPunCallbacks Callbacks

        // �濡 ���� �۵�
        public override void OnJoinedRoom()
        {
            Debug.Log("Room Create!");

            // �� ����� â ��
            createRoomMenu.SetActive(false);
            // �� ������ �̵�
            SceneManager.LoadScene("Room");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("Room create Failed!!");
            Debug.LogError(message);
        }
        #endregion

        #region Public Methods

        // ���� ������ �� ǥ��
        public void CheckPlayerCount()
        {
            int connectPlayer = PhotonNetwork.CountOfPlayers;
            Debug.Log("���� ���������ڼ� " + connectPlayer);
            curPlayerText.text = "���� ������ �� : " + connectPlayer.ToString();
        }

        // �� ����� ��ư ������ ��
        public void CreateRoom()
        {
            // �� ����� �޴� ���̰� ��
            createRoomMenu.SetActive(true);

            
            Debug.Log("roomname : " + roomName.text);

            // �� �̸��� ��ĭ�̸� �� �����
            if (string.IsNullOrEmpty(roomName.text))
                return;

            // �� �̸����� �� ����
            PhotonNetwork.CreateRoom(roomName.text);

        }

        #endregion
    }

}
