using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ThreeCS.McCree
{
    public class LogAnim : MonoBehaviour
    {
        public RectTransform wrapTransform;


        IEnumerator EndAnim_Start()
        {
            wrapTransform.anchoredPosition = new Vector2(0f, 0f);

            yield return new WaitForSeconds(5.0f);

            gameObject.GetComponent<Animator>().Play("LogEnd");
        }

        void ReturnLogObject()
        {
            ObjectPool.Instance.ReturnObject(gameObject, 1); // 오브젝트풀에 반환
        }
    }
}