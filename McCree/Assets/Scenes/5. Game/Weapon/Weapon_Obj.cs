using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ThreeCS.McCree
{
    public class Weapon_Obj : MonoBehaviour
    {
        [SerializeField]
        protected Weapon _weapon;
        public Weapon wepaon
        {
            get { return _weapon; }
        }

        [SerializeField]
        protected Transform _bulletPos;
        public Transform bulletPos
        {
            get { return _bulletPos; }
        }
    }
}