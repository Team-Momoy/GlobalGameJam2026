using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour
{
    [SerializeField]
    protected SpriteRenderer spriteRenderer;
    protected MaterialPropertyBlock materialPropertyBlock;

    [SerializeField]
    private bool hasOutline;
    public bool HasOutline => hasOutline;

    [SerializeField]
    private Tooltip tooltip;

    InputActions inputActions;

    [SerializeField] public UnityEvent onClick;
    [SerializeField] protected UnityEvent onHoverEnter;
    [SerializeField] protected UnityEvent onHoverExit;

    protected virtual void Awake()
    {
        materialPropertyBlock = new MaterialPropertyBlock();
        if(tooltip != null){
            onHoverEnter.AddListener(() => tooltip.Open());
            onHoverExit.AddListener(() => tooltip.Close());
        }
    }

    public void SetOutline(float value)
    {
        SetFloat("_OutlineAlpha", value);
    }

    protected void SetFloat(string property, float value)
    {
        spriteRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetFloat(property, value);
        spriteRenderer.SetPropertyBlock(materialPropertyBlock);
    }

    private bool isHovered;

    public bool IsHovered { 
        get { return isHovered; }
        set 
        {
            if (isHovered == value) return;
            isHovered = value;
            if (isHovered)
            {
                onHoverEnter?.Invoke();
            }
            else
            {
                onHoverExit?.Invoke();
            }
        }
    }

    void OnEnable()
    {
        //Setup unity input system events
        inputActions = new InputActions();
        inputActions.UI.Click.started += OnClick;
        inputActions.Enable();
    }

    void OnDisable()
    {
        if (inputActions != null)
        {
            inputActions.UI.Click.started -= OnClick;
            inputActions.Disable();
            inputActions = null;
        }
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        //Invoke the onClick event only if the mouse is hovering over this interactable
        if (!isHovered || FindAnyObjectByType<CardManager>().IsDealing) return;
        onClick?.Invoke();
    }
}
