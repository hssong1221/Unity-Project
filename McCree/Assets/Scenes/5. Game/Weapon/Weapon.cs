using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.ThreeCS.McCree
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Weapon")]
    public class Weapon : ScriptableObject
    {
        public Sprite weaponImg;
        public string wepaonName;
        public string weaponExplain;
        [Header("무기 사정 거리")]
        public int Range; 

        public enum iType
        {
            Pistol,
            Rifle,
            ShotGun
        }

        public iType kind;
    }





}
