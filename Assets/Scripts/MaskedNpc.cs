using System;
using System.Collections;
using DG.Tweening;
using Sirenix.Utilities;
using UnityEngine;

public class MaskedNpc : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Card impostorCardPrefab;
    [SerializeField] private Transform cardPlayingPoint;
    public Transform CardPlayingPoint => cardPlayingPoint;

    [SerializeField] private Transform cardSpawnPoint;

    [SerializeField] public SpriteRenderer maskRenderer;

    [SerializeField] public Sprite maskDestroyedSprite;


    [SerializeField] private SpriteRenderer faceRenderer;
    [SerializeField] private Sprite demon;

    private bool isImpostor = false;
    public bool IsImpostor {
        get { return isImpostor; }
        set {
            isImpostor = value;
            if (value)
            {
                faceRenderer.sprite = demon;
            }
        }
    }


    private MaterialPropertyBlock materialPropertyBlock;

    void Awake()
    {
        if (maskRenderer != null)
        {
            materialPropertyBlock = new MaterialPropertyBlock();
        }
    }

    // Returns the created card and the sequence driving its play animation so callers can wait for completion
    public (Card card, Sequence sequence) PlayCard(bool isReal)
    {
        Card card = Instantiate(isImpostor && isReal ? impostorCardPrefab : cardPrefab, cardSpawnPoint.position, Quaternion.identity, null);

        card.transform.rotation = Quaternion.Euler(180, cardPlayingPoint.eulerAngles.y, card.transform.eulerAngles.z);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(card.transform.DOMoveY(transform.position.y + 7f, 0.75f).SetEase(Ease.OutBack));
        sequence.Append(card.transform.DOMove(cardPlayingPoint.position + Vector3.up * 7, 0.5f).SetEase(Ease.OutBack));
        sequence.Append(card.transform.DORotate(new Vector3(-70f, cardPlayingPoint.eulerAngles.y, cardPlayingPoint.eulerAngles.z), 0.5f, RotateMode.Fast).SetEase(Ease.OutQuad));
        sequence.AppendCallback(() =>
        {
            card.FrontRenderer.sortingOrder = 1;
            card.BackRenderer.sortingOrder = 1;
        });
        sequence.Append(card.transform.DOMoveY(cardPlayingPoint.position.y, 1f).SetEase(Ease.OutQuad));
        sequence.Join(card.transform.DORotate(new Vector3(-90f, cardPlayingPoint.eulerAngles.y, cardPlayingPoint.eulerAngles.z), 0.20f, RotateMode.Fast).SetDelay(0.8f).SetEase(Ease.OutQuad));

        return (card, sequence);
    }

    public void DestroyMask()
    {
        StartCoroutine(AnimateHitEffect());
    }

    private IEnumerator AnimateHitEffect()
    {
        maskRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetFloat("_HitEffectBlend", 1f);
        maskRenderer.SetPropertyBlock(materialPropertyBlock);

        yield return new WaitForSeconds(0.1f);
        
        maskRenderer.sprite = maskDestroyedSprite;
        faceRenderer.gameObject.SetActive(true);

        maskRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetFloat("_HitEffectBlend", 0f);
        maskRenderer.SetPropertyBlock(materialPropertyBlock);

        yield return new WaitForSeconds(0.75f);

        maskRenderer.transform.DOMoveY(transform.position.y - 10f, 1f).SetEase(Ease.InQuad);

        yield return new WaitForSeconds(1.25f);

        FindAnyObjectByType<EndGameScreen>().EndGame(isImpostor);
    }
}
