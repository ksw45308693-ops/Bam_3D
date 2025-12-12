using UnityEngine;

public class ContactWeapon : MonoBehaviour
{
    public int damage = 15;
    public float knockbackForce = 5f; // (선택사항) 적을 밀어내는 힘

    // 3D 충돌 감지
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                // (선택사항) 적을 살짝 뒤로 밀어내기 (타격감 UP)
                Rigidbody enemyRb = other.GetComponent<Rigidbody>();
                if (enemyRb != null)
                {
                    Vector3 pushDir = (other.transform.position - transform.position).normalized;
                    enemyRb.AddForce(pushDir * knockbackForce, ForceMode.Impulse);
                }
            }
        }
    }
}