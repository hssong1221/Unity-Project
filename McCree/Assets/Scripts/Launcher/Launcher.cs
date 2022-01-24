using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Fields

        // ���� �����ε� ���� ��ť��Ʈ���� ������ ���ٰ� �ϴ���� ���ϴ��� �� ��
        string gameVersion = "1";

        [SerializeField]
        private Text statusText; // ���� ���� �ؽ�Ʈ

        #endregion


        #region MonoBehaviour CallBacks

        void Start()
        {
            // ������ ȥ�� ���� �ε� �ϸ�, ������ ������� �ڵ����� ��ũ��
            //PhotonNetwork.AutomaticallySyncScene = true;

            // ���� ���� ������ ���� ������ ������ ����
            Debug.Log("Connect to MasterServer....");
            statusText.text = "������ ���� ��..";
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }

        #endregion

        #region MonoBehaviourPunCallbacks Callbacks

        // ������ ������ ���� �� �۵�
        public override void OnConnectedToMaster()
        {
            // �κ� ����
            Debug.Log("Connected MasterServer");
            statusText.text = "������ ���� ����!";
        }

        // ���� ���� ���н� �ڵ� ����
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Connecting failed!! Trying re-connect..");
            statusText.text = "�������� : ������ ���� ����\n ���� ��õ� ��...";
            // ������ ������ ������ ���� ���� �õ�
            PhotonNetwork.ConnectUsingSettings();
        }

        // �κ�� ���� �� �۵�
        public override void OnJoinedLobby()
        {
            // �κ� ������ �ʿ��� �޴��� ������
            Debug.Log("Connected Lobby");
            
        }

        #endregion

        #region Public Methods

        public void JoinLobby()
        {
            PhotonNetwork.JoinLobby();
            SceneManager.LoadScene("Lobby");
        }

        #endregion
    }

}

