using DG.Tweening;
using Sirenix.Utilities;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer frontRenderer, backRenderer;
    public SpriteRenderer FrontRenderer => frontRenderer;
    public SpriteRenderer BackRenderer => backRenderer;
}
