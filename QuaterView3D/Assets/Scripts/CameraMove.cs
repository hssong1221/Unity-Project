using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    // 계속 플레이어 뒷통수 따라다님
    void Update()
    {
        transform.position = target.position + offset;
    }
}
