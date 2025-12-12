using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Basic Settings (기본 설정)")]
    public GameObject enemyPrefab; // 일반 적 프리팹
    public GameObject bossPrefab;  // ⭐ [추가] 보스 전용 프리팹 (인스펙터에서 연결하세요!)
    public Transform player;
    public float spawnRadius = 10f;

    [Header("Performance Limit (최적화 설정)")]
    public int maxEnemyCount = 300;

    [Header("Wave Settings (웨이브 시스템)")]
    public float waveDuration = 10f;
    public int currentWave = 0;
    private float waveTimer;

    [Header("Difficulty Scaling (난이도 조절)")]
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
            int currentCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

            if (currentCount < maxEnemyCount)
            {
                SpawnEnemy(false); // 일반 적 생성
            }

            timer = 0f;
        }
    }

    void NextWave()
    {
        currentWave++;
        waveTimer = 0;
        spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval - spawnIntervalDecrease);

        // 5 웨이브마다 보스 등장
        if (currentWave % 5 == 0) SpawnEnemy(true);

        Debug.Log($"🌊 웨이브 {currentWave} 시작! (현재 적: {GameObject.FindGameObjectsWithTag("Enemy").Length}마리)");
    }

    int GetWaveHealth()
    {
        return initialHealth + (currentWave * healthIncreasePerWave);
    }

    void SpawnEnemy(bool isBoss)
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPos = player.position + new Vector3(randomCircle.x, 0, randomCircle.y);

        // ⭐ [수정] 보스냐 아니냐에 따라 생성할 프리팹 결정
        GameObject prefabToSpawn = enemyPrefab; // 기본값: 일반 적

        if (isBoss && bossPrefab != null)
        {
            prefabToSpawn = bossPrefab; // 보스면 보스 프리팹 사용
        }

        // 결정된 프리팹 생성
        GameObject enemy = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        EnemyHealth hp = enemy.GetComponent<EnemyHealth>();

        if (hp != null)
        {
            if (isBoss)
            {
                // 보스 설정
                hp.maxHealth = GetWaveHealth() * 10; // 체력 10배
                enemy.name = "Boss"; // 이름 변경

                // (참고: 보스 프리팹은 이미 크기가 클 테니, 강제 크기 조절 코드는 뺐습니다.
                // 만약 보스도 크기를 키우고 싶다면 아래 주석을 해제하세요)
                // enemy.transform.localScale = Vector3.one * 2f;
            }
            else
            {
                // 일반 적 설정
                hp.maxHealth = GetWaveHealth();
            }
        }
    }
}