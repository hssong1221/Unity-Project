using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class PlayerInfo : Controller
    {
        private static PlayerInfo pInstance;

        public static PlayerInfo Instance
        {
            get { return pInstance; }
        }

        private int _hp;

        public int hp
        {
            get { return _hp;  }
            set
            {
                int prehp = _hp;
                
                _hp = value;

                if (prehp - 1 == _hp) // hp가 1깎였을때 캐릭터 UI 총알 이미지 바꿈
                {
                    Image tempImg = ui.hpImgs[playerInfo.hp].GetComponent<Image>();
                    tempImg.sprite = ui.emptyBullet;

                    if (photonView.IsMine) // MineUI 체력 이미지 바꿈
                    {
                        Image tempImg2 = MineUI.Instance.mineUIhpImgs[playerInfo.hp].GetComponent<Image>();
                        tempImg2.sprite = MineUI.Instance.emptyHealth;
                    }
                }
                else if (prehp + 1 == _hp) // hp가 1업했을때 
                {
                    Image tempImg = ui.hpImgs[playerInfo.hp - 1].GetComponent<Image>();
                    tempImg.sprite = ui.fullBullet;

                    if (photonView.IsMine) // MineUI 체력 이미지 바꿈
                    {
                        Image tempImg2 = MineUI.Instance.mineUIhpImgs[playerInfo.hp - 1].GetComponent<Image>();
                        tempImg2.sprite = MineUI.Instance.fullHealth;
                    }
                }
                
                Debug.Log(hp + "  " + _hp);
            }
        }
        public int maxHp;
        //public int curhp; // 현재 체력 알고 싶으면 update랑 같이 켜

        [Header("내가 지금 가지고 있는 카드리스트")]
        public List<Card> mycards = new List<Card>();

        public int mycardNum;
        public int mycardNumView;

        // 공격자 상태
        public bool waitAvoid = false;  // 본인이 지금 상대방의 avoid를 기다리고 있는 중인지
        public int waitAvoids = -1;      // 본인이 전체avoid를 기다리고 있음(-1이 기본 MG사용자만 0으로 바뀜)
        public int waitBangs = -1;      // 본인이 전체 bang을 기다리고 잇음(인디언 사용 시)

        public bool usedBang = false; // 뱅은 한턴에 1번만 써야하니까

        // 방어자 상태
        public int isTarget = 0; // 본인이 상대편 카드에 의해 타겟팅이 되었는지
                                 // 0 : 타겟 아님
                                 // 1 : 타겟 상태 들어감(enter) - bang MG
                                 // 2 : 타겟 상태 들어감(enter) - indian
                                 // 3 : 타겟 상태 지속 중(stay)

        public bool targetedBang = false; // bang MG에의해 타겟팅
        public bool targetedIndian = false; // indian에 의해 타겟팅

        public int attackerIdx; // 나를 공격한 사람의 idx

        [HideInInspector]
        public bool sendAvoid = false;  // 본인이 avoid카드를 냈는지 안냈는지

        public bool sendBang = false; // 본인이 bang카드를 냈는지 안냈는지(인디언 회피용)

        // 기관총 상태인지 아닌지
        public bool isMG = false;

        // 인디언 상태인지 아닌지
        public bool isIndian = false;

        // 잡화점 상태인지 아닌지
        public bool isStore = false;

        // 본인의 기본 사거리
        public int atkRange;
        // 본인 무기 사거리
        public int weaponRange;


        // 스코프 장착 상태
        protected bool scope;
        public bool isScope
        {
            get { return scope; } 
            set {
                scope = value;
    
                if (isScope)
                    atkRange += 1;
                else
                {
                    if (atkRange > 1)
                        atkRange -= 1;
                    else if (atkRange == 1)
                        atkRange = 1;
                }
            }
        }

        // 야생마 장착 상태
        public bool isMustang = false;

        // 술통 장착 상태
        public bool isBarrel = false;

        // 다이너마이트 장착
        public bool isDynamite = false;

        // 감옥 장착
        public bool isJail = false;

        // 캣벌로우의 타겟
        public bool isCat = false;
        // 캣벌로우 사용한 사람
        public bool useCat = false;

        // 강탈의 타겟
        public bool isPanic = false;
        // 강탈의 사용한 사람
        public bool usePanic = false;

        // 결투중
        public bool isDuel = false;

        // 무기 장착 상태 (기본 무기가 아닌 무기들)
        public bool isWeapon = false;

        //무기 이름
        public string wName;

        // 죽음 상태
        public bool isDeath;


        // ------------------ 삭제 예정
        public List<ItemList> myItemList;
        // 0번째 Bang
        // 1번째 Avoid
        // 2번째 Beer
        public List<SubQuestList> myQuestList;

        protected Transform content;

        public Transform pistolPos;
        //public Transform shotgunPos;
        public Transform riflePos;

        [SerializeField]
        protected Weapon_Obj _equipedWeapon;
        public Weapon_Obj equipedWeapon
        { 
            get { return _equipedWeapon; }
            set
            {

                _equipedWeapon = value;

                if(_equipedWeapon.wepaon.kind == Weapon.iType.Pistol)
                {
                    playerManager.EquipedNone = false;
                    playerManager.EquipedPistol = true;
                    playerManager.EquipedRifle = false;
                    //animSync.SendPlayAnimationEvent(photonView.ViewID, "Pistol", "Trigger");

                    _equipedWeapon.gameObject.transform.SetParent(pistolPos);
                    _equipedWeapon.transform.localPosition = new Vector3(0f, 0f, 0f);
                    _equipedWeapon.transform.localRotation = Quaternion.identity;

                    riflePos.gameObject.SetActive(false);
                    pistolPos.gameObject.SetActive(true);
                }
                else if (_equipedWeapon.wepaon.kind == Weapon.iType.Rifle)
                {
                    playerManager.EquipedNone = false;
                    playerManager.EquipedPistol = false;
                    playerManager.EquipedRifle = true;
                    //animSync.SendPlayAnimationEvent(photonView.ViewID, "Rifle", "Trigger");

                    _equipedWeapon.gameObject.transform.SetParent(riflePos);
                    _equipedWeapon.transform.localPosition = new Vector3(0f, 0f, 0f);
                    _equipedWeapon.transform.localRotation = Quaternion.identity;

                    pistolPos.gameObject.SetActive(false);
                    riflePos.gameObject.SetActive(true);
                }
            }
        }


        void Awake()
        {
            base.Awake();  // Controller Awake 함수 그대로 사용
            pInstance = this;

            isDeath = false;

            attackerIdx = -1;

            atkRange = 1;
            weaponRange = 0;

            wName = "Colt";

            mycardNumView = 5;

            MineUI.Instance.statusPanel.gameObject.SetActive(true);

            content = GameObject.FindGameObjectWithTag("Content").transform;

            myItemList = new List<ItemList>();

            myQuestList = new List<SubQuestList>();

            for (int i=0; i<content.childCount; i++)
            {
                myItemList.Add(content.GetChild(i).GetComponent<ItemList>());
            }
            MineUI.Instance.statusPanel.gameObject.SetActive(false);

            StartCoroutine(CurrentHP());
        }
        
        void Update()
        {
            if (photonView.IsMine)
            {
                mycardNumView = mycards.Count;
                //Debug.Log("c n : " + mycardNum);
            }
        }

        public void Show_Hp()
        {
            for (int i = 0; i < ui.hpImgs.Length; i++)
            {
                if (i < maxHp)
                {
                    ui.hpImgs[i].SetActive(true);

                    if (photonView.IsMine)
                        MineUI.Instance.mineUIhpImgs[i].SetActive(true);
                }
                else
                {
                    ui.hpImgs[i].SetActive(false);

                    if (photonView.IsMine)
                        MineUI.Instance.mineUIhpImgs[i].SetActive(false);
                }
            }
        }

        IEnumerator CurrentHP()
        {
            var wait1f = new WaitForSeconds(1f);
            // 너무 빨리 측정하면 hp 동기화 하기 전이라 사망처리됨 
            yield return new WaitForSeconds(7f);
            Debug.Log("hp 측정 시작");

            while (true)
            {
                yield return null;
                //Debug.Log("현재 체력 : " + hp);
                if (hp <= 0)
                {
                    yield return null;
                    photonView.RPC("Death", RpcTarget.All);
                    break;
                }

                yield return wait1f;
            }
            yield return null;
        }

        [PunRPC]
        public void CardNumView(int num)
        {

            ui.cardNumText.enabled = true;
            ui.cardNumText.gameObject.SetActive(true);
            ui.cardNumText.text = num.ToString();
        }

        [PunRPC]
        public void Death()
        {
            // 죽음 상태 켜주고 비석도 켜줌
            isDeath = true;
            GameObject grave = playerManager.mygamePlate.transform.Find("Grave").gameObject;
            grave.SetActive(true);

            // 장착 아이템 확인 후 덱으로 돌려보냄
            if (photonView.IsMine)
            {
                // 죽으면 직업 알림 - 잘 적용 되지 않고 있따.
                if(playerManager.playerType == GameManager.jType.Sheriff)
                    photonView.RPC("AlertInfo", RpcTarget.All, "SDie", ui.nickName.text, "");
                else if (playerManager.playerType == GameManager.jType.Sheriff)
                    photonView.RPC("AlertInfo", RpcTarget.All, "VDie", ui.nickName.text, "");
                else if (playerManager.playerType == GameManager.jType.Sheriff)
                    photonView.RPC("AlertInfo", RpcTarget.All, "ODie", ui.nickName.text, "");
                else if (playerManager.playerType == GameManager.jType.Sheriff)
                    photonView.RPC("AlertInfo", RpcTarget.All, "RDie", ui.nickName.text, "");

                if (isScope)
                {
                    photonView.RPC("ScopeSync", RpcTarget.All, 1);
                    photonView.RPC("CardDeckSync", RpcTarget.All, Card.cType.Scope);
                }
                if (isMustang)
                {
                    photonView.RPC("MustangSync", RpcTarget.All, 1);
                    photonView.RPC("CardDeckSync", RpcTarget.All, Card.cType.Mustang);
                }
                if (isBarrel)
                {
                    photonView.RPC("BarrelSync", RpcTarget.All, 1);
                    photonView.RPC("CardDeckSync", RpcTarget.All, Card.cType.Barrel);
                }
                if (isJail)
                {
                    photonView.RPC("JailSync", RpcTarget.All, 1);
                    photonView.RPC("CardDeckSync", RpcTarget.All, Card.cType.Jail);
                }
                if (isDynamite)
                {
                    photonView.RPC("DynamiteSync", RpcTarget.All, 1);
                    photonView.RPC("CardDeckSync", RpcTarget.All, Card.cType.Dynamite);
                }

                // 쓰던 총 임시저장
                Card.cType temp = GameManager.Instance.StringtoEnum(wName);

                // 일단 colt로 만들고 setactive false
                photonView.RPC("WeaponSync", RpcTarget.All, 1);
                
                // ------------- 본인 눈에만 없이지는 중임
                GameObject gun = playerManager.mygamePlate.transform.Find("Colt").gameObject;
                gun.SetActive(false);

                // 쓰던 총 반납
                photonView.RPC("CardDeckSync", RpcTarget.All, temp);


                // 손 덱 확인후 전체 덱으로 돌려보냄
                foreach (Card card in mycards)
                    photonView.RPC("CardDeckSync", RpcTarget.All, card.cardContent);

                // 손덱 정보 삭제
                mycards.Clear();

                // 손덱 오브젝트 삭제
                GameObject[] cards = GameObject.FindGameObjectsWithTag("Card");
                foreach (GameObject card in cards)
                    Destroy(card);

                // 혹시나 end turn 버튼 보이면 없애기 그리고 카드 블록 패널도 켜기
                MineUI.Instance.cardblockingPanel.SetActive(true);
                MineUI.Instance.NextButton.gameObject.SetActive(false);

                // 3인 룰에선 죽이면 3장임
                if (GameManager.Instance.playerList.Length == 3)
                {
                    // 유효하게 죽었다면
                    if (attackerIdx >= 0 && attackerIdx != GameManager.Instance.turnList[attackerIdx].GetComponent<PlayerInfo>().attackerIdx)
                        GameManager.Instance.turnList[attackerIdx].GetPhotonView().RPC("GiveCards", RpcTarget.AllViaServer, 3);
                }
                else // 4 - 7인 룰
                {
                    // 내가 무법자인데 죽으면 죽인사람한테 카드 3장
                    if (playerManager.playerType == GameManager.jType.Outlaw)
                    {
                        // 유효하게 죽었다면 그리고 자해데미지가 아니라면
                        if (attackerIdx >= 0 && attackerIdx != GameManager.Instance.turnList[attackerIdx].GetComponent<PlayerInfo>().attackerIdx)
                        {
                            GameManager.Instance.turnList[attackerIdx].GetPhotonView().RPC("GiveCards", RpcTarget.AllViaServer, 3);
                            // alert
                            //GameManager.Instance.turnList[playerInfo.attackerIdx].GetPhotonView().RPC("AlertInfo", RpcTarget.All, "6", "", "");
                        }
                    }
                    // 구현 예정
                    // 내가 부관이고 죽었는데
                    if (playerManager.playerType == GameManager.jType.Vice)
                    {
                        // 보안관이 죽인거면 손 카드 압수
                        if (GameManager.Instance.turnList[attackerIdx].GetComponent<PlayerManager>().playerType == GameManager.jType.Sheriff)
                        {
                            GameManager.Instance.turnList[attackerIdx].GetPhotonView().RPC("KillVice", RpcTarget.All);
                        }
                    }
                }
            }

            // 다른 사람들이 선택 못하게 투명하게 만듬
            character.SetActive(false);
            // 공격자 초기화
            //attackerIdx = -1;
        }

        [PunRPC]
        public void KillVice()
        {
            if (photonView.IsMine)
            {
                // 손 덱 확인후 전체 덱으로 돌려보냄
                foreach (Card card in mycards)
                    photonView.RPC("CardDeckSync", RpcTarget.All, card.cardContent);
                // 손덱 정보 삭제
                mycards.Clear();

                // 손덱 오브젝트 삭제
                GameObject[] cards = GameObject.FindGameObjectsWithTag("Card");
                foreach (GameObject card in cards)
                    Destroy(card);

                // alert
                GameManager.Instance.alertOrder(7);
            }
        }
    }
}