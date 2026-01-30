using System;
using System.Linq;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class WeaponBlueprint : MonoBehaviour
{
    private Vector3 velocity;

    [SerializeField]
    private WeaponRecipeSO weaponRecipe;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private PriceTag priceTag;
    public PriceTag PriceTag => priceTag;

    [SerializeField]
    private FinishedWeapon finishedWeaponPrefab;

    public Weapon Weapon => weaponRecipe.Weapon;
    private int weight;
    public int Weight => weight;

    [SerializeField]
    private WeaponFragment[] weaponFragments;

    [SerializeField]
    private SpriteRenderer weaponOutline, hammerIcon;

    [SerializeField]
    private ParticleSystem dustParticleSystem;

    private bool isWeaponFinished = false;
    public bool IsWeaponFinished => isWeaponFinished;

    public Action<int> OnWeightChanged;

    void Start()
    {
        foreach (WeaponFragment fragment in weaponFragments)
        {
            fragment.OnPlacedChange += CheckIfReadyToBuild;
        }
    }

    void OnDisable()
    {
        foreach (WeaponFragment fragment in weaponFragments)
        {
            fragment.OnPlacedChange -= CheckIfReadyToBuild;
        }
    }

    private void CheckIfReadyToBuild()
    {
        hammerIcon.gameObject.SetActive(ArePiecesPlaced);
        weight = GetComponentsInChildren<Item>().Sum(item => item.Weight);
        OnWeightChanged?.Invoke(weight);

        if(weight > 0)
        {
            PriceTag.Show();
        }
        else
        {
            PriceTag.Hide();
        }
    }

    public void SetWeight(int newWeight)
    {
        weight = newWeight;
        OnWeightChanged?.Invoke(weight);

        if(weight <= 0)
        {
            PriceTag.Hide();
        }
    }

    public void FinishWeapon()
    {
        // dustParticleSystem.Play();
        FinishedWeapon finishedWeapon = Instantiate(finishedWeaponPrefab, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity, null);
        finishedWeapon.Initialize(Weapon, Weight);
        
        foreach (WeaponFragment fragment in weaponFragments)
        {
            fragment.transform.SetParent(finishedWeapon.transform);
            fragment.transform.position = new (fragment.transform.position.x, fragment.transform.position.y, 0);
        }

        CustomersQueueManager.Instance.CurrentCustomer.ReceiveItem(finishedWeapon);

        PressurePlate.Instance.CameraZoomOut();
        PressurePlate.Instance.DisableMinigame();

        Destroy(gameObject);
    }

    public bool ArePiecesPlaced => weaponFragments.All(p => p.IsPlaced);

    public void Reset()
    {
        foreach (WeaponFragment fragment in weaponFragments)
        {
            if(!fragment.IsPlaced) continue;
            Transform item = fragment.transform.GetChild(0);
            item.SetParent(null);
            item.DOMove(item.transform.position + Vector3.left * UnityEngine.Random.Range(4f, 6f), 0.25f).SetEase(Ease.OutQuad);
        }
        PriceTag.Hide();
    }

    // public override void OnPointerUp(PointerEventData eventData)
    // {
    //     base.OnPointerUp(eventData);

    //     Vector2 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
    //     RaycastHit2D[] raycastHit2Ds = Physics2D.RaycastAll(worldPos, Vector2.zero);

    //     foreach (var hit in raycastHit2Ds)
    //     {
    //         if (hit.collider != null)
    //         {
    //             if (hit.collider.TryGetComponent<Customer>(out var customer))
    //             {
    //                 customer.ReceiveItem(this);
    //                 enabled = false;
    //                 return;
    //             }
    //         }
    //     }

    //     if (transform.position.y > 0)
    //     {
    //         transform.DOMoveY(-2.5f, 1f).SetEase(Ease.OutBounce);
    //     }
    //     else
    //     {
    //         transform.DOMove(worldPos, 0.25f).SetEase(Ease.OutQuad);
    //     }
    // }

    // public override void OnPointerDown(PointerEventData eventData)
    // {
    //     base.OnPointerDown(eventData);
    // }

    // void Update()
    // {
    //     if (isBeingDragged)
    //     {
    //         Vector3 worldPos = Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue());
    //         worldPos.z = 0;

    //         transform.position = Vector3.SmoothDamp(transform.position, worldPos + dragOffset, ref velocity, 0.1f);
    //     }
    //     else
    //     {
    //         transform.position = new Vector3(transform.position.x, transform.position.y, 20 + transform.position.y);
    //     }
    // }
}

