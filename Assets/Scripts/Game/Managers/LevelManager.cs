using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

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
        
        _currentDay = PlayerPrefs.GetInt("CurrentDay", 0);
    }

    private int _currentDay = 0;

    [ShowInInspector]
    public int CurrentDay => _currentDay;

    private readonly Dictionary<DeliveryRating, int> _deliveries =
        Enum.GetValues(typeof(DeliveryRating))
            .Cast<DeliveryRating>()
            .ToDictionary(r => r, r => 0);

    public Dictionary<DeliveryRating, int> Deliveries => _deliveries;

    public void AddDelivery(bool weight, bool weapon)
    {
        if (weight && weapon)
        {
            _deliveries[DeliveryRating.Good]++;
        }
        else if (weight || weapon)
        {
            _deliveries[DeliveryRating.Neutral]++;
        }
        else
        {
            _deliveries[DeliveryRating.Bad]++;
        }
    }

    public void AdvanceToNextDay()
    {
        _currentDay++;
        PlayerPrefs.SetInt("CurrentDay", _currentDay);
    }


}

public enum DeliveryRating
{
    Good,
    Neutral,
    Bad
}