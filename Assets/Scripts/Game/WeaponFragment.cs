using System;
using DG.Tweening;
using UnityEngine;

public class WeaponFragment : MonoBehaviour
{
    [SerializeField]
    private Collider2D fragmentCollider;
    private bool isPlaced;
    public bool IsPlaced => isPlaced;

    public Action OnPlacedChange;

    [SerializeField]
    private MaterialType materialType;

    private Item currentItem;

    public bool CanPlaceItem(Item item) => !IsPlaced && item.MaterialType == materialType;

    void OnTransformChildrenChanged()
    {
        if (transform.childCount > 0)
        {
            fragmentCollider.enabled = false;
            isPlaced = true;
        }
        else
        {
            if(currentItem != null)
            {
                currentItem.enabled = true;
                currentItem = null;
            }
            fragmentCollider.enabled = true;
            isPlaced = false;
        }
        OnPlacedChange?.Invoke();
    }

    public void PlaceItem(Item item)
    {
        currentItem = item;
        currentItem.enabled = false;
        item.transform.SetParent(transform);
        item.Tooltip.Close();
        item.DisableOutline();
    }
}
