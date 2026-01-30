using UnityEngine;
using TMPro;
using MoreMountains.Feedbacks;
using DG.Tweening;

public class GoldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private MMF_Player addGoldSFX, spendGoldSFX;

    private int lastGold;

    private void Start()
    {
        lastGold = GoldManager.Instance.CurrentGold;
        GoldManager.Instance.OnGoldChanged += UpdateGoldUI;
        UpdateGoldUI(lastGold);
    }

    private void OnDisable()
    {
        GoldManager.Instance.OnGoldChanged -= UpdateGoldUI;
    }

    private int displayedGold = 0;
    private void UpdateGoldUI(int currentGold)
    {
        DOTween.To(() => displayedGold, x => 
        {
            displayedGold = x;
            goldText.text = $"${displayedGold:D4}";
        }, currentGold, 1f)
        .SetTarget(this)
        .SetEase(Ease.OutCubic);

        // Play sound depending on increase/decrease
        if (currentGold > lastGold)
            addGoldSFX.PlayFeedbacks();
        else if (currentGold < lastGold)
            spendGoldSFX.PlayFeedbacks();

        lastGold = currentGold;
    }
}
