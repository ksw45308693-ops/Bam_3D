using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float attackRate = 1f;
    public float range = 10f; // 사거리

    private float timer;
    private PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        timer += Time.deltaTime;

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

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(enemy.transform.position, currentPos);
            if (dist < minDistance && dist <= range)
            {
                nearestEnemy = enemy;
                minDistance = dist;
            }
        }

        if (nearestEnemy != null)
        {
            // ⭐ [핵심 수정] 발사 위치 보정 (발바닥이 아닌 1m 위에서 발사)
            // 캐릭터 모델의 피벗(중심)이 발바닥에 있어서 총알이 땅에 묻히는 걸 방지합니다.
            Vector3 spawnPos = transform.position + Vector3.up * 1.0f;

            // 수정된 위치에서 총알 생성
            GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

            // 방향 계산도 '발사 위치(spawnPos)'를 기준으로 다시 계산해야 정확합니다.
            Vector3 direction = (nearestEnemy.transform.position - spawnPos);
            direction.y = 0; // 수평 유지

            int baseDamage = 10;
            int finalDamage = baseDamage;

            if (playerController != null)
            {
                finalDamage = Mathf.RoundToInt(baseDamage * playerController.damageMultiplier);
            }

            Bullet b = bullet.GetComponent<Bullet>();
            if (b != null)
            {
                b.damage = finalDamage;
                b.SetDirection(direction.normalized);
            }
        }
    }
}