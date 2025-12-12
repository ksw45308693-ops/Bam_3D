using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Basic Settings")]
    public GameObject enemyPrefab;
    public Transform player;
    public float spawnRadius = 10f; // 플레이어로부터 생성되는 거리

    [Header("Wave Settings")]
    public float waveDuration = 10f; // 한 웨이브의 지속 시간 (10초마다 난이도 상승)
    public int currentWave = 0;
    private float waveTimer;

    [Header("Difficulty Scaling")] // 난이도 조절 변수들
    public float spawnInterval = 1f; // 현재 생성 간격
    public float minSpawnInterval = 0.1f; // 최소 생성 간격 (이보다 빨라지진 않음)
    public float spawnIntervalDecrease = 0.1f; // 웨이브당 줄어들 생성 시간

    public int initialHealth = 30; // 적 기본 체력
    public int healthIncreasePerWave = 10; // 웨이브당 늘어날 체력

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
            SpawnEnemy(false); // 일반 적 생성
            timer = 0f;
        }
    }

    void NextWave()
    {
        currentWave++;
        waveTimer = 0;

        // 생성 속도 빠르게 (최소값보다는 작아지지 않게)
        spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval - spawnIntervalDecrease);

        Debug.Log($"🌊 웨이브 {currentWave} 시작! 생성간격: {spawnInterval}초 / 적 체력: {GetWaveHealth()}");

        // ⭐ 5 웨이브마다 보스 등장!
        if (currentWave % 5 == 0)
        {
            SpawnEnemy(true); // 보스 생성
        }
    }

    // 현재 웨이브에 맞는 체력 계산
    int GetWaveHealth()
    {
        return initialHealth + (currentWave * healthIncreasePerWave);
    }

    void SpawnEnemy(bool isBoss)
    {
        // 3D 랜덤 위치 계산
        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPos = player.position + new Vector3(randomCircle.x, 0, randomCircle.y);

        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        // --- 난이도 적용 (체력 & 크기) ---
        EnemyHealth hp = enemy.GetComponent<EnemyHealth>();
        if (hp != null)
        {
            if (isBoss)
            {
                // 보스 스펙: 체력 10배, 크기 2배, 빨간색(선택)
                hp.maxHealth = GetWaveHealth() * 10;
                enemy.transform.localScale = Vector3.one * 2f;
                enemy.name = "Boss";
                Debug.Log("👹 보스 출현!");
            }
            else
            {
                // 일반 적: 웨이브에 따른 체력 증가
                hp.maxHealth = GetWaveHealth();
            }
        }
    }
}