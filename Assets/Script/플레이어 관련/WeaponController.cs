using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float attackRate = 1f;
    public float range = 10f;

    private float timer;

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
            // 3D 거리 계산
            float dist = Vector3.Distance(enemy.transform.position, currentPos);
            if (dist < minDistance && dist <= range)
            {
                nearestEnemy = enemy;
                minDistance = dist;
            }
        }

        if (nearestEnemy != null)
        {
            // 총알 생성 (플레이어 높이에 맞춰서)
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            // 방향 계산 (Y축 높이 차이 무시하고 수평 발사)
            Vector3 direction = (nearestEnemy.transform.position - transform.position);
            direction.y = 0;

            bullet.GetComponent<Bullet>().SetDirection(direction.normalized);
        }
    }
}