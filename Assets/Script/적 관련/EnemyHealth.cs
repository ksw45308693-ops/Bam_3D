using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 30; // 적의 최대 체력
    private int currentHealth;

    public GameObject expGemPrefab; // ⭐ 인스펙터에서 보석 프리팹을 꼭 넣어주세요!

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // 보석 생성 (프리팹이 연결되어 있을 때만)
        if (expGemPrefab != null)
        {
            Instantiate(expGemPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject); // 적 삭제
    }
}