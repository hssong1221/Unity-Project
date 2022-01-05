using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject meshObj;
    public GameObject effectObj;
    public Rigidbody rigid;

    void Start()
    {
        StartCoroutine(Explosion());
    }

    // ����ź�� ������ �� ����
    IEnumerator Explosion()
    { 
        // �������� �ƿ� ����
        yield return new WaitForSeconds(3f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        meshObj.SetActive(false);
        effectObj.SetActive(true);

        // ���� ������ �����ȿ� ��� �� ����
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 12, Vector3.up, 0f, LayerMask.GetMask("Enemy"));

        // �����ȿ��� ������ ���
        foreach(RaycastHit hitObj in rayHits)
        {
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 5);
    }
}
