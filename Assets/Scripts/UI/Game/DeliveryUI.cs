using AYellowpaper.SerializedCollections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectTransform;

    [SerializeField]
    private RectTransform containerTransform;

    // [SerializeField]
    // private LayoutElement layoutElement;

    [SerializeField]
    private Image timerUI;

    [SerializeField]
    private Image weaponUI;


    private float initialPatience;
    private float currentPatience;

    [SerializeField]
    private SerializedDictionary<Weapon, Sprite> spriteByWeapon;

    public void Setup(float patience, Weapon weapon, int weaponWeight)
    {
        initialPatience = patience;
        currentPatience = patience;
        weaponUI.sprite = spriteByWeapon[weapon];
    }

    void Start()
    {
        rectTransform.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutBack).SetDelay(1.5f);
    }

    void Update()
    {
        currentPatience -= Time.deltaTime;
        timerUI.fillAmount = currentPatience / initialPatience;
    }

    public void DeleteWidget()
    {
        RectTransform layoutTransform = containerTransform.parent.GetComponent<RectTransform>();

        rectTransform.DOAnchorPos(Vector2.up * 150, 0.5f).SetEase(Ease.InBack);

        containerTransform.DOSizeDelta(new Vector2(-10, containerTransform.sizeDelta.y), 0.5f).SetEase(Ease.OutQuad)
        .OnUpdate(()=>
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutTransform);
        })
        .OnComplete(() =>
        {
            Destroy(gameObject, 0.5f);
        });
    }
}
