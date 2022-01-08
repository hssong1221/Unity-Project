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
        // ź�ǰ� �ٴڿ� �������� �����
        if (!isRock && collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        // �Ѿ��� ���̳� �ٴڿ� ������ �����
        if (!isMelee && other.gameObject.tag == "Wall" || other.gameObject.tag == "Floor" && !isRock)
        {
            Destroy(gameObject);
        }
    }
}
