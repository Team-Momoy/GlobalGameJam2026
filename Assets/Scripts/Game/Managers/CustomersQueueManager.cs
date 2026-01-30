using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class CustomersQueueManager : MonoBehaviour
{
    public static CustomersQueueManager Instance;

    private Customer[] _customerSlots;

    public Customer CurrentCustomer => _customerSlots[0];
    
    private bool _dayEnded = false;
    private bool _firedAfterDayEnd = false;

    // Event fired when the store day has ended and the last customer leaves the queue
    public event Action OnStoreClosedAndEmpty;

    public Action<Customer> OnCustomerChange;

    [Range(1, 5)]
    [SerializeField]
    private int maxCustomers = 1;

    [SceneObjectsOnly]
    [ChildGameObjectsOnly]
    [SerializeField]
    private Transform[] queuePositions;

    void Awake()
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

    private void Start()
    {
        _customerSlots = new Customer[maxCustomers];
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnDayEnded += HandleDayEnded;
        }
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnDayEnded -= HandleDayEnded;
        }
    }

    public bool IsQueueFull => _customerSlots[maxCustomers - 1] != null;

    public void RemoveFromQueue(Customer customer)
    {
        if (customer == null) return;

        for (int i = 0; i < _customerSlots.Length; i++)
        {
            if (_customerSlots[i] == customer)
            {
                // Remove reference from the slot
                _customerSlots[i] = null;

                // Move the rest of the queue forward to fill the gap
                MoveTheQueueForward();
                // If the day already ended and there are no customers left, fire the event once

                if (_dayEnded && !_firedAfterDayEnd && !AnyCustomersLeft())
                {
                    _firedAfterDayEnd = true;
                    OnStoreClosedAndEmpty?.Invoke();
                }
                return;
            }
        }
    }

    private bool AnyCustomersLeft()
    {
        for (int i = 0; i < _customerSlots.Length; i++)
        {
            if (_customerSlots[i] != null) return true;
        }
        return false;
    }

    private void HandleDayEnded()
    {
        _dayEnded = true;
        _firedAfterDayEnd = false; // reset so the event can fire when the last customer leaves
        // If there are already no customers when the day ends, fire immediately
        if (!AnyCustomersLeft())
        {
            _firedAfterDayEnd = true;
            OnStoreClosedAndEmpty?.Invoke();
        }
    }

    public void MoveTheQueueForward()
    {
        for (int i = 1; i < maxCustomers; i++)
        {
            if (_customerSlots[i] != null && _customerSlots[i - 1] == null)
            {
                _customerSlots[i].MoveToPosition(queuePositions[i - 1].position, i - 1 == 0, 0);

                _customerSlots[i - 1] = _customerSlots[i];
                _customerSlots[i] = null;

                // Check again from the start in case multiple gaps exist
                i = 0;
            }
        }
        OnCustomerChange?.Invoke(CurrentCustomer);
    }

    public bool TryFindAvailableQueueSlot(out int availableSlotIndex)
    {
        availableSlotIndex = 0;

        for (int i = 0; i < _customerSlots.Length; i++)
        {
            if (_customerSlots[i] == null) // empty slot
            {
                availableSlotIndex = i;
                return true;
            }
        }
        return false;
    }

    public void AddCustomerToQueueAtIndex(int index, Customer customer)
    {
        if (_customerSlots[index] != null)
        {
            Debug.LogError("You are trying to put 2 customers on the same queue index");
            return;
        }
        _customerSlots[index] = customer;
        customer.MoveToPosition(queuePositions[index].position, index == 0);

        OnCustomerChange?.Invoke(CurrentCustomer);
    }
    

}
