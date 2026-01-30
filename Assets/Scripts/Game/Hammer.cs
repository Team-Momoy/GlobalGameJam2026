using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Hammer : InteractableElement
{
    private bool isActive = false;

    private Vector3 initialPosition;

    protected override void Awake()
    {
        base.Awake();
        initialPosition = transform.position;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!isActive)
        {
            isActive = true;
            return;
        }

        // soundPlayer.PlayRandomizedPitchSound(hammerHitSFX);

        //Check if the hammer is over a weapon assembly
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        RaycastHit2D[] raycastHit2Ds = Physics2D.RaycastAll(worldPos, Vector2.zero);

        foreach (var hit in raycastHit2Ds)
        {
            if (hit.collider != null)
            {
                if (hit.collider.TryGetComponent<PressurePlate>(out var pressurePlate))
                {
                    if (!pressurePlate.isAssigned)
                    {
                        break;
                    }

                    StartCoroutine(pressurePlate.EnableMinigame());
                    Reset();


                    return;
                }
            }
        }
        Reset();
    }

    private void Reset()
    {
        isActive = false;
        transform.position = initialPosition;
        DisableOutline();
    }

    private Vector3 velocity;
    void Update()
    {
        if (isActive)
        {
            //Follow mouse position using new input system
            Vector2 mousePosition = Pointer.current.position.ReadValue();
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            worldPosition.z = -5;
            transform.position = Vector3.SmoothDamp(transform.position, worldPosition, ref velocity, 0.05f);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if(isActive) EnableOutline();
    }
}
