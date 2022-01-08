using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public int hasGrenades;
    public GameObject grenadeObj;

    public Camera followCamera;

    public int ammo;
    public int coin;
    public int health;

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;


    float hAxis;
    float vAxis;
    bool wDown;
    bool jDown;
    bool iDown;
    bool fDown;
    bool gDown;
    bool rDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;

    bool isJump;
    bool isDodge;
    bool isFireReady;
    bool isReload;
    bool isBorder;
    bool isDamage;

    Vector3 moveVec;

    Rigidbody rigid;
    Animator anim;
    MeshRenderer[] mesh;

    GameObject nearObject;
    Weapon equipWeapon;
    float fireDelay;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        // Player �ȿ� Mesh Object�� animator�־ Inchildren 
        anim = GetComponentInChildren<Animator>();
        mesh = GetComponentsInChildren<MeshRenderer>();
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Attack();
        Grenade();
        Reload();
        Dodge();
        Interaction();
        Swap();
    }

    void GetInput()
    {
        // �÷��̾� �̵� (x�� z��)
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");
        // Ư��Ű �Է�
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        iDown = Input.GetButtonDown("Interaction");
        fDown = Input.GetButton("Fire1");
        gDown = Input.GetButtonDown("Fire2");
        rDown = Input.GetButtonDown("Reload");
        //���ⱳü
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
        if(!isDodge)
            moveVec = new Vector3(hAxis, 0, vAxis).normalized; // ���Ͱ��� 1�� ����
        if(!isBorder)
           

        // �޸��� �ȱ� �ӵ�
        // Time.deltatime�� ��ǻ�� ���� ������� ������ ����
        if (wDown && !isBorder)
            transform.position += moveVec * speed * 0.4f * Time.deltaTime;
        else
            transform.position += moveVec * speed * Time.deltaTime;

        // �޸��� ���
        anim.SetBool("isRun", moveVec != Vector3.zero);
        // �ȱ� ���
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        // Ű����� �����̴� ���� �ٶ�
        transform.LookAt(transform.position + moveVec);

        // �� �߻� �� ���콺 ���� �ٶ�
        if (fDown)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
    }

    void Jump()
    {
        // �������� - ���� �߿� ���� �ȉ�
        if (jDown && moveVec == Vector3.zero &&!isJump && !isDodge)
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            // ���� ���
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if(fDown && isFireReady && !isDodge && !isReload)
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }

    }

    void Grenade()
    {
        if (hasGrenades == 0)
            return;

        if(gDown && !isReload)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 10;

                GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrenades--;
                grenades[hasGrenades].SetActive(false);
            }
        }
    }
    void Reload()
    {
        if (equipWeapon == null)
            return;

        if (equipWeapon.type == Weapon.Type.Melee)
            return;

        if (ammo == 0)
            return;

        if(rDown && !isJump && !isDodge && isFireReady && !isReload)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 2f);
        }
    }

    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        ammo -= reAmmo;
        isReload = false;
    }

    void Dodge()
    {
        // ȸ�� ������ ����
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge)
        {
            // ȸ�� ���
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f);
        }
    }
    void DodgeOut()
    {
        // ������� ���ƿ���
        speed *= 0.5f;
        isDodge = false;
    }

    void Swap()
    {
        // ���� ���� ����
        // 1 ��ġ 2 ���� 3 �������
        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;

        // ����Ű ������ ������ 
        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge)
        {
            if(equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);
            
            if(hasWeapons[weaponIndex])
            {
                equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
                equipWeapon.gameObject.SetActive(true);

                // ���� ���� �ִϸ��̼�
                anim.SetTrigger("doSwap");
            }
            
        }
    }

    void Interaction()
    {
        // ��ȣ�ۿ� ����
        if(iDown && nearObject != null && !isJump && !isDodge)
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }

    // ��ü�� ����� �� �÷��̾ ������ ��ġ�� �ʰ� ��
    void FreezeRotation() 
    {
        rigid.angularVelocity = Vector3.zero;
    }
    // ���� �հ� ������ ���� ����
    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, moveVec, 3, LayerMask.GetMask("Wall"));
    }
    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

    // player �ݶ��̴��� �浹 �� ���� 
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // ������ ȹ��� ����
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    if (hasGrenades == maxHasGrenades)
                        return;
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;

                    break;
            }
            Destroy(other.gameObject);
        }
        // �� ����ü�� �ǰݽ� ����
        else if(other.tag == "EnemyBullet")
        {
            if (!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;

                bool isBossAtk = other.name == "Boss Melee Area";
                StartCoroutine(OnDamage(isBossAtk));
            }

            if (other.GetComponent<Rigidbody>() != null)
                Destroy(other.gameObject);
        }
    }

    IEnumerator OnDamage(bool isBossAtk)
    {
        isDamage = true;
        foreach(MeshRenderer m in mesh)
            m.material.color = Color.yellow;

        if (isBossAtk)
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse);
        // �ǰ� �� �����ð� �ο�
        yield return new WaitForSeconds(1f);
        isDamage = false;
        foreach (MeshRenderer m in mesh)
            m.material.color = Color.white;

        if (isBossAtk)
            rigid.velocity = Vector3.zero;
    }
    // player�� ���Ⱑ ������� �� ������
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject;
        //Debug.Log(nearObject.name);
    }
    // player�� ����� �������� �ٽ� ���� ���
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }
}
