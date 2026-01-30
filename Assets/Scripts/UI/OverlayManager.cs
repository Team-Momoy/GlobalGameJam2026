using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class OverlayManager : MonoBehaviour
{
    public static OverlayManager Instance;

    public bool IsOverlayClear => !overlay.enabled || overlay.color.a <= 0f;

    [SerializeField]
    private Image overlay;

    [SerializeField]
    private Color overlayColor;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void FadeToBlack(float alpha = 1f, float duration = 0.15f)
    {
        overlay.enabled = true;

        DOTween.Kill(overlay);
        overlay.DOColor(new Color(overlayColor.r, overlayColor.g, overlayColor.b, alpha), duration).SetEase(Ease.InOutSine);
    }

    public void Clear(float duration = 0.15f)
    {
        DOTween.Kill(overlay);
        overlay.DOColor(Color.clear, duration).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            overlay.enabled = false;
        });
    }
}
