using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    //Set limits for camera movement
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;

    [SerializeField] private float speed = 5f;

    // Make the camera rotate around the Y axis using the mouse position, and the new input system
    void Update()
    {
        float mouseX = Mouse.current.position.ReadValue().x - (Screen.width / 2);

        // Use mouse position relative to screen width to make camera rotation go from min to max but not more
        float rotationY = Mathf.Lerp(minX, maxX, (mouseX + (Screen.width / 2)) / Screen.width);
        // Smoothly rotate around Y using angle-aware interpolation to avoid 360° wrap-around
        float smoothY = Mathf.LerpAngle(transform.eulerAngles.y, rotationY, Time.deltaTime * speed);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, smoothY, transform.eulerAngles.z);
    }
}
