using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement (이동 설정)")]
    public float moveSpeed = 5f; // 플레이어 이동 속도

    [Header("Stats (플레이어 능력치)")]
    public float damageMultiplier = 1f; // 공격력 배율 (기본 1배, 강화 시 증가)
    public float magnetRange = 4f;      // 자석 범위 (기본 4, 강화 시 증가)
    public int maxHealth = 100;         // 최대 체력
    public int currentHealth;           // 현재 체력

    [Header("AI Auto Dodge (오토 모드)")]
    public bool useAutoMode = true;     // 오토 모드 사용 여부
    public float detectionRadius = 5f;  // 적 감지 반경
    public LayerMask enemyLayer;        // 적 레이어 (최적화용)

    [Header("UI Controls (조작 UI)")]
    public VirtualJoystick virtualJoystick; // 조이스틱 연결

    [Header("Experience (경험치 & 레벨)")]
    public int level = 1;
    public int currentExp = 0;
    public int maxExp = 100;
    public bool isAutoLevelUp = false; // 자동 레벨업 토글 상태

    [Header("UI (패널 및 슬라이더)")]
    public GameObject levelUpPanel;  // 레벨업 선택창
    public GameObject gameOverPanel; // 게임오버 창
    public Slider expSlider;         // 경험치 바
    public Slider hpSlider;          // 체력 바

    // 내부 변수들
    private Rigidbody rb;
    private Vector3 moveInput;       // 최종 이동 방향 벡터
    private WeaponController weapon; // 무기 관리자
    private LevelUpManager levelUpManager; // 레벨업 매니저

    private bool isDead = false;     // 사망 여부 체크
    private float damageCooldown = 0.5f; // 무적 시간 (0.5초)
    private float lastDamageTime;    // 마지막으로 맞은 시간

    void Start()
    {
        // 필수 컴포넌트 가져오기
        rb = GetComponent<Rigidbody>();
        weapon = GetComponent<WeaponController>();
        levelUpManager = FindObjectOfType<LevelUpManager>(); // 씬에 있는 매니저 찾기

        // 체력 초기화
        currentHealth = maxHealth;

        // UI 초기화
        UpdateExpUI();
        UpdateHealthUI();
    }

    void Update()
    {
        if (isDead) return; // 죽었으면 아무것도 안 함

        // 1. 입력 받기 (조이스틱 우선, 없으면 키보드)
        float x = 0, z = 0;
        if (virtualJoystick != null && virtualJoystick.InputVector != Vector2.zero)
        {
            x = virtualJoystick.InputVector.x;
            z = virtualJoystick.InputVector.y;
        }
        else
        {
            x = Input.GetAxisRaw("Horizontal"); // 키보드 A, D
            z = Input.GetAxisRaw("Vertical");   // 키보드 W, S
        }

        // 수동 입력 벡터 만들기
        Vector3 manualInput = new Vector3(x, 0, z).normalized;

        // 2. 이동 방향 결정
        // 수동 조작 중이면 수동 입력 사용, 아니면 오토 모드 AI 사용
        if (manualInput != Vector3.zero)
        {
            moveInput = manualInput;
        }
        else if (useAutoMode)
        {
            moveInput = GetAutoDodgeVector(); // AI 회피 로직
        }
        else
        {
            moveInput = Vector3.zero; // 정지
        }

        // 3. 회전 (이동하는 방향 바라보기)
        if (moveInput != Vector3.zero)
        {
            transform.forward = Vector3.Lerp(transform.forward, moveInput, 10f * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;
        // 물리 이동 (Rigidbody를 사용해 벽 뚫기 방지 및 충돌 처리)
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    // ⭐ [중요 버그 수정] 충돌 감지 로직
    // 기존 OnTriggerStay -> OnCollisionStay로 변경 (단단한 적과 부딪힐 때 사용)
    void OnCollisionStay(Collision collision)
    {
        if (isDead) return;

        // ⭐ "Enemy" 태그가 달린 녀석하고만 충돌 처리!
        // (바닥(Ground)이나 보석(Gem)이 닿았을 때 체력이 깎이는 버그를 막아줍니다.)
        if (collision.collider.CompareTag("Enemy"))
        {
            // 무적 시간 체크 (0.5초마다 데미지)
            if (Time.time > lastDamageTime + damageCooldown)
            {
                TakeDamage(10); // 데미지 10 입음
                lastDamageTime = Time.time;
            }
        }
    }

    // 데미지 처리 함수
    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthUI(); // 체력바 갱신
        if (currentHealth <= 0) Die(); // 체력 0이면 사망
    }

    // 사망 처리 함수
    void Die()
    {
        isDead = true;

        // 게임 매니저에게 "최고 점수 저장해!"라고 요청
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null) gm.GameOverSave();

        // 게임오버 패널 띄우고 시간 정지
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // 경험치 획득 함수
    public void AddExp(int amount)
    {
        currentExp += amount;

        // ⭐ [중요 버그 수정] if -> while 변경
        // 한 번에 많은 경험치를 먹었을 때, 레벨업을 연속으로 처리하기 위함
        // (예: 경험치가 500 들어왔는데 필요 경험치가 100이면, 5업을 한 번에 함)
        while (currentExp >= maxExp)
        {
            LevelUp();
        }
        UpdateExpUI();
    }

    // 레벨업 처리 함수
    void LevelUp()
    {
        currentExp -= maxExp; // 초과된 경험치는 다음 레벨로 이월
        level++;
        maxExp += 50; // 다음 레벨 필요 경험치 증가

        // 매니저에게 레벨업 창 띄우기 요청 (오토 모드인지 정보 전달)
        if (levelUpManager != null)
        {
            levelUpManager.ShowLevelUpWindow(isAutoLevelUp);
        }

        // (여기서는 Time.timeScale 정지를 하지 않음. 매니저가 창을 띄울 때 정지함)
        UpdateExpUI();
    }

    // ⭐ 능력을 실제로 적용하는 함수 (버튼을 눌렀을 때 실행됨)
    public void ApplyUpgrade(int upgradeType)
    {
        switch (upgradeType)
        {
            case 0: // 이동 속도
                moveSpeed += 1f;
                break;
            case 1: // 공격 속도
                if (weapon != null) weapon.attackRate *= 0.9f;
                break;
            case 2: // 공격력 (New)
                damageMultiplier += 0.2f;
                break;
            case 3: // 최대 체력 (New)
                maxHealth += 20;
                currentHealth = maxHealth; // 체력 풀 회복 보너스
                UpdateHealthUI();
                break;
            case 4: // 자석 범위 (New)
                magnetRange += 2f;
                break;
        }
    }

    // 오토 모드용 회피 벡터 계산 (가장 가까운 적 반대 방향)
    Vector3 GetAutoDodgeVector()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        if (enemies.Length == 0) return Vector3.zero;

        Vector3 flee = Vector3.zero;
        foreach (var e in enemies)
        {
            Vector3 diff = transform.position - e.transform.position;
            diff.y = 0;
            // 가까운 적일수록 더 강하게 밀어내는 벡터 계산
            flee += diff.normalized / (diff.magnitude + 0.1f);
        }
        return flee.normalized;
    }

    // 게임 재시작 (버튼 연결용)
    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 오토 레벨업 토글 (UI 연결용)
    public void SetAutoLevelUp(bool isOn)
    {
        isAutoLevelUp = isOn;
    }

    // UI 갱신 헬퍼 함수들
    void UpdateHealthUI() { if (hpSlider != null) hpSlider.value = (float)currentHealth / maxHealth; }
    void UpdateExpUI() { if (expSlider != null) expSlider.value = (float)currentExp / maxExp; }
}