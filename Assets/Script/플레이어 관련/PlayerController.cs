using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("AI Auto Dodge")]
    public bool useAutoMode = true;
    public float detectionRadius = 5f;
    public LayerMask enemyLayer;

    [Header("UI Controls")]
    public VirtualJoystick virtualJoystick;

    [Header("Stats")]
    public int maxHealth = 100;
    public int currentHealth;
    public float damageCooldown = 0.5f;
    private float lastDamageTime;

    [Header("Experience")]
    public int level = 1;
    public int currentExp = 0;
    public int maxExp = 100;
    public bool isAutoLevelUp = false;

    [Header("UI")]
    public GameObject levelUpPanel;
    public GameObject gameOverPanel;
    public Slider expSlider;
    public Slider hpSlider;
    public Text levelText;

    private Rigidbody rb; // 3D Rigidbody
    private Vector3 moveInput; // Vector3
    private WeaponController weapon;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // 3D 컴포넌트 가져오기
        weapon = GetComponent<WeaponController>();

        currentHealth = maxHealth;
        UpdateExpUI();
        UpdateHealthUI();
    }

    void Update()
    {
        if (isDead) return;

        // 1. 입력 받기
        float x = 0;
        float z = 0; // Y 대신 Z 사용

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

        // 2. 이동 결정 (수동 vs 오토)
        if (manualInput != Vector3.zero)
        {
            moveInput = manualInput;
        }
        else if (useAutoMode)
        {
            moveInput = GetAutoDodgeVector();
        }
        else
        {
            moveInput = Vector3.zero;
        }

        // 3. 캐릭터 회전 (이동 방향 바라보기)
        if (moveInput != Vector3.zero)
        {
            transform.forward = Vector3.Lerp(transform.forward, moveInput, 10f * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;
        // 3D 물리 이동
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    // 3D 충돌 감지 (OnTriggerEnter)
    void OnTriggerStay(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Enemy"))
        {
            if (Time.time > lastDamageTime + damageCooldown)
            {
                TakeDamage(10);
                lastDamageTime = Time.time;
            }
        }
    }

    // --- AI 로직 (3D 버전) ---
    Vector3 GetAutoDodgeVector()
    {
        // 3D에서는 구(Sphere) 형태로 주변 감지
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);

        if (enemies.Length == 0) return Vector3.zero;

        Vector3 fleeDirection = Vector3.zero;

        foreach (Collider enemy in enemies)
        {
            Vector3 directionToMe = transform.position - enemy.transform.position;
            directionToMe.y = 0; // 높이 차이는 무시

            fleeDirection += directionToMe.normalized / (directionToMe.magnitude + 0.1f);
        }

        return fleeDirection.normalized;
    }

    // 범위 그리기 (디버깅용)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    // --- (이하 기존 로직 유지) ---
    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthUI();
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetAutoLevelUp(bool isOn) { isAutoLevelUp = isOn; }

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

        if (isAutoLevelUp)
        {
            int randomChoice = Random.Range(0, 2);
            if (randomChoice == 0) UpgradeSpeed();
            else UpgradeAttack();
        }
        else
        {
            if (levelUpPanel != null) { levelUpPanel.SetActive(true); Time.timeScale = 0f; }
        }
        UpdateExpUI();
    }

    public void UpgradeSpeed() { moveSpeed += 1f; CloseLevelUpPanel(); }
    public void UpgradeAttack() { if (weapon != null) weapon.attackRate *= 0.9f; CloseLevelUpPanel(); }
    void CloseLevelUpPanel() { levelUpPanel.SetActive(false); Time.timeScale = 1f; }
    void UpdateHealthUI() { if (hpSlider != null) hpSlider.value = (float)currentHealth / maxHealth; }
    void UpdateExpUI() { if (expSlider != null) expSlider.value = (float)currentExp / maxExp; }
}