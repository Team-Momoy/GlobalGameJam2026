using DG.Tweening;
using UnityEngine;

public class TimeDialUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform dialHand;

    [SerializeField]
    private float startAngle, endAngle;

    [SerializeField]
    private RectTransform closedSign;

    void Start()
    {
        TimeManager.Instance.OnTimeChanged += UpdateClockHand;
        TimeManager.Instance.OnDayEnded += ShowClosedSign;
    }

    void OnDisable()
    {
        TimeManager.Instance.OnTimeChanged -= UpdateClockHand;
        TimeManager.Instance.OnDayEnded -= ShowClosedSign;
    }

    private void UpdateClockHand(float remainingTime)
    {
        float angle = Mathf.Lerp(startAngle, endAngle, TimeManager.Instance.RemainingFraction);
        dialHand.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void ShowClosedSign()
    {
        closedSign.gameObject.SetActive(true);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(closedSign.DOAnchorPosX(0, 0.2f));
        sequence.Join(closedSign.DORotate(new Vector3(0, 0, -45), 0.4f).SetDelay(0.15f).SetLoops(2, LoopType.Yoyo));
        sequence.Append(closedSign.DORotate(new Vector3(0, 0, 30), 0.35f).SetLoops(2, LoopType.Yoyo));
        sequence.Append(closedSign.DORotate(new Vector3(0, 0, -20), 0.3f).SetLoops(2, LoopType.Yoyo));
        sequence.Append(closedSign.DORotate(new Vector3(0, 0, 10), 0.25f).SetLoops(2, LoopType.Yoyo));
        sequence.Append(closedSign.DORotate(new Vector3(0, 0, -5), 0.20f));
        sequence.Append(closedSign.DORotate(new Vector3(0, 0, 0), 0.35f).SetEase(Ease.OutSine));
    }

}
