using UnityEngine;
using UnityEngine.UI; // UI 사용

public class UltimateSkill : MonoBehaviour
{
    [Header("Skill Settings")]
    public float cooldownTime = 10f; // 쿨타임 (10초)
    public float damageRange = 15f;  // 폭발 범위
    public int damageAmount = 9999;  // 데미지 (거의 즉사)
    public LayerMask enemyLayer;     // 적 레이어 (최적화용)

    [Header("VFX")]
    public GameObject explosionEffect; // 폭발 이펙트 프리팹

    [Header("UI")]
    public Image skillButtonImage;   // 버튼의 쿨타임 덮개 이미지 (Filled 타입)
    public Button uiButton;          // 실제 클릭 버튼

    private float currentCooldown = 0f;
    private bool isCooldown = false;

    void Start()
    {
        // 시작할 때 쿨타임 0으로 초기화
        currentCooldown = 0;
        if (skillButtonImage != null) skillButtonImage.fillAmount = 0;
    }

    void Update()
    {
        // PC 테스트용 키보드 입력 (스페이스바)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ActivateSkill();
        }

        // 쿨타임 계산
        if (isCooldown)
        {
            currentCooldown -= Time.deltaTime;

            // UI 갱신 (남은 시간만큼 그림이 채워짐/줄어듦)
            if (skillButtonImage != null)
            {
                skillButtonImage.fillAmount = currentCooldown / cooldownTime;
            }

            if (currentCooldown <= 0)
            {
                currentCooldown = 0;
                isCooldown = false;
                if (uiButton != null) uiButton.interactable = true; // 버튼 다시 활성화
            }
        }
    }

    // ⭐ 필살기 발동 함수 (버튼에 연결)
    public void ActivateSkill()
    {
        if (isCooldown) return; // 쿨타임 중이면 무시

        // 1. 적 감지 (내 주변 반경 damageRange 안의 모든 적)
        Collider[] enemies = Physics.OverlapSphere(transform.position, damageRange);

        foreach (Collider enemy in enemies)
        {
            // 태그가 Enemy인 녀석들만 골라서
            if (enemy.CompareTag("Enemy"))
            {
                // 데미지 주기
                EnemyHealth hp = enemy.GetComponent<EnemyHealth>();
                if (hp != null)
                {
                    hp.TakeDamage(damageAmount);
                }
            }
        }

        // 2. 이펙트 재생
        if (explosionEffect != null)
        {
            // 플레이어 위치에 폭발 이펙트 생성
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // 3. 쿨타임 시작
        isCooldown = true;
        currentCooldown = cooldownTime;
        if (uiButton != null) uiButton.interactable = false; // 버튼 클릭 금지

        Debug.Log("💥 필살기 발동! 주변 적 소멸!");
    }

    // 범위 확인용 기즈모 그리기
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRange);
    }
}