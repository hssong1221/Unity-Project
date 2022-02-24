using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class PlayerAutoMove : Controller
    {
        public GameObject targetedEnemy;
        //public bool isPlayerAlive;


        private void Awake()
        {
            base.Awake();
        }

        void Start()
        {

        }

        void Update()
        {
            if (targetedEnemy != null) // PlayerManager에서 isAiming상태에서 적을 클릭했을시에만 발생
            {
                // 뱅 범위 사거리안에 적 플레이어가 안들어와있을때
                if (Vector3.Distance(character.transform.position,
                    targetedEnemy.transform.position) > playerManager.maxAttackDistance)
                {
                    // 이동
                    playerManager.agent.SetDestination(targetedEnemy.transform.position);
                    playerManager.agent.stoppingDistance = playerManager.maxAttackDistance;
                    // 회전은 Move가 해줌
     
                }
                // 뱅 범위 사거리안에 적이 들어와 있을때
                else
                {
                    transform.LookAt(targetedEnemy.transform); // 쏠때 적 바라보기
                    // 적 한테 데미지
                    targetedEnemy.GetComponent<PhotonView>().RPC("Damaged", RpcTarget.All);
                    targetedEnemy = null;

                    // Bang 말풍선 띄우기
                    photonView.RPC("TurnOnBangBubble", RpcTarget.All);
                }
            }
        }

        [PunRPC]
        void Damaged()
        {
            Image tempImg = ui.hpImgs[playerInfo.hp - 1].GetComponent<Image>();
            tempImg.sprite = ui.emptyBullet;
            playerInfo.hp -= playerInfo.damage;

            //if (this.hpUI.hp == 0)
            //{
            //    playerManager.isDeath = true;
            //    animator.SetBool("isDeath", playerManager.isDeath);
            //}

        }

        [PunRPC]
        void TurnOnBangBubble()
        {
            ui.bangGifImg.GetComponent<Image>().enabled = true;
            ui.bangGifImg.GetComponent<Animator>().Play("BangAnim", -1, 0f);
            Invoke("TurnOffBangBubble", 2.0f);
        }

        void TurnOffBangBubble()
        {
            ui.bangGifImg.GetComponent<Image>().enabled = false;
        }
    }
}
