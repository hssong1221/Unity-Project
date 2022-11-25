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
                // 의자와 상호작용해서 의자에 앉기
                if(other.tag == "chair")
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
                if (other.tag == "chair")
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
    
    }
}