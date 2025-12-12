using UnityEngine;

public class ExpGem : MonoBehaviour
{
    public int expAmount = 10;
    public float magnetRange = 4f;
    public float moveSpeed = 8f;

    private Transform playerTransform;
    private bool isAttracted = false;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // 3D 거리 계산
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance < magnetRange)
        {
            isAttracted = true;
        }

        if (isAttracted)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
            moveSpeed += 5f * Time.deltaTime;
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