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
        // Player 안에 Mesh Object에 animator있어서 Inchildren 
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
        // 플레이어 이동 (x축 z축)
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");
        // 특정키 입력
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        iDown = Input.GetButtonDown("Interaction");
        fDown = Input.GetButton("Fire1");
        gDown = Input.GetButtonDown("Fire2");
        rDown = Input.GetButtonDown("Reload");
        //무기교체
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
        if(!isDodge)
            moveVec = new Vector3(hAxis, 0, vAxis).normalized; // 벡터값을 1로 보정
        if(!isBorder)
           

        // 달리기 걷기 속도
        // Time.deltatime은 컴퓨터 사양과 상관없이 일정한 간격
        if (wDown && !isBorder)
            transform.position += moveVec * speed * 0.4f * Time.deltaTime;
        else
            transform.position += moveVec * speed * Time.deltaTime;

        // 달리기 모션
        anim.SetBool("isRun", moveVec != Vector3.zero);
        // 걷기 모션
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        // 키보드로 움직이는 방향 바라봄
        transform.LookAt(transform.position + moveVec);

        // 총 발사 시 마우스 방향 바라봄
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
        // 점프구현 - 점프 중엔 점프 안됌
        if (jDown && moveVec == Vector3.zero &&!isJump && !isDodge)
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            // 점프 모션
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
        // 회피 구르기 구현
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge)
        {
            // 회피 모션
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f);
        }
    }
    void DodgeOut()
    {
        // 원래대로 돌아오기
        speed *= 0.5f;
        isDodge = false;
    }

    void Swap()
    {
        // 무기 스왑 구현
        // 1 망치 2 권총 3 기관단총
        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;

        // 숫자키 누르면 스왑함 
        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge)
        {
            if(equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);
            
            if(hasWeapons[weaponIndex])
            {
                equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
                equipWeapon.gameObject.SetActive(true);

                // 무기 스왑 애니메이션
                anim.SetTrigger("doSwap");
            }
            
        }
    }

    void Interaction()
    {
        // 상호작용 구현
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

    // 물체와 닿았을 때 플레이어에 영향이 미치지 않게 함
    void FreezeRotation() 
    {
        rigid.angularVelocity = Vector3.zero;
    }
    // 벽을 뚫고 나가는 것을 방지
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

    // player 콜라이더와 충돌 시 실행 
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
        // 아이템 획득시 로직
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
        // 적 투사체에 피격시 로직
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
        // 피격 시 무적시간 부여
        yield return new WaitForSeconds(1f);
        isDamage = false;
        foreach (MeshRenderer m in mesh)
            m.material.color = Color.white;

        if (isBossAtk)
            rigid.velocity = Vector3.zero;
    }
    // player와 무기가 닿아있을 때 반응함
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject;
        //Debug.Log(nearObject.name);
    }
    // player가 무기와 떨어지면 다시 값을 비움
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }
}
