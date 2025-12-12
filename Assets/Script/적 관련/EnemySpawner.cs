using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Basic Settings")]
    public GameObject enemyPrefab;
    public Transform player;
    public float spawnRadius = 10f;

    [Header("Performance Limit")]
    public int maxEnemyCount = 300; // ⭐ 최대 300마리까지만 유지

    [Header("Wave Settings")]
    public float waveDuration = 10f;
    public int currentWave = 0;
    private float waveTimer;

    [Header("Difficulty Scaling")]
    public float spawnInterval = 1f;
    public float minSpawnInterval = 0.1f;
    public float spawnIntervalDecrease = 0.1f;

    public int initialHealth = 30;
    public int healthIncreasePerWave = 10;

    private float timer;

    void Update()
    {
        if (player == null) return;

        // 1. 웨이브 시간 체크
        waveTimer += Time.deltaTime;
        if (waveTimer >= waveDuration)
        {
            NextWave();
        }

        // 2. 적 생성 타이머
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            // ⭐ [최적화 핵심] 현재 적의 숫자를 세고, 제한보다 적을 때만 생성
            // (Tag로 찾는 방식은 적이 많을 때 약간의 부하가 있지만, Instantiate를 무작정 하는 것보다는 낫습니다)
            int currentCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

            if (currentCount < maxEnemyCount)
            {
                SpawnEnemy(false);
            }

            timer = 0f;
        }
    }

    void NextWave()
    {
        currentWave++;
        waveTimer = 0;
        spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval - spawnIntervalDecrease);

        // 보스는 마릿수 제한 무시하고 중요하게 등장
        if (currentWave % 5 == 0) SpawnEnemy(true);

        Debug.Log($"🌊 웨이브 {currentWave} 시작! (현재 적: {GameObject.FindGameObjectsWithTag("Enemy").Length}마리)");
    }

    int GetWaveHealth() { return initialHealth + (currentWave * healthIncreasePerWave); }

    void SpawnEnemy(bool isBoss)
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPos = player.position + new Vector3(randomCircle.x, 0, randomCircle.y);

        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        EnemyHealth hp = enemy.GetComponent<EnemyHealth>();

        if (hp != null)
        {
            if (isBoss)
            {
                hp.maxHealth = GetWaveHealth() * 10;
                enemy.transform.localScale = Vector3.one * 2f;
                enemy.name = "Boss";
            }
            else
            {
                hp.maxHealth = GetWaveHealth();
            }
        }
    }
}