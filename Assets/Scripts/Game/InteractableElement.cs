using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InteractableElement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    protected SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock materialPropertyBlock;

    protected virtual bool canDisableOutline => true;

    protected bool isMouseOver;

    [FoldoutGroup("Pointer Events")]
    [SerializeField]
    protected UnityEvent onPointerDown, onPointerUp, onPointerEnter, onPointerExit;


    protected virtual void Awake()
    {
        if (spriteRenderer != null)
        {
            materialPropertyBlock = new MaterialPropertyBlock();
        }
    }

    protected void EnableOutline()
    {
        spriteRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetFloat("_OutlineAlpha", 1);
        spriteRenderer.SetPropertyBlock(materialPropertyBlock);
    }

    public void DisableOutline()
    {
        spriteRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetFloat("_OutlineAlpha", 0);
        spriteRenderer.SetPropertyBlock(materialPropertyBlock);
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        onPointerDown?.Invoke();
    }
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        onPointerUp?.Invoke();
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
        onPointerEnter?.Invoke();
        EnableOutline();
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        onPointerExit?.Invoke();
        DisableOutline();
    }

}
