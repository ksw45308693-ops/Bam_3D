using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 30;
    private int currentHealth;

    public GameObject expGemPrefab;
    public GameObject damageTextPrefab; // ⭐ 데미지 텍스트 프리팹 추가

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        // ⭐ 데미지 텍스트 띄우기
        if (damageTextPrefab != null)
        {
            ShowDamageText(damageAmount);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void ShowDamageText(int damage)
    {
        // 적 머리 위쪽(Y + 1.5 정도)에 생성
        Vector3 spawnPos = transform.position + Vector3.up * 1.5f;

        GameObject hudText = Instantiate(damageTextPrefab, spawnPos, Quaternion.identity);

        // 데미지 숫자 전달
        hudText.GetComponent<DamageText>().SetDamage(damage);
    }

    void Die()
    {
        if (expGemPrefab != null)
        {
            Instantiate(expGemPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}