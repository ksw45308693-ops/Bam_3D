using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; // 리스트 사용을 위해 필수

public class LevelUpManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject levelUpPanel;
    public Button[] optionButtons; // 인스펙터에서 버튼들을 꼭 다 채워주세요!
    public Text[] optionTexts;     // 텍스트들도 개수를 맞춰주세요!

    private PlayerController player;

    private string[] upgradeNames = {
        "이동 속도 UP", "공격 속도 UP", "공격력 UP", "체력 회복", "자석 범위 UP"
    };

    private int[] assignedUpgrades;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        levelUpPanel.SetActive(false);

        // 버튼 개수만큼 배열 생성 (안전장치)
        if (optionButtons != null)
        {
            assignedUpgrades = new int[optionButtons.Length];
        }
    }

    public void ShowLevelUpWindow(bool isAuto)
    {
        // ⭐ [중복 방지] 덱 만들기 (0, 1, 2, 3, 4)
        List<int> deck = new List<int>();
        for (int i = 0; i < upgradeNames.Length; i++)
        {
            deck.Add(i);
        }

        // 버튼 개수만큼 반복
        for (int i = 0; i < optionButtons.Length; i++)
        {
            // 더 이상 뽑을 카드가 없으면 중단
            if (deck.Count == 0) break;

            // 덱에서 랜덤으로 하나 뽑기
            int randomIndex = Random.Range(0, deck.Count);
            int selectedUpgrade = deck[randomIndex];

            // 뽑은 카드는 덱에서 제거 (중복 방지)
            deck.RemoveAt(randomIndex);

            // 해당 버튼에 능력 할당
            assignedUpgrades[i] = selectedUpgrade;

            // 텍스트 갱신 (배열 범위 확인)
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
            levelUpPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void SelectOption(int buttonIndex)
    {
        // ⭐ [에러 방지] 버튼 번호가 배열 범위를 넘어가면 무시 (게임 멈춤 방지)
        if (assignedUpgrades == null || buttonIndex < 0 || buttonIndex >= assignedUpgrades.Length)
        {
            Debug.LogWarning($"버튼 설정 오류! 입력된 번호: {buttonIndex}, 현재 배열 크기: {(assignedUpgrades != null ? assignedUpgrades.Length : 0)}");
            // 인스펙터의 Option Buttons에 버튼을 모두 연결했는지 확인하세요!
            return;
        }

        if (player != null)
        {
            int upgradeType = assignedUpgrades[buttonIndex];
            player.ApplyUpgrade(upgradeType);
        }

        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}