using System.Collections;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;

public class TargetPrefab : MonoBehaviour
{
    // Callback when target is destroyed without being hit correctly
    public static System.Action OnTargetMissed;
    
    [SerializeField]
    private float initialOuterScale = 2f;

    [SerializeField]
    private float scaleToEnableHit = 1.11f;

    [SerializeField]
    private float scaleToDestroy = 0.3f;

    [SerializeField]
    private float speedMultiplier = 1;


    [SerializeField]
    private Collider2D circleCollider;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Animator starBurstAnimator;

    [SerializeField]
    private SpriteRenderer outerCircleRenderer;

    [SerializeField]
    private MMF_Player squashAndStretchMMF;

    [SerializeField]
    private Color normalColor, hitColor;

    private bool hitEnabled = false;

    private bool hasBeenHit = false;

    void Awake()
    {
        outerCircleRenderer.transform.localScale = Vector3.one * initialOuterScale;
    }

    private void Start()
    {
        spriteRenderer.DOColor(normalColor, 0.25f).SetEase(Ease.OutQuad);
        outerCircleRenderer.DOColor(normalColor, 0.25f).SetEase(Ease.OutQuad);

        outerCircleRenderer.transform.DOScale(Vector3.zero, 1.25f / speedMultiplier);
    }

    private void Update()
    {
        if(hasBeenHit) return;

        if(outerCircleRenderer.transform.localScale.x <= scaleToEnableHit)
        {
            outerCircleRenderer.color = hitColor;
            spriteRenderer.color = hitColor;
            hitEnabled = true;
        }

        if(outerCircleRenderer.transform.localScale.x <= scaleToDestroy)
        {
            spriteRenderer.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
            {
               // Target was not hit in time - trigger miss
               if (!hasBeenHit)
               {
                   OnTargetMissed?.Invoke();
               }
               Destroy(gameObject); 
            });
        }
    }

    public bool Hit()
    {
        hasBeenHit = true;
        circleCollider.enabled = false;

        outerCircleRenderer.DOKill(false);

        if (hitEnabled)
        {
            squashAndStretchMMF.PlayFeedbacks();
            
            starBurstAnimator.transform.SetParent(null);
            starBurstAnimator.SetTrigger("Burst");
            Destroy(starBurstAnimator.gameObject, 2f);

            spriteRenderer.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
            {
               Destroy(gameObject);
            });
            return true;
        }
        else
        {
            transform.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
            {
               Destroy(gameObject); 
            });
            return false;
        }
    }

    void OnDisable()
    {
        DOTween.Kill(spriteRenderer);
        DOTween.Kill(outerCircleRenderer);
        DOTween.Kill(transform);
    }
}
