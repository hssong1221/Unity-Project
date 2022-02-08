using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace com.ThreeCS.McCree
{
    public class InputTargeting : Controller
    {

        protected GameObject selectedPlayer;
        protected bool isCharacterPlayer;
        RaycastHit hit;

        // Start is called before the first frame update
        void Start()
        {
            selectedPlayer = GameObject.FindWithTag("Player");
        }

        // Update is called once per frame
        void Update()
        {
            if (isAiming && Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
                {
                    if (hit.collider.gameObject != character && hit.collider.gameObject.tag == "Player")
                    { // 클릭한 오브젝트가 자기 자신이 아닌 다른 플레이어 일때
                        Debug.Log(hit.collider.gameObject + "  "+hit.collider.gameObject.name +"  "+ hit.collider.gameObject.tag);
                        //selectedPlayer.GetComponent<PlayerCombat>().targetedEnemy = hit.collider.gameObject;
                    }
                    else
                    {
                        //selectedPlayer.GetComponent<PlayerCombat>().targetedEnemy = null;
                    }
                }
            }
        }
    }
}
