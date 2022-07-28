using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the content of the storage chest
/// </summary>
public class StorageChest : MonoBehaviour, Chests {

    [SerializeField]
    [Header("Current values: 1 - 6 (Set different value to each chest)")]
    private int storageSlot;

    private PickableSO[] items;
    private int itemAmount = 1;

    void Start() {
        // Create item array so ChangeItems works correctly and it's easier
        // to modify the amount later
        items = new PickableSO[itemAmount];

        // Destroy chest if player has not unlocked enough slots
        if (Storage.Instance.GetUnlockedStorageSlotsAmount() < storageSlot) {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Displays the chest content in ChestCanvas.
    /// </summary>
    public void OpenChest() {
        // Retrieve item from storage. This could be moved to Start(),
        // but for testing purposes it's simpler if it's here.
        items[0] = Storage.Instance.GetFromStorageSlot(storageSlot);
        CanvasMaster.Instance.chestCanvas.ShowChest(this, items);
    }

    /// <summary>
    /// Modifies the chest content.
    /// 
    /// Called when the player retrieves the item
    /// from the chest.
    /// </summary>
    /// <param name="items"></param>
    public void ChangeItems(PickableSO[] items) {
        this.items = items;
    }

    /// <summary>
    /// Removes the item from the storage slot that 
    /// this chest points to.
    /// </summary>
    public void RemoveStorageItem() {
        Storage.Instance.RemoveItemFromStorageSlot(storageSlot);
    }
}
