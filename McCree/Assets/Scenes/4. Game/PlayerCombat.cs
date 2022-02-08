using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ThreeCS.McCree
{
    public class PlayerCombat : Controller
    {
        public GameObject targetedEnemy;
        public float attackRange;

        private PlayerManager playerManager;

        public bool isPlayerAlive;


        private void Awake()
        {
            base.Awake();
        }

        void Start()
        {
            playerManager = GetComponent<PlayerManager>();
        }

        void Update()
        {
            if (targetedEnemy != null)
            {
                //if (Vector3.Distance(character.transform.position,
                //    targetedEnemy.transform.position) > sd )
                //{

                //}
            }
        }
    }
}
