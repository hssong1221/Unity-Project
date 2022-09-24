using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace com.ThreeCS.McCree 
{
    public class UseCardPanelUI : MonoBehaviour
    {
        public Transform target;

        [Header("중앙")]
        public GameObject center;

        void Start()
        {

            EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry entry_PointerEnter = new EventTrigger.Entry();
            entry_PointerEnter.eventID = EventTriggerType.PointerEnter;
            entry_PointerEnter.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data); });
            eventTrigger.triggers.Add(entry_PointerEnter);

            EventTrigger.Entry entry_PointerExit = new EventTrigger.Entry();
            entry_PointerExit.eventID = EventTriggerType.PointerExit;
            entry_PointerExit.callback.AddListener((data) => { OnPointerExit((PointerEventData)data); });
            eventTrigger.triggers.Add(entry_PointerExit);
        }


        void OnPointerEnter(PointerEventData data)
        {
            try
            {
                //Debug.Log("Pointer Enter");
                target.GetComponent<Card>().useCard = true;
            }
            catch(Exception e)
            {
                Debug.Log(e);
            }
        }

        void OnPointerExit(PointerEventData data)
        {
            try
            {
                //Debug.Log("Pointer Exit");
                target.GetComponent<Card>().useCard = false;
            }
            catch(Exception e)
            {
                Debug.Log(e);
            }
        }

        // 마우스로 선택한 카드를 의미
        public void TargetMatch(Transform receive)
        {
            target = receive;
        }

        public void Des()
        {
            Destroy(target.gameObject);
        }
    }

}

