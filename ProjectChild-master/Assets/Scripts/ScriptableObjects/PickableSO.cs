using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Use this to save and load PickableSO items.
/// </summary>
[System.Serializable]
public class SerializablePickableSO {

    // Empty is needed for save and load to function correctly
    public const int EMPTY = -1, WEAPON_TYPE = 0, ARMOR_TYPE = 1, CONSUMABLE_TYPE = 2;

    public string name;
    public int itemType;

    /*  CONSUMABLES
        All variables that are changed from code needs to be added here
        in order to saving and loading work correctly. ConsumableSO will
        be generated in SOCreator class, so if you add new variables,
        remember to add them there too. Also remember to add them to the
        ConsumableSO.Equals() so that items with different values will
        show correctly in inventory.
     */
    public int quantity;
    public string batteryTypeString;
    public string toyWordsTypeString;

    /// <summary>
    /// Creates an empty serializable object.
    /// 
    /// Used when saving items to storage, since these
    /// values can't be null for some reason.
    /// </summary>
    public SerializablePickableSO() {
        itemType = EMPTY;
    }

    /// <summary>
    /// Creates an serializable version of an item.
    /// 
    /// Sets values based on the type of item.
    /// </summary>
    /// <param name="item">item to serialize</param>
    public SerializablePickableSO(PickableSO item) {
        name = item.name;
        if (item is WeaponSO) {
            itemType = WEAPON_TYPE;
        } else if (item is ArmorSO) {
            itemType = ARMOR_TYPE;
        } else if (item is ConsumableSO) {
            itemType = CONSUMABLE_TYPE;
            ConsumableSO con = (ConsumableSO)item;
            quantity = con.quantity;
            batteryTypeString = con.batteryType.ToString();
            toyWordsTypeString = con.toyWordsType.ToString();
        }
    }
}

/// <summary>
/// All the different conditions that the items can have.
/// </summary>
public enum Condition { Volatile, Damaged, Intact, Supercharged }

/// <summary>
/// Scriptable object parent which holds all the values that
/// item scriptable objects share.
/// </summary>
public class PickableSO : ScriptableObject {

    public new string name;
    public Sprite sprite;
    public Condition condition;

    // Use item's name as id
    public bool EqualsItem(PickableSO otherItem) {
        return this.name.Equals(otherItem.name);
    }
}
