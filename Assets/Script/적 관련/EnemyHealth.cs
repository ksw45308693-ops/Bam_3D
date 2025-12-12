using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 30;
    private int currentHealth;

    public GameObject expGemPrefab;
    public GameObject damageTextPrefab;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

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
        Vector3 spawnPos = transform.position + Vector3.up * 1.5f;
        GameObject hudText = Instantiate(damageTextPrefab, spawnPos, Quaternion.identity);
        hudText.GetComponent<DamageText>().SetDamage(damage);
    }

    void Die()
    {
        // ⭐ 추가: 게임 매니저를 찾아서 킬수 증가 함수 호출
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.AddKill();
        }

        if (expGemPrefab != null)
        {
            Instantiate(expGemPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}