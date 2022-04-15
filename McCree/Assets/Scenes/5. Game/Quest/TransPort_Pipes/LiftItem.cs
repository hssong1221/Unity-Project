using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftItem : MonoBehaviour
{
    [SerializeField]
    private bool _isLifting;

    public bool isLifting
    {
        get { return _isLifting; }
        set
        {
            _isLifting = value;

            if (_isLifting == true) // 들때
            {
                Destroy(gameObject.GetComponent<Rigidbody>()); // 들면 rigidbody적용 x하기위해 destroy
                ps.Stop();
                ps.Clear(); // 반짝이 꺼줌

                transform.localPosition = new Vector3(0f, 0f, 0f);
                transform.localRotation = Quaternion.identity;
                transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                // position 오브젝트의 위치를 항상 월드의 원점을 기준으로 월드 공간상에 선언한다.
                // localPosition 부모의 위치 기준으로 설정한다
            }

            else if (_isLifting == false) // 떨굴 때
            {
                Rigidbody rb = gameObject.AddComponent<Rigidbody>(); // 떨굴때 중력주려고 rigidbody추가
                rb.AddForce(new Vector3(0.0f, 5.0f, 0.0f), ForceMode.Impulse);

                ps.Play();

                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                // 크기 되돌림
            }
        }
    }

    [SerializeField]
    private bool _isGrounded;

    public bool isGrounded
    {
        get { return _isGrounded; }
        set
        {
            _isGrounded = value;
            if (_isGrounded == true) // isground가 true되면 리지드바디삭제함
            {
                if (gameObject.GetComponent<Rigidbody>())
                    Destroy(gameObject.GetComponent<Rigidbody>());
            }
        }

    }
    private int floorLayerMask;

    private Collider cd;
    private ParticleSystem ps;
    
    private float rotate_x = 0f;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        cd = GetComponent<Collider>();
        ps = GetComponent<ParticleSystem>();

        floorLayerMask = 1 << LayerMask.NameToLayer("Floor");
    }

    private void Start()
    {
        _isLifting = false;
        _isGrounded = false;
    }

    private void FixedUpdate()
    {
        transform.rotation = new Quaternion(rotate_x, transform.rotation.y, transform.rotation.z, transform.rotation.w);

        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.7f, floorLayerMask))
        {   // 대략 0.6부터 지면이랑 충돌했다고 인식함
            Invoke("IsKinematic_True", 1f); // 1초후에 isground true, rigidbody삭제
        }
        else
        {
            isGrounded = false;
            CancelInvoke();
        }

    }

    private void IsKinematic_True()
    {
        isGrounded = true;
    }

}
