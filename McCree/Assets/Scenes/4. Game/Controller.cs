using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

namespace com.ThreeCS.McCree
{
    public class Controller : MonoBehaviourPunCallbacks
    {
        protected GameObject character; // Character객체 (상속가능) 
        protected Animator animator;

        protected void Awake()
        {
            character = GameObject.FindWithTag("Player");

            animator = character.GetComponent<Animator>();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}