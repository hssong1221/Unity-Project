using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ThreeCS.McCree
{
    public class ObjectPool : MonoBehaviour
    {
        private static ObjectPool pInstance;

        public static ObjectPool Instance
        {
            get { return pInstance; }
        }

        // 얕은복사를 위한 변수
        private GameObject poolingObject;
        private Queue<GameObject> poolingObjectQueue;


        [SerializeField]
        private GameObject bangLogObject; // 뱅 로그
        Queue<GameObject> bangLogListQueue = new Queue<GameObject>();

        [SerializeField]
        private GameObject questLogObject; // 월드퀘스트 로그
        Queue<GameObject> questLogListQueue = new Queue<GameObject>();

        [SerializeField]
        private GameObject avoidLogObject; // 회피 로그
        Queue<GameObject> avoidLogListQueue = new Queue<GameObject>();

        [SerializeField]
        private GameObject bulletObject; // 총알 프리팹
        Queue<GameObject> bulletListQueue = new Queue<GameObject>();

        private void Awake()
        {
            pInstance = this;
            Initialize();
        }

        private void Initialize()
        {
            for (int i = 0; i < 7; i++)
            {
                bangLogListQueue.Enqueue(CreateNewObject(1));
            }
            for (int i= 0; i < 3; i++)
            {
                questLogListQueue.Enqueue(CreateNewObject(2));
            }
            for (int i = 0; i < 7; i++)
            {
                avoidLogListQueue.Enqueue(CreateNewObject(3));
            }
            for (int i = 0; i < 7; i++)
            {
                bulletListQueue.Enqueue(CreateNewObject(4));
            }
        }

        private GameObject CreateNewObject(int num)
        {
            if (num == 1)
                poolingObject = bangLogObject;
            else if (num == 2)
                poolingObject = questLogObject;
            else if (num == 3)
                poolingObject = avoidLogObject;
            else if (num == 4)
                poolingObject = bulletObject;

            GameObject newObj = Instantiate(poolingObject);
            newObj.SetActive(false);
            newObj.transform.SetParent(transform);


            if (poolingObject == bulletObject)
            {
                newObj.GetComponent<Bullet>().enabled = false;
            }


            return newObj;
        }

        public GameObject GetObject(int num)
        {
            if (num == 1)
                poolingObjectQueue = bangLogListQueue; // 아마 얕은복사될듯 (주소만)
            else if (num == 2)
                poolingObjectQueue = questLogListQueue;
            else if (num == 3)
                poolingObjectQueue = avoidLogListQueue;
            else if (num == 4)
                poolingObjectQueue = bulletListQueue;



            if (Instance.poolingObjectQueue.Count > 0)
            {
                // 오브젝트 풀에 담겨있는 오브젝트를 가져온다. 
                var obj = poolingObjectQueue.Dequeue();
                obj.transform.SetParent(null);
                obj.gameObject.SetActive(true);
                return obj;
            }

            else
            {
                var newObj = CreateNewObject(num);
                newObj.SetActive(true);
                newObj.transform.SetParent(null);
                return newObj;
            }
        }


        public void ReturnObject(GameObject obj, int num)
        {
            if (num == 1)
                poolingObjectQueue = bangLogListQueue;
            else if (num == 2)
                poolingObjectQueue = questLogListQueue;
            else if (num == 3)
                poolingObjectQueue = avoidLogListQueue;
            else if (num == 4)
                poolingObjectQueue = bulletListQueue;


            obj.SetActive(false);
            obj.transform.SetParent(Instance.transform);
            poolingObjectQueue.Enqueue(obj);
            // 사용한 오브젝트는 다시 오브젝트 풀에 돌려준다
        }
    }
}