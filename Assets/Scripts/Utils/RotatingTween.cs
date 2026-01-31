using DG.Tweening;
using UnityEngine;

public class RotatingTween : MonoBehaviour
{
    [SerializeField] private float rotationAmount = 15f;
    void Update()
    {
        // Rotate around the Z axis continuously without dotween
        transform.Rotate(Vector3.forward * rotationAmount * Time.deltaTime);
    }
}
