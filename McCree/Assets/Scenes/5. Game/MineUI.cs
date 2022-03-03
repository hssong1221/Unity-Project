using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class MineUI : MonoBehaviour
    {
        // 로컬 플레이어의 UI 어디서든 사용가능 (로컬 한정)  
        static public MineUI Instance;

        private PhotonView photonView;
        private PlayerManager playerManager;
        private PlayerInfo playerInfo;

        public Transform pos_CardSpwan;
        public Transform pos_CardParent;
        public Transform pos_CardLeft;
        public Transform pos_CardRight;


        void Awake()
        {
            // 어디서든 쓸 수 있게 인스턴스화
            Instance = this;
        }

        public void FindMinePv(GameObject player)
        {
            photonView = player.GetComponent<PhotonView>();
            playerManager = player.GetComponent<PlayerManager>();
            playerInfo = player.GetComponent<PlayerInfo>();
        }

        public void CardAlignment()
        {

            List<PRS> originMyCards = new List<PRS>();
            originMyCards = RoundAlignment(pos_CardLeft, pos_CardRight, playerInfo.mycards.Count, 0.5f, Vector3.one * 1.0f);

            for (int i = 0; i < playerInfo.mycards.Count; i++)
            {
                var targetCard = playerInfo.mycards[i];

                targetCard.originPRS = originMyCards[i];
                targetCard.MoveTransform(targetCard.originPRS, true, 0.9f);
            }

        }

        List<PRS> RoundAlignment(Transform leftTr, Transform rightTr, int objCount, float height, Vector3 scale)
        {
            float[] objLerps = new float[objCount];
            List<PRS> results = new List<PRS>(objCount);

            switch (objCount)
            {
                case 1: objLerps = new float[] { 0.5f }; break;
                case 2: objLerps = new float[] { 0.27f, 0.73f }; break;
                case 3: objLerps = new float[] { 0.1f, 0.5f, 0.9f }; break;
                default:
                    float interval = 1f / (objCount - 1);
                    for (int i = 0; i < objCount; i++)
                        objLerps[i] = interval * i;
                    break;
            }

            for (int i = 0; i < objCount; i++)
            {
                var targetPos = Vector3.Lerp(leftTr.position, rightTr.position, objLerps[i]);
                var targetRot = Quaternion.identity;
                if (objCount >= 4)
                {
                    float curve = Mathf.Sqrt(Mathf.Pow(height, 2) - Mathf.Pow(objLerps[i] - 0.5f, 2));
                    curve = height >= 0 ? curve : -curve;
                    targetPos.y += curve;
                    targetRot = Quaternion.Slerp(leftTr.rotation, rightTr.rotation, objLerps[i]);
                }
                results.Add(new PRS(targetPos, targetRot, scale));
            }
            return results;
        }


        //public void Show_Start_Cards()
        //{
        //    for (int i = 0; i < cardImgs.Length; i++)
        //    {
        //        if (i < playerInfo.mycards.Count)
        //        {
        //            cardImgs[i].GetComponent<Image>().sprite = playerInfo.mycards[i].cardImg;
        //            cardImgs[i].SetActive(true);
        //        }
        //        else
        //            cardBorders[i].SetActive(false);
        //    }
        //}
    }
}