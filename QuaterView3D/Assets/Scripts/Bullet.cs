using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;
    public bool isRock;

    void OnCollisionEnter(Collision collision)
    {
        // ÅºÇÇ°¡ ¹Ù´Ú¿¡ ¶³¾îÁö¸é »ç¶óÁü
        if (!isRock && collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        // ÃÑ¾ËÀÌ º®ÀÌ³ª ¹Ù´Ú¿¡ ´êÀ¸¸é »ç¶óÁü
        if (!isMelee && other.gameObject.tag == "Wall" || other.gameObject.tag == "Floor" && !isRock)
        {
            Destroy(gameObject);
        }
    }
}
