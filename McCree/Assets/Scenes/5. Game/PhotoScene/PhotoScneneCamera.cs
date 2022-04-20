using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoScneneCamera : MonoBehaviour
{

    public float angleX;
    public float angleY;
    public float angleZ;

    public float offsetX; 
    public float offsetY;
    public float offsetZ;

    public float CameraSpeed = 10.0f;
    public GameObject Target;
    Vector3 TargetPos;

    void Start()
    {

    }

    void FixedUpdate()
    {
        TargetPos = new Vector3(
            Target.transform.position.x + offsetX, 
            Target.transform.position.y + offsetY, 
            Target.transform.position.z + offsetZ
        );



        transform.position = Vector3.Lerp(transform.position, TargetPos, Time.deltaTime * CameraSpeed);

        transform.rotation = Quaternion.Euler(angleX, angleY, angleZ);
    }
}
