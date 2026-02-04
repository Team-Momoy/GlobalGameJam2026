using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Dagger : Interactable
{
    bool isSelected = false;

    private Vector3 originalPosition;

    protected override void Awake()
    {
        base.Awake();
        originalPosition = transform.position;
    }

    public void SelectDagger()
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();

        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (!isSelected)
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    MoveUpAndFloat();
                    isSelected = true;
                }
            }
        }
        else
        {
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
        sequence.AppendCallback(() => transform.DOMoveY(originalPosition.y + 2f, 2f).SetEase(Ease.InOutSine).SetId("DaggerFloat").SetLoops(-1, LoopType.Yoyo));
        sequence.SetId("DaggerFloat");
    }

    private void ResetPosition()
    {
        DOTween.Kill("DaggerFloat", false);
        transform.DOMove(originalPosition, 0.5f).SetId("DaggerFloat").SetEase(Ease.OutSine);
        transform.DORotate(new Vector3(0f, 0f, 0f), 0.5f).SetId("DaggerFloat").SetEase(Ease.OutSine);
    }

    void Update()
    {
        if (!isSelected) return;


        Vector2 screenPos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if(!hit.transform.CompareTag("NPC")) return;

            Vector3 direction = hit.transform.position - transform.position;
            if (direction.sqrMagnitude < 0.0001f) return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.DORotate(targetRotation.eulerAngles, 0.75f).SetId("DaggerPointing").SetEase(Ease.OutBack);

            if (hit.transform.TryGetComponent(out MaskedNpc npc) && Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
            {
                FindAnyObjectByType<CardManager>().IsDealing = true;
                // Then move the dagger to the NPC over 0.5 seconds
                transform.DOMove(hit.transform.position + direction.normalized * 2f, 0.15f).SetEase(Ease.InSine).OnComplete(() =>
                {
                    npc.DestroyMask();
                    gameObject.SetActive(false);
                });
                return;
            }
        }
    }
}
