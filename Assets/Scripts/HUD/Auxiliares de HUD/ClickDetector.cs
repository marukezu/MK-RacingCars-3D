using UnityEngine;
using UnityEngine.EventSystems;

public class ClickDetector : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public System.Action onDown;
    public System.Action onUp;
    public System.Action onClick;
    public System.Action onEnter;
    public System.Action onExit;

    public void OnPointerDown(PointerEventData eventData)
    {
        onDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onUp?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onExit?.Invoke();
    }
}
