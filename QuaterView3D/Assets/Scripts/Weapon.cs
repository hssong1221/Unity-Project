using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public int damage;
    public float rate;
    public int maxAmmo;
    public int curAmmo;

    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;
    
    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;

    // ���η�ƾ Use -> �����ƾ Swing -> ���η�ƾ
    // ���η�ƾ Use + �ڷ�ƾ Swing
    public void Use()
    {
        // ���� ����, ���Ÿ� ����
        if(type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if(type == Type.Range && curAmmo > 0)
        {
            curAmmo--;
            StartCoroutine("Shot");
        }
    }

    //invoke ���� ���� �ɸ��� �ڷ�ƾ���� �ѹ��� ����
    IEnumerator Swing()
    {
        // 1 ���⿡ Ʈ���ϰ� �ݶ��̴� Ȱ��ȭ
        yield return new WaitForSeconds(0.1f); // 0.1�� ���
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        // 2 ���� �ݶ��̴� ��Ȱ��
        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        // 3 ���� Ʈ���� ��Ȱ��
        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }

    IEnumerator Shot()
    {
        // 1 �Ѿ� �߻�
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;
        yield return null; // 1������ ���

        // 2 ź�� ����
        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * Random.Range(8,12) , ForceMode.Impulse);
    }
}
