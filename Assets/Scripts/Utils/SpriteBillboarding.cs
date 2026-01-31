using UnityEngine;

public class SpriteBillboarding : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    void Awake()
    {
        targetCamera = Camera.main;
    }
    // Update to make the sprite always face the camera
    void Update()
    {
        if (targetCamera != null)
        {
            Vector3 cameraPosition = targetCamera.transform.position;
            cameraPosition.y = transform.position.y; // Keep the sprite upright
            transform.LookAt(cameraPosition);
            // Flip the sprite to face the camera correctly
            transform.Rotate(0f, 180f, 0f);
        }
    }
}
