using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ThreeCS.McCree
{
    public class PlayerAutoMove : Controller
    {
        public GameObject targetedEnemy;

        public bool isPlayerAlive;


        private void Awake()
        {
            base.Awake();
        }

        void Start()
        {
            
        }

        void Update()
        {
            if (targetedEnemy != null)
            {
                if (Vector3.Distance(character.transform.position,
                    targetedEnemy.transform.position) > playerManager.maxAttackDistance)
                {
                    // 이동
                    playerManager.agent.SetDestination(targetedEnemy.transform.position);
                    playerManager.agent.stoppingDistance = playerManager.maxAttackDistance;

                    // 회전
                    Quaternion rotationToLookAt = Quaternion.LookRotation(targetedEnemy.transform.position - transform.position);
                    float rotationY = Mathf.SmoothDampAngle(transform.eulerAngles.y,
                        rotationToLookAt.eulerAngles.y,
                        ref playerManager.rotateVelocity,
                        playerManager.rotateSpeedMovement * (Time.deltaTime * 5));

                    transform.eulerAngles = new Vector3(0, rotationY, 0);
                }
            }
        }
    }
}
