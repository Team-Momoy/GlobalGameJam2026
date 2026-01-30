using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DarkHole : DraggableElement
{

    [SerializeField]
    private float speed;

    [SerializeField]
    private Transform initialPoint;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (!isBeingDragged && !repositioning)
        {
            DOTween.Kill(transform);
            transform.DOScale(Vector3.one * 1.1f, 0.15f).SetEase(Ease.OutQuad);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (!isBeingDragged && !repositioning)
        {
            DOTween.Kill(transform);
            spriteRenderer.DOColor(Color.clear, 0.3f);
            transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutQuad);
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.back * speed * (isBeingDragged ? 20 : 1) * Time.deltaTime);

        if (isBeingDragged)
        {
            if (repositioning) return;
            Vector2 mouseScreenPos = Pointer.current.position.ReadValue();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            worldPos.z = -1;
            transform.position = Vector3.SmoothDamp(transform.position, worldPos, ref velocity, 0.1f);
            CheckForItems(transform.position);
        }
    }

    private Vector3 velocity;

    private bool repositioning;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (repositioning) return;
        spriteRenderer.DOColor(Color.white, 0.3f);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        repositioning = true;
        DOTween.Kill(transform);
        spriteRenderer.DOColor(Color.clear, 0.3f);
        transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutQuad);
        transform.DOMove(initialPoint.position, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            repositioning = false;
        });
    }

    public float radius = 1f;
    public LayerMask itemsLayer; // Set in Inspector for performance (e.g., "Scrap" layer)

    /// <summary>
    /// Looks for Items in a circle and triggers a function on them.
    /// </summary>
    public void CheckForItems(Vector2 position)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius, itemsLayer);

        foreach (Collider2D hit in hits)
        {
            Item item = hit.GetComponentInParent<Item>();
            if (item != null)
            {
                item.GetComponentInChildren<SpriteRenderer>().sortingLayerID = spriteRenderer.sortingLayerID;
                item.GetComponentInChildren<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder + 1;
                item.EnterDarkHole(transform);
            }
        }
    }

    // Visualize the overlap circle in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
