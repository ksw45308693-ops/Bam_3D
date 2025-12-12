using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform bgRect;
    private RectTransform handleRect;

    public Vector2 InputVector { get; private set; }
    private float joystickRange;

    void Start()
    {
        bgRect = GetComponent<RectTransform>();
        handleRect = transform.GetChild(0).GetComponent<RectTransform>();
        joystickRange = bgRect.rect.width * 0.5f; // 반지름
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        // 터치 위치를 UI 좌표로 변환
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgRect, eventData.position, eventData.pressEventCamera, out pos))
        {
            // 이동 범위를 반지름 안으로 제한
            pos = Vector2.ClampMagnitude(pos, joystickRange);
            handleRect.anchoredPosition = pos;

            // 벡터값 계산 (-1 ~ 1)
            InputVector = pos / joystickRange;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        InputVector = Vector2.zero;
        handleRect.anchoredPosition = Vector2.zero;
    }
}