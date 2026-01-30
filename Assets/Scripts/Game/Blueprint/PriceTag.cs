using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;

public class PriceTag : SerializedMonoBehaviour
{
    [SerializeField]
    private SpriteRenderer tagRenderer;

    [SerializeField]
    private Dictionary<int, Sprite> tagsByPrice;

    [SerializeField]
    private ParticleSystem glowingStarPS;

    [SerializeField]
    private SpriteRenderer[] ropeStrings;

    [SerializeField]
    private TextMeshPro priceTMP;

    [SerializeField]
    private MMF_Player mMF_Player;

    private int price;

    void Awake()
    {
        mMF_Player.Initialization();
        Reset();
    }

    private void Reset()
    {
        var c = priceTMP.color;
        c.a = 0f;
        priceTMP.color = c;

        tagRenderer.color = Color.clear;
        tagRenderer.transform.localEulerAngles = Vector3.forward * 20;
        ropeStrings.ForEach(el =>
        {
            el.color = Color.clear;
        });
    }

    [Button("Show")]
    public void Show()
    {
        var c = priceTMP.color;
        c.a = 1f;
        priceTMP.DOColor(c, 0f).SetEase(Ease.OutQuad);

        tagRenderer.DOColor(Color.white, 0.25f).SetEase(Ease.OutQuad);
        tagRenderer.transform.DOLocalRotate(Vector3.forward * -10, 0.25f).SetEase(Ease.OutBack);
        ropeStrings.ForEach(el =>
        {
            el.DOColor(Color.white, 0.25f).SetEase(Ease.OutQuad);
        });
        
    }

    public void Hide()
    {
        var c = priceTMP.color;
        c.a = 0f;
        priceTMP.DOColor(c, 0f).SetEase(Ease.InQuad);

        tagRenderer.DOColor(Color.clear, 0f).SetEase(Ease.InQuad);
        tagRenderer.transform.DOLocalRotate(Vector3.forward * 20, 0.25f).SetEase(Ease.InBack);
        ropeStrings.ForEach(el =>
        {
            el.DOColor(Color.clear, 0.25f).SetEase(Ease.InQuad);
        });
    }

    [Button("Upgrade")]
    public void Upgrade(int newPrice, bool isIdealWeight)
    {
        price += newPrice;

        mMF_Player.PlayFeedbacks();
        priceTMP.transform.DOKill(true);
        priceTMP.transform.DOScale(priceTMP.transform.localScale.x * 1.35f, 0.15f).SetEase(Ease.InOutQuad).SetLoops(2, LoopType.Yoyo);
        
        priceTMP.text = $"{price}$";
        tagRenderer.sprite = GetTagForPrice(price);

        if(isIdealWeight) {
            glowingStarPS.Play();
        } else {
            glowingStarPS.Stop();  
        } 
    }

    private Sprite GetTagForPrice(int price)
    {
        Sprite result = null;
        int bestKey = int.MinValue;

        foreach (var kvp in tagsByPrice)
        {
            if (kvp.Key <= price && kvp.Key > bestKey)
            {
                bestKey = kvp.Key;
                result = kvp.Value;
            }
        }

        return result;
    }

    // protected void StartShine()
    // {
    //     tagRenderer.GetPropertyBlock(materialPropertyBlock);
    //     materialPropertyBlock.SetFloat("_ShineLocation", 0.4f);
    //     tagRenderer.SetPropertyBlock(materialPropertyBlock);

    //     Sequence shineLoop = DOTween.Sequence();

    //     // Tween from 0.4 → 0.7 over 0.25s
    //     shineLoop.Append(DOTween.To(
    //     () =>
    //     {
    //         tagRenderer.GetPropertyBlock(materialPropertyBlock);
    //         return materialPropertyBlock.GetFloat("_ShineLocation");
    //     },
    //     x =>
    //     {
    //         materialPropertyBlock.SetFloat("_ShineLocation", x);
    //         tagRenderer.SetPropertyBlock(materialPropertyBlock);
    //     },
    //     0.7f,
    //     0.75f)
    //     .SetEase(Ease.InQuad));
        
    //     shineLoop.AppendInterval(1f);

    //     shineLoop.SetLoops(-1);
    //     shineLoop.SetId("TagShine");
    // }
}
