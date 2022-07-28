using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Adds consumables to a scrollables system.
/// 
/// Used in InventoryCanvas and ItemSelectorCanvas.
/// </summary>
public class ConsumablesScrollSystem : MonoBehaviour {

    [SerializeField]
    private GameObject consumableContent, consumableItemPrefab;

    /// <summary>
    /// Destroy all previous consumable item buttons.
    /// </summary>
    public void ClearAllItems() {
        foreach (Transform child in consumableContent.transform)
            Destroy(child.gameObject);
    }

    /// <summary>
    /// Add an item to the content.
    /// </summary>
    /// <param name="con">consumable item to add</param>
    /// <returns>The created button so it can be listened</returns>
    public Button AddItem(ConsumableSO con) {
        // Create button for each consumable
        GameObject btn = Helper.Instance.CreateObjectChild(consumableItemPrefab, consumableContent);
        // Add name and quantity to the consumable
        btn.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = con.name;
        btn.transform.Find("Quantity").GetComponent<TextMeshProUGUI>().text = con.quantity.ToString();
        // Show item information when clicked
        return btn.GetComponent<Button>();
    }
}
