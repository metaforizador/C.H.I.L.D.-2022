using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates scriptable objects and holds loaded resources.
/// 
/// Creates objects by using their name or their serialized version.
/// </summary>
public class SOCreator : MonoBehaviour {

    // Make class singleton and destroy if script already exists
    private static SOCreator _instance; // **<- reference link to the class
    public static SOCreator Instance { get { return _instance; } }

    private List<PickableSO> weaponsAndArmors = new List<PickableSO>();
    private List<ConsumableSO> consumables = new List<ConsumableSO>();

    private void Awake() {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            // Load all static pickable items at start
            LoadAllPickableItems();
        } else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Loads all the pickable items from memory.
    /// 
    /// Saves the items to variables, so they are needed to be
    /// loaded only once during play session.
    /// </summary>
    private void LoadAllPickableItems() {
        PickableSO[] items = Resources.LoadAll<PickableSO>("ScriptableObjects/PickableItems/");

        foreach (PickableSO item in items) {
            if (item is ConsumableSO) {
                // Instantiate so that if code changes it's values, the Asset values won't get changed
                ConsumableSO inst = Instantiate((ConsumableSO) item);
                consumables.Add(inst);
            } else {
                // Weapons and armors are static at the moment, so they don't need to be instantiated
                weaponsAndArmors.Add(item);
            }
        }
    }

    /// <summary>
    /// Returns all the pickable items.
    /// </summary>
    /// <returns>all pickable items</returns>
    public List<PickableSO> GetAllPickableItems() {
        // Create list and add static pickable items there first
        List<PickableSO> items = weaponsAndArmors;
        // Add randomized consumables to the list
        items.AddRange(GetAllConsumables());

        return items;
    }

    /// <summary>
    /// Returns all the consumables.
    /// </summary>
    /// <returns>randomized consumables</returns>
    public List<ConsumableSO> GetAllConsumables() {
        foreach (ConsumableSO item in consumables) {
            // Randomize consumables values
            item.Initialize(true);
        }

        return consumables;
    }

    /// <summary>
    /// Loads a weapon from assets with a given name.
    /// 
    /// Weapon does not need to be instantiated since none of it's values change from code.
    /// </summary>
    /// <param name="name">name of the weapon</param>
    /// <returns>retrieved weapon</returns>
    public WeaponSO CreateWeapon(SerializablePickableSO serialized) {
        return Resources.Load<WeaponSO>("ScriptableObjects/PickableItems/Weapons/" + serialized.name);
    }

    /// <summary>
    /// Loads a armor from assets with a given name.
    /// 
    /// Armor does not need to be instantiated since none of it's values change from code.
    /// </summary>
    /// <param name="name">name of the armor</param>
    /// <returns> retrieved armor</returns>
    public ArmorSO CreateArmor(SerializablePickableSO serialized) {
        return Resources.Load<ArmorSO>("ScriptableObjects/PickableItems/Armors/" + serialized.name);
    }

    /// <summary>
    /// Creates an instantiated consumable from assets.
    /// </summary>
    /// <param name="serialized">serialized consumable scriptable object</param>
    /// <returns>created consumable</returns>
    public ConsumableSO CreateConsumable(SerializablePickableSO serialized) {
        ConsumableSO con = Instantiate(Resources.Load<ConsumableSO>("ScriptableObjects/PickableItems/Consumables/" + serialized.name));
        con.batteryType = (ConsumableSO.BatteryType)System.Enum.Parse(typeof(ConsumableSO.BatteryType), serialized.batteryTypeString);
        con.toyWordsType = (WordsType)System.Enum.Parse(typeof(WordsType), serialized.toyWordsTypeString);
        con.quantity = serialized.quantity;
        return con;
    }

    /// <summary>
    /// Creates an instantiated consumable from assets.
    /// </summary>
    /// <param name="name">name of the consumable scriptable object</param>
    /// <returns>created consumable</returns>
    public ConsumableSO CreateConsumable(string name) {
        ConsumableSO con = Instantiate(Resources.Load<ConsumableSO>("ScriptableObjects/PickableItems/Consumables/" + name));
        con.Initialize(false);
        return con;
    }
}
