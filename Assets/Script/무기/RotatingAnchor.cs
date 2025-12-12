using UnityEngine;

public class RotatingAnchor : MonoBehaviour
{
    public float rotationSpeed = 150f; // 회전 속도

    void Update()
    {
        // Y축(위쪽)을 기준으로 빙글빙글 돕니다.
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}