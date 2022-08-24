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
        private Animator anim;

        [SerializeField]
        private GameObject playerPrefab;
        [SerializeField]
        private GameObject pipePrefab;


        [SerializeField]
        private List<NPC> npcObjList; // 하이어라키에서 순서대로 끌고오기

        [SerializeField]
        private List<Transform> npcSpawnPointList;


        private const byte CharacterInstantiate = 1; // 캐릭터 생성
        private const byte NPCInstantiate = 2;       // NPC 생성
        private const byte PipesInstantiate = 3;     // 월드퀘스트 파이프 생성
        private const byte AddWorldQuest = 4;        // 월드퀘트스 동기화

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
            anim = player.GetComponent<Animator>();
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

            else if (obj.Code == NPCInstantiate)
            {
                object[] data = (object[])obj.CustomData;

                int[] npcArray = (int[])data[0];
                int[] posArray = (int[])data[1];

                for (int i = 0; i < npcArray.Length; i++)
                {
                    npcObjList[npcArray[i]].transform.position = npcSpawnPointList[posArray[i]].position;
                }
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

                // 상단 목표에 부착되는 오브젝트 생성
                GameObject subquestObj = Instantiate(MineUI.Instance.questObj, MineUI.Instance.worldQuestPanel);

                // NPC리스트에서 해당 퀘스트에 맞는 퀘스트 오브젝트 생성
                Quest_Obj questObj = npcObjList[questIndex].questObj;

                subquestObj.GetComponent<SubQuestList>().questObj = questObj; // 오브젝트에 퀘스트 연동
                subquestObj.GetComponent<SubQuestList>().questTitle.text = questObj.questTitle_progress;
                subquestObj.GetComponent<SubQuestList>().questObj.qState = Quest_Obj.qType.Progress; 
                // 누군가 실행시켜놓은것이므로 퀘스트 진행중이라 바꿈 

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


        // NPC 스폰 --------------------------------------------------------------------
        public void Spawn_NPC()
        {
            int[] npcArray = new int[npcObjList.Count];
            for (int i = 0; i < npcObjList.Count; i++)
                npcArray[i] = i;
            npcArray = CommonFunction.ShuffleArray(npcArray); // npc배열

            int[] posArray = new int[npcSpawnPointList.Count];
            for (int i = 0; i < npcSpawnPointList.Count; i++)
                posArray[i] = i;
            posArray = CommonFunction.ShuffleArray(posArray); // npc spawnPoint 배열

            for(int i=0; i<npcArray.Length; i++)
            {
                Debug.Log("npc[" + i + "]" + npcArray[i]);
            }
            for(int i=0; i<posArray.Length; i++)
            {
                Debug.Log("pos["+i+"]"+posArray[i]);
            }

            object[] content = new object[] { npcArray , posArray };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(NPCInstantiate, content, raiseEventOptions, SendOptions.SendReliable);
        }
        // -----------------------------------------------------------------------------



        // 파이프
        public void Pipe_Instantiate(object[] data)
        {
            if(PhotonNetwork.IsMasterClient)
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

            //RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
            //PhotonNetwork.RaiseEvent(PipesInstantiate, data, raiseEventOptions, SendOptions.SendReliable);


            


            //if (PhotonNetwork.AllocateViewID(photonView))
            //{
            //    object[] data2 = new object[]
            //    {
            //        (Vector3)data[0], (Quaternion)data[1], photonView.ViewID
            //    };

            //    RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            //    {
            //        Receivers = ReceiverGroup.Others,
            //        CachingOption = EventCaching.AddToRoomCache
            //    };

            //    SendOptions sendOptions = new SendOptions
            //    {
            //        Reliability = true
            //    };
            //    PhotonNetwork.RaiseEvent(PipesInstantiate, data2, raiseEventOptions, sendOptions);
            //}
            //else
            //{
            //    Debug.LogError("Failed to allocate a ViewId.");
            //    Destroy(pipe);
            //}
        }

        // 월드 퀘스트 캐릭터 퀘스트리스트에 추가
        public void Add_World_Quest(NPC npc)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

            for (int i = 0; i < npcObjList.Count; i++)
            {
                if (npc.questObj.name == npcObjList[i].questObj.name)
                {
                    object[] data = new object[] { i };
                    PhotonNetwork.RaiseEvent(AddWorldQuest, data, raiseEventOptions, SendOptions.SendReliable);
                    break;
                }
            }
        }
    }
}