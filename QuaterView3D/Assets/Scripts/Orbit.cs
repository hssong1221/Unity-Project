using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform target;
    public float orbitSpeed;
    Vector3 offSet;

    void Start()
    {
        // 공전체 위치에서 플레이어 위치 뺀 값
        offSet = transform.position - target.position;
    }

    void Update()
    {
        // rotatearound는 움직이면 어그러지기 때문에 기준 값에 offset을 줘서 계속 동일하게 해줌
        transform.position = target.position + offSet;
        transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime);
        offSet = transform.position - target.position;
    }
}
