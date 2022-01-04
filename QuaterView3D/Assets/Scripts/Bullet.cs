using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    void OnCollisionEnter(Collision collision)
    {
        // ź�ǰ� �ٴڿ� �������� �����
        if (collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        // �Ѿ��� ���̳� �ٴڿ� ������ �����
        if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Floor")
        {
            Destroy(gameObject);
        }
    }
}
