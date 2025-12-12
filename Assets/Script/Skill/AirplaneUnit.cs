using UnityEngine;

public class AirplaneUnit : MonoBehaviour
{
    [Header("Flight Settings")]
    public float speed = 15f;          // 비행 속도 (빠르게!)
    public float turnSpeed = 3f;       // 선회 속도 (낮을수록 크게 돔)
    public float searchRange = 25f;    // 적 감지 범위
    public int damage = 30;            // 몸통 박치기 데미지

    private Transform target;          // 현재 추적 중인 적
    private Transform player;          // 플레이어 (순찰 기준점)
    private Vector3 patrolPoint;       // 적이 없을 때 비행할 목표 지점

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        // 처음에 랜덤한 순찰 지점 잡기
        GetNewPatrolPoint();
    }

    void Update()
    {
        if (player == null) return;

        // ⭐ 1. 비행기는 절대 멈추지 않고 항상 앞으로 전진함
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // 2. 타겟 관리 (적이 없거나 죽었으면 새로 찾기)
        if (target == null)
        {
            FindNearestEnemy();

            // 적이 여전히 없으면 플레이어 주변을 순찰(배회)
            if (target == null)
            {
                PatrolAroundPlayer();
            }
        }
        else
        {
            // 적이 있으면 적을 향해 부드럽게 방향을 틈
            SteerTowards(target.position);
        }
    }

    void SteerTowards(Vector3 targetPos)
    {
        // 목표 방향 계산
        Vector3 direction = (targetPos - transform.position).normalized;

        // 비행기가 땅을 보고 처박히지 않게 높이(Y)는 완만하게 조정 (선택 사항)
        // direction.y = 0; 

        if (direction != Vector3.zero)
        {
            // 목표 회전값
            Quaternion lookRot = Quaternion.LookRotation(direction);

            // ⭐ 부드럽게 회전 (이게 비행기 느낌의 핵심!)
            // 즉시 바라보지 않고 turnSpeed에 맞춰 천천히 고개를 돌림
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, turnSpeed * Time.deltaTime);
        }
    }

    void PatrolAroundPlayer()
    {
        // 순찰 지점에 가까워지면(5m 이내) 새로운 지점 갱신
        if (Vector3.Distance(transform.position, patrolPoint) < 5f)
        {
            GetNewPatrolPoint();
        }

        // 순찰 지점을 향해 선회
        SteerTowards(patrolPoint);
    }

    void GetNewPatrolPoint()
    {
        // 플레이어 주변 반경 15m 공중에 랜덤 좌표 생성
        Vector2 randomPoint = Random.insideUnitCircle * 15f;
        // 플레이어보다 5m 높은 곳에서 날아다니도록 설정
        patrolPoint = player.position + new Vector3(randomPoint.x, 5f, randomPoint.y);
    }

    void FindNearestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, searchRange);
        float minDist = Mathf.Infinity;
        Transform bestTarget = null;

        foreach (Collider col in enemies)
        {
            if (col.CompareTag("Enemy"))
            {
                float d = Vector3.Distance(transform.position, col.transform.position);
                if (d < minDist)
                {
                    minDist = d;
                    bestTarget = col.transform;
                }
            }
        }
        target = bestTarget;
    }

    // 충돌 처리 (비행기는 적을 뚫고 지나감)
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth hp = other.GetComponent<EnemyHealth>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }
            // 부딪혀도 멈추지 않음! (폭격기처럼 지나감)
        }
    }
}