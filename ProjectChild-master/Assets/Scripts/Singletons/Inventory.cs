using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Holds the current inventory that the player has.
/// </summary>
public class Inventory : MonoBehaviour {

    // Make class singleton and destroy if script already exists
    private static Inventory _instance; // **<- reference link to the class
    public static Inventory Instance { get { return _instance; } }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    // Equipped weapon and armor
    public WeaponSO equippedWeapon;
    public ArmorSO equippedArmor;

    private List<ConsumableSO> inventoryConsumables = new List<ConsumableSO>();

    void Start() {
        // Add test items
        List<ConsumableSO> consumables = SOCreator.Instance.GetAllConsumables();

        foreach (ConsumableSO con in consumables)
            AddConsumable(con);
    }

    /// <summary>
    /// Loads inventory from a save object.
    /// </summary>
    /// <param name="save">save to load from</param>
    public void LoadInventory(Save save) {
        SOCreator creator = SOCreator.Instance;
        equippedWeapon = creator.CreateWeapon(save.equippedWeapon);
        equippedArmor = creator.CreateArmor(save.equippedArmor);
        // Load consumables
        inventoryConsumables.Clear();
        foreach (SerializablePickableSO serialized in save.inventoryConsumables) {
            ConsumableSO con = creator.CreateConsumable(serialized);
            inventoryConsumables.Add(con);
        }
    }

    /// <summary>
    /// Saves inventory to a save object.
    /// </summary>
    /// <param name="save">save to save to</param>
    public void SaveInventory(Save save) {
        save.equippedWeapon = new SerializablePickableSO(equippedWeapon);
        save.equippedArmor = new SerializablePickableSO(equippedArmor);
        // Save consumables
        List<SerializablePickableSO> serializableConsumables = new List<SerializablePickableSO>();
        foreach (ConsumableSO con in inventoryConsumables) {
            SerializablePickableSO serialized = new SerializablePickableSO(con);
            serializableConsumables.Add(serialized);
        }
        save.inventoryConsumables = serializableConsumables;
    }

    /// <summary>
    /// Adds a consumable to the inventory.
    /// 
    /// Either adds + 1 quantity to an item or an completely
    /// new item.
    /// </summary>
    /// <param name="consumable">item to add</param>
    public void AddConsumable(ConsumableSO consumable) {
        // If the consumable is already in inventory, add +1 to quantity and return
        foreach (ConsumableSO item in inventoryConsumables) {
            // Check using different equals methods based on ConsumableType
            if (item.EqualsConsumable(consumable)) {
                item.quantity++;
                return;
            }
        }

        // Else add new item
        consumable.quantity = 1;
        inventoryConsumables.Add(consumable);
    }

    /// <summary>
    /// Returns all the consumables in a sorted list.
    /// </summary>
    /// <returns>all consumables from inventory</returns>
    public List<ConsumableSO> GetConsumables() {
        // Sort list by name
        var sortedList = inventoryConsumables.OrderBy(go => go.name).ToList();

        return sortedList;
    }

    /// <summary>
    /// Uses an consumable.
    /// 
    /// Different types consumable types have different
    /// effects when using them.
    /// </summary>
    /// <param name="consumable">consumable to use</param>
    public void UseConsumable(ConsumableSO consumable) {
        bool removeItem = true;
        // Use items (Might have to be moved to somewhere else later)
        switch (consumable.consumableType) {
            case ConsumableType.Battery:
            case ConsumableType.ComsatLink:
            case ConsumableType.Rig:
                Player player = PlayerStats.Instance.player;
                player.UseConsumable(consumable);
                break;
            case ConsumableType.Toy:
                PlayerStats.Instance.UseToy(consumable);
                break;
            case ConsumableType.Scrap:
                if (consumable.CheckIfUsageSuccessful()) {
                    ConsumableSO toy = consumable.ConvertScrapToToy();
                    AddConsumable(toy);
                    CanvasMaster.Instance.topInfoCanvas.ShowScrapToToy(consumable.name, toy.name);
                }
                break;
            case ConsumableType.Scanner:
                removeItem = false; // Don't remove scanner at this point
                CanvasMaster.Instance.itemSelectorCanvas.OpenIdentifyCanvas(consumable);
                break;
        }

        if (removeItem)
            RemoveConsumable(consumable);
    }

    /// <summary>
    /// Removes a consumable from the inventory.
    /// </summary>
    /// <param name="consumable">consumable to remove</param>
    public void RemoveConsumable(ConsumableSO consumable) {
        consumable.quantity--;
        // If all consumables are used, remove item from inventory
        if (consumable.quantity <= 0) {
            inventoryConsumables.Remove(consumable);
        }

        RefreshInventoryItems();
    }

    /// <summary>
    /// Refreshes the inventory and hotbar items.
    /// 
    /// Called when consumables are removed from the inventory.
    /// </summary>
    public void RefreshInventoryItems() {
        CanvasMaster cm = CanvasMaster.Instance;
        cm.hotbarCanvas.RefreshHotbarImages();
        cm.inventoryCanvas.GetComponent<InventoryCanvas>().RefreshConsumables();
    }
}
