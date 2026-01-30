using System;
using Assets.SimpleLocalization.Scripts;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RecipeBookUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform containerTransform;

    [SerializeField]
    private Image weaponSprite;

    [SerializeField]
    private PressurePlate pressurePlate;

    [SerializeField]
    private SerializedDictionary<MaterialType, RecipeIngredient> ingredientsByMaterialType;

    [SerializeField]
    private SerializedDictionary<Weapon, WeaponBlueprint> blueprintsByWeapon;

    [SerializeField]
    private LocalizedTextMeshPro hitsNeededTMP;

    [SerializeField]
    private WeaponRecipeSO defaultRecipe;

    [SerializeField]
    private WeaponRecipeSO selectedRecipe;
    public WeaponRecipeSO SelectedRecipe => selectedRecipe;

    [SerializeField]
    private UnityEvent createBlueprintEvent;


    public UnityEvent OnOpened, OnClosed, OnWeaponChanged;

    private void Awake()
    {
        SelectWeaponRecipe(defaultRecipe);
        containerTransform.localScale = Vector3.zero;
        containerTransform.anchoredPosition = new Vector2(0, -1100);
    }

    public void Close()
    {
        OnClosed?.Invoke();

        OverlayManager.Instance.Clear();
        Sequence sequence = DOTween.Sequence();

        sequence.Append(containerTransform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InSine));
        sequence.Join(containerTransform.DOAnchorPosY(-1100, 0.25f).SetEase(Ease.InSine));
        sequence.AppendCallback(() => containerTransform.gameObject.SetActive(false));
    }

    public void Open()
    {
        OnOpened?.Invoke();

        containerTransform.gameObject.SetActive(true);
        OverlayManager.Instance.FadeToBlack(0.7f);

        containerTransform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutSine);
        containerTransform.DOAnchorPosY(0, 0.25f).SetEase(Ease.OutSine);
    }

    public void DisableAllIngredients()
    {
        foreach (var ingredient in ingredientsByMaterialType)
        {
            ingredient.Value.Container.SetActive(false);
        }
    }

    public void CreateBlueprint()
    {
        WeaponBlueprint blueprint = Instantiate(blueprintsByWeapon[selectedRecipe.Weapon], new Vector3(pressurePlate.transform.position.x - 1, -10, 20), Quaternion.identity);
        pressurePlate.AssignObject(blueprint);
        createBlueprintEvent?.Invoke();
    }

    public void SelectWeaponRecipe(WeaponRecipeSO weaponRecipe)
    {
        OnWeaponChanged?.Invoke();
        DisableAllIngredients();

        selectedRecipe = weaponRecipe;

        weaponSprite.sprite = weaponRecipe.Sprite;

        foreach (var materialNeeded in weaponRecipe.MaterialsNeeded)
        {
            RecipeIngredient recipeIngredient = ingredientsByMaterialType[materialNeeded.Key];

            recipeIngredient.Container.SetActive(true);

            string localizedVariable = materialNeeded.Value > 1 ? $"X{materialNeeded.Value}" : "";

            recipeIngredient.DescriptionTMP.LocalizationVariables = new object[] { localizedVariable };
        }

        hitsNeededTMP.LocalizationVariables = new object[] { weaponRecipe.HitsNeeded };
    }
}

[Serializable]
public class RecipeIngredient {
    public GameObject Container;
    public LocalizedTextMeshPro DescriptionTMP;
}
