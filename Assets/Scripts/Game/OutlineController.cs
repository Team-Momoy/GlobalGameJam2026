using UnityEngine;

public class OutlineTrigger : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock materialPropertyBlock;

    void Awake()
    {
        materialPropertyBlock = new MaterialPropertyBlock();
    }

    public void EnableOutline()
    {
        spriteRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetFloat("_OutlineAlpha", 1);
        spriteRenderer.SetPropertyBlock(materialPropertyBlock);
    }

    public void DisableOutline()
    {
        spriteRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetFloat("_OutlineAlpha", 0);
        spriteRenderer.SetPropertyBlock(materialPropertyBlock);
    }
}
