using DG.Tweening;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DraggableUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private AudioClip mouseEnterSFX, mouseDownSFX;

    private bool _isDragging = false;
    private Vector3 _velocity;

    public void OnPointerUp(PointerEventData eventData)
    {
        DOTween.Kill(transform, true);
        _isDragging = false;
        
        transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBounce);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isDragging = true;

        transform.SetAsLastSibling();

        if (mouseDownSFX != null)
        {
            MMSoundManager.Instance.PlaySound(mouseDownSFX, MMSoundManager.MMSoundManagerTracks.Sfx, Vector3.zero);
        }
    }

    void Update()
    {
        if (!_isDragging) return;

        //Drag the object to the mouse position using new input system
        Vector3 mousePosition = Pointer.current.position.ReadValue();

        transform.position = Vector3.SmoothDamp(transform.position, mousePosition, ref _velocity, 0.1f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DOTween.Kill(transform, true);
        
        transform.DOScale(Vector3.one * 1.2f, 0.25f).SetEase(Ease.OutBack);

        if (mouseEnterSFX != null)
        {
            MMSoundManager.Instance.PlaySound(mouseEnterSFX, MMSoundManager.MMSoundManagerTracks.Sfx, Vector3.zero);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isDragging) return;
        
        DOTween.Kill(transform);
        transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
    }
}
