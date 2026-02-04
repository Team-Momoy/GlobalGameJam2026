using DG.Tweening;
using Sirenix.Utilities;
using UnityEngine;

public class Card : Interactable
{
    [SerializeField]
    private bool isImpostorCard = false;
    public bool IsImpostorCard => isImpostorCard;

    [SerializeField]
    private SpriteRenderer frontRenderer, backRenderer;
    public SpriteRenderer FrontRenderer => frontRenderer;
    public SpriteRenderer BackRenderer => backRenderer;

    // protected MaterialPropertyBlock materialPropertyBlock;

    // protected virtual void Awake()
    // {
    //     materialPropertyBlock = new MaterialPropertyBlock();
    // }

    // public void SetOutline(float value)
    // {
    //     SetFloat("_OutlineAlpha", value);
    // }

    // protected void SetFloat(string property, float value)
    // {
    //     frontRenderer.GetPropertyBlock(materialPropertyBlock);
    //     materialPropertyBlock.SetFloat(property, value);
    //     frontRenderer.SetPropertyBlock(materialPropertyBlock);
    // }

    public void ClearOnClickListeners()
    {
        onClick.RemoveAllListeners();
    }

    public void MarkAsPlayed()
    {
        SetOutline(1f);
    }
}
