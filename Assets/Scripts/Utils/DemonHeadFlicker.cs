using UnityEngine;

public class DemonHeadFlicker : MonoBehaviour
{
    //Make a function that executes randomly every 2 variable amount of seconds between min and max
    //That changes the transform Z rotation to a random value between -maxAngle and maxAngle
    //And then back to 0 after a short delay, then repeats

    [SerializeField] private float minInterval = 0.5f;
    [SerializeField] private float maxInterval = 2f;
    [SerializeField] private float maxAngle = 15f;
    [SerializeField] private float flickerDuration = 0.1f;
    private float nextFlickerTime = 0f;

    void Awake()
    {
        nextFlickerTime = Time.time + Random.Range(minInterval, maxInterval);
    }

    void Update()
    {
        if (Time.time >= nextFlickerTime)
        {
            float randomAngle = Random.Range(-maxAngle, maxAngle);
            // Rotate to random angle on Z axis
            transform.rotation = Quaternion.Euler(0f, 0f, randomAngle);
            // Return to 0 rotation after flickerDuration
            Invoke("ResetRotation", flickerDuration);
            // Set next flicker time
            nextFlickerTime = Time.time + Random.Range(minInterval, maxInterval);
        }
    }
    void ResetRotation()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

}
