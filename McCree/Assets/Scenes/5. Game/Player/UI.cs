using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class UI : Controller
    {
        #region Variable Field

        [Header("체력관련 UI")]
        public Canvas hpCanvas;

        public GameObject[] hpImgs;

        public Sprite fullBullet;
        public Sprite emptyBullet;
        public Text nickName; // 닉네임

        [Header("뱅 준비 사거리 표시 관련 UI")]
        public Canvas attackRangeCanvas;  // 공격 사거리 캔버스
        public Image indicatorRangeCircle;// 공격 사거리 이미지
        public int attackRange; // 기본 공격 사거리 일단 1로 설정해놓은상태

        [Header("Bang! 말풍선 관련 UI")]
        public Canvas bangCanvas;         // 뱅 말풍선 캔버스
        public Image bangGifImg;          // 뱅 말풍선 gif 이미지


        [Header("아이템 관련 UI")]
        public Canvas itemCanvas;   // 아이템 알림 캔버스
        public Text itemNotice;     // 아이템 알림 텍스트


        [Header("ProgressBar 관련 UI")]
        public Canvas progressCanvas;
        public Text progressText;
        public Text progressPercent;
        public Image progressBar;
        float currentValue;

        public IEnumerator coroutine;

        Vector3 hpOffset;
        Vector3 bangOffset;
        Vector3 itemOffset;
        Vector3 progressOffset;

        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
            base.Awake();  // Controller Awake 함수 그대로 사용

            hpOffset = new Vector3(0, 2.0f, 0);
            bangOffset = new Vector3(0, 3.0f, 0);
            itemOffset = new Vector3(0.5f, 1.0f, 0f);
            progressOffset = new Vector3(0, 2.5f, 0f);

            
        }


        private void Start()
        {
            // 기본사거리
            attackRange = 1;
            // 기본 Indicator Range img 크기
            indicatorRangeCircle.rectTransform.localScale = new Vector3(attackRange, attackRange, 0);
            // 공격범위 UI 꺼주기
            indicatorRangeCircle.enabled = false;
            // 뱅 말풍선 UI 꺼주기
            bangGifImg.enabled = false;
            // 아이템 공지 UI 꺼주기
            itemNotice.enabled = false;

            // 프로그레스바 UI 꺼주기
            progressText.enabled = false;
            progressBar.enabled = false;
            progressPercent.enabled = false;

            if (photonView.IsMine)
            {
                nickName.text = PhotonNetwork.LocalPlayer.NickName;
            }
            else
            {
                nickName.text = GetComponent<PhotonView>().Owner.NickName;
            }

        }

        void Update()
        {
            // 플레이어 머리 위 UI 각도 고정
            hpCanvas.transform.LookAt(hpCanvas.transform.position + Camera.main.transform.forward);
            bangCanvas.transform.LookAt(bangCanvas.transform.position + Camera.main.transform.forward);
            itemCanvas.transform.LookAt(itemCanvas.transform.position + Camera.main.transform.forward);
            progressCanvas.transform.LookAt(progressCanvas.transform.position + Camera.main.transform.forward);

            if (photonView.IsMine)
            {
                // 각자 캐릭터 머리 위 UI 위치 고정
                hpCanvas.transform.position = character.transform.position + hpOffset;
                bangCanvas.transform.position = character.transform.position + bangOffset;
                itemCanvas.transform.position = character.transform.position + itemOffset;
                progressCanvas.transform.position = character.transform.position + progressOffset;

                if (playerInfo.isDeath && playerInfo.hp <= 0)
                    GameManager.Instance.LeaveRoom();
            }
            else
                return;
        }


        public void PickInterAction(float time, GameObject interactObj)
        {
            if (!playerManager.isPicking) // 이미 줍고있을때 또 누르면 코루틴이 겹쳐서 빨라짐
            {                             // 또 줍는걸 방지
                animator.SetBool("IsPicking", true);
                playerManager.isPicking = true;
                LoadingProgreeCircle(time, interactObj);
            }
        }

        public void LoadingProgreeCircle(float time, GameObject interactObj)
        {
            progressText.enabled = true;
            progressBar.enabled = true;
            progressPercent.enabled = true;

            currentValue = 0;
            coroutine = LoadingProgreeCircleCoroutine(time, interactObj);
            StartCoroutine(coroutine);
        }

        IEnumerator LoadingProgreeCircleCoroutine(float time, GameObject interactObj)
        {
            while (true && playerManager.isPicking)
            {
                currentValue += 100 * Time.deltaTime / time; // 아마 초단위 맞을듯?
                progressPercent.text = ((int)currentValue).ToString() + "%";
                progressBar.fillAmount = currentValue / 100;


                if (progressBar.fillAmount > 0.99)
                {

                    foreach (SubQuestList subQuestObj in playerInfo.myQuestList)
                    {
                        Quest_PickUp_Obj pickQuest = (Quest_PickUp_Obj)subQuestObj.questObj;

                        if (interactObj.name == pickQuest.quest.bringGameObj.name)
                        {
                            pickQuest.count++; // 퀘스트 아이템 개수 증가
                            MineUI.Instance.interactionPanel.SetActive(false);

                            // 증가한 아이템 개수로 텍스트 바꿔줌
                            subQuestObj.questTitle.text = pickQuest.questTitle_progress;

                            Debug.Log("성공!");
                            break;
                        }
                    }
                    Off_ProgressUI();
                    //interactObj.SetActive(false);

                    Destroy(interactObj); // 애니메이션 끝나면 오브젝트 파괴
                }
                yield return null;
            }
        }

        public void Off_ProgressUI() // 원 진행도, interaction 코루틴 스탑
        {
            playerManager.isPicking = false;
            animator.SetBool("IsPicking", false);
            if (coroutine != null) 
                StopCoroutine(coroutine);
            if (interaction.coroutine != null)
                interaction.Direct_StopCoroutine();
            progressText.enabled = false;
            progressBar.enabled = false;
            progressPercent.enabled = false;
        }

        public void CanCel_Animation()
        {
            MineUI.Instance.interactionPanel.SetActive(true);

            playerManager.isPicking = false;
            animator.SetBool("IsPicking", false);
            if (coroutine != null) // 원 진행도 코루틴만 스탑, 다시 주울수있도록 interaction코루틴은 끄지않음
                StopCoroutine(coroutine);
            progressText.enabled = false;
            progressBar.enabled = false;
            progressPercent.enabled = false;
        }
        #endregion



    }


}