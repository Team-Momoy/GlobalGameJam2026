using UnityEngine;
using UnityEngine.InputSystem;

public class HoverOutline : MonoBehaviour
{
    Interactable previousInteractable;

    void Update()
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();

        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            if(hit.transform.TryGetComponent<Interactable>(out var interactable))
            {
                if (interactable.HasOutline)
                {
                    interactable.SetOutline(1f);
                }
                interactable.IsHovered = true;

                if(previousInteractable != null && previousInteractable != interactable)
                {
                    previousInteractable.IsHovered = false;
                    if (previousInteractable.HasOutline)
                    {
                        previousInteractable.SetOutline(0f);
                    }
                }
                previousInteractable = interactable;
            }
        }
        
        if(previousInteractable != null)
        {
            if(hit.transform == previousInteractable.transform) return;

            previousInteractable.IsHovered = false;
            if (previousInteractable.HasOutline)
            {
                previousInteractable.SetOutline(0f);
            }
            previousInteractable = null;
        }
    }
}
