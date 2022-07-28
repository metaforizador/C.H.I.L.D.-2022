using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All the consumable types which consumables can be.
/// </summary>
public enum ConsumableType { Scanner, Battery, ComsatLink, Rig, Scrap, Toy }

/// <summary>
/// Scriptable object which holds all the values that an consumable has.
/// </summary>
[CreateAssetMenu(fileName = "New Consumable", menuName = "Consumable")]
public class ConsumableSO : PickableSO {

    public ConsumableType consumableType;
    public int quantity;

    /************ SCANNER ************/
    public const string DESCRIPTION_SCANNER = "As a consumable, Scanner can identify items.It can also be used " +
                    "on certain stations to display the location of enemies, loot in a given map and traps.";
    [Range(0f, 100f)]
    public float identificationChance;

    /************ BATTERY ************/
    public const string DESCRIPTION_BATTERY = "As a consumable, it can be used to restore either shields or increase " +
                    "the speed of stamina recovery or ammo recovery (same type speeds do not stack). To know which one of those three the battery will increase, it " +
                    "needs to be identified with a scanner.";

    public enum BatteryType { Unknown, Shield, Stamina, Ammo }

    public BatteryType batteryType = BatteryType.Unknown;
    [Range(0f, 100f)]
    public float shieldRecoveryPercentage;
    [Range(1.1f, 3f)]
    public float boostStaminaRecoverySpeed = 1.1f, boostAmmoRecoverySpeed = 1.1f;
    [Range(1, 60)]
    public int boostTimeInSeconds = 1;

    /// <summary>
    /// Determines the final battery type of the item.
    /// 
    /// Randomizes the battery type if it was not already set.
    /// </summary>
    public void DetermineFinalBatteryType() {
        // If batterytype is not unknown, it's already set
        if (!batteryType.Equals(BatteryType.Unknown))
            return;

        // Else batterytype needs to be randomized
        int randomNumber = Random.Range(1, 100);
        if (randomNumber <= 33)
            this.batteryType = BatteryType.Shield;
        else if (randomNumber <= 66)
            this.batteryType = BatteryType.Stamina;
        else
            this.batteryType = BatteryType.Ammo;
    }

    /************ COMSAT LINK & RIG ************/
    public const string DESCRIPTION_COMSAT_LINK = "As a consumable, it can call up an airstrike. It can also be used on item " +
                    "chests to send the found item back to the home base so the player can start the game with it." +
                    "When used in level exists, it can work as a town portal to send the player back to the beginning " +
                    "of the game with all his money and stats (items are lost, though).";
    public const string DESCRIPTION_RIG = "As a consumable, it repairs some HP. On stations, it can also be used to upgrade " +
                    "weapons and armor.";
    public const int RIG_HP_TO_RECOVER_PERCENTAGE = 50;
    [Range(0f, 100f)]
    public float chanceToBeSuccessful;

    /************ SCRAP ************/
    public const string DESCRIPTION_SCRAP = "As a consumable, using it has a chance of turning it into a toy, which gives XP to the " +
                    "child. Success replaces item 'Scrap' with the item 'Toy'. On stations, it can be used to craft new weapons " +
                    "or armor and in vendors, turned in for credits. Credits allow the purchase of items on vendor. Once the scrap " +
                    "gets turned into a toy, it can no longer be used on stations or sold.";
    [Range(0f, 100f)]
    public float chanceToTurnIntoToy;
    public int creditValue;
    [Range(1, 4)]
    public int craftValue = 1;

    public ConsumableSO ConvertScrapToToy() {
        // Get toy with same condition as the scrap
        string cond = name.Substring(name.IndexOf(' ') + 1);
        string condName = "Toy " + cond;
        return SOCreator.Instance.CreateConsumable(condName);
    }

    /************ TOY ************/
    public const string DESCRIPTION_TOY = "As a consumable, it gives percentage amount of exp needed for the next level. If the player " +
                    "gains a level when using the toy, he will gain a stat boost determined by the type of the toy.";
    [Range(0f, 100f)]
    public float expToGain;
    public WordsType toyWordsType;

    /************ GLOBAL METHODS ************/

    /// <summary>
    /// Initializes randomized values on certain items.
    /// 
    /// Force is used mainly in SOCreator so that consumables does not need to be
    /// loaded every single time again from resources.
    /// </summary>
    /// <param name="force">true if to randomize regardless if they are already randomized</param>
    public void Initialize(bool force) {
        switch (consumableType) {
            case ConsumableType.Toy:
                // Randomize toy type if it's None
                if (force || toyWordsType.Equals(WordsType.None)) {
                    toyWordsType = (WordsType)Random.Range(1, System.Enum.GetValues(typeof(WordsType)).Length);
                }
                break;
        }
    }

    /// <summary>
    /// Checks if item is basically the same item (might have different pointer).
    /// 
    /// Uses name for the check in most cases.
    /// </summary>
    /// <param name="otherItem">item to compare with</param>
    /// <returns>true if it's the same</returns>
    public bool EqualsConsumable(ConsumableSO otherItem) {
        if (consumableType.Equals(ConsumableType.Battery)) {
            // Check battery type too if item is battery
            return this.name.Equals(otherItem.name) && this.batteryType.Equals(otherItem.batteryType);
        } else if (consumableType.Equals(ConsumableType.Toy)) {
            // Check toy words type too if item is toy
            return this.name.Equals(otherItem.name) && this.toyWordsType.Equals(otherItem.toyWordsType);
        }

        // Else the name check is enough
        return this.name.Equals(otherItem.name);
    }

    /// <summary>
    /// Checks if using the consumable was a success or not.
    /// </summary>
    /// <returns>true if it was successful</returns>
    public bool CheckIfUsageSuccessful() {
        float chance = 0f;

        // Check using different variables based on the consumable type
        if (consumableType.Equals(ConsumableType.ComsatLink) || consumableType.Equals(ConsumableType.Rig))
            chance = chanceToBeSuccessful;
        else if (consumableType.Equals(ConsumableType.Scrap))
            chance = chanceToTurnIntoToy;
        else if (consumableType.Equals(ConsumableType.Scanner))
            chance = identificationChance;

        if (Helper.CheckPercentage(chance))
            return true;

        // Else show item broke info
        CanvasMaster.Instance.topInfoCanvas.ShowItemBrokeText(this.name);
        return false;
    }
}
