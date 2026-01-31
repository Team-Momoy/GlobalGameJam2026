using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Dagger : MonoBehaviour
{
    InputActions inputActions;

    bool isSelected = false;

    private Vector3 originalPosition;

    void Awake()
    {
        originalPosition = transform.position;
    }

    void OnEnable()
    {
        //Setup unity input system events
        inputActions = new InputActions();
        inputActions.UI.Click.started += SelectDagger;
        inputActions.Enable();
    }

    private void SelectDagger(InputAction.CallbackContext context)
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();

        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform == transform)
            {
                Debug.Log("Dagger clicked!");
                if (!isSelected)
                {
                    MoveUpAndFloat();
                }
                isSelected = true;

                // Add additional logic for when the dagger is clicked
            }
        }
        else
        {
            if(!isSelected) return;
            isSelected = false;
            ResetPosition();
        }
    }

    private void MoveUpAndFloat()
    {
        DOTween.Kill("DaggerFloat", false);
        Sequence sequence = DOTween.Sequence();

        // Move the dagger up slightly
        sequence.Append(transform.DOMoveY(originalPosition.y + 3f, 1f).SetEase(Ease.OutBack));

        // Add a floating effect
        sequence.Append(transform.DOMoveY(originalPosition.y + 2f, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo));
        sequence.SetId("DaggerFloat");
    }

    private void ResetPosition()
    {
        DOTween.Kill("DaggerFloat", false);
        transform.DOMove(originalPosition, 0.5f).SetId("DaggerFloat").SetEase(Ease.OutSine);
    }

    void OnDisable()
    {
        inputActions.UI.Click.performed -= SelectDagger;
        inputActions.Disable();
    }

    void Update()
    {
        if (!isSelected) return;


        Vector2 screenPos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 direction = hit.transform.position - transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.DORotate(targetRotation.eulerAngles, 0.75f).SetId("DaggerPointing").SetEase(Ease.OutBack);
        }
    }
}
