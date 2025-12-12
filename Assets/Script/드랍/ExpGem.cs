using UnityEngine;

public class ExpGem : MonoBehaviour
{
    public int expAmount = 10;
    // public float magnetRange = 4f; // 이제 이 변수는 안 쓰고 플레이어 스탯을 씁니다.
    public float moveSpeed = 8f;

    private Transform playerTransform;
    private PlayerController playerController; // 플레이어 스탯 확인용
    private bool isAttracted = false;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerController = player.GetComponent<PlayerController>();
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // 3D 거리 계산
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // ⭐ [핵심 수정] 플레이어의 현재 자석 범위(magnetRange)를 가져와서 비교
        float currentMagnetRange = 4f; // 기본값 (플레이어를 못 찾았을 때 대비)

        if (playerController != null)
        {
            currentMagnetRange = playerController.magnetRange;
        }

        // 플레이어의 자석 범위 안에 들어왔다면 빨려감
        if (distance < currentMagnetRange)
        {
            isAttracted = true;
        }

        // 자석 효과 발동 중
        if (isAttracted)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
            moveSpeed += 5f * Time.deltaTime; // 점점 빨라짐
        }
    }

    // 3D 충돌 감지
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.AddExp(expAmount);
                Destroy(gameObject);
            }
        }
    }
}