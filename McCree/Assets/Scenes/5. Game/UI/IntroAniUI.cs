using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace com.ThreeCS.McCree
{
    public class IntroAniUI : MonoBehaviour
    {
        [Header("직업 관련 UI")]
        public GameObject jobPanel;
        public RectTransform jobBoard;
        public Text jobText;
        public Image jobImage1;         // 직업마다 이미지가 다르게 할 것
        public Image jobImage2;
        public Image jobImage3;
        private Animator jobUIAnimator;
        private Animator abilUIAnimator;

        [Header("고유 능력 관련 UI")]
        public GameObject abilPanel;
        public Image abilImage;
        public Text abilText;

        [Header("직업 일러스트")]
        public Sprite sheriff1;  // 보안관 일러스트
        public Sprite sheriff2;
        public Sprite sheriff3;

        public Sprite deputy1;     // 부관   일러스트
        public Sprite deputy2;
        public Sprite deputy3;

        public Sprite outlaw1;   // 무법자 일러스트
        public Sprite outlaw2;
        public Sprite outlaw3;

        public Sprite renegade1;  // 배신자 일러스트
        public Sprite renegade2;
        public Sprite renegade3;

        [Header("어빌 일러스트")]
        public Sprite sheriff4;  // 능력 나올 때의 직업 일러
        public Sprite deputy4;
        public Sprite outlaw4;
        public Sprite renegade4;

        // --------------코루틴 캐싱
        WaitForSeconds wait05f = new WaitForSeconds(0.5f);
        WaitForSeconds wait1f = new WaitForSeconds(1f);
        WaitForSeconds wait2f = new WaitForSeconds(2f);
        WaitForSeconds wait3f = new WaitForSeconds(3f);

        PlayerManager playerManager;

        public static Action introAction;

        void Awake()
        {
            jobUIAnimator = jobPanel.GetComponent<Animator>();
            abilUIAnimator = abilPanel.GetComponent<Animator>();
        }

        private void Start()
        {
            introAction += IntroAniPlayHelper;
        }

        public void IntroAniPlayHelper()
        {
            StartCoroutine(IntroAniPlay());
        }

        public IEnumerator IntroAniPlay()
        {
            playerManager = GameManager.Instance.GetPlayerManager();

            // 직업 선택 텍스트랑 애니메이션 재생
            yield return YieldCache.WaitForEndOfFrame;

            jobPanel.SetActive(true);
            jobText.text = JobText();

            yield return new WaitForSeconds(6f);
            jobPanel.SetActive(false);
            abilPanel.SetActive(true);

            yield return wait05f;
            //abilUIAnimator.SetTrigger("Abil");
            abilText.text += AblityText();
            //abilText.text += "\n3. 당신의 능력을 잘 활용하십시오";

            // Status 창 (인벤토리창 동기화)
            StatusUI.Instance.job_Name.text = jobText.text;
            StatusUI.Instance.job_Explain.text = abilText.text;

            // ------------------------------------------------ 사람들이 텍스트를 읽을 시간 부여(나중에 다시 활성화) ----------------------------
            yield return new WaitForSeconds(7.5f);
            abilPanel.SetActive(false);
            MineUI.Instance.leftTopPanel.SetActive(true);
            MineUI.Instance.rightBottomPanel.SetActive(true);
            MineUI.Instance.rightTop.SetActive(true);
        }
        
        // 직업 관련 텍스트
        public string JobText()
        {
            var gm = GameManager.Instance;
            string temp = "";
            Debug.Log(playerManager.playerType);
            switch (playerManager.playerType)
            {
                // 플레이어 타입마다 다른 그림 부여 (애니메이션도 다 다르게 하려 햇는데 일단 하나로 함)
                case GameManager.jType.Sheriff:
                    Debug.Log("당신은 보안관입니다.");
                    jobUIAnimator.SetTrigger("Sheriff");
                    temp = "SHERIFF\n보 안 관.";

                    // 직업 이미지 3장 
                    jobImage1.sprite = sheriff1;
                    jobImage2.sprite = sheriff2;
                    jobImage3.sprite = sheriff3;

                    // 능력 이미지랑 텍스트
                    abilImage.sprite = sheriff4;
                    if (gm.playerList.Length == 4)
                        abilText.text = "1. 무법자를 모두 사살하십시오.";
                    else
                        abilText.text = "1. 부관을 찾고 무법자를 모두 사살하십시오.";

                    break;
                case GameManager.jType.Vice:
                    Debug.Log("당신은 부관입니다.");
                    jobUIAnimator.SetTrigger("Deputy");
                    temp = "DEPUTY\n 부  관";

                    jobImage1.sprite = deputy1;
                    jobImage2.sprite = deputy2;
                    jobImage3.sprite = deputy3;

                    abilImage.sprite = deputy4;
                    if (gm.playerList.Length == 3)
                        abilText.text = "1. 배신자를 처단 하십시오.";
                    else
                        abilText.text = "1. 보안관을 도와 무법자를 모두 사살하십시오.";

                    break;
                case GameManager.jType.Outlaw:
                    Debug.Log("당신은 무법자입니다.");
                    jobUIAnimator.SetTrigger("Outlaw");
                    temp = "OUTLAW\n무 법 자";

                    jobImage1.sprite = outlaw1;
                    jobImage2.sprite = outlaw2;
                    jobImage3.sprite = outlaw3;

                    abilImage.sprite = outlaw4;
                    //abilText.text = "1. 무법자들과 함께 보안관을 사살하십시오.";
                    if (gm.playerList.Length == 3)
                        abilText.text = "1. 부관을 사살하십시오.";
                    else
                        abilText.text = "1. 다른 무법자들과 함께 보안관을 사살하십시오.";

                    break;
                case GameManager.jType.Renegade:
                    Debug.Log("당신은 배신자입니다.");
                    jobUIAnimator.SetTrigger("Renegade");
                    temp = "RENEGADE\n배 신 자";

                    jobImage1.sprite = renegade1;
                    jobImage2.sprite = renegade2;
                    jobImage3.sprite = renegade3;

                    abilImage.sprite = renegade4;
                    if (gm.playerList.Length == 3)
                        abilText.text = "1. 무법자를 배신하십시오.";
                    else if (gm.playerList.Length == 4)
                        abilText.text = "1. 보안관을 도와 무법자를 제거하고 마지막에 보안관을 배신하십시오.";
                    else
                        abilText.text = "1. 부관과 무법자를 모두 제거하고 마지막에 보안관을 배신하십시오.";

                    break;
            }
            MineUI.Instance.questTitle.text = abilText.text.Substring(2);

            return temp;
        }

        // 능력 관련 텍스트
        public string AblityText()
        {
            // 능력 UI 애니메이션 
            string temp = "";
            Debug.Log(playerManager.abilityType);
            switch (playerManager.abilityType)
            {
                case GameManager.aType.BangMissed:
                    //temp = "\n2. 뱅과 빗나감이 같은 능력이 됩니다.";
                    break;
                case GameManager.aType.DrinkBottle:
                    //temp = "\n2. 당신옆에 항상 술통이 있습니다.";
                    break;
                case GameManager.aType.HumanVolcanic:
                    //temp = "\n2. 뱅을 마구 쏠 수 있습니다.";
                    break;
                case GameManager.aType.OnehpOnecard:
                    //temp = "\n2. 체력이 달았다면 카드를 얻습니다.";
                    break;
                case GameManager.aType.ThreeCard:
                    //temp = "\n2. 카드를 뽑을 때 3장을 보고 2장을 가져옵니다.";
                    break;
                case GameManager.aType.TwocardOnecard:
                    //temp = "\n2. 카드 펼치기를 할 때 2장을 뽑고 한장을 선택할 수 있습니다.";
                    break;
                case GameManager.aType.TwocardOnehp:
                    //temp = "\n2. 카드 2장을 버리고 체력을 얻습니다.";
                    break;
            }
            return temp;
        }
    }
}
    
