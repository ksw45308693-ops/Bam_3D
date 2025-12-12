using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToggleEffect : MonoBehaviour
{
    [Header("Settings")]
    public Toggle targetToggle;      // 제어할 토글 컴포넌트
    public Image targetImage;        // 색상을 바꿀 이미지 (보통 Background)

    [Header("Colors")]
    public Color offColor = Color.white;  // 꺼졌을 때 색상
    public Color onColor = Color.green;   // 켜졌을 때 색상

    [Header("Effect")]
    public bool useBlinking = false;      // 반짝임 효과 사용 여부
    public float blinkSpeed = 0.5f;       // 반짝이는 속도

    private Coroutine blinkCoroutine;

    void Start()
    {
        // 1. 시작할 때 현재 토글 상태에 맞춰 초기화
        if (targetToggle == null) targetToggle = GetComponent<Toggle>();
        if (targetImage == null) targetImage = targetToggle.targetGraphic as Image;

        // 2. 토글 값이 바뀔 때 실행될 함수 연결
        targetToggle.onValueChanged.AddListener(OnToggleValueChanged);

        // 3. 초기 상태 적용
        OnToggleValueChanged(targetToggle.isOn);
    }

    // 토글 값이 변경될 때 호출되는 함수
    void OnToggleValueChanged(bool isOn)
    {
        // 기존에 실행 중인 반짝임이 있다면 중지
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);

        if (isOn)
        {
            // 켜졌을 때: 반짝임 효과가 켜져 있으면 코루틴 실행, 아니면 그냥 색상 변경
            if (useBlinking)
            {
                blinkCoroutine = StartCoroutine(BlinkRoutine());
            }
            else
            {
                targetImage.color = onColor;
            }
        }
        else
        {
            // 꺼졌을 때: 기본 색상으로 복귀
            targetImage.color = offColor;
        }
    }

    // 반짝이는 효과를 주는 코루틴
    IEnumerator BlinkRoutine()
    {
        while (true)
        {
            targetImage.color = onColor;
            yield return new WaitForSeconds(blinkSpeed);
            targetImage.color = offColor; // 또는 흰색 등 원하는 반짝임 교차 색상
            yield return new WaitForSeconds(blinkSpeed);
        }
    }
}