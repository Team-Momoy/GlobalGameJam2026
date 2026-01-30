using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.SimpleLocalization.Scripts;
using Coffee.UIExtensions;
using DG.Tweening;
using Febucci.UI;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Diogenes : MonoBehaviour
{
    [Title("Setup")]
    [SerializeField]
    private InputAction clickAction;

    [SerializeField]
    private List<GameObject> managers;
    
    [SerializeField]
    private GameObject overlay;
    
    [SerializeField]
    private GameObject darkHole;

    [SerializeField]
    private List<Button> bookRecipeIndicators;

    [SerializeField]
    private UnmaskRaycastFilter unmaskRaycastFilter;

    [SerializeField]
    private TrashCan trashCan;

    [SerializeField]
    private AudioClip musicLoop;

    [SerializeField]
    private GameObject hud;

    [Title("Store References")]

    [SerializeField]
    private Transform diogenesSprite;
    
    [SerializeField]
    private Transform primarySpotlight, secondarySpotlight;

    [SerializeField]
    private Transform dialogBoxTransform;

    [SerializeField]
    private TypewriterByCharacter storeTypewriter;
    [SerializeField]
    private LocalizedTextMeshPro storeDialogText;

    [SerializeField]
    private GameObject hammer;

    [Title("Act 2 References")]

    [SerializeField]
    private GameObject recipeBook;

    [SerializeField]
    private RecipeBookUI recipeBookUI;

    [SerializeField]
    private GameObject recipeBookContainer;

    [SerializeField]
    private GameObject closeRecipeBookButton;

    [SerializeField]
    private Transform canvasDiogenes;
    [SerializeField]
    private Transform canvasDiogenesDialog;
    [SerializeField]
    private LocalizedTextMeshPro canvasDiogenesDialogText;
    [SerializeField]
    private TypewriterByCharacter canvasTypewriterDiogenes;
    [SerializeField]
    private GameObject selectRecipeButton;

    [Title("Act 3 References")]
    [SerializeField]
    private Transform diogenesCanvasNewPosition;

    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private ItemDataSO axeHead, axeHandle;

    [SerializeField]
    private ConveyorBelt conveyorBelt;

    [SerializeField]
    private MMF_Player sceneLoader;

    [SerializeField]
    private Transform diogenesHoldingPoint;
    
    private bool clicked = false;
    private bool textFinished = false;

    private WeaponBlueprint weaponBlueprint = null;

    void Awake()
    {
        // PlayerPrefs.DeleteAll();

        storeTypewriter.onTextShowed.AddListener(() => textFinished = true);
        canvasTypewriterDiogenes.onTextShowed.AddListener(() => textFinished = true);
        clickAction.performed += _ => clicked = true;
        clickAction.performed += CheckTypewriter;
        clickAction.canceled += _ => clicked = false;
        clickAction.canceled += IsBlueprintDraggedToDiogenes;
        clickAction.Enable();
    }


    void OnDisable()
    {
        storeTypewriter.onTextShowed.RemoveAllListeners();
        canvasTypewriterDiogenes.onTextShowed.RemoveAllListeners();
        clickAction.performed -= CheckTypewriter;
        clickAction.canceled -= IsBlueprintDraggedToDiogenes;
        clickAction.Disable();

    }

    private IEnumerator Start()
    {
        //Check if it's the first day
        if (LevelManager.Instance.CurrentDay != 0 || PlayerPrefs.GetInt("TutorialCompleted", 0) == 1)
        {
            gameObject.SetActive(false);
            yield break;
        }

        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.SetInt("CurrentDay", 1);

        //Setup
        hud.SetActive(false);
        managers.ForEach(m => m.SetActive(false));
        overlay.SetActive(true);
        darkHole.SetActive(false);
        hammer.transform.localScale = Vector3.zero;
        trashCan.GetComponent<Collider2D>().enabled = false;
        //Disable all tabs on the recipe book except the axe tab
        bookRecipeIndicators.ForEach(el => el.enabled = false);

        // || Act One ||
        MMSoundManager.Instance.PlaySound(musicLoop, MMSoundManager.MMSoundManagerTracks.Music, Vector3.zero);
        
        // Move Diogenes to the center
        diogenesSprite.DOMoveX(0, 1f).SetEase(Ease.OutBack);
        // Animate diogenes up and down a few times
        Tween tween = diogenesSprite.DOMoveY(diogenesSprite.position.y + 0.2f, 0.25f).SetEase(Ease.InOutSine).SetLoops(4, LoopType.Yoyo);
        yield return tween.WaitForCompletion();
        
        // Animate Diogenes squash and stretch
        diogenesSprite.DOScale(diogenesSprite.localScale.x * new Vector3(0.95f, 1.05f, 1), 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

        // Show dialog box
        dialogBoxTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuad);
        // Animate dialog box up and down
        dialogBoxTransform.DOMoveY(dialogBoxTransform.position.y + 0.2f, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

        // Make the spotlight appear
        primarySpotlight.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        // Show first dialog
        storeDialogText.LocalizationKey = "Tutorial.Dialogue1";

        clicked = false;
        textFinished = false;
        yield return new WaitUntil(() => textFinished);
        clicked = false;
        yield return new WaitUntil(() => clicked);

        // Show second dialog
        storeDialogText.LocalizationKey = "Tutorial.Dialogue2";

        // Move the secondary spotlight to the recipe book
        secondarySpotlight.DOLocalMove(new Vector2(1772, -107), 0.5f).SetEase(Ease.OutBack);
        secondarySpotlight.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        yield return new WaitUntil(() => recipeBookContainer.activeSelf);
        
        // || Act Two ||

        recipeBook.SetActive(false);
        selectRecipeButton.transform.localScale = Vector3.zero;
        closeRecipeBookButton.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        Tween canvasDiogenesTweenEnters = canvasDiogenes.DOMoveX(-20, 0.2f).SetEase(Ease.OutBack);
        yield return canvasDiogenesTweenEnters.WaitForCompletion();

        canvasDiogenesDialog.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        canvasDiogenesDialogText.LocalizationKey = "Tutorial.Dialogue3";

        clicked = false;
        textFinished = false;
        yield return new WaitUntil(() => textFinished);
        yield return new WaitUntil(() => recipeBookUI.SelectedRecipe.Weapon == Weapon.Axe);

        canvasDiogenesDialog.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
        Tween canvasDiogenesTweenExits = canvasDiogenes.DOMoveX(-150, 0.2f).SetEase(Ease.InBack);
        yield return canvasDiogenesTweenExits.WaitForCompletion();
        canvasDiogenes.position = diogenesCanvasNewPosition.position;
        canvasDiogenes.localScale = new Vector3(-1, canvasDiogenes.localScale.y, canvasDiogenes.localScale.z);
        canvasDiogenes.eulerAngles = new Vector3(0, 0, -canvasDiogenes.eulerAngles.z);

        canvasDiogenes.DOLocalMoveX(canvasDiogenes.localPosition.x - 225, 0.2f).SetEase(Ease.OutBack);

        canvasDiogenesDialog.position = diogenesCanvasNewPosition.position - new Vector3(200, 100, 0);
        canvasDiogenesDialog.DOScale(new Vector3(-1, 1, 1), 0.5f).SetEase(Ease.OutBack);
        canvasDiogenesDialogText.transform.localScale = new Vector3(-1, 1, 1);
        canvasDiogenesDialogText.LocalizationKey = "Tutorial.Dialogue4";
        
        clicked = false;
        textFinished = false;
        yield return new WaitUntil(() => textFinished);
        clicked = false;
        yield return new WaitUntil(() => clicked);

        // Show another dialog
        canvasDiogenesDialogText.LocalizationKey = "Tutorial.Dialogue5";

        selectRecipeButton.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        yield return new WaitUntil(() => !recipeBookContainer.activeSelf);

        canvasDiogenes.DOLocalMoveX(canvasDiogenes.localPosition.x + 300, 0.2f).SetEase(Ease.InBack);
        canvasDiogenesDialog.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
        secondarySpotlight.DOLocalMove(new Vector2(144, 0), 0.5f).SetEase(Ease.OutBack);
        storeDialogText.LocalizationKey = "Tutorial.Dialogue6";

        yield return new WaitUntil(() => cameraTransform.position.x < -5);
        secondarySpotlight.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
        primarySpotlight.DOLocalMove(new Vector2(-638, 178), 0.5f).SetEase(Ease.OutBack);
        primarySpotlight.DOScale(Vector3.one * 1.35f, 0.5f).SetEase(Ease.OutBack);
        dialogBoxTransform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.InQuad);

        Tween movingToGardenTween = diogenesSprite.DOMove(new Vector2(-17, 0.8f), 0.5f).SetEase(Ease.OutBack).SetDelay(0.5f);
        yield return movingToGardenTween.WaitForCompletion();

        unmaskRaycastFilter.targetUnmask = primarySpotlight.GetComponent<Unmask>();
        dialogBoxTransform.position = new Vector2(-15, dialogBoxTransform.transform.position.y);
        dialogBoxTransform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutQuad);
        storeDialogText.LocalizationKey = "Tutorial.Dialogue7";

        clicked = false;
        textFinished = false;
        yield return new WaitUntil(() => textFinished);
        clicked = false;
        yield return new WaitUntil(() => clicked);

        trashCan.SpawnSpecificItem(axeHead);
        trashCan.SpawnSpecificItem(axeHandle);

        unmaskRaycastFilter.gameObject.SetActive(false);

        storeDialogText.LocalizationKey = "Tutorial.Dialogue8";

        clicked = false;
        textFinished = false;
        yield return new WaitUntil(() => textFinished);

        yield return new WaitUntil(() => conveyorBelt.IsRunning);
        yield return new WaitUntil(() => !conveyorBelt.IsRunning);

        storeDialogText.LocalizationKey = "Tutorial.Dialogue9";
        clicked = false;
        textFinished = false;
        yield return new WaitUntil(() => textFinished || cameraTransform.position.x > -17);

        yield return new WaitUntil(() => cameraTransform.position.x > -17);
        dialogBoxTransform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.InQuad);
        Tween diogenesBackToWorkshopTween = diogenesSprite.DOMoveX(0, 1f).SetEase(Ease.OutBack);
        yield return diogenesBackToWorkshopTween.WaitForCompletion();

        
        dialogBoxTransform.position = new Vector2(1.5f, dialogBoxTransform.transform.position.y);
        dialogBoxTransform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutQuad);
        storeDialogText.LocalizationKey = "Tutorial.Dialogue10";

        weaponBlueprint = FindAnyObjectByType<WeaponBlueprint>();

        clicked = false;
        textFinished = false;
        yield return new WaitUntil(() => textFinished && weaponBlueprint.ArePiecesPlaced);

        hammer.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        storeDialogText.LocalizationKey = "Tutorial.Dialogue11";

        clicked = false;
        textFinished = false;
        yield return new WaitUntil(() => textFinished && weaponBlueprint.IsWeaponFinished);

        storeDialogText.LocalizationKey = "Tutorial.Dialogue12";

        // Detect if the player drags the blueprint towards Diogenes
        yield return new WaitUntil(() => diogenesHasBlueprint);

        weaponBlueprint.GetComponentsInChildren<Collider2D>().ToList().ForEach(col => col.enabled = false);
        weaponBlueprint.transform.DOKill();
        weaponBlueprint.transform.SetParent(diogenesHoldingPoint);
        weaponBlueprint.transform.localPosition = Vector3.zero;
        weaponBlueprint.GetComponent<SortingGroup>().sortingOrder = 10;
        weaponBlueprint.GetComponent<SortingGroup>().sortingLayerName = "Middleground";

        storeDialogText.LocalizationKey = "Tutorial.Dialogue13";

        clicked = false;
        textFinished = false;
        yield return new WaitUntil(() => textFinished);
        clicked = false;
        yield return new WaitUntil(() => clicked);

        storeDialogText.LocalizationKey = "Tutorial.Dialogue14";

        clicked = false;
        textFinished = false;
        yield return new WaitUntil(() => textFinished);
        clicked = false;
        yield return new WaitUntil(() => clicked);

        storeDialogText.LocalizationKey = "Tutorial.Dialogue15";

        clicked = false;
        textFinished = false;
        yield return new WaitUntil(() => textFinished);
        clicked = false;
        yield return new WaitUntil(() => clicked);

        LevelManager.Instance.AdvanceToNextDay();
        sceneLoader.PlayFeedbacks();
    }

    private void CheckTypewriter(InputAction.CallbackContext context)
    {
        if (storeTypewriter.isShowingText)
        {
            storeTypewriter.SkipTypewriter();
        }
        if (canvasTypewriterDiogenes.isShowingText)
        {
            canvasTypewriterDiogenes.SkipTypewriter();
        }
    }

    private bool diogenesHasBlueprint = false;

    private void IsBlueprintDraggedToDiogenes(InputAction.CallbackContext context)
    {
        if (weaponBlueprint == null)
            return;
        // if (!weaponBlueprint.IsBeingDragged)
        //     return;
        if (!diogenesSprite.TryGetComponent<Collider2D>(out _))
            return;

        Vector3 blueprintPos = weaponBlueprint.transform.position;

        // Check if blueprint is within a reasonable distance of Diogenes
        float distance = Vector3.Distance(blueprintPos, diogenesSprite.position);
        const float dropOffDistance = 2f; // Adjust this value to control how close the blueprint needs to be

        // Also check if the blueprint was recently dragged (moved from its original position)
        diogenesHasBlueprint = distance < dropOffDistance && weaponBlueprint.IsWeaponFinished;
    }
}
