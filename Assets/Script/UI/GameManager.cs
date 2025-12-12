using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public Text timerText;
    public Text killText;
    public Text bestScoreText; // ⭐ 추가: 최고 기록 보여줄 텍스트

    private float survivalTime;
    private bool isLive = true;
    private int killCount = 0;

    // 이 씬에서 관리하는 최고 기록
    private int bestScore = 0;

    void Start()
    {
        killCount = 0;

        // ⭐ 1. 저장된 최고 기록 불러오기 (없으면 0)
        bestScore = PlayerPrefs.GetInt("BestKill", 0);
        UpdateKillUI();
    }

    void Update()
    {
        if (!isLive) return;

        survivalTime += Time.deltaTime;
        int min = (int)(survivalTime / 60);
        int sec = (int)(survivalTime % 60);

        if (timerText != null)
        {
            timerText.text = string.Format("{0:D2}:{1:D2}", min, sec);
        }
    }

    public void AddKill()
    {
        killCount++;
        UpdateKillUI();
    }

    void UpdateKillUI()
    {
        if (killText != null)
        {
            killText.text = "☠️ " + killCount;
        }

        // ⭐ 2. 최고 기록 UI 갱신 (현재 점수가 더 높으면 실시간 갱신 효과)
        if (bestScoreText != null)
        {
            int displayScore = Mathf.Max(killCount, bestScore);
            bestScoreText.text = "🏆 Best: " + displayScore;
        }
    }

    // ⭐ 3. 게임 오버 시 저장하는 함수 (PlayerController에서 부를 예정)
    public void GameOverSave()
    {
        isLive = false;

        // 현재 기록이 최고 기록보다 높으면 저장
        if (killCount > bestScore)
        {
            PlayerPrefs.SetInt("BestKill", killCount); // 내 기기에 'BestKill'이라는 이름으로 숫자 저장
            PlayerPrefs.Save(); // 확실하게 저장
            Debug.Log("🎉 신기록 달성! 저장 완료!");
        }
    }
}