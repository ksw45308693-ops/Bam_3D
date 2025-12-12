using UnityEngine;

public class Reposition : MonoBehaviour
{
    private GameObject player;
    private float tileSize;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        // 3D에서는 Collider(MeshCollider 등)의 크기를 가져옵니다.
        if (GetComponent<Collider>() != null)
        {
            // bounds.size.x는 타일의 실제 가로 길이입니다.
            tileSize = GetComponent<Collider>().bounds.size.x;
        }
    }

    void Update()
    {
        if (player == null) return;

        // 플레이어와의 거리 계산 (3D이므로 Z축 사용)
        float diffX = player.transform.position.x - transform.position.x;
        float diffZ = player.transform.position.z - transform.position.z; // ⭐ Y -> Z 변경

        float dirX = diffX < 0 ? -1 : 1;
        float dirZ = diffZ < 0 ? -1 : 1;

        // 타일 크기만큼 멀어졌을 때 위치 재배치
        if (Mathf.Abs(diffX) > tileSize)
        {
            transform.Translate(Vector3.right * dirX * tileSize * 2);
        }

        if (Mathf.Abs(diffZ) > tileSize) // ⭐ Y -> Z 변경
        {
            transform.Translate(Vector3.forward * dirZ * tileSize * 2); // ⭐ up -> forward 변경
        }
    }
}