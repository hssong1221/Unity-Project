using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace com.ThreeCS.McCree
{
    public class DelCardPanelUI : MonoBehaviour
    {

        [HideInInspector]
        public Transform target; // 판정 패널위로 올라온 카드

        void Start()
        {
            // 이벤트 트리거 수동 구현
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
                target.GetComponent<Card>().delCard = true;
            }
            catch (Exception e)
            {
                e.ToString();
                //Debug.Log(e);
            }
        }

        void OnPointerExit(PointerEventData data)
        {
            try
            {
                //Debug.Log("Pointer Exit");
                target.GetComponent<Card>().delCard = false;
            }
            catch (Exception e)
            {
                e.ToString();
                //Debug.Log(e);
            }
        }

        // 마우스로 선택한 카드를 의미
        public void TargetMatch(Transform receive)
        {
            target = receive;
        }

        // 사용 후 파괴
        public void Des()
        {
            Destroy(target.gameObject);
        }
    }
}

