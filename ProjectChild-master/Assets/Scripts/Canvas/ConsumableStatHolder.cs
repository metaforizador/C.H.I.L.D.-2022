using UnityEngine;
using TMPro;

/// <summary>
/// Holds all the stats that Consumables has.
/// 
/// It's used for displaying the stats in UI text at
/// chests and at inventory.
/// </summary>
public class ConsumableStatHolder : MonoBehaviour{
    public TextMeshProUGUI
        // Scanner
        identificationChance,

        // Battery
        batteryType, shieldRecoveryPercentage, boostStaminaRecoverySpeed, boostAmmoRecoverySpeed, boostTimeInSeconds,

        // Comsat Link & Rig
        chanceToBeSuccessful,

        // Scrap
        chanceToTurnIntoToy, creditValue, craftValue,

        // Toy
        toyWordsType, expToGain;
}
