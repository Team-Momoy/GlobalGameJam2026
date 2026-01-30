using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableElement : InteractableElement
{
    protected bool canBeDragged;

    protected Vector3 dragOffset;

    protected override bool canDisableOutline => !isBeingDragged;

    // Virtual property that derived classes can override
    public virtual bool CanBeDragged
    {
        get => canBeDragged;
        set => canBeDragged = value;
    }

    [SerializeField]
    protected bool isBeingDragged;

    public bool IsBeingDragged => isBeingDragged;

    public override void OnPointerDown(PointerEventData eventData)
    {
        DOTween.Kill(transform);
        base.OnPointerDown(eventData);
        isBeingDragged = true;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        worldPos.z = transform.position.z;
        dragOffset = transform.position - worldPos;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        isBeingDragged = false;

        if (!isMouseOver)
        {
            DisableOutline();
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if(isBeingDragged) EnableOutline();
    }
}
