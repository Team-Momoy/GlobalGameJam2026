using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightingController : MonoBehaviour
{
    [SerializeField] private Color dayLightColor, nightLightColor;
    [SerializeField] private float dayIntensity, nightIntensity;
    [SerializeField] private Light2D globalLight;

    private void Start()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnTimeChanged += HandleTimeChanged;
    }

    private void OnDisable()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnTimeChanged -= HandleTimeChanged;
    }

    private void HandleTimeChanged(float remainingTime)
    {
        float fraction = TimeManager.Instance.RemainingFraction;
        globalLight.color = Color.Lerp(nightLightColor, dayLightColor, fraction);
        globalLight.intensity = Mathf.Lerp(nightIntensity, dayIntensity, fraction);
    }
}

