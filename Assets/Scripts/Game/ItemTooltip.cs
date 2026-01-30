using Assets.SimpleLocalization.Scripts;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    [SerializeField]
    private Item item;

    [SerializeField]
    private Image background;

    [SerializeField]
    private SerializedDictionary<MaterialType, string> descriptionByType;

    [SerializeField]
    private SerializedDictionary<MaterialRarity, Sprite> backgroundByRarity;

    [SerializeField]
    private TextMeshProUGUI weightTMP;

    [SerializeField]
    private LocalizedTextMeshPro typeTMP;

    void Start()
    {
        background.sprite = backgroundByRarity[item.MaterialRarity];
        typeTMP.LocalizationKey = descriptionByType[item.MaterialType];
        weightTMP.text = $"<size=30><sprite=0></size>{WeightConversionTable.GetWeaponWeightIcon(item.Weight)}";
    }

    public void Open()
    {
        DOTween.Kill(transform, true);
        transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).SetDelay(0.25f);
    }

    public void Close()
    {
        DOTween.Kill(transform);
        transform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.InQuad);
    }
}
