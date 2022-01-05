using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public bool isChase;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;
    NavMeshAgent nav;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        Invoke("ChaseStart", 2);
    }

    // ����ٴϱ� ����
    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    void Update()
    {
        // ������ �ذ��� ����ٴ�
        if(isChase)
            nav.SetDestination(target.position);
    }
    void FreezeVelocity()
    {
        // ��ü���� ����� �� ���� ������ �ִ� ���� ����
        if (isChase)
        {
            rigid.angularVelocity = Vector3.zero;
            rigid.velocity = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        FreezeVelocity();
    }

    void OnTriggerEnter(Collider other)
    {
        // ���� ���ݿ� �¾��� ��
        if(other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec, false));
        }
        // ���Ÿ� ���ݿ� �¾��� ��
        else if(other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            // �Ѿ��� ���� ������ �����
            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVec, false)) ;
        }
    }
    // ����ź�� �¾��� ��
    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec, true));
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        // ������ ������ �� ���̴� ����
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0)
        {
            mat.color = Color.white;
        }
        else
        {
            mat.color = Color.gray;
            gameObject.layer = 14;
            
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie");

            // ������ �˹��ϴ� ���
            if (isGrenade) // ����ź�� ������ �ƿ� ���ư����� ����
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;

                rigid.freezeRotation = false;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 10, ForceMode.Impulse);
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }
            
            Destroy(gameObject, 3);
        }
    }
}
