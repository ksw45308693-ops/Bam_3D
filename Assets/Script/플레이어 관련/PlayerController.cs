using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Stats")] // ⭐ 새로운 능력치들
    public float damageMultiplier = 1f; // 공격력 배율 (기본 1.0)
    public float magnetRange = 4f;      // 자석 범위
    public int maxHealth = 100;
    public int currentHealth;

    [Header("AI Auto Dodge")]
    public bool useAutoMode = true;
    public float detectionRadius = 5f;
    public LayerMask enemyLayer;

    [Header("UI Controls")]
    public VirtualJoystick virtualJoystick;

    [Header("Experience")]
    public int level = 1;
    public int currentExp = 0;
    public int maxExp = 100;
    public bool isAutoLevelUp = false;

    [Header("UI")]
    public GameObject levelUpPanel; // 패널 (이제 매니저가 관리하지만 참조는 유지)
    public GameObject gameOverPanel;
    public Slider expSlider;
    public Slider hpSlider;

    private Rigidbody rb;
    private Vector3 moveInput;
    private WeaponController weapon;
    private LevelUpManager levelUpManager; // ⭐ 레벨업 매니저 연결

    private bool isDead = false;
    private float damageCooldown = 0.5f;
    private float lastDamageTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        weapon = GetComponent<WeaponController>();
        levelUpManager = FindObjectOfType<LevelUpManager>(); // 매니저 찾기

        currentHealth = maxHealth;
        UpdateExpUI();
        UpdateHealthUI();
    }

    void Update()
    {
        if (isDead) return;

        float x = 0, z = 0;
        if (virtualJoystick != null && virtualJoystick.InputVector != Vector2.zero)
        {
            x = virtualJoystick.InputVector.x;
            z = virtualJoystick.InputVector.y;
        }
        else
        {
            x = Input.GetAxisRaw("Horizontal");
            z = Input.GetAxisRaw("Vertical");
        }

        Vector3 manualInput = new Vector3(x, 0, z).normalized;
        moveInput = (manualInput != Vector3.zero) ? manualInput : (useAutoMode ? GetAutoDodgeVector() : Vector3.zero);

        if (moveInput != Vector3.zero)
            transform.forward = Vector3.Lerp(transform.forward, moveInput, 10f * Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (isDead) return;
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    // --- 충돌 및 데미지 ---
    void OnCollisionStay(Collision collision)
    {
        if (isDead) return;
        if (collision.collider.CompareTag("Enemy"))
        {
            if (Time.time > lastDamageTime + damageCooldown)
            {
                TakeDamage(10);
                lastDamageTime = Time.time;
            }
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthUI();
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        isDead = true;

        // ⭐ 추가: 게임 매니저에게 저장 요청
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.GameOverSave();
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // --- 레벨업 시스템 ---
    public void AddExp(int amount)
    {
        currentExp += amount;
        if (currentExp >= maxExp) LevelUp();
        UpdateExpUI();
    }

    void LevelUp()
    {
        currentExp -= maxExp;
        level++;
        maxExp += 50;

        // ⭐ 오토 모드면 매니저에게 랜덤 선택 요청, 아니면 UI 띄우기 요청
        if (levelUpManager != null)
        {
            levelUpManager.ShowLevelUpWindow(isAutoLevelUp);
        }

        UpdateExpUI();
    }

    // ⭐ 통합 업그레이드 함수 (매니저가 호출함)
    public void ApplyUpgrade(int upgradeType)
    {
        switch (upgradeType)
        {
            case 0: // 이동 속도
                moveSpeed += 1f;
                Debug.Log("이동 속도 증가!");
                break;
            case 1: // 공격 속도
                if (weapon != null) weapon.attackRate *= 0.9f;
                Debug.Log("공격 속도 증가!");
                break;
            case 2: // 공격력 (New!)
                damageMultiplier += 0.2f; // 20% 증가
                Debug.Log("공격력 증가!");
                break;
            case 3: // 최대 체력 & 회복 (New!)
                maxHealth += 20;
                currentHealth = maxHealth; // 체력 회복
                UpdateHealthUI();
                Debug.Log("최대 체력 증가 & 회복!");
                break;
            case 4: // 자석 범위 (New!)
                magnetRange += 2f;
                Debug.Log("자석 범위 증가!");
                break;
        }
    }

    // --- 기타 기능 ---
    Vector3 GetAutoDodgeVector()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        if (enemies.Length == 0) return Vector3.zero;
        Vector3 flee = Vector3.zero;
        foreach (var e in enemies)
        {
            Vector3 diff = transform.position - e.transform.position;
            diff.y = 0;
            flee += diff.normalized / (diff.magnitude + 0.1f);
        }
        return flee.normalized;
    }

    public void RetryGame() { Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void SetAutoLevelUp(bool isOn) { isAutoLevelUp = isOn; }
    void UpdateHealthUI() { if (hpSlider != null) hpSlider.value = (float)currentHealth / maxHealth; }
    void UpdateExpUI() { if (expSlider != null) expSlider.value = (float)currentExp / maxExp; }
}