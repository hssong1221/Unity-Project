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

    // 수류탄이 터지는 것 구현
    IEnumerator Explosion()
    { 
        // 굴러가다 아예 멈춤
        yield return new WaitForSeconds(3f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        meshObj.SetActive(false);
        effectObj.SetActive(true);

        // 폭발 범위랑 범위안에 닿는 적 구별
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 12, Vector3.up, 0f, LayerMask.GetMask("Enemy"));

        // 범위안에서 맞으면 사망
        foreach(RaycastHit hitObj in rayHits)
        {
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 5);
    }
}
