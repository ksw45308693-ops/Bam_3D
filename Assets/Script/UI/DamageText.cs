using UnityEngine;
using UnityEngine.UI; // Text 사용

public class DamageText : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float alphaSpeed = 2f;
    public float destroyTime = 1f;

    public Text textComponent; // 자식에 있는 Text 연결
    private Color alpha;

    void Start()
    {
        // 텍스트 컴포넌트 자동 찾기 (없으면 수동 연결)
        if (textComponent == null)
            textComponent = GetComponentInChildren<Text>();

        alpha = textComponent.color;

        // 일정 시간 뒤 자동 삭제
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        // 1. 위로 천천히 올라가기
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        // 2. 투명해지기 (Alpha 값 감소)
        alpha.a = Mathf.Lerp(alpha.a, 0, alphaSpeed * Time.deltaTime);
        textComponent.color = alpha;

        // 3. (선택) 카메라 바라보기 (빌보드 효과)
        // 3D라서 카메라가 비스듬히 있으면 글자가 찌그러져 보일 수 있음
        // transform.rotation = Camera.main.transform.rotation;
    }

    // 외부에서 데미지 숫자를 설정하는 함수
    public void SetDamage(int damage)
    {
        if (textComponent == null)
            textComponent = GetComponentInChildren<Text>();

        textComponent.text = damage.ToString();
    }
}