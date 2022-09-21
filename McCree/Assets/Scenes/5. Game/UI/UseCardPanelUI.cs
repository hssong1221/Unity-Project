using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace com.ThreeCS.McCree 
{
    public class UseCardPanelUI : MonoBehaviour
    {
        public Transform target;

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
                Debug.Log("Pointer Enter");
                target.GetComponent<Card>().useCard = true;
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }
        }

        void OnPointerExit(PointerEventData data)
        {
            try
            {
                Debug.Log("Pointer Exit");
                target.GetComponent<Card>().useCard = false;
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }
        }

        public void TargetMatch(Transform receive)
        {
            target = receive;
        }
    }

}

