using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 3f;
    private Transform target;
    private Rigidbody rb;

    // ⭐ 플레이어와 이 거리보다 멀어지면 삭제됨 (메모리 절약)
    // 생성 반경(10)보다 충분히 커야 생성되자마자 삭제되는 일이 없습니다.
    private float despawnDistance = 40f;

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

        // 1. 플레이어와의 거리 계산
        float distance = Vector3.Distance(transform.position, target.position);

        // ⭐ [최적화 핵심] 너무 멀리 있는 적은 삭제 (새로운 적이 나올 공간 확보)
        if (distance > despawnDistance)
        {
            Destroy(gameObject);
            return; // 삭제했으니 아래 이동 코드는 실행 안 함
        }

        // 2. 방향 구하기
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0; // 높이는 무시하고 수평 이동만

        // 3. 이동
        Vector3 nextPos = rb.position + direction * speed * Time.fixedDeltaTime;
        rb.MovePosition(nextPos);

        // 4. 회전 (플레이어 바라보기)
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            rb.rotation = Quaternion.Slerp(rb.rotation, lookRotation, 10f * Time.fixedDeltaTime);
        }
    }
}