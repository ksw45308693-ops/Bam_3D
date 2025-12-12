using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // 따라갈 대상 (플레이어)
    public float smoothSpeed = 5f; // 따라가는 속도 (클수록 빠름)

    private Vector3 offset; // 플레이어와 카메라 사이의 거리 유지용

    void Start()
    {
        // 게임 시작 시점의 카메라 위치와 플레이어 위치의 차이(거리)를 기억해둡니다.
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }

    // 플레이어가 움직인 뒤에 카메라가 움직여야 덜덜 떨리지 않습니다 (LateUpdate 사용)
    void LateUpdate()
    {
        if (target == null) return;

        // 1. 목표 위치 계산 (플레이어 위치 + 아까 기억해둔 거리)
        Vector3 targetPosition = target.position + offset;

        // 2. 부드럽게 이동 (Lerp 사용)
        // 바로 텔레포트하지 않고, 현재 위치에서 목표 위치로 조금씩 다가갑니다.
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}