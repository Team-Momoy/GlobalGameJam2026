using System;
using DG.Tweening;
using UnityEngine;

public class FloatingTween : MonoBehaviour
{
    [SerializeField] private float floatAmount = 0.75f;
    [SerializeField] private float floatDuration = 3f;
    [SerializeField] private float randomDurationOffset = 0.5f;
    [SerializeField] private bool randomDirection = true;

    [SerializeField] private Vector3 moveOffset = Vector3.zero;

    void Start()
    {
        transform.DOLocalMove(transform.localPosition + moveOffset * floatAmount * (randomDirection ? Math.Sign(UnityEngine.Random.Range(-1f, 1f)) : 1), floatDuration + UnityEngine.Random.Range(-randomDurationOffset, randomDurationOffset))
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutQuad);
    }
}
