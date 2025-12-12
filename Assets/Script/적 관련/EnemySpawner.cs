using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform player;
    public float spawnInterval = 1f;
    public float spawnRadius = 10f;

    private float timer;

    void Update()
    {
        if (player == null) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        // 2D 원형 랜덤 좌표를 구함
        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;

        // 2D(x, y) -> 3D(x, 0, z)로 변환하여 배치
        Vector3 spawnPos = player.position + new Vector3(randomCircle.x, 0, randomCircle.y);

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}