using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform target;
    public Vector3 offset;
    public float rotationSpeed = 30.0f; // 회전 속도
    public float zoomSpeed = 30.0f;

    void Update()
    {
        transform.position = target.position + offset;
        transform.LookAt(target);

        if (Input.GetKey(KeyCode.Q))
        {
            RotateAroundTarget(rotationSpeed);
        }

        if (Input.GetKey(KeyCode.E))
        {
            RotateAroundTarget(-rotationSpeed);
        }

        if (Input.GetKey(KeyCode.W))
        {
            offset = offset.normalized * (offset.magnitude - zoomSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S))
        {
            offset = offset.normalized * (offset.magnitude + zoomSpeed * Time.deltaTime);
        }
    }


    // 타겟을 중심으로 회전
    void RotateAroundTarget(float speed)
    {
        // 카메라의 현재 위치를 중심으로 target 주위를 회전
        transform.RotateAround(target.position, Vector3.up, speed * Time.deltaTime);
        // 회전 후의 위치에서 오프셋 업데이트
        offset = transform.position - target.position;
    }
    
}
