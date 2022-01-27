using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

using Photon.Chat;
using ExitGames.Client.Photon;

public class RoomChat : MonoBehaviourPunCallbacks
{

    public List<string> chatList = new List<string>();
    public Button sendBtn;
    public Text chatLog;
    public InputField chatInput;
    public ScrollRect scr;

    ChatClient chatClient;

    private void Awake()
    {
        sendBtn.onClick.AddListener(Send_Chat);
    }

    void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        //chatClient = new ChatClient(this);
        //chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,
        //    PhotonNetwork.AppVersion, new Photon.Chat.AuthenticationValues(PhotonNetwork.LocalPlayer.UserId));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !chatInput.isFocused)
            Send_Chat();
    }

    public void Send_Chat()
    {
        if (chatInput.text.Equals(""))
        {
            Debug.Log("Empty Chat");
            return;
        }
        string msg = string.Format("[{0}] {1}",
            PhotonNetwork.LocalPlayer.NickName, chatInput.text);
        photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, msg);
        ReceiveMsg(msg);
        chatInput.ActivateInputField();
        chatInput.text = "";
    }
    [PunRPC]
    public void ReceiveMsg(string msg)
    {
        chatLog.text += msg + "\n";
        scr.verticalNormalizedPosition = 0.0f;
    }

}
