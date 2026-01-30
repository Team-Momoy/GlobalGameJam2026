using System.Collections;
using DG.Tweening;
using UnityEngine;

public class WeightScaleDial : MonoBehaviour
{
    [SerializeField]
    private Transform needle;

    [SerializeField]
    private Transform indicator;

    [SerializeField]
    private Transform dialContainer;

    private float currentWeight;
    public float CurrentWeight
    {
        get => currentWeight;
        set
        {
            ToggleDialPosition(value != 0);
            currentWeight = value;
        } 
    }

    public void SetWeight(float weight)
    {
        float needleAngle = Mathf.Lerp(82, -82, weight / 10);

        needle.DOLocalRotate(new Vector3(0, 0, needleAngle), 0.4f).SetEase(Ease.OutBack);

        CurrentWeight = weight;
    }

    public void SetIdealWeight(float weight)
    {
        float indicatorAngle = Mathf.Lerp(-11.5f, -173.5f, weight / 10);

        indicator.DOLocalRotate(new Vector3(0, 0, indicatorAngle), 0.4f).SetEase(Ease.OutBack);
    }

    public void ToggleDialPosition(bool isActive)
    {
        if(isActive && dialContainer.localPosition.y > 2) return;
        if(!isActive && dialContainer.localPosition.y < 1.5f) return;

        dialContainer.DOKill();
        if (isActive)
        {
            dialContainer.DOLocalMoveY(2.75f, 0.2f).SetEase(Ease.OutQuad);
        }
        else
        {
            dialContainer.DOLocalMoveY(0.6f, 0.2f).SetEase(Ease.InQuad);
        }
    }
}
