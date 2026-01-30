using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class FinishedWeapon : DraggableElement
{
    private Vector3 velocity;

    private Weapon weapon;
    public Weapon Weapon => weapon;

    private int weight;
    public int Weight => weight;

    public void Initialize(Weapon weapon, int weight)
    {
        this.weapon = weapon;
        this.weight = weight;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        RaycastHit2D[] raycastHit2Ds = Physics2D.RaycastAll(worldPos, Vector2.zero);

        foreach (var hit in raycastHit2Ds)
        {
            if (hit.collider != null)
            {
                if (hit.collider.TryGetComponent<Customer>(out var customer))
                {
                    customer.ReceiveItem(this);
                    enabled = false;
                    return;
                }
            }
        }

        if (transform.position.y > 0)
        {
            transform.DOMoveY(-2.5f, 1f).SetEase(Ease.OutBounce);
        }
        else
        {
            transform.DOMove(worldPos, 0.25f).SetEase(Ease.OutQuad);
        }
    }

    void Update()
    {
        if (isBeingDragged)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue());
            worldPos.z = transform.position.z;
            worldPos += dragOffset;

            transform.position = Vector3.SmoothDamp(transform.position, worldPos, ref velocity, 0.1f);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
        }
    }
}
