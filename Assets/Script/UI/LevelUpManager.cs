using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelUpManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject levelUpPanel;
    public Button[] optionButtons;
    public Text[] optionTexts;

    private PlayerController player;

    private string[] upgradeNames = {
        "이동 속도 UP", "공격 속도 UP", "공격력 UP", "최대 체력 UP", "자석 범위 UP"
    };

    private int[] assignedUpgrades;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        if (levelUpPanel != null) levelUpPanel.SetActive(false);

        // 안전장치: 버튼 배열이 비어있으면 에러 로그 띄우고 중단 (게임 멈춤 방지)
        if (optionButtons == null || optionButtons.Length == 0)
        {
            Debug.LogError("🚨 LevelUpManager: 버튼이 연결되지 않았습니다! 인스펙터를 확인하세요.");
            return;
        }

        // 버튼 개수만큼 배열 생성
        assignedUpgrades = new int[optionButtons.Length];
    }

    public void ShowLevelUpWindow(bool isAuto)
    {
        if (optionButtons == null || optionButtons.Length == 0) return;

        // 중복 방지 덱 생성
        List<int> deck = new List<int>();
        for (int i = 0; i < upgradeNames.Length; i++) deck.Add(i);

        for (int i = 0; i < optionButtons.Length; i++)
        {
            // 카드가 부족하면 멈춤
            if (deck.Count == 0) break;

            int randomIndex = Random.Range(0, deck.Count);
            int selectedUpgrade = deck[randomIndex];
            deck.RemoveAt(randomIndex);

            // 배열 범위 안전 체크
            if (assignedUpgrades != null && i < assignedUpgrades.Length)
            {
                assignedUpgrades[i] = selectedUpgrade;
            }

            // 텍스트 갱신 (텍스트 배열이 버튼보다 짧아도 에러 안 나게 처리)
            if (optionTexts != null && i < optionTexts.Length && optionTexts[i] != null)
            {
                optionTexts[i].text = upgradeNames[selectedUpgrade];
            }
        }

        if (isAuto)
        {
            SelectOption(0);
        }
        else
        {
            if (levelUpPanel != null) levelUpPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void SelectOption(int buttonIndex)
    {
        // ⭐ [에러 원천 차단] 
        // 배열이 안 만들어졌거나, 인덱스가 범위를 벗어나면 그냥 무시함 (에러 안 띄움)
        if (assignedUpgrades == null || buttonIndex < 0 || buttonIndex >= assignedUpgrades.Length)
        {
            Debug.LogWarning($"⚠️ 버튼 설정 오류! 입력된 번호: {buttonIndex}. (리스트를 다시 연결해보세요)");
            return;
        }

        if (player != null)
        {
            player.ApplyUpgrade(assignedUpgrades[buttonIndex]);
        }

        if (levelUpPanel != null) levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}