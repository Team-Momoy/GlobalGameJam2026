using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class HighStrikerMinigame : Minigame, IPointerDownHandler
{
    [SerializeField]
    protected SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock materialPropertyBlock;

    private bool hasPressed = false;

    private float clipValue = 1f;

    void Awake()
    {
        if (spriteRenderer != null)
        {
            materialPropertyBlock = new MaterialPropertyBlock();
        }
        StartCoroutine(Play());
    }

    public override void Reset()
    {
        DOTween.Kill("HighStrikerMinigameTween");
        SetClipping(1f);
        hasPressed = false;
    }

    private void SetClipping(float value)
    {
        spriteRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetFloat("_ClipUvUp", value);
        spriteRenderer.SetPropertyBlock(materialPropertyBlock);

        clipValue = Mathf.InverseLerp(0.2f, 1f, value);
    }

    public override IEnumerator Play()
    {
        DOVirtual.Float(1f, 0.2f, Random.Range(0.7f, 1f), SetClipping)
            .SetEase(Ease.InQuint)
            .SetId("HighStrikerMinigameTween")
            .SetLoops(-1, LoopType.Yoyo);

        yield return null;
    }

    private IEnumerator ButtonPressCoroutine()
    {
        DOTween.Kill("HighStrikerMinigameTween");
        yield return new WaitForSeconds(0.5f);
        if (clipValue <= 0.3f)
        {
            OnGameFinished?.Invoke(true);
        }
        else
        {
            OnGameFinished?.Invoke(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (hasPressed) return;
        hasPressed = true;
        StartCoroutine(ButtonPressCoroutine());
    }
}
