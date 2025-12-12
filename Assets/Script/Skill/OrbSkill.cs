using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OrbSkill : MonoBehaviour
{
    [Header("Skill Settings")]
    public GameObject orbPrefab;     // ⭐ 추격형 Orb 프리팹 (ChasingOrb 스크립트 붙은 거)
    public int orbCount = 5;         // 소환할 개수
    public float duration = 30f;     // 지속 시간
    public float cooldownTime = 60f; // 쿨타임

    [Header("UI")]
    public Image skillButtonImage;
    public Button uiButton;

    private bool isActive = false;
    private bool isCooldown = false;
    private float activeTimer = 0f;
    private float cooldownTimer = 0f;

    // 소환된 Orb들을 관리할 리스트
    private List<GameObject> spawnedOrbs = new List<GameObject>();

    void Start()
    {
        if (skillButtonImage != null) skillButtonImage.fillAmount = 0;
    }

    void Update()
    {
        // 지속 시간 체크
        if (isActive)
        {
            activeTimer -= Time.deltaTime;
            if (activeTimer <= 0) DeactivateSkill();
        }

        // 쿨타임 체크
        if (isCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (skillButtonImage != null) skillButtonImage.fillAmount = cooldownTimer / cooldownTime;

            if (cooldownTimer <= 0)
            {
                isCooldown = false;
                if (uiButton != null) uiButton.interactable = true;
            }
        }
    }

    public void OnSkillButton()
    {
        if (isActive || isCooldown) return;
        ActivateSkill();
    }

    void ActivateSkill()
    {
        isActive = true;
        activeTimer = duration;

        // ⭐ Orb 여러 개 소환!
        for (int i = 0; i < orbCount; i++)
        {
            // 플레이어 주변 랜덤한 위치에 생성
            Vector3 spawnPos = transform.position + Random.insideUnitSphere * 2f;
            GameObject orb = Instantiate(orbPrefab, spawnPos, Quaternion.identity);
            spawnedOrbs.Add(orb);
        }

        if (uiButton != null) uiButton.interactable = false;
        Debug.Log("🚀 추격형 비트 가동!");
    }

    void DeactivateSkill()
    {
        isActive = false;

        // ⭐ 소환했던 Orb들 모두 삭제
        foreach (GameObject orb in spawnedOrbs)
        {
            if (orb != null) Destroy(orb);
        }
        spawnedOrbs.Clear();

        StartCooldown();
    }

    void StartCooldown()
    {
        isCooldown = true;
        cooldownTimer = cooldownTime;
        if (skillButtonImage != null) skillButtonImage.fillAmount = 1;
    }
}