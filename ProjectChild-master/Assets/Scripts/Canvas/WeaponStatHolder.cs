using UnityEngine;
using TMPro;

/// <summary>
/// Holds all the stats that Weapons has.
/// 
/// It's used for displaying the stats in UI text at
/// chests and at inventory.
/// </summary>
public class WeaponStatHolder : MonoBehaviour {
    public TextMeshProUGUI name, type, damage, bulletSpeed, ammoSize, rateOfFire;
}
