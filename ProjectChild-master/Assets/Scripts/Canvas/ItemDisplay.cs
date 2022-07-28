using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Displays the info about selected consumable.
/// </summary>
public class ItemDisplay : MonoBehaviour{
    public TextMeshProUGUI selectedItemName, selectedItemDescription;
    public GameObject scannerStats, batteryStats, comsatLinkStats, rigStats, scrapStats, toyStats;

    /// <summary>
    /// Shows the correct info about the provided consumable.
    /// </summary>
    /// <param name="item">selected consumable</param>
    public void ShowItemDisplay(ConsumableSO item) {
        ConsumableStatHolder holder;
        ConsumableType type = item.consumableType;
        ShowCorrectItemStats(type);

        selectedItemName.text = item.name;

        // Set description text and other stat texts based on consumable type
        switch (type) {
            case ConsumableType.Scanner:
                // Set the description text of the consumable
                selectedItemDescription.text = ConsumableSO.DESCRIPTION_SCANNER;
                // Get consumable stat holder from the correct object
                holder = scannerStats.GetComponent<ConsumableStatHolder>();
                // Show required text stats which the consumable has
                holder.identificationChance.text = item.identificationChance.ToString() + "%";
                break;
            case ConsumableType.Battery:
                selectedItemDescription.text = ConsumableSO.DESCRIPTION_BATTERY;
                holder = batteryStats.GetComponent<ConsumableStatHolder>();
                holder.shieldRecoveryPercentage.text = item.shieldRecoveryPercentage.ToString() + "%";
                holder.boostStaminaRecoverySpeed.text = (item.boostStaminaRecoverySpeed * 100).ToString() + "%";
                holder.boostAmmoRecoverySpeed.text = (item.boostAmmoRecoverySpeed * 100).ToString() + "%";
                holder.boostTimeInSeconds.text = item.boostTimeInSeconds.ToString();
                holder.batteryType.text = item.batteryType.ToString();
                break;
            case ConsumableType.ComsatLink:
                selectedItemDescription.text = ConsumableSO.DESCRIPTION_COMSAT_LINK;
                holder = comsatLinkStats.GetComponent<ConsumableStatHolder>();
                holder.chanceToBeSuccessful.text = item.chanceToBeSuccessful.ToString() + "%";
                break;
            case ConsumableType.Rig:
                selectedItemDescription.text = ConsumableSO.DESCRIPTION_RIG;
                holder = rigStats.GetComponent<ConsumableStatHolder>();
                holder.chanceToBeSuccessful.text = item.chanceToBeSuccessful.ToString() + "%";
                break;
            case ConsumableType.Scrap:
                selectedItemDescription.text = ConsumableSO.DESCRIPTION_SCRAP;
                holder = scrapStats.GetComponent<ConsumableStatHolder>();
                holder.creditValue.text = item.creditValue.ToString();
                holder.craftValue.text = item.craftValue.ToString();
                holder.chanceToTurnIntoToy.text = item.chanceToTurnIntoToy.ToString() + "%";
                break;
            case ConsumableType.Toy:
                selectedItemDescription.text = ConsumableSO.DESCRIPTION_TOY;
                holder = toyStats.GetComponent<ConsumableStatHolder>();
                holder.toyWordsType.text = item.toyWordsType.ToString();
                holder.expToGain.text = item.expToGain.ToString() + "%";
                break;
        }
    }

    /// <summary>
    /// Shows and hides item stats which should be shown and hidden.
    /// </summary>
    /// <param name="type">type of stats to show</param>
    private void ShowCorrectItemStats(ConsumableType type) {
        scannerStats.SetActive(type.Equals(ConsumableType.Scanner));
        batteryStats.SetActive(type.Equals(ConsumableType.Battery));
        comsatLinkStats.SetActive(type.Equals(ConsumableType.ComsatLink));
        rigStats.SetActive(type.Equals(ConsumableType.Rig));
        scrapStats.SetActive(type.Equals(ConsumableType.Scrap));
        toyStats.SetActive(type.Equals(ConsumableType.Toy));
    }
}
