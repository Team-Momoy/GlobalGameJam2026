using DG.Tweening;
using UnityEngine;

public class CreditsScreen : MonoBehaviour
{
    [SerializeField]
    private RectTransform creditsScreen;

    public void Open()
    {
        OverlayManager.Instance.FadeToBlack(0.7f);

        creditsScreen.anchoredPosition = new Vector3(2000, creditsScreen.anchoredPosition.y);
        creditsScreen.gameObject.SetActive(true);
        creditsScreen.DOAnchorPos(new Vector3(0, creditsScreen.anchoredPosition.y   ), 0.3f).SetEase(Ease.OutSine);
    }

    public void Close()
    {
        OverlayManager.Instance.Clear();

        creditsScreen.anchoredPosition = Vector3.zero;
        creditsScreen.DOAnchorPos(new Vector3(-2000, creditsScreen.anchoredPosition.y), 0.4f).SetEase(Ease.InBack, 4f).OnComplete(() =>
        {
            creditsScreen.gameObject.SetActive(false);
        });
    }
}
