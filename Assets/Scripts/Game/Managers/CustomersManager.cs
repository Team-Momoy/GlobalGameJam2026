using System.Collections;
using UnityEngine;

public class CustomersManager : MonoBehaviour
{
    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    private Customer customerPrefab;

    [SerializeField]
    private CustomerSpawnDataSO[] customerSpawnDataList;

    [SerializeField]
    private Sprite[] initialCustomerSprites;
    private Bag<Sprite> _customerSpritesBag;

    private float _spawnTimer = 10f;
    private CustomerSpawnDataSO currentSpawnData;

    private void Start()
    {
        currentSpawnData = customerSpawnDataList[LevelManager.Instance.CurrentDay];

        _customerSpritesBag = new(initialCustomerSprites);
        _spawnTimer = currentSpawnData.SpawnInterval;

        TimeManager.Instance.OnDayEnded += DayEnded;
    }

    private void DayEnded()
    {
        this.enabled = false;
    }

    private void Update()
    {
        _spawnTimer += Time.deltaTime;
        if (_spawnTimer >= currentSpawnData.SpawnInterval - 3 * LevelManager.Instance.CurrentDay)
        {
            _spawnTimer = 0f;
            StartCoroutine(SpawnCustomer());
        }
    }

    private IEnumerator SpawnCustomer()
    {
        float evaluation = currentSpawnData.CustomerFrequency.Evaluate(1 - TimeManager.Instance.RemainingFraction);
        evaluation = Mathf.Floor(evaluation);


        for (int i = 0; i < evaluation; i++)
        {
            if (!CustomersQueueManager.Instance.TryFindAvailableQueueSlot(out int availableSlotIndex)) yield break;

            Customer customer = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity, transform);
            CustomersQueueManager.Instance.AddCustomerToQueueAtIndex(availableSlotIndex, customer);

            customer.Initialize(_customerSpritesBag.GetRandom());

            yield return new WaitForSeconds(currentSpawnData.SpawnDelay);
            if(!enabled) yield break;
        }
    }
}
