using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ThreeCS.McCree
{
    public class WeaponRandomBox : MonoBehaviour
    {
        public Weapon weapon;
        private GameObject obj;

        private void Update()
        {
            transform.Rotate(Vector3.up * 40.0f * Time.deltaTime);
        }

        private void Awake()
        {
            obj = Instantiate(weapon.obj, transform);
            obj.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
        }

    }
}
