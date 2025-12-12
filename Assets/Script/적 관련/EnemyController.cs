using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 3f;
    private Transform target;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // 3D Rigidbody

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        // 1. 방향 구하기
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0; // 높이는 무시하고 수평 이동만

        // 2. 이동
        Vector3 nextPos = rb.position + direction * speed * Time.fixedDeltaTime;
        rb.MovePosition(nextPos);

        // 3. 회전 (플레이어 바라보기)
        // Lerp를 사용하여 부드럽게 회전
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            rb.rotation = Quaternion.Slerp(rb.rotation, lookRotation, 10f * Time.fixedDeltaTime);
        }
    }
}