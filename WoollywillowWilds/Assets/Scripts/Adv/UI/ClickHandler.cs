using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ClickHandler : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent onClickEvent;
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        onClickEvent.Invoke();
    }
}
