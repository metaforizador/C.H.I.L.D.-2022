using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the player's values.
/// </summary>
public class Player : CharacterParent {

    // References to classes
    private GameMaster gm;
    private CanvasMaster cm;
    private HotbarCanvas hotbar;
    private Inventory inventory;
    private PlayerStats stats;
    private PlayerSounds sounds;

    private Stat[] recoveryStats;

    // Testing purposes
    public float testDamageKeyU = 20;
    public int testXpKeyX = 100;

    // Checking trigger presses to avoid "input not always registering"
    private Collider triggerCollider;

    public override void Start() {
        // Retrieve references
        gm = GameMaster.Instance;
        cm = CanvasMaster.Instance;
        hotbar = cm.hotbarCanvas;
        stats = PlayerStats.Instance;
        sounds = PlayerSounds.Instance;
        inventory = Inventory.Instance;

        // Change state to movement when the player spawns
        gm.SetState(GameState.Movement);
        characterType = CharacterType.Player;
        
        stats.player = this;    // Add this player to singleton variable for better access

        // Get weapon and stats from the singleton stats
        ChangeWeapon(inventory.equippedWeapon);
        ChangeArmor(inventory.equippedArmor);

        RefreshStats();

        base.Start();

        // Load player stuff if game is loaded
        if (stats.loadPlayer) {
            stats.loadPlayer = false;   // Inform that player has loaded
            LoadPlayer(gm.latestSave);  // Load player stuff
        }

        // Show player's hud
        hud.gameObject.SetActive(true);
    }

    /// <summary>
    /// Refreshes the stats of the player.
    /// 
    /// Called when for example the player changes weapon or armor
    /// or gains a level.
    /// </summary>
    public void RefreshStats() {
        shieldRecovery = stats.shieldRecovery.currentValue;
        staminaRecovery = stats.staminaRecovery.currentValue;
        ammoRecovery = stats.ammoRecovery.currentValue;

        dodgeRate = stats.dodgeRate.currentValue;
        criticalRate = stats.criticalRate.currentValue;
        rareItemFindRate = stats.rareItemFindRate.currentValue;

        piercingDmg = stats.piercingDmg.currentValue;
        kineticDmg = stats.kineticDmg.currentValue;
        energyDmg = stats.energyDmg.currentValue;

        piercingRes = stats.piercingRes.currentValue;
        kineticRes = stats.kineticRes.currentValue;
        energyRes = stats.energyRes.currentValue;

        attackSpd = stats.attackSpd.currentValue;
        movementSpd = stats.movementSpd.currentValue;
        fireRate = stats.fireRate.currentValue;

        // Some armor values are affected by stats
        RetrieveArmorValues();
    }

    /// <summary>
    /// Saves the player values.
    /// </summary>
    /// <param name="save">save object to save to</param>
    public void SavePlayer(Save save) {
        // Save position
        save.playerPosition = transform.position;
        save.playerQuaternion = transform.rotation;
    }

    /// <summary>
    /// Loads the player values.
    /// </summary>
    /// <param name="save">save object to load from</param>
    public void LoadPlayer(Save save) {
        // Load position
        transform.position = save.playerPosition;
        transform.rotation = save.playerQuaternion;
    }

    /// <summary>
    /// Changes the weapon for the player.
    /// </summary>
    /// <param name="weapon">weapon to change to</param>
    /// <returns>old weapon</returns>
    public override WeaponSO ChangeWeapon(WeaponSO weapon) {
        inventory.equippedWeapon = weapon;  // Add weapon to singleton inventory
        return base.ChangeWeapon(weapon);
    }

    /// <summary>
    /// Changes the armor for the player.
    /// </summary>
    /// <param name="armor">armor to change to</param>
    /// <returns>old armor</returns>
    public override ArmorSO ChangeArmor(ArmorSO armor) {
        inventory.equippedArmor = armor;    // Add armor to singleton inventory
        return base.ChangeArmor(armor);
    }

    protected override void Update() {
        base.Update();

        bool inputEnabled = gm.gameState.Equals(GameState.Movement);

        // Check trigger interact presses
        if (triggerCollider != null && Input.GetButtonDown("Interact") && inputEnabled) {
            if (triggerCollider.CompareTag("Chest")) {
                triggerCollider.GetComponent<Chests>().OpenChest();
            }
        }

        // Debug tests
        if (Application.isEditor) {
            // Test taking damage
            if (Input.GetKeyDown(KeyCode.U) && inputEnabled) {
                TakeDamage(DamageType.Piercing, testDamageKeyU, 0);
            }

            // Test gaining xp
            if (Input.GetKeyDown(KeyCode.X) && inputEnabled) {
                stats.GainXP(testXpKeyX);
            }

            // Test saving
            if (Input.GetKeyDown(KeyCode.O) && inputEnabled) {
                GameMaster.Instance.SaveGame();
            }

            // Test loading
            if (Input.GetKeyDown(KeyCode.P) && inputEnabled) {
                GameMaster.Instance.LoadGame();
            }
        }

        // Check hotbar presses
        bool hotbarInputEnabled = gm.gameState.Equals(GameState.Movement) ||
            gm.gameState.Equals(GameState.Menu) || gm.gameState.Equals(GameState.Hotbar);

        if (hotbarInputEnabled) {
            int hotbarButtonAmount = hotbar.hotbarButtonAmount;
            for (int i = 1; i <= hotbarButtonAmount; ++i) {
                if (Input.GetKeyDown("" + i)) {
                    hotbar.HotbarButtonClicked(i - 1); // index is 1 lower than button number
                }
            }
        }
    }

    /// <summary>
    /// Uses a consumable.
    /// 
    /// Some of the items are used here instead of Inventory,
    /// since they are so heavily tied to player's values.
    /// </summary>
    /// <param name="consumable"></param>
    public void UseConsumable(ConsumableSO consumable) {
        TopInfoCanvas info = CanvasMaster.Instance.topInfoCanvas;

        switch (consumable.consumableType) {
            /************ BATTERY ************/
            case ConsumableType.Battery:
                consumable.DetermineFinalBatteryType();
                switch (consumable.batteryType) {
                    case ConsumableSO.BatteryType.Shield:
                        float amount = consumable.shieldRecoveryPercentage;
                        info.ShowShieldRecoveredText(amount);
                        SHIELD += amount;
                        break;
                    case ConsumableSO.BatteryType.Stamina:
                        float staminaAmount = consumable.boostStaminaRecoverySpeed;
                        float staminaTime = consumable.boostTimeInSeconds;
                        info.ShowBoostText("stamina", staminaAmount, staminaTime);
                        BoostRecovery(B_STAMINA, staminaAmount, staminaTime);
                        break;
                    case ConsumableSO.BatteryType.Ammo:
                        float ammoAmount = consumable.boostAmmoRecoverySpeed;
                        float ammoTime = consumable.boostTimeInSeconds;
                        info.ShowBoostText("ammo", ammoAmount, ammoTime);
                        BoostRecovery(B_AMMO, ammoAmount, ammoTime);
                        break;
                }
                break;
            /************ COMSAT LINK ************/
            case ConsumableType.ComsatLink:
                if (consumable.CheckIfUsageSuccessful()) {
                    // Call an airstrike
                }
                break;
            /************ RIG ************/
            case ConsumableType.Rig:
                if (consumable.CheckIfUsageSuccessful()) {
                    info.ShowHealthRecoveredText();
                    HP += ConsumableSO.RIG_HP_TO_RECOVER_PERCENTAGE;
                }
                break;
        }
    }

    /// <summary>
    /// Plays the correct sound when taking damage.
    /// </summary>
    /// <param name="type">type of damage</param>
    /// <param name="amount">amount of damage</param>
    /// <param name="criticalRate">critical rate of damage</param>
    protected override void TakeDamage(DamageType type, float amount, float criticalRate) {
        // Save hp to a variable before taking damage
        float hpBefore = HP;

        // Call the base method which might adjust hp
        base.TakeDamage(type, amount, criticalRate);

        // Quit method if hp did not take damage
        if (hpBefore == HP) {
            return;
        } else {
            for (int i = 0; i < PlayerSounds.HIT_SPEECH_PERCENTAGES.Length; i++) {
                int percentage = PlayerSounds.HIT_SPEECH_PERCENTAGES[i];

                if (hpBefore > percentage && HP <= percentage) {
                    sounds.PlayHealthLowAudio(i);
                    return;
                }
            }

            // If code makes it this far, then hit speech was not played,
            // so play a hit grunt sound
            sounds.PlayRandomTakeHitGrunt();
        }
    }

    /// <summary>
    /// Shows the game over canvas when the player dies.
    /// </summary>
    protected override void Die() {
        base.Die();
        cm.ShowGameOverCanvas(true);
    }

    /**************** Collisions ****************/

    /// <summary>
    /// Handles collisions with enemy bullets.
    /// </summary>
    /// <param name="collision">happened collision</param>
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Bullet")) {
            bulletController bullet = collision.gameObject.GetComponent<bulletController>();
            if (bullet.shooter == CharacterType.Enemy) {
                TakeDamage(bullet.damageType, bullet.damage, bullet.criticalRate);
            }
        }
    }

    /// <summary>
    /// Handles trigger enter events with chests.
    /// </summary>
    /// <param name="collider">happened trigger collision</param>
    void OnTriggerEnter(Collider collider) {
        triggerCollider = collider;

        if (collider.CompareTag("Chest")) {
            hud.ShowInteract(HUDCanvas.CHEST_OPEN);
        }
    }

    /// <summary>
    /// Handles trigger exit events with chests.
    /// </summary>
    /// <param name="collider">exited trigger collision</param>
    void OnTriggerExit(Collider collider) {
        triggerCollider = null;

        if (collider.CompareTag("Chest")) {
            // Hide the interact panel
            hud.HideInteract();

            // Close the chest
            // (this is not basically needed anymore, since the
            // player is not able to move when the chest is open)
            cm.chestCanvas.CloseChest();
        }
    }
}
