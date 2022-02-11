using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;


namespace com.ThreeCS.McCree
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields
        // 어디서든 사용 가능
        static public GameManager Instance;

        public UI ui;
        public PlayerManager playerManager;

        #endregion


        #region Private Fields

        // 플레이어 프리팹
        [SerializeField]
        private GameObject playerPrefab;

        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
            // 어디서든 쓸 수 있게 인스턴스화
            Instance = this;

            ui = GetComponent<UI>();

            // 접속 못하면 초기화면으로 쫓아냄
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene("Launcher");
                return;
            }
        }

        void Start()
        {
            if (playerPrefab == null)
            {
                Debug.Log("플레이어 프리팹이 생성되지 못했습니다. 게임매니저에서 확인하세요.");
            }
            else
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    SpawnPlayer();

                    // 게임을 시작하면 나오는 UI
                    StartCoroutine(GameStart());
                }
            }
        }

        void Update()
        {

        }

        #endregion

        #region Photon Callback

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.Log("플레이어가 입장했습니다 : " + other.NickName); 
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("당신이 마스터 클라이언트(방장)입니다."); 

                //LoadArena();
            }
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.Log("플레이어가 나갔습니다 : " + other.NickName); 
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("마스터클라이언트가 바뀌었습니다. :  {0}", PhotonNetwork.IsMasterClient);

                //LoadArena();
            }
        }
        #endregion

        #region Public Methods

        IEnumerator GameStart()
        {
            playerManager = GameObject.FindWithTag("Player").GetComponent<PlayerManager>();

            yield return new WaitForEndOfFrame();
            yield return new AsyncOperation();
            
            float t = 0;
            while (t < ui.uiSpeed)
            {
                t += 1 * Time.deltaTime;
                ui.jobBoard.anchoredPosition = Vector3.Lerp(Vector3.up * 1000, Vector3.zero, t);
                Debug.Log("--------직업 텍스트 출력--------");
                ui.jobText.text = JobText();

                yield return null;
            }
            yield return new WaitForSeconds(1f);

            Debug.Log("내려오기");

            t = 0;
            while (t < ui.uiSpeed)
            {
                t += 1 * Time.deltaTime;
                ui.jobBoard.anchoredPosition = Vector3.Lerp(Vector3.zero, Vector3.down * 1000, t);
                yield return null;
            }

            Debug.Log("사라지기");

            yield return new WaitForSeconds(1f);

            ui.jobPanel.SetActive(false);

            
        }

        
        // 플레이어 생성
        public void SpawnPlayer()
        {
            Debug.Log("로컬플레이어를 생성합니다.");
            int ran1 = Random.Range(-10, 10);
            int ran2 = Random.Range(-10, 10);
            // 로컬 플레이어를 스폰합니다. 동기화도 됨
            PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(ran1, 5f, ran2), Quaternion.identity, 0);
        }

        // 방 나가기 임시 구현
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        /*void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("마스터클라이언트가 아니면 씬, 레벨을 로드할수 없습니다.");
            }

            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);

            PhotonNetwork.LoadLevel("Game");
        }*/

        #endregion

        #region

        // 직업 관련 내용
        public string JobText()
        {
            string temp = "";
            Debug.Log(playerManager.playerType);
            switch (playerManager.playerType)
            {
                case PlayerManager.Type.Sheriff:
                    Debug.Log("당신은 보안관입니다.");
                    temp = "보안관 입니다. 부관을 찾고 무법자를 전부 제거하십시오.";
                    break;
                case PlayerManager.Type.Vice:
                    Debug.Log("당신은 부관입니다.");
                    temp = "부관 입니다. 보안관을 도와 무법자를 전부 제거하십시오.";
                    break;
                case PlayerManager.Type.Outlaw:
                    Debug.Log("당신은 무법자입니다.");
                    temp = "무법자 입니다. 다른 무법자와 함께 보안관을 살해하십시오.";
                    break;
                case PlayerManager.Type.Renegade:
                    Debug.Log("당신은 배신자입니다.");
                    temp = "배신자 입니다. 당신은 보안관에겐 부관처럼 무법자에겐 친구처럼 보이십시오. 하지만 마지막에 살아남는건 당신 혼자이어야합니다.";
                    break;
            }

            return temp;
        }

        #endregion


    }
}