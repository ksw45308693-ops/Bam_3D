using UnityEngine;
using UnityEngine.UI; // UI 사용

public class GameManager : MonoBehaviour
{
    public Text timerText; // 타이머 텍스트 연결
    private float survivalTime;
    private bool isLive = true;

    void Update()
    {
        if (!isLive) return;

        survivalTime += Time.deltaTime; // 시간 더하기

        // 시간 포맷 변경 (분:초)
        int min = (int)(survivalTime / 60);
        int sec = (int)(survivalTime % 60);

        if (timerText != null)
        {
            // "00:00" 형식으로 문자열 만들기
            timerText.text = string.Format("{0:D2}:{1:D2}", min, sec);
        }
    }
}