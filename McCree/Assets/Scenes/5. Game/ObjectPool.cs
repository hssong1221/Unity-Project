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
        private GameObject playerList; // 플레이어 리스트
        Queue<GameObject> playerListQueue = new Queue<GameObject>();

        [SerializeField]
        private GameObject roomList; // 플레이어 리스트
        Queue<GameObject> roomListQueue = new Queue<GameObject>();

        private void Awake()
        {
            pInstance = this;
            Initialize();
        }

        private void Initialize()
        {
            for (int i = 0; i < 7; i++) // 플레이어 리스트 7
            {
                playerListQueue.Enqueue(CreateNewObject(1));
            }
        }

        private GameObject CreateNewObject(int num)
        {
            if (num == 1)
                poolingObject = playerList;
            if (num == 2)
                poolingObject = roomList;

            GameObject newObj = Instantiate(poolingObject);
            newObj.SetActive(false);
            newObj.transform.SetParent(transform);
            return newObj;
        }

        public GameObject GetObject(int num)
        {
            if (num == 1)
                poolingObjectQueue = playerListQueue; // 아마 얕은복사될듯 (주소만)
            else if (num == 2)
                poolingObjectQueue = roomListQueue;


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
                poolingObjectQueue = playerListQueue;
            else if (num == 2)
                poolingObjectQueue = roomListQueue;


            obj.SetActive(false);
            obj.transform.SetParent(Instance.transform);
            poolingObjectQueue.Enqueue(obj);
            // 사용한 오브젝트는 다시 오브젝트 풀에 돌려준다
        }
    }
}