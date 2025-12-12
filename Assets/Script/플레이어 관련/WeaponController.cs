using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float attackRate = 1f;
    public float range = 10f; // 사거리

    private float timer;
    private PlayerController playerController; // 플레이어 스탯을 가져오기 위한 참조

    void Start()
    {
        // 같은 오브젝트에 있는 PlayerController를 미리 찾아둠 (최적화)
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 공격 속도 업그레이드가 있다면 attackRate가 줄어들었을 것임
        if (timer > attackRate)
        {
            AttackNearestEnemy();
            timer = 0;
        }
    }

    void AttackNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        // 가장 가까운 적 찾기
        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(enemy.transform.position, currentPos);
            if (dist < minDistance && dist <= range)
            {
                nearestEnemy = enemy;
                minDistance = dist;
            }
        }

        // 적이 있으면 발사
        if (nearestEnemy != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            // Y축(높이) 차이는 무시하고 수평 방향 계산
            Vector3 direction = (nearestEnemy.transform.position - transform.position);
            direction.y = 0;

            // ⭐ [핵심 수정] 플레이어의 공격력 스탯 적용
            int baseDamage = 10; // 기본 데미지
            int finalDamage = baseDamage;

            if (playerController != null)
            {
                // 기본 데미지 * 배율 (반올림)
                finalDamage = Mathf.RoundToInt(baseDamage * playerController.damageMultiplier);
            }

            // 총알 스크립트에 데미지와 방향 전달
            Bullet b = bullet.GetComponent<Bullet>();
            if (b != null)
            {
                b.damage = finalDamage;
                b.SetDirection(direction.normalized);
            }
        }
    }
}