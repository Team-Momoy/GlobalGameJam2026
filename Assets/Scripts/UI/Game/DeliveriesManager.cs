using UnityEngine;

public class DeliveriesManager : MonoBehaviour
{
    public static DeliveriesManager Instance;

    [SerializeField]
    private DeliveryUI deliveryWidget;

    [SerializeField]
    private Transform layoutGroup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public DeliveryUI CreateDeliveryWidget(float currentPatience, Weapon weaponType, int weight)
    {
        DeliveryUI deliveryUI = Instantiate(deliveryWidget, transform.position, Quaternion.identity, layoutGroup);
        deliveryUI.Setup(currentPatience, weaponType, weight);

        return deliveryUI;
    }
}
