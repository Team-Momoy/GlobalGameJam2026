using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Weapon/WeaponRecipe")]
public class WeaponRecipeSO : ScriptableObject
{
    public Weapon Weapon;
    public Sprite Sprite;
    public int HitsNeeded;
    public SerializedDictionary<MaterialType, int> MaterialsNeeded;
}
