using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 10;

    public void SetDirection(Vector3 dir)
    {
        // 총알이 날아가는 방향을 바라보게 설정
        transform.forward = dir;
    }

    void Update()
    {
        // 앞으로 전진
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        Destroy(gameObject, 3f);
    }

    // 3D 충돌 감지 (이름 뒤에 2D 빠짐)
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}