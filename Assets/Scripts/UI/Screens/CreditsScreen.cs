using DG.Tweening;
using UnityEngine;

public class CreditsScreen : MonoBehaviour
{
    [SerializeField]
    private RectTransform creditsScreen;

    public void Open()
    {
        OverlayManager.Instance.FadeToBlack(0.7f);

        creditsScreen.localRotation = Quaternion.Euler(0, 0, -45);
        creditsScreen.gameObject.SetActive(true);
        creditsScreen.DORotate(Vector3.zero, 0.3f).SetEase(Ease.OutSine);
    }

    public void Close()
    {
        OverlayManager.Instance.Clear();

        creditsScreen.localRotation = Quaternion.Euler(0, 0, 0);
        creditsScreen.DORotate(new Vector3(0, 0, 45), 0.4f).SetEase(Ease.InBack, 4f).OnComplete(() =>
        {
            creditsScreen.gameObject.SetActive(false);
        });
    }
}
