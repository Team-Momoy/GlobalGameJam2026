using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;
using DG.Tweening;

public class CardManager : MonoBehaviour
{
    [SerializeField] private Transform gatherPoint;
    [SerializeField] private float gatherDuration = 0.8f;
    [SerializeField] private float gatherDelay = 0.5f;
    [SerializeField] private float rotateDuration = 0.5f;

    private readonly List<Card> cards = new List<Card>();

    IEnumerator Start()
    {
        var npcs = FindObjectsByType<MaskedNpc>(FindObjectsSortMode.None);
        if (npcs == null || npcs.Length == 0)
            yield break;

        int remaining = npcs.Length;

        foreach (var npc in npcs)
        {
            var (card, sequence) = npc.PlayCard();
            if (card != null) cards.Add(card);

            if (sequence != null)
            {
                sequence.OnComplete(() =>
                {
                    remaining--;
                    if (remaining == 0)
                        MoveAllCardsToPoint(gatherPoint ? gatherPoint.position : Vector3.zero);
                });
            }
            else
            {
                remaining--;
            }
        }

        // In case everything finished synchronously
        if (remaining == 0)
            MoveAllCardsToPoint(gatherPoint ? gatherPoint.position : Vector3.zero);

        yield return null;
    }

    private void MoveAllCardsToPoint(Vector3 point)
    {
        Sequence seq = DOTween.Sequence();
        bool hasTweens = false;

        foreach (var card in cards)
        {
            if (card == null) continue;
            card.transform.SetParent(gatherPoint);

            var move = card.transform.DOMove(point, gatherDuration).SetDelay(gatherDelay).SetEase(Ease.InOutSine);
            var rot = card.transform.DORotate(new Vector3(-90f, 0f, 0f), gatherDuration).SetDelay(gatherDelay).SetEase(Ease.InOutSine);

            seq.Join(move);
            seq.Join(rot);

            hasTweens = true;
        }

        if (hasTweens)
        {
            seq.OnComplete(() =>
            {
                RotateGatherPoint(rotateDuration);
            });
        }
        else
        {
            // No cards, just flip immediately
            RotateGatherPoint(rotateDuration);
        }
    }

    // Make a function that rotates the gather point around the Z axis
    public void RotateGatherPoint(float duration)
    {
        gatherPoint.DORotate(new Vector3(0f, 0f, 180), duration).SetEase(Ease.InOutSine).OnComplete(() => {
            int index = -1;
            foreach (var card in cards)
            {
                card.transform.DOMoveX(gatherPoint.position.x + (index * 4f), 0.5f);
                index++;
            }
        });
    }
}
