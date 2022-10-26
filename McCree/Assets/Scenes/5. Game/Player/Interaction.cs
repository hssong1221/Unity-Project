using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

namespace com.ThreeCS.McCree
{
    public class Interaction : Controller
    {

        private IEnumerator _coroutine_Chat;
        public IEnumerator coroutine_Chat
        {
            get { return _coroutine_Chat; }
            set
            {
                _coroutine_Chat = value;
            }
        }

        private IEnumerator _coroutine_Interact;
        public IEnumerator coroutine_Interact
        {
            get { return _coroutine_Interact; }
            set
            {
                _coroutine_Interact = value;
            }
        }

        // 의자 접촉 시 하이라이트 관련
        [HideInInspector]
        public MeshRenderer mr;
        [HideInInspector]
        public Material mat;

        // 의자에 앉기
        public bool isSit = false;
        // 의자에 닿기
        public bool triggerStay = false;
        public int sitNum = 0;   // 본인

        public string chairName; // 앉은 의자 이름

        //private IEnumerator _coroutine;
        //public IEnumerator coroutine
        //{
        //    get { return _coroutine; }
        //    set
        //    {
        //        _coroutine = value;
        //    }
        //}

        // 의자 착석 플래그

        void Awake()
        {
            base.Awake();
            MineUI.Instance.rejectBtn.onClick.AddListener(Close_NPC_Chat);

        }

        private void Update()
        {
            // 의자와 상호작용
            if (Input.GetButtonDown("Interaction") && triggerStay)
            {
                isSit = !isSit;
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (photonView.IsMine && playerManager.canBehave)
            {
                if (other.tag == "NPC")
                {
                    if (!other.GetComponent<NPC>().isComplete)
                    {
                        MineUI.Instance.range_x = Random.Range(-150, 150);
                        MineUI.Instance.range_y = Random.Range(-100, 100);
                        MineUI.Instance.interactionRect.anchoredPosition = new Vector2(MineUI.Instance.range_x, MineUI.Instance.range_y);
                        MineUI.Instance.interactionPanel.SetActive(true);
                        MineUI.Instance.interactionText.text = "대화 하기";
                        // F 상호작용 랜덤 위치

                        if (coroutine_Chat != null)
                            StopCoroutine(coroutine_Chat);
                        coroutine_Chat = returnchatList(0, other);
                        StartCoroutine(coroutine_Chat);
                        // 트리거된 상태에서 F누르면 대화창 뜰수있도록 코루틴함수 실행
                    }
                }
                else if (other.gameObject.layer == LayerMask.NameToLayer("QuestItem"))
                {

                    if (other.gameObject.GetComponent<LiftItem>() != null)
                    {   // 월드퀘스트 진행 중 다른애가 들고있는 운반중인물품 줍기 불가
                        if (other.gameObject.GetComponent<LiftItem>().isLifting)
                        {
                            return;
                        }
                    }

                    foreach (SubQuestList subQuestObj in playerInfo.myQuestList)
                    {
                        Quest_Interface_PT_Obj pickQuest = (Quest_Interface_PT_Obj) subQuestObj.questObj;
                        // 줍거나 운반가능한 아이템일때 Quest_Interface_PT

                        // 내가 가지고있는 퀘스트 중 줍기퀘스트 아이템이있을때 
                        if (other.name == pickQuest.quest.bringGameObj.name)
                        {
                            MineUI.Instance.range_x = Random.Range(-100, 100);
                            MineUI.Instance.range_y = Random.Range(-75, 75);
                            MineUI.Instance.interactionRect.anchoredPosition = new Vector2(MineUI.Instance.range_x, MineUI.Instance.range_y);
                            MineUI.Instance.interactionPanel.SetActive(true);
                            MineUI.Instance.interactionText.text = "줍기";
                            // F 상호작용 랜덤 위치

                            if (coroutine_Interact != null)
                                StopCoroutine(coroutine_Interact);
                            coroutine_Interact = QuestItemInteraction(other);
                            StartCoroutine(coroutine_Interact);
                            // 트리거된 상태에서 F누르면 줍는 애니메이션 실행될수있도록
                            break;
                        }
                    }
                }
                // 의자와 상호작용해서 의자에 앉기
                else if(other.tag == "chair")
                {
                    if (other.GetComponent<ChairManager>().isPlayer == false)
                    {
                        // 의자 선착순 구현
                        photonView.RPC("ChairCheck", RpcTarget.All, other.name, 0);

                        triggerStay = true; //의자에 닿아있나 여부

                        // F 상호작용
                        MineUI.Instance.range_x = Random.Range(-50, 50);
                        MineUI.Instance.range_y = Random.Range(-100, 100);
                        MineUI.Instance.interactionRect.anchoredPosition = new Vector2(MineUI.Instance.range_x, MineUI.Instance.range_y);
                        MineUI.Instance.interactionPanel.SetActive(true);
                        MineUI.Instance.interactionText.text = "앉기";
                        Debug.Log("의자");

                        //의자 하이라이트
                        mr = other.GetComponent<MeshRenderer>();
                        mat = mr.material;
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", Color.white * 0.5f);

                        sitNum++;
                        GameManager.Instance.NumCheckSit();
                        // 앉는 위치에 따라서 플레이어 턴을 정함
                        photonView.RPC("TurnSync", RpcTarget.All, other.name, "sit");
                    }
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!playerManager.canBehave)
            {
                MineUI.Instance.interactionPanel.SetActive(false);
                if (coroutine_Interact != null)
                    StopCoroutine(coroutine_Interact);
            }
            if (photonView.IsMine && other.CompareTag("chair"))
            {
                if (other.GetComponent<ChairManager>().isPlayer == true && triggerStay)
                {
                    // 전체 인원 체크
                    if (isSit)
                    {
                        // 내가 앉은 의자
                        chairName = other.name;
                        playerManager.Sit(other.GetComponent<Transform>().transform, other.GetComponent<MeshRenderer>(), chairName);
                        MineUI.Instance.interactionPanel.SetActive(false);
                    }
                    else
                    {
                        playerManager.StandUp(other.GetComponent<Transform>().transform);
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (photonView.IsMine)
            {
                if (other.tag == "NPC")
                {
                    MineUI.Instance.interactionPanel.SetActive(false);
                    MineUI.Instance.chatPanel.SetActive(false);
                    playerManager.isInteraction = false;
                }
                else if (other.gameObject.layer == LayerMask.NameToLayer("QuestItem"))
                {
                    MineUI.Instance.interactionPanel.SetActive(false);
                }
                else if (other.tag == "chair")
                {
                    if (other.GetComponent<ChairManager>().isPlayer == true && triggerStay)
                    {
                        // 의자에 앉은 사람이 없다고 알려줌
                        photonView.RPC("ChairCheck", RpcTarget.All, other.name, 1);

                        triggerStay = false; // 의자에 계속 닿아있나 여부

                        MineUI.Instance.interactionPanel.SetActive(false);
                        mat.SetColor("_EmissionColor", Color.black);

                        sitNum--;
                        GameManager.Instance.NumCheckStand();

                        // 일어서서 나가면 그 위치에 저장된 플레이어 정보 삭제
                        photonView.RPC("TurnSync", RpcTarget.All, other.name, "stand");
                    }
                }

                if (coroutine_Chat != null)
                    StopCoroutine(coroutine_Chat);
                if (coroutine_Interact != null)
                    StopCoroutine(coroutine_Interact);
            }
        }

        [PunRPC]
        public void NumCheck(int n) // 탁자 위 인원수 UI
        {
            sitNum = n;
            // 앉은 인원 / 전체 인원
            GameManager.Instance.pnumText.text = sitNum + " / " + GameManager.Instance.playerList.Length;
        }

        [PunRPC]    
        public void TurnSync(string chairname, string state) // 앉은 사람을 list에 넣어주고 동기화 
        {
            if (state == "stand")
            {
                //의자 앉은 위치
                int temp = int.Parse(chairname);
                // turnList에 의자 위치대로 플레이어를 넣어줌
                foreach (GameObject player in GameManager.Instance.playerList)
                {
                    if (player.GetComponent<PhotonView>().ViewID == photonView.ViewID)
                    {
                        GameManager.Instance.sitList[temp] = GameManager.Instance.tempsit;
                    }
                }
            }
            else if(state == "sit")
            {
                //의자 앉은 위치
                int temp = int.Parse(chairname);
                // turnList에 의자 위치대로 플레이어를 넣어줌
                foreach (GameObject player in GameManager.Instance.playerList)
                {
                    if (player.GetComponent<PhotonView>().ViewID == photonView.ViewID)
                    {
                        GameManager.Instance.sitList[temp] = player;
                    }
                }
            }
        }

        [PunRPC]
        public void ChairCheck(string chairname, int num)   // 의자에 중복으로 앉는거 방지 
        {
            GameObject playerchair = null;
            GameObject[] chairs = GameObject.FindGameObjectsWithTag("chair");
            foreach(GameObject chair in chairs)
            {
                if (chair.name.Equals(chairname))
                    playerchair = chair;
            }

            if(num == 0)
                playerchair.GetComponent<ChairManager>().isPlayer = true;
            else if(num == 1)
                playerchair.GetComponent<ChairManager>().isPlayer = false;
        }

        


        #region npc대화 관련
        IEnumerator returnchatList(int num, Collider other)
        {
            while (true)
            {
                Debug.Log("npc겹치는중");
                if (Input.GetButtonDown("Interaction"))
                {
                    MineUI.Instance.interactionPanel.SetActive(false);

                    NPC npc = other.GetComponent<NPC>();
                    if (!MineUI.Instance.chatPanel.activeSelf)
                    {   // chatPanel창 안켜져있으면 변수 저장
                        playerManager.isInteraction = true;

                        MineUI.Instance.npcName.text = npc.questObj.quest.npcName;
                        MineUI.Instance.npcImg.sprite = npc.questObj.quest.npcImg;

                        
                        MineUI.Instance.npcChat.text = npc.questObj.npcChatList[num++];
                        // 첫번째 대화

                        MineUI.Instance.npcbtns.SetActive(false);
                        MineUI.Instance.chatPanel.SetActive(true);
                    }
                    else
                    {
                        // chatPanel창 켜져있으면 다음꺼 보여주고
                        if (num < npc.questObj.npcChatList.Count)
                        {
                            MineUI.Instance.npcChat.text = npc.questObj.npcChatList[num++]; ;
                        }
                        else // 끝이면 비활성화
                        {
                            if (npc.questObj.qState == Quest_Obj.qType.Start)
                            {  // 퀘스트 받을때만 수락/거절 버튼
                                MineUI.Instance.npcbtns.SetActive(true);

                                MineUI.Instance.acceptBtn.onClick.RemoveAllListeners();
                                
                                MineUI.Instance.acceptBtn.onClick.AddListener( // 수락할때 해당 퀘스트로 다시 이어주기위해 AddListner
                                    delegate ()
                                    {
                                        Accept_NPC_Chat(npc);
                                    });
                            }
                            else if (npc.questObj.qState == Quest_Obj.qType.Complete)
                            {
                                // 보상
                                foreach (ItemList itemList in playerInfo.myItemList)
                                {
                                    if (itemList.item.ability == npc.questObj.quest.reward)
                                    {
                                        itemList.itemCount++;
                                        break;
                                    }
                                }

                                // 완료한 퀘스트 목록에서 삭제
                                foreach (SubQuestList subQuest in playerInfo.myQuestList)
                                {
                                    if (subQuest.questObj == npc.questObj)
                                    {
                                        subQuest.GetComponent<QuestListAnim>().Destroy_Object_End_Anim();
                                        break;
                                    }
                                }

                                npc.isComplete = true;
                                Close_NPC_Chat();
                                photonView.RPC("QuestComplete", RpcTarget.All, npc.questObj.quest.questTitle);
                            }

                            else
                            {   // 나머지 대화 끝나면 닫아줌
                                Close_NPC_Chat();
                            }
                            
                        }
                    }
                }

                yield return null;
            }
        }


        void Accept_NPC_Chat(NPC npc)
        {
            if (photonView.IsMine)
            {
                if (npc.questObj.qrange == Quest_Obj.oType.World) 
                {
                    RaiseEventManager.Instance.Add_World_Quest(npc); // 월드 퀘스트라면 전체에게 퀘스트 추가
                    photonView.RPC("QuestLog", RpcTarget.All, npc.questObj.quest.questTitle); // 퀘스트 알림
                }
                else // 서브 퀘스트면 나한테만 추가
                {
                    // 퀘스트 진행중으로 상태 바꿈
                    npc.questObj.qState = Quest_Obj.qType.Progress;
                    npc.questObj.npcChatList = npc.questObj.quest.npcChatList_progress;

                    GameObject subquestObj;

                    subquestObj = Instantiate(MineUI.Instance.questObj, MineUI.Instance.subQuestPanel);

                    subquestObj.GetComponent<SubQuestList>().questObj = npc.questObj;
                    subquestObj.GetComponent<SubQuestList>().questTitle.text = npc.questObj.questTitle_progress;

                    // 내 퀘스트리스트에 붙임
                    playerInfo.myQuestList.Add(subquestObj.GetComponent<SubQuestList>());
                }


                // contentsizefillter가 적용이 안되는 오류때메 재배치하는 함수
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)MineUI.Instance.worldQuestPanel);

                // npc대화 끔
                Close_NPC_Chat();
            }

            photonView.RPC("GetQuest", RpcTarget.All, npc.questObj.quest.questTitle);
        }

        void Close_NPC_Chat()
        {
            if (photonView.IsMine)
            {
                if (coroutine_Chat != null)
                    StopCoroutine(coroutine_Chat);
                MineUI.Instance.interactionPanel.SetActive(false);
                MineUI.Instance.chatPanel.SetActive(false);
                playerManager.isInteraction = false;
            }
        }
        #endregion

        #region 퀘스트 아이템 관련
        IEnumerator QuestItemInteraction(Collider other)
        {
            while (true)
            {
                Debug.Log("화제의 코루틴");
                if (Input.GetButtonDown("Interaction") && !playerManager.isPicking)
                {   // 이미 줍고있을때 또 누르면 코루틴이 겹쳐서 빨라짐
                    // 또 줍는걸 방지
                    ui.InterAction(5, other.gameObject);
                    MineUI.Instance.interactionPanel.SetActive(false);
                }

                yield return null;
            }
        }

        public void StopCoroutine_Direct(IEnumerator get_Coroutine) // 코루틴은 해당 파일에서 꺼야함
        {
            if (get_Coroutine != null)
            {
                StopCoroutine(get_Coroutine);
            }
        }

        #endregion
    }
}