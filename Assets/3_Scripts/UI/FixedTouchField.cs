using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class FixedTouchField : MonoBehaviour, IEndDragHandler, IDragHandler
{
    public Vector2 dragDir;
    private Vector2 screenDragStartPos, screenDragCurrPos;

    public event Action OnScreenDrag, OnScreenDragEnd;
    public void OnDrag(PointerEventData eventData)
    {
        OnScreenDrag?.Invoke();

        screenDragCurrPos = eventData.position;
        dragDir = (screenDragStartPos - screenDragCurrPos).normalized;
        
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        OnScreenDragEnd?.Invoke();

        dragDir = Vector2.zero;
    }
}
