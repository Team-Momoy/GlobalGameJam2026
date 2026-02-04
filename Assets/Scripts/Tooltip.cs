using DG.Tweening;
using Febucci.UI;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private TextAnimator_TMP[] textTMPs;

    public void Open()
    {
        DOTween.Kill(transform, true);
        transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
        textTMPs.ForEach(textTmp =>
        {
            textTmp.ResetState();
        });
    }

    public void Close()
    {
        DOTween.Kill(transform);
        transform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.InQuad);
    }
}
