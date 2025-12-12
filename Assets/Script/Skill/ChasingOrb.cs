using UnityEngine;

public class ChasingOrb : MonoBehaviour
{
    public float moveSpeed = 8f;     // 이동 속도
    public float searchRange = 15f;  // 적 감지 범위
    public int damage = 20;          // 데미지

    private Transform player;        // 돌아올 플레이어 위치
    private Transform targetEnemy;   // 추격할 적

    void Start()
    {
        // 플레이어 찾기
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        // 1. 타겟이 없거나 죽었으면, 새로운 적 찾기
        if (targetEnemy == null)
        {
            FindNearestEnemy();
        }

        // 2. 이동 로직
        Vector3 destination;

        if (targetEnemy != null)
        {
            // 적이 있으면 적에게 돌진
            destination = targetEnemy.position;
        }
        else
        {
            // 적이 없으면 플레이어 주변(머리 위)에서 대기
            destination = player.position + Vector3.up * 2f;
        }

        // 3. 실제 이동 (부드럽게)
        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);

        // (선택) 무기가 바라보는 방향 회전
        transform.Rotate(Vector3.up * 200f * Time.deltaTime);
    }

    // 가장 가까운 적 찾는 함수
    void FindNearestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(player.position, searchRange);
        float minDistance = Mathf.Infinity;
        Transform nearest = null;

        foreach (Collider col in enemies)
        {
            if (col.CompareTag("Enemy"))
            {
                float dist = Vector3.Distance(transform.position, col.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearest = col.transform;
                }
            }
        }
        targetEnemy = nearest;
    }

    // 충돌 시 데미지 주기
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth hp = other.GetComponent<EnemyHealth>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }
            // 적을 때렸으면 타겟을 잊어버리고(null) 다시 검색하게 함 (혹은 계속 때리게 둘 수도 있음)
            // targetEnemy = null; 
        }
    }
}