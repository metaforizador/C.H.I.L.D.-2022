using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the content inside chests.
/// </summary>
public class Chest : MonoBehaviour, Chests {

    private PickableSO[] items;
    private int maxItems = 4;

    private const int VOLATILE_VALUE = 20, DAMAGED_VALUE = 50, INTACT_VALUE = 80;

    void Start() {
        // Randomize chest contents
        int itemAmount = Random.Range(1, maxItems + 1); // + 1 because max is not inclusive

        // Randomize condition
        Condition condition;
        float number = Random.Range(1, 101);
        number += PlayerStats.Instance.rareItemFindRate.currentValue;

        if (number <= VOLATILE_VALUE) {
            condition = Condition.Volatile;
        } else if (number <= DAMAGED_VALUE) {
            condition = Condition.Damaged;
        } else if (number <= INTACT_VALUE) {
            condition = Condition.Intact;
        } else {
            condition = Condition.Supercharged;
        }

        // Create list and add all items with correct condition to it
        List<PickableSO> pickList = new List<PickableSO>();

        // Load all pickable items from resources
        List<PickableSO> loaded = SOCreator.Instance.GetAllPickableItems();

        // Filter list based on condition
        foreach (PickableSO item in loaded) {
            if (item.condition.Equals(condition)) {
                pickList.Add(item);
            }
        }

        // Create new items array based on random size
        items = new PickableSO[itemAmount];

        // Add random items to the chest items array
        for (int i = 0; i < itemAmount; i++) {
            int randomIndex = Random.Range(0, pickList.Count);
            PickableSO pickable = pickList[randomIndex];
            items[i] = pickable;
            pickList.Remove(pickable);
        }
    }

    /// <summary>
    /// Opens a chestCanvas which displays the chest content.
    /// </summary>
    public void OpenChest() {
        CanvasMaster.Instance.chestCanvas.ShowChest(this, items);
    }

    /// <summary>
    /// Changes items inside the chest.
    /// 
    /// Used when player swaps, picks up or uses an item.
    /// </summary>
    /// <param name="items">modified items array</param>
    public void ChangeItems(PickableSO[] items) {
        this.items = items;
    }
}
