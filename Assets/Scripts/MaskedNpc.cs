using DG.Tweening;
using Sirenix.Utilities;
using UnityEngine;

public class MaskedNpc : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Card impostorCardPrefab;
    [SerializeField] private Transform cardPlayingPoint;

    [SerializeField] private bool isImpostor = false;

    // Returns the created card and the sequence driving its play animation so callers can wait for completion
    public (Card card, Sequence sequence) PlayCard()
    {
        Card card = Instantiate(isImpostor ? impostorCardPrefab : cardPrefab, transform.position + new Vector3(0, -5, 0), Quaternion.identity, null);

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
}
