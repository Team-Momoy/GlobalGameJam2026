using DG.Tweening;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField]
    private AudioClip mouseEnterSFX, mouseExitSFX, mouseDownSFX;

    [SerializeField]
    private UnityEvent onClick;

    [SerializeField]
    private MMF_Player mMF_Player;

    private string _instanceId;

    void Awake()
    {
        _instanceId = GetInstanceID().ToString();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (mouseDownSFX != null)
        {
            MMSoundManager.Instance.PlaySound(mouseDownSFX, MMSoundManager.MMSoundManagerTracks.Sfx, Vector3.zero);
        }

        DOTween.Kill(transform);

        if (mMF_Player != null)
        {
            mMF_Player.PlayFeedbacks();
        }

        onClick?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (mouseEnterSFX != null)
        {
            MMSoundManager.Instance.PlaySound(mouseEnterSFX, MMSoundManager.MMSoundManagerTracks.Sfx, Vector3.zero);
        }

        transform.DOScale(Vector3.one * 1.2f, 0.2f).SetEase(Ease.OutBack).SetId($"ButtonScaling{_instanceId}").SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (mouseExitSFX != null)
        {
            MMSoundManager.Instance.PlaySound(mouseExitSFX, MMSoundManager.MMSoundManagerTracks.Sfx, Vector3.zero);
        }

        DOTween.Kill($"ButtonScaling{_instanceId}");
        transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack, 3f).SetId($"ButtonScaling{_instanceId}").SetUpdate(true);
    }
    
    public void ResetScale()
    {
        DOTween.Kill($"ButtonScaling{_instanceId}");
        transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack, 3f).SetId($"ButtonScaling{_instanceId}").SetUpdate(true);
    }
}
