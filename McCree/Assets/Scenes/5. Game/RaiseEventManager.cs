using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

namespace com.ThreeCS.McCree
{
    public class RaiseEventManager : MonoBehaviour
    {
        private static RaiseEventManager pInstance;

        public static RaiseEventManager Instance
        {
            get { return pInstance; }
        }

        private PhotonView photonView;
        private PlayerManager playerManager;
        private PlayerInfo playerInfo;
        private UI ui;


        [SerializeField]
        private GameObject playerPrefab; // 1
        [SerializeField]
        private GameObject pipePrefab; // 2
        [SerializeField]
        private List<Quest_Obj> questObjList; // 3 , 퀘스트 리스트

        private const byte CharacterInstantiate = 1;
        private const byte PipesInstantiate = 2;
        private const byte AddWorldQuest = 3;


        void Awake()
        {
            pInstance = this;
        }

        public void FindMinePv(GameObject player)
        {
            photonView = player.GetComponent<PhotonView>();
            playerManager = player.GetComponent<PlayerManager>();
            playerInfo = player.GetComponent<PlayerInfo>();
            ui = player.GetComponent<UI>();
        }


        void OnEnable()
        {
            // 활성화 될 때마다 호출되는 함수 (Awake/Start와 달리 활성화 될 때마다)

            PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
        }

        void OnDisable()
        {
            // 비활성화 될 때마다 호출되는 함수 (스크립트든 오브젝트든)

            PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
        }

        private void NetworkingClient_EventReceived(EventData obj)
        {
            if (obj.Code == CharacterInstantiate)
            {
                object[] data = (object[])obj.CustomData;

                GameObject player = (GameObject)Instantiate(playerPrefab, (Vector3)data[0], (Quaternion)data[1]);
                PhotonView photonView = player.GetComponent<PhotonView>();
                photonView.ViewID = (int)data[2];
            }
            else if (obj.Code == PipesInstantiate)
            {
                object[] data = (object[])obj.CustomData;

                GameObject pipe = (GameObject)Instantiate(pipePrefab, (Vector3)data[0], (Quaternion)data[1]);
                pipe.name = pipePrefab.name;

                PhotonView photonView = pipe.GetComponent<PhotonView>();
                photonView.ViewID = (int)data[2];

            }
            else if (obj.Code == AddWorldQuest)
            {
                object[] data = (object[])obj.CustomData;

                int questIndex = (int)data[0];




                // 퀘스트 생성해서 오른쪽 상단 목표에 붙여줌
                GameObject subquestObj = Instantiate(MineUI.Instance.questObj, MineUI.Instance.worldQuestPanel);

                Quest_Obj questObj = Instantiate(questObjList[questIndex]);
                subquestObj.GetComponent<SubQuestList>().questObj = questObj;
                subquestObj.GetComponent<SubQuestList>().questTitle.text = questObj.questTitle_progress;

                // 내 퀘스트리스트에 붙임
                playerInfo.myQuestList.Add(subquestObj.GetComponent<SubQuestList>());

                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)MineUI.Instance.worldQuestPanel);
                // contentsizefillter가 적용이 안되는 오류때메 재배치하는 함수
            }
        }

        // 캐릭터 생성 --------------------------------------------------------------------
        public void Spwan_Player()
        {
            float ran1 = Random.Range(-2, 2);
            float ran2 = Random.Range(-2, 2);

            GameObject player = Instantiate(playerPrefab);
            PhotonView photonView = player.GetComponent<PhotonView>();

            if (PhotonNetwork.AllocateViewID(photonView))
            {
                object[] data = new object[]
                {
                    new Vector3(ran1, 2f, ran2), Quaternion.identity, photonView.ViewID
                };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.Others,
                    CachingOption = EventCaching.AddToRoomCache
                };

                SendOptions sendOptions = new SendOptions
                {
                    Reliability = true
                };
                PhotonNetwork.RaiseEvent(CharacterInstantiate, data, raiseEventOptions, sendOptions);
            }
            else
            {
                Debug.LogError("Failed to allocate a ViewId.");
                Destroy(player);
            }
        }

        // -----------------------------------------------------------------------------

        // 파이프
        public void Pipe_Instantiate(object[] data)
        {
            GameObject pipe = Instantiate(pipePrefab, (Vector3)data[0], (Quaternion)data[1]);
            pipe.name = pipePrefab.name;
            PhotonView photonView = pipe.GetComponent<PhotonView>();


            if (PhotonNetwork.AllocateViewID(photonView))
            {
                object[] data2 = new object[]
                {
                    (Vector3)data[0], (Quaternion)data[1], photonView.ViewID
                };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.Others,
                    CachingOption = EventCaching.AddToRoomCache
                };

                SendOptions sendOptions = new SendOptions
                {
                    Reliability = true
                };
                PhotonNetwork.RaiseEvent(PipesInstantiate, data2, raiseEventOptions, sendOptions);
            }
            else
            {
                Debug.LogError("Failed to allocate a ViewId.");
                Destroy(pipe);
            }
        }


        public void Add_World_Quest(NPC npc)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.Others,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            for (int i = 0; i < questObjList.Count; i++)
            {
                Debug.Log(questObjList[i].qrange);
                if (npc.questObj.name == questObjList[i].name)
                {
                    Debug.Log("aaaaaaaaaaaa");
                    object[] data = new object[]{ i };
                    PhotonNetwork.RaiseEvent(AddWorldQuest, data, raiseEventOptions, sendOptions);
                    break;
                }
            }
        }
    }
}