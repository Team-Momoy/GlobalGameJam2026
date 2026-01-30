using Sirenix.OdinInspector;
using UnityEngine;

public enum ColliderShapeType
{
    Box,
    Circle,
    Capsule,
    Polygon
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/ItemData")]
public class ItemDataSO : ScriptableObject
{
    [Header("Item Properties")]
    public string itemName;
    public int weight;

    [PropertySpace(SpaceBefore = 30, SpaceAfter = 30)]
    [EnumToggleButtons]
    public MaterialType materialType;

    [EnumToggleButtons]
    [PropertySpace(SpaceAfter = 30)]
    public MaterialRarity materialRarity;

    [PreviewField]
    public Sprite sprite;

    [Title("Collider Data")]
    [EnumToggleButtons]
    public ColliderShapeType colliderType;

    public Vector2 colliderOffset;

    [ShowIf("IsBoxOrCapsule")] 
    public Vector2 colliderSize;          // Box, Capsule

    [ShowIf("IsCircle")]
    public float colliderRadius;          // Circle

    [ShowIf("IsCapsule")]
    public CapsuleDirection2D capsuleDir; // Capsule

    [ShowIf("IsPolygon")] 
    public Vector2[] polygonPoints;       // Polygon


    private bool IsBoxOrCapsule()
    {
        return colliderType == ColliderShapeType.Box || colliderType == ColliderShapeType.Capsule;
    }

    private bool IsCircle()
    {
        return colliderType == ColliderShapeType.Circle;
    }

    private bool IsCapsule()
    {
        return colliderType == ColliderShapeType.Capsule;
    }

    private bool IsPolygon()
    {
        return colliderType == ColliderShapeType.Polygon;
    }
}
