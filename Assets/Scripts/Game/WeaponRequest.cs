using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponRequest : MonoBehaviour
{
    private Weapon _weapon;
    public Weapon Weapon => _weapon;

    private int _weight;
    public int Weight => _weight;

    [SerializeField]
    private Transform weightPerformanceIndicator;

    [SerializeField]
    private Transform rightWeightIndicator, wrongWeightIndicator;

    [SerializeField]
    private Transform weaponPerformanceIndicator;

    [SerializeField]
    private Transform rightWeaponIndicator, wrongWeaponIndicator;

    [SerializeField]
    private TextMeshProUGUI weightTMP;

    [SerializeField]
    private Image weaponImage;

    [SerializeField]
    private Transform dialogBubble;

    [SerializeField]
    private WeaponRequestSO weaponRequestData;

    public void RandomizeRequest()
    {
        Weapon[] availableWeapons = weaponRequestData.WeaponsByDay[LevelManager.Instance.CurrentDay];

        _weapon = availableWeapons[Random.Range(0, availableWeapons.Length)];

        _weight = Random.Range(weaponRequestData.WeightByWeapon[_weapon].MinWeight, weaponRequestData.WeightByWeapon[_weapon].MaxWeight + 1);

        weightTMP.text = $"{WeightConversionTable.GetWeaponWeightIcon(_weight)}\U0001F60A";

        weaponImage.sprite = weaponRequestData.WeaponSprites[_weapon];
    }

    public void ShowRequestBubble()
    {
        dialogBubble.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
    }

    public void DisplayPerformance(bool rightWeapon, bool rightWeight)
    {
        weaponPerformanceIndicator.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        weightPerformanceIndicator.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        rightWeaponIndicator.gameObject.SetActive(rightWeapon);
        wrongWeaponIndicator.gameObject.SetActive(!rightWeapon);

        rightWeightIndicator.gameObject.SetActive(rightWeight);
        wrongWeightIndicator.gameObject.SetActive(!rightWeight);
    }
}
