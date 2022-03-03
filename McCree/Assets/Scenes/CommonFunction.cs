using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



namespace com.ThreeCS.McCree
{
    public class PRS
    {
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;

        public PRS(Vector3 pos, Quaternion rot, Vector3 scale)
        {
            this.pos = pos;
            this.rot = rot;
            this.scale = scale;
        }
    }



    public static class CommonFunction
    {
        // InputField 초기화
        public static void clear(this InputField inputfield)
        {
            inputfield.Select();
            inputfield.text = "";
        }

        public static void clear(this TMP_InputField inputfield)
        {
            inputfield.Select();
            inputfield.text = "";
        }

        // 리스트 셔플, 자바에는 그냥 있는데 씨썁에는 없다ㅋㅋ
        public static List<T> ShuffleList<T>(List<T> list)
        {
            int random1, random2;
            T temp;

            for (int i = 0; i < list.Count; ++i)
            {
                random1 = Random.Range(0, list.Count);
                random2 = Random.Range(0, list.Count);

                temp = list[random1];
                list[random1] = list[random2];
                list[random2] = temp;
            }
            return list;
        }
    }
}