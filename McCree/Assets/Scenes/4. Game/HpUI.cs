using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class HpUI : Controller, IPunObservable
    {
        #region Variable Field

        public Canvas canvas;
        public int hp;
        public int maxHp;
        public int damage = 1;

        public GameObject[] hpImgs;

        public Sprite fullBullet;
        public Sprite emptyBullet;


        Vector3 offset;

        Image healthImg;

        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
            base.Awake();  // Controller Awake 함수 그대로 사용

            for (int i = 0; i < hpImgs.Length; i++)
            {

                if (i < maxHp)
                    hpImgs[i].SetActive(true);
                else
                    hpImgs[i].SetActive(false);

            }
            offset = new Vector3(0, 2.0f, 0);
        }


        private void Start()
        {

        }

        void Update()
        {
            if (photonView.IsMine)
            {
                if (this.hp <= 0)
                    GameManager.Instance.LeaveRoom();

                this.Fire();
                
            }
            else
                return;

            // 플레이어 머리 위 UI 각도 고정
            canvas.transform.LookAt(canvas.transform.position + Camera.main.transform.forward);

            //if (photonView.IsMine)
            canvas.transform.position = character.transform.position + offset;
        }

        #endregion

        #region Methods
        void Fire()
        {
            if (Input.GetButtonDown("Fire2"))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Debug.DrawRay(ray.origin, ray.direction * 50f, Color.red);
                Debug.Log("우클 눌렀다");

                //ray가 플레이어레이어에 닿았을 때
                if (Physics.Raycast(ray, out RaycastHit raycastHit, 20f, 1 << LayerMask.NameToLayer("Player")))
                {
                    // ray가 내 플레이어 맞는거 제외
                    if (raycastHit.collider.GetComponentInParent<PhotonView>().ViewID != photonView.ViewID)
                    {
                        Debug.Log(raycastHit.collider.GetComponentInParent<PhotonView>().ViewID);

                        raycastHit.collider.GetComponentInParent<PhotonView>().RPC("Damaged", RpcTarget.All);
                    }
                    else
                    {
                        Debug.Log("나임");
                    }
                }
                else
                {
                    Debug.Log("안 닿음");
                }
            }
            if (Input.GetButtonUp("Fire2"))
            {
                Debug.Log("떼기");
            }
            
        }

        [PunRPC]
        void Damaged()
        {
            Image tempImg = hpImgs[hp - 1].GetComponent<Image>();
            tempImg.sprite = emptyBullet;
            this.hp -= damage;

            if (this.hp == 0)
            {
                isDeath = true;
                animator.SetBool("isDeath", isDeath);
            }

            Debug.Log("hp: " + hp);
        }


        #endregion

        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(this.hp);

            }
            else
            {
                // Network player, receive data
                this.hp = (int)stream.ReceiveNext();
            }
        }

        #endregion
    }


}