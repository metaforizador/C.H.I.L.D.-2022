using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays everything that is only visible when the
/// player opens the inventory.
/// </summary>
public class InventoryCanvas : MonoBehaviour {

    public GameObject menuAndCategories, categoriesParent, openedCategoryParent;
    public GameObject weaponObj, armorObj, consumablesObj, miscObj;
    private GameObject[] categoryObjects;
    private float objCategoryStartX = -90;
    private Vector3 scaledMenuAndCategories = new Vector3(0.7f, 0.7f, 0.7f);

    private UIAnimator animator;

    // DELETE LATER
    public GameObject openDebugMenuButton;
    public GameObject debugMenu;

    public void ToggleDebugMenu() {
        debugMenu.SetActive(!debugMenu.activeSelf);
    }

    private bool menuOpen = false;
    private int currentlyOpen;

    private const int NONE = 0, WEAPON = 1, ARMOR = 2, CONSUMABLES = 3, MISC = 4;

    // Categories
    public GameObject weaponStatsObject, armorStatsObject, consumablesObject;

    // ConsumableItems
    public ConsumablesScrollSystem consumableScrollSystem;
    public GameObject itemStatsDisplay;
    private ConsumableSO selectedItem;
    public ItemDisplay itemDisplay;
    private HotbarCanvas hotbar;

    private CanvasSounds sounds;

    public LeanTweenType tweenType;
    private float tweenTime = 0.35f;

    void Awake() {
        // Retrieve required references
        CanvasMaster cv = CanvasMaster.Instance;
        animator = cv.uiAnimator;
        sounds = cv.canvasSounds;
        hotbar = cv.hotbarCanvas;

        // Hide debug menu stuff
        debugMenu.SetActive(false);
        openDebugMenuButton.SetActive(false);

        // Hide categories
        categoriesParent.SetActive(false);
        categoryObjects = new GameObject[] { weaponObj, armorObj, consumablesObj, miscObj };

        foreach (GameObject obj in categoryObjects) {
            obj.transform.LeanMoveLocalX(objCategoryStartX, 0f);
            obj.transform.localScale = Vector3.zero;
        }

        ShowRequiredCategory(NONE);
    }

    /// <summary>
    /// Shows and hides all of the categories.
    /// </summary>
    public void ToggleMenu() {
        // Loop through all categories and animate them
        foreach (GameObject obj in categoryObjects) {
            if (!menuOpen) {
                // Open menu
                animator.MoveX(obj, 0, tweenTime, tweenType);
                animator.Scale(obj, Vector3.one, tweenTime, tweenType);
            } else {
                // Close menu
                animator.Scale(obj, Vector3.zero, tweenTime, tweenType);
                animator.MoveX(obj, objCategoryStartX, tweenTime, tweenType);
            }
        }

        // Toggle menu state
        menuOpen = !menuOpen;

        GameMaster gm = GameMaster.Instance;

        if (menuOpen) {
            // Play sound when opening the menu
            sounds.PlaySound(sounds.BUTTON_SELECT);
            openDebugMenuButton.SetActive(true);
            categoriesParent.SetActive(true);
            gm.SetState(GameState.Menu);
        } else {
            // Play sound when closing the menu and hide opened categories
            sounds.PlaySound(sounds.BUTTON_BACK);
            ShowRequiredCategory(NONE);
            // Change state and hide debug stuff
            Helper.Instance.InvokeRealTime(() => {
                categoriesParent.SetActive(false);
                gm.SetState(GameState.Movement);
                openDebugMenuButton.SetActive(false);
                debugMenu.SetActive(false);
            }, tweenTime);
        }
    }

    /// <summary>
    /// Shows and hides weapon stats information.
    /// </summary>
    public void ToggleWeapon() {
        ShowRequiredCategory(WEAPON);
        
        WeaponStatHolder holder = weaponStatsObject.GetComponent<WeaponStatHolder>();
        WeaponSO weapon = Inventory.Instance.equippedWeapon;
        Helper.Instance.SetupWeaponStats(holder, weapon);
    }

    /// <summary>
    /// Shows and hides armor stats information.
    /// </summary>
    public void ToggleArmor() {
        ShowRequiredCategory(ARMOR);

        ArmorStatHolder holder = armorStatsObject.GetComponent<ArmorStatHolder>();
        ArmorSO armor = Inventory.Instance.equippedArmor;
        Helper.Instance.SetupArmorStats(holder, armor);
    }

    /// <summary>
    /// Shows and hides consumables in inventory.
    /// </summary>
    public void ToggleConsumables() {
        // Destroy all previous consumable item buttons
        consumableScrollSystem.ClearAllItems();

        // Get consumables from inventory
        List<ConsumableSO> consumables = Inventory.Instance.GetConsumables();
        foreach (ConsumableSO con in consumables) {
            consumableScrollSystem.AddItem(con).
                // Show item information when clicked
                onClick.AddListener(() => ShowItemInfo(con));
        }

        ShowRequiredCategory(CONSUMABLES);
        itemStatsDisplay.SetActive(false); // Disable item stats display since item is not yet chosen
    }

    /// <summary>
    /// Closes all sub categories.
    /// 
    /// Close button calls this method.
    /// </summary>
    public void CloseSubcategory() {
        ShowRequiredCategory(NONE);
    }

    /// <summary>
    /// Shows item's information.
    /// </summary>
    /// <param name="con">item to get the information from</param>
    private void ShowItemInfo(ConsumableSO con) {
        // Activate correct objects
        itemStatsDisplay.SetActive(true);
        // Set selected item for equip and use buttons
        selectedItem = con;
        // Set item info
        itemDisplay.ShowItemDisplay(selectedItem);
    }

    /// <summary>
    /// Displays the info of the item equipped to the hotbar.
    /// 
    /// Player opens this if he clicks the hotbar button or
    /// presses the hotbar number button when inside the inventory.
    /// </summary>
    /// <param name="consumable">consumable item to display</param>
    public void ShowHotbarItemInfo(ConsumableSO consumable) {
        // If consumables subcaterogy is not open at the moment, open it
        if (currentlyOpen != CONSUMABLES) {
            ToggleConsumables();
        }

        ShowItemInfo(consumable);
    }

    /// <summary>
    /// Opens the hot bar panel for equipping a consumable to the hotbar.
    /// 
    /// Called when the player has an consumable info open and clicks
    /// the "Equip" button.
    /// </summary>
    public void EquipItem() {
        hotbar.SetIncomingItem(selectedItem);
    }

    /// <summary>
    /// Uses the displayed item.
    /// 
    /// Called when the player has an consumable info open and clicks
    /// the "Use" button.
    /// </summary>
    public void UseItem() {
        Inventory.Instance.UseConsumable(selectedItem);
    }

    /// <summary>
    /// Refresh consumables when they are used.
    /// 
    /// Called from Inventory singleton.
    /// </summary>
    public void RefreshConsumables() {
        // Refresh only if menu is open
        if (GameMaster.Instance.gameState.Equals(GameState.Menu)) {
            ShowRequiredCategory(NONE);
            ToggleConsumables();

            // If there is still the selectedItem left, open it again
            if (selectedItem.quantity > 0)
                ShowItemInfo(selectedItem);
            else
                selectedItem = null;
        }
    }

    /// <summary>
    /// Scales menu and categories smaller or to default size.
    /// </summary>
    /// <param name="smaller">whether to scale smaller or not</param>
    private void ScaleMenuAndCategories(bool smaller) {
        if (smaller)
            animator.Scale(menuAndCategories, scaledMenuAndCategories, tweenTime, tweenType);
        else
            animator.Scale(menuAndCategories, Vector3.one, tweenTime, tweenType);
    }

    /// <summary>
    /// Shows and hides correct categories.
    /// </summary>
    /// <param name="category">category which should be open</param>
    private void ShowRequiredCategory(int category) {
        // If element is none, reset menu and categories scale
        if (category == NONE) {
            ScaleMenuAndCategories(false);
        } else {
            // Else category is opened, so play sound
            ScaleMenuAndCategories(true);
        }

        currentlyOpen = category;

        // Set objects active based on if they should be currently open
        openedCategoryParent.SetActive(currentlyOpen != NONE);
        weaponStatsObject.SetActive(currentlyOpen == WEAPON);
        armorStatsObject.SetActive(currentlyOpen == ARMOR);
        consumablesObject.SetActive(currentlyOpen == CONSUMABLES);
    }
}
