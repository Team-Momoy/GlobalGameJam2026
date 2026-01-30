using System;
using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }

    public int CurrentGold { get; private set; }

    public event Action<int> OnGoldChanged;

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

    public bool TrySpend(int amount)
    {
        if (CurrentGold < amount) return false;
        CurrentGold -= amount;
        OnGoldChanged?.Invoke(CurrentGold);
        return true;
    }

    public void AddGold(int amount)
    {
        CurrentGold += amount;
        OnGoldChanged?.Invoke(CurrentGold);
    }
}
