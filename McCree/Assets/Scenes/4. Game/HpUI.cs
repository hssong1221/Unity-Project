using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.ThreeCS.McCree
{
    public class HpUI : Controller
    {
        Slider hpBar;

        Vector3 offset;


        void Awake()
        {
            base.Awake();  // PlayerController Awake 함수 그대로 사용
            hpBar = transform.Find("HpSlider").GetComponent<Slider>();
            offset = new Vector3(0, 150.0f, 0);
        }


        private void Start()
        {

        }

        void Update()
        {
            hpBar.transform.position = Camera.main.WorldToScreenPoint(character.transform.position + new Vector3(0f, 2.0f, 0f));
        }
    }
}