using System;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponRequest", menuName = "Weapon/WeaponRequest")]
public class WeaponRequestSO : ScriptableObject
{
    public SerializedDictionary<Weapon, WeaponWeight> WeightByWeapon;

    [SerializeField]
    public SerializedDictionary<int, Weapon[]> WeaponsByDay;
    public SerializedDictionary<Weapon, Sprite> WeaponSprites;

    [Serializable]
    public class WeaponWeight
    {
        public int MinWeight;
        public int MaxWeight;
    }
}

