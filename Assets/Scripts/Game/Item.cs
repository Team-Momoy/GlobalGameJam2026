using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Item : DraggableElement
{
    #region Data

    private MaterialType materialType;
    public MaterialType MaterialType => materialType;

    private int weight = 0;
    public int Weight => weight;
    private MaterialRarity materialRarity;
    public MaterialRarity MaterialRarity => materialRarity;

    #endregion
    private Vector3 velocity;
    private Vector3 initialScale;

    [SerializeField] private Transform sprite;
    [SerializeField] private ItemTooltip tooltip;
    public ItemTooltip Tooltip => tooltip;
    [SerializeField] private TextMeshProUGUI weightTMP;
    [SerializeField] private TextMeshProUGUI itemTMP;
    [SerializeField] private Image itemTypeImage;

    private Collider2D activeCollider;

    protected override void Awake()
    {
        base.Awake();
        initialScale = sprite.localScale;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        RandomizeRotation();
    }

    [Serializable]
    public class MaterialData
    {
        public Sprite Sprite;
        public string Description;
    }

    public void Initialize(ItemDataSO itemData)
    {
        materialType = itemData.materialType;
        materialRarity = itemData.materialRarity;
        weight = itemData.weight;

        if (spriteRenderer != null)
            spriteRenderer.sprite = itemData.sprite;

        // Remove any old collider
        foreach (var col in GetComponents<Collider2D>())
            Destroy(col);

        // Add correct collider based on SO
        switch (itemData.colliderType)
        {
            case ColliderShapeType.Box:
                var box = sprite.gameObject.AddComponent<BoxCollider2D>();
                box.size = itemData.colliderSize;
                box.offset = itemData.colliderOffset;
                activeCollider = box;
                break;

            case ColliderShapeType.Circle:
                var circle = sprite.gameObject.AddComponent<CircleCollider2D>();
                circle.radius = itemData.colliderRadius;
                circle.offset = itemData.colliderOffset;
                activeCollider = circle;
                break;

            case ColliderShapeType.Capsule:
                var capsule = sprite.gameObject.AddComponent<CapsuleCollider2D>();
                capsule.size = itemData.colliderSize;
                capsule.offset = itemData.colliderOffset;
                capsule.direction = itemData.capsuleDir;
                activeCollider = capsule;
                break;

            case ColliderShapeType.Polygon:
                var poly = sprite.gameObject.AddComponent<PolygonCollider2D>();
                poly.points = itemData.polygonPoints;
                poly.offset = itemData.colliderOffset;
                activeCollider = poly;
                break;
        }
    }

    public void RandomizeRotation()
    {
        sprite.rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        DOTween.Kill(transform);
        velocity = Vector3.zero;

        sprite.DOScale(initialScale * 1.3f, 0.5f).SetEase(Ease.OutBack);
        transform.SetParent(null);
        transform.position = new(transform.position.x, transform.position.y, -5);
        tooltip.Close();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if (transform.position.y > 0)
        {
            DOTween.Kill(transform, true);
            transform.DOMove(new Vector3(transform.position.x, -2f, transform.position.z), 0.75f).SetEase(Ease.OutBounce);
            transform.GetChild(0).DOScale(Vector3.one, 0.75f).SetEase(Ease.OutQuad);
            return;
        }

        sprite.DOScale(initialScale, 1f).SetEase(Ease.OutBounce);

        Vector3 targetPos = transform.position;
        targetPos.z = 0;
        transform.position = targetPos;

        Vector3 pointerPosition = Pointer.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(pointerPosition);
        worldPos.z = 0;

        if (!VerifyItemPlacement(worldPos))
        {
            transform.DOMove(worldPos += dragOffset, 0.25f).SetEase(Ease.OutQuad);
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if(!isBeingDragged) tooltip.Open();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        tooltip.Close();
    }


    public bool VerifyItemPlacement(Vector3 pointerWorldPosition)
    {
        RaycastHit2D[] raycastHit2Ds = Physics2D.RaycastAll(pointerWorldPosition, Vector2.zero);

        bool droppedOnInvalidLocation = false;
        foreach (var hit in raycastHit2Ds)
        {
            if (hit.collider != null)
            {
                if (hit.collider.TryGetComponent<ConveyorBelt>(out var conveyor))
                {
                    conveyor.PlaceItem(transform);
                    return false;
                }
                if (hit.collider.TryGetComponent<WeaponFragment>(out var weaponPiece))
                {
                    if (weaponPiece.CanPlaceItem(this))
                    {
                        weaponPiece.PlaceItem(this);
                        transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutQuad);
                        transform.DOLocalMove(Vector3.zero + Vector3.forward * weaponPiece.transform.position.z, 0.25f).SetEase(Ease.OutQuad);
                        sprite.DOLocalRotate(Vector3.forward * weaponPiece.transform.rotation.eulerAngles.z, 0.25f).SetEase(Ease.OutQuad);
                        return true;
                    }
                    else
                    {
                        droppedOnInvalidLocation = true;
                    }
                }
                if (hit.collider.TryGetComponent<PressurePlate>(out var pressurePlate)){
                    droppedOnInvalidLocation = true;
                }
            }
        }
        if (droppedOnInvalidLocation)
        {
            transform.DOMove(transform.position + Vector3.left * UnityEngine.Random.Range(4f, 6f), 0.25f).SetEase(Ease.OutQuad);
            return true;
        }
        return false;
    }

    public void DisableCollider()
    {
        if (activeCollider != null)
        {
            activeCollider.enabled = false;
        }
    }

    public void EnableCollider()
    {
        if (activeCollider != null)
            activeCollider.enabled = true;
    }

    private Transform darkHoleTransform;

    public void EnterDarkHole(Transform darkHole)
    {
        darkHoleTransform = darkHole;
        if (activeCollider != null)
            activeCollider.enabled = false;

        DOTween.Kill(transform);
        transform.DOScale(Vector3.zero, 0.8f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    void Update()
    {
        if (darkHoleTransform != null)
        {
            transform.position = Vector3.SmoothDamp(transform.position, darkHoleTransform.position, ref velocity, 0.075f);
            return;
        }

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
