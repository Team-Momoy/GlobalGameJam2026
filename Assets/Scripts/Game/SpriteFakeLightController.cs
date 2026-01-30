using System;
using UnityEngine;

public class SpriteFakeLightController : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Color finalColor;

    void Start()
    {
        spriteRenderer.color = Color.clear;

        TimeManager.Instance.OnTimeChanged += UpdateLighting;
        UpdateLighting(0);
    }

    void OnDisable()
    {
        TimeManager.Instance.OnTimeChanged -= UpdateLighting;
    }

    private void UpdateLighting(float remainingTime)
    {
        spriteRenderer.color = Color.Lerp(finalColor, Color.clear, TimeManager.Instance.RemainingFraction);
    }
}
