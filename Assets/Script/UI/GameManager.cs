using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public Text timerText;
    public Text killText; // ⭐ 추가: 킬수 텍스트 연결

    private float survivalTime;
    private bool isLive = true;
    private int killCount = 0; // ⭐ 추가: 내부 카운트 변수

    void Start()
    {
        // 시작할 때 0으로 초기화
        killCount = 0;
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

    // ⭐ 외부(적)에서 호출할 함수: 처치 수 1 증가
    public void AddKill()
    {
        killCount++;
        UpdateKillUI();
    }

    // UI 갱신 함수
    void UpdateKillUI()
    {
        if (killText != null)
        {
            // 원하는 형식으로 표시 (예: "☠️ 150")
            killText.text = "Kill :  " + killCount.ToString();
        }
    }
}