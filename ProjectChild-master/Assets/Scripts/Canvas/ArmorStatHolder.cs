using UnityEngine;
using TMPro;

/// <summary>
/// Holds all the stats that Armors has.
/// 
/// It's used for displaying the stats in UI text at
/// chests and at inventory.
/// </summary>
public class ArmorStatHolder : MonoBehaviour {
    public TextMeshProUGUI name, decreaseShieldDelay, increaseShield, lowerOpponentsCritChance,
        lowerOpponentsCritMultiplier, decreaseMovementSpeed, increaseStaminaRecoveryDelay;
}
