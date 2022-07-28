using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Shows a scrollable list of items in the middle of the screen.
/// 
/// Currently this class is used for showing the identifiable items
/// (Batteries) when player uses a Scanner and Comsat Links when player
/// is sending an item to the storage.
/// </summary>
public class ItemSelectorCanvas : MonoBehaviour {

    [SerializeField]
    private ConsumablesScrollSystem consumablesScrollSystem;

    [SerializeField]
    private TextMeshProUGUI headerText;

    private const string IDENTIFY_HEADER = "Select which item to identify:";
    private const string STORAGE_HEADER = "Select Comsat Link to use:";

    // Identify items
    private ConsumableSO usedScanner;

    // Send items to storage
    private PickableSO itemToSend;

    private GameState previousGameState;

    /// <summary>
    /// Sets up all the stuff which is needed when opening the canvas.
    /// 
    /// No matter which type of items are shown, they all need to change
    /// these values.
    /// </summary>
    /// <returns>all the consumables from the inventory</returns>
    private List<ConsumableSO> OpenThisCanvas() {
        gameObject.SetActive(true);

        // Change previous state to a variable so that after
        // this canvas is closed the correct state can be chosen
        previousGameState = GameMaster.Instance.gameState;
        GameMaster.Instance.SetState(GameState.ItemSelector);

        // Clear old items from the scroll system
        consumablesScrollSystem.ClearAllItems();

        // Load all consumables from inventory
        return Inventory.Instance.GetConsumables();
    }

    /// <summary>
    /// Displays all the identifiable items.
    /// 
    /// Called when using a scanner as a consumable.
    /// </summary>
    /// <param name="usedScanner">used scanner</param>
    public void OpenIdentifyCanvas(ConsumableSO usedScanner) {
        List<ConsumableSO> consumables = OpenThisCanvas();
        this.usedScanner = usedScanner;
        headerText.text = IDENTIFY_HEADER;

        // Get only items which are identifiable
        List<ConsumableSO> identifiableItems = new List<ConsumableSO>();
        foreach (ConsumableSO con in consumables) {
            if (con.consumableType.Equals(ConsumableType.Battery)) {
                if (con.batteryType.Equals(ConsumableSO.BatteryType.Unknown)) {
                    identifiableItems.Add(con);
                }
            }
        }

        // If there are no identifiable items, close the canvas
        if (identifiableItems.Count == 0) {
            CanvasMaster.Instance.topInfoCanvas.ShowIdentifiableEmpty();
            CloseItemSelectorCanvas();
            return;
        }

        // Add items to the scroll system and listen for their clicks
        foreach (ConsumableSO con in identifiableItems) {
            consumablesScrollSystem.AddItem(con)
                .onClick.AddListener(() => IdentifyItem(con));
        }
    }

    /// <summary>
    /// Displays all the Comsat Links.
    /// 
    /// Called when player tries to send an item to the storage.
    /// </summary>
    /// <param name="itemToSend">item to send</param>
    public void OpenSendItemToStorageCanvas(PickableSO itemToSend) {
        List<ConsumableSO> consumables = OpenThisCanvas();
        this.itemToSend = itemToSend;
        headerText.text = STORAGE_HEADER;

        // Get only items which are used for sending items to storage
        List<ConsumableSO> comsatLinks = new List<ConsumableSO>();
        foreach (ConsumableSO con in consumables) {
            if (con.consumableType.Equals(ConsumableType.ComsatLink)) {
                comsatLinks.Add(con);
            }
        }

        // If player does not have any comsat links, close the item selector
        if (comsatLinks.Count == 0) {
            CanvasMaster.Instance.topInfoCanvas.ShowComsatLinkEmpty();
            CloseItemSelectorCanvas();
            return;
        }

        // Add items to the scroll system and listen for their clicks
        foreach (ConsumableSO con in comsatLinks) {
            consumablesScrollSystem.AddItem(con)
                .onClick.AddListener(() => SendItemToStorage(con));
        }
    }

    /// <summary>
    /// Uses the scanner and tries to identify an item.
    /// </summary>
    /// <param name="item">item to identify</param>
    private void IdentifyItem(ConsumableSO item) {
        Inventory inv = Inventory.Instance;

        // Check if usage was successful
        if (usedScanner.CheckIfUsageSuccessful()) {
            // Remove the item from the inventory
            inv.RemoveConsumable(item);

            // Change battery type and add it as a new item
            item.DetermineFinalBatteryType();
            inv.AddConsumable(item);

            // Show info about battery change
            CanvasMaster.Instance.topInfoCanvas.ShowBatteryIdentified(item.batteryType.ToString());
        }

        // Remove the scanner from inventory and close the canvas
        inv.RemoveConsumable(usedScanner);
        CloseItemSelectorCanvas();
    }

    /// <summary>
    /// Uses the comsat link and tries to send the item to the storage.
    /// </summary>
    /// <param name="comsatLink">used comsat link</param>
    private void SendItemToStorage(ConsumableSO comsatLink) {
        Inventory inv = Inventory.Instance;
        CanvasMaster cv = CanvasMaster.Instance;

        // Check if usage was successful
        if (comsatLink.CheckIfUsageSuccessful()) {
            // Remove the item from the chest
            cv.chestCanvas.RemoveItemFromChest();

            // Add item to the storage
            Storage.Instance.AddToStorage(itemToSend);

            // Show info about sending the item
            cv.topInfoCanvas.ShowItemSentToStorage(itemToSend.name);
        }

        // Remove the comsat link from inventory and close the canvas
        inv.RemoveConsumable(comsatLink);
        CloseItemSelectorCanvas();
    }

    /// <summary>
    /// Closes this canvas.
    /// </summary>
    public void CloseItemSelectorCanvas() {
        GameMaster.Instance.SetState(previousGameState);
        gameObject.SetActive(false);

        // Refreshes inventory for changes
        Inventory.Instance.RefreshInventoryItems();
    }
}
