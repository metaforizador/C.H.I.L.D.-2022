using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Interface for different kinds of chests, which share the same ChestCanvas.
/// 
/// Holds all the methods that the chests need to have in order for this
/// canvas to work correctly.
/// </summary>
public interface Chests {
    void OpenChest();
    void ChangeItems(PickableSO[] items);
}

/// <summary>
/// Displays the chest's content in UI.
/// 
/// Used for displaying a normal chest content and
/// a storage chest content. It also modifies chest
/// content when needed.
/// </summary>
public class ChestCanvas : MonoBehaviour {

    public GameObject buttonLayout;
    public Button itemPrefab;

    private Chests openedChest;

    public TextMeshProUGUI currentItemView, foundItemView, swapAndUseView, storageAmount;
    private const string SWAP_TEXT = "Swap", USE_TEXT = "Use", EQUIP_TEXT = "Equip";

    private bool weaponPickupSoundPlayed;

    private List<Button> createdItemButtons = new List<Button>();

    public GameObject itemSelectedObject;
    private PickableSO[] items;
    private int selectedItemIndex;
    private PickableSO selectedItem;

    public GameObject buttons;
    public Button collectButton, storageButton;

    // Weapon and armor stats
    public GameObject weaponStatsPrefabLeft, weaponStatsPrefabRight, armorStatsPrefabLeft, armorStatsPrefabRight;
    public GameObject currentItemStats, selectedItemStats;
    public GameObject currentItemButton, foundItemButton;

    // Consumables
    public GameObject itemDisplayObject;
    public ItemDisplay itemDisplay;

    /// <summary>
    /// Shows the content of the chest.
    /// </summary>
    /// <param name="chest">opened chest</param>
    /// <param name="items">items that the chest holds</param>
    public void ShowChest(Chests chest, PickableSO[] items) {
        // Close if already open
        if (gameObject.activeSelf) {
            CloseChest();
            return;
        }

        // Change game state
        GameMaster.Instance.SetState(GameState.Chest);

        // Play pickup sound only once per opening a chest
        weaponPickupSoundPlayed = false;

        this.openedChest = chest;
        this.items = items;

        gameObject.SetActive(true);

        RefreshChestCanvasContent();
    }

    private void RefreshChestCanvasContent() {
        // Don't show any item info when chest opens
        itemSelectedObject.SetActive(false);

        // Remove old buttons
        foreach (Button btn in createdItemButtons) {
            Destroy(btn.gameObject);
        }
        createdItemButtons.Clear();

        // Create a button for all of the chest items
        for (int i = 0; i < items.Length; i++) {
            PickableSO item = items[i];

            if (item != null) {
                Button button = Instantiate(itemPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                // Add button to the list and set the scale to 1 (parent.transform changes it to around 0,6)
                button.transform.SetParent(buttonLayout.transform);
                button.transform.localScale = Vector3.one;

                // Set name or image to the button
                if (item.sprite == null)
                    button.GetComponentInChildren<TextMeshProUGUI>().text = item.name;
                else
                    button.GetComponent<Image>().sprite = item.sprite;

                createdItemButtons.Add(button);

                // Set button click listener
                int index = i;  // Local variable needed because of for loop
                button.onClick.AddListener(() => ItemSelected(index));
            }
        }
    }

    /// <summary>
    /// Called when item is selected.
    /// 
    /// Opens item select elements and changes texts.
    /// </summary>
    /// <param name="index"></param>
    private void ItemSelected(int index) {
        // Clear old stats if exists
        foreach (Transform child in currentItemStats.transform)
            Destroy(child.gameObject);
        foreach (Transform child in selectedItemStats.transform)
            Destroy(child.gameObject);

        // Save index to local variable
        selectedItemIndex = index;
        selectedItem = items[index];

        itemSelectedObject.SetActive(true); // Activate item select elements

        // Change storage button amount text
        Storage storage = Storage.Instance;
        int unlocked = storage.GetUnlockedStorageSlotsAmount();
        storageAmount.text = $"{unlocked - storage.EmptyStorageSlotsAmount()} / {unlocked}";

        // Set item texts and stats
        if (selectedItem is WeaponSO) {
            currentItemView.text = PlayerStats.Instance.player.GetWeapon().name;
            ShowWeaponStats();
        } else if (selectedItem is ArmorSO) {
            currentItemView.text = PlayerStats.Instance.player.GetArmor().name;
            ShowArmorStats();
        } else if (selectedItem is ConsumableSO) {
            itemDisplay.ShowItemDisplay((ConsumableSO) selectedItem);
        }

        ToggleItemTypeStuff();
    }

    /// <summary>
    /// Toggles all necessary item type UI elements.
    /// 
    /// Weapons and armors show different UI elements than consumables
    /// and storage chest modifies some UI elements compared to a
    /// normal chest.
    /// </summary>
    private void ToggleItemTypeStuff() {
        bool isConsumable = selectedItem is ConsumableSO;

        // Enable / disable correct objects
        itemDisplayObject.SetActive(isConsumable);
        currentItemButton.SetActive(!isConsumable);
        foundItemButton.SetActive(!isConsumable);
        
        // Determine swap / use / equip button text
        if (openedChest is Chest) {
            swapAndUseView.text = isConsumable ? USE_TEXT : SWAP_TEXT;
        } else if (openedChest is StorageChest) {
            swapAndUseView.text = isConsumable ? USE_TEXT : EQUIP_TEXT;
        }

        // Enable / disable correct buttons
        collectButton.interactable = isConsumable ? true : false;
        storageButton.interactable = openedChest is StorageChest ? false : true;

        if (!isConsumable) {
            foundItemView.text = selectedItem.name;     // Change found item text
        }
    }

    /// <summary>
    /// Displays weapon stats for current and found weapon.
    /// </summary>
    private void ShowWeaponStats() {
        // Setup current weapon stats
        WeaponStatHolder holder = Helper.Instance.CreateObjectChild(weaponStatsPrefabLeft, currentItemStats).
            GetComponent<WeaponStatHolder>();
        WeaponSO weapon = PlayerStats.Instance.player.GetWeapon();
        Helper.Instance.SetupWeaponStats(holder, weapon);

        // Setup found weapon stats
        holder = Helper.Instance.CreateObjectChild(weaponStatsPrefabRight, selectedItemStats).
            GetComponent<WeaponStatHolder>();
        weapon = (WeaponSO) items[selectedItemIndex];
        Helper.Instance.SetupWeaponStats(holder, weapon);
    }

    /// <summary>
    /// Displays armor stats for current and found armor.
    /// </summary>
    private void ShowArmorStats() {
        // Setup current armor stats
        ArmorStatHolder holder = Helper.Instance.CreateObjectChild(armorStatsPrefabLeft, currentItemStats).
            GetComponent<ArmorStatHolder>();
        ArmorSO armor = PlayerStats.Instance.player.GetArmor();
        Helper.Instance.SetupArmorStats(holder, armor);

        // Setup found armor stats
        holder = Helper.Instance.CreateObjectChild(armorStatsPrefabRight, selectedItemStats).
            GetComponent<ArmorStatHolder>();
        armor = (ArmorSO)items[selectedItemIndex];
        Helper.Instance.SetupArmorStats(holder, armor);
    }

    /// <summary>
    /// Adds the selected consumable to the inventory.
    /// </summary>
    public void CollectItem() {
        Inventory.Instance.AddConsumable((ConsumableSO) selectedItem);      // Add item
        CanvasMaster.Instance.topInfoCanvas.ShowItemCollected(selectedItem);// Show top info about item collected
        RemoveItemFromChest();                                              // Removes the item from chest
    }

    /// <summary>
    /// Swaps player's weapon or armor or uses an consumable.
    /// </summary>
    public void SwapOrUseItem() {
        if (selectedItem is ConsumableSO) {
            // Use item
            Inventory.Instance.UseConsumable((ConsumableSO) selectedItem);
            RemoveItemFromChest();
        } else {
            // Swap item
            Player player = PlayerStats.Instance.player;

            PickableSO oldItem = null;

            // Check the type of PickableSO
            if (selectedItem is WeaponSO) {
                oldItem = player.ChangeWeapon((WeaponSO)selectedItem);

                // Play a weapon pickup sound if not yet played
                if (!weaponPickupSoundPlayed) {
                    weaponPickupSoundPlayed = true;
                    PlayerSounds.Instance.PlayWeaponPickup();
                }
            } else if (selectedItem is ArmorSO) {
                oldItem = player.ChangeArmor((ArmorSO)selectedItem);
            }

            // Replace selected item with the player's old item
            items[selectedItemIndex] = oldItem;

        }

        // If openedChest is StorageChest, leave the slot empty and remove item from Storage
        if (openedChest is StorageChest) {
            items[selectedItemIndex] = null;
            StorageChest sto = (StorageChest)openedChest;
            sto.RemoveStorageItem();
        }

        RefreshChestContents();
    }

    /// <summary>
    /// Sends the item to storage.
    /// </summary>
    public void StorageClicked() {
        // If storage is full, exit
        if (Storage.Instance.EmptyStorageSlotsAmount() == 0) {
            CanvasMaster.Instance.topInfoCanvas.ShowStorageFull();
            return;
        }

        CanvasMaster.Instance.itemSelectorCanvas.OpenSendItemToStorageCanvas(selectedItem);
    }

    public void RemoveItemFromChest() {
        items[selectedItemIndex] = null;
        RefreshChestContents();
    }

    private void RefreshChestContents() {
        // Change item contents inside the chest
        openedChest.ChangeItems(items);

        // Refresh chest canvas contents
        RefreshChestCanvasContent();
    }

    /// <summary>
    /// Hide selected item info.
    /// </summary>
    public void Cancel() {
        itemSelectedObject.SetActive(false);
    }

    /// <summary>
    /// Closes the chest view.
    /// </summary>
    public void CloseChest() {
        // Change game state
        GameMaster.Instance.SetState(GameState.Movement);

        gameObject.SetActive(false);
    }
}
