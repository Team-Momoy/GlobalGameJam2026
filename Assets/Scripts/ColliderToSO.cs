using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ColliderToSO : MonoBehaviour
{
    [BoxGroup("ScriptableObject Settings")]
    [Required]
    [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
    public ItemDataSO templateSO;

    [BoxGroup("ScriptableObject Settings")]
    [FolderPath(RequireExistingPath = false)]
    public string savePath = "Assets/";

    [BoxGroup("ScriptableObject Settings")]
    [EnumToggleButtons]
    public MaterialType materialType;

    [BoxGroup("ScriptableObject Settings")]
    public string itemName;

    [BoxGroup("ScriptableObject Settings")]
    [Button(ButtonSizes.Large, Name = "Create SO From Collider")]
    private void CreateSOFromCollider()
    {
#if UNITY_EDITOR
        if (templateSO == null)
        {
            Debug.LogWarning("Assign a template SO first!");
            return;
        }

        Collider2D collider = GetComponent<Collider2D>();
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (collider == null)
        {
            Debug.LogWarning("No Collider2D found on this GameObject.");
            return;
        }

        if (sr == null)
        {
            Debug.LogWarning("No SpriteRenderer found on this GameObject.");
            return;
        }

        // Create new SO
        ItemDataSO newSO = ScriptableObject.CreateInstance<ItemDataSO>();
        EditorUtility.CopySerialized(templateSO, newSO);

        newSO.itemName = string.IsNullOrWhiteSpace(itemName) ? gameObject.name : itemName;
        newSO.sprite = sr.sprite;
        newSO.materialType = materialType;

        // Save collider data
        switch (collider)
        {
            case BoxCollider2D box:
                newSO.colliderType = ColliderShapeType.Box;
                newSO.colliderOffset = box.offset;
                newSO.colliderSize = box.size;
                break;

            case CircleCollider2D circle:
                newSO.colliderType = ColliderShapeType.Circle;
                newSO.colliderOffset = circle.offset;
                newSO.colliderRadius = circle.radius;
                break;

            case CapsuleCollider2D capsule:
                newSO.colliderType = ColliderShapeType.Capsule;
                newSO.colliderOffset = capsule.offset;
                newSO.colliderSize = capsule.size;
                newSO.capsuleDir = capsule.direction;
                break;

            case PolygonCollider2D poly:
                newSO.colliderType = ColliderShapeType.Polygon;
                newSO.colliderOffset = poly.offset;
                newSO.polygonPoints = poly.points;
                break;

            default:
                Debug.LogWarning($"Collider type {collider.GetType()} not supported.");
                break;
        }

        // Save asset
        string path = $"{savePath}/{materialType}/{newSO.itemName}.asset";
        AssetDatabase.CreateAsset(newSO, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newSO;

        Debug.Log($"ScriptableObject created at {path}");
#endif
    }
}
