using DG.Tweening;
using UnityEngine;

public class MainMenuTitleAnimation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float delay = 0f;
        //Go through all children and set their position to itself minus 10 in the y axis using dotween with ease out bounce over 1 second
        foreach (RectTransform child in transform)
        {
            child.DOAnchorPosY(child.anchoredPosition.y - 950f, 1f).SetDelay(delay).SetEase(Ease.OutBounce);
            delay += 0.1f;
        }   
    }
}