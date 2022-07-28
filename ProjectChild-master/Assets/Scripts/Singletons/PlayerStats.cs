using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Different damage types that guns can deal.
/// 
/// This should probably be moved to CharacterParent.
/// </summary>
public enum DamageType { Piercing, Kinetic, Energy };

/// <summary>
/// Holds the stats of the player.
/// </summary>
public class PlayerStats : MonoBehaviour {

    // Make class static and destroy if script already exists
    private static PlayerStats _instance; // **<- reference link to the class
    public static PlayerStats Instance { get { return _instance; } }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private const int XP_MULTIPLIER = 100;

    private HUDCanvas hud;

    public Player player;

    // Stats to save and load
    public bool loadPlayer;

    // Nurturing
    public Stat shieldRecovery { get; private set; }
    public Stat staminaRecovery { get; private set; }
    public Stat ammoRecovery { get; private set; }

    // Rational
    public Stat dodgeRate { get; private set; }
    public Stat criticalRate { get; private set; }
    public Stat rareItemFindRate { get; private set; }

    // Idealistic
    public Stat piercingDmg { get; private set; }
    public Stat kineticDmg { get; private set; }
    public Stat energyDmg { get; private set; }

    // Stoic
    public Stat piercingRes { get; private set; }
    public Stat kineticRes { get; private set; }
    public Stat energyRes { get; private set; }

    // Nihilistic
    public Stat attackSpd { get; private set; }
    public Stat movementSpd { get; private set; }
    public Stat fireRate { get; private set; }

    // Other stats
    private int LEVEL, XP, REDEEMABLE_LEVEL_POINTS;
    // Update canvas UI element when level changes
    public int level { get { return LEVEL; }
        private set { LEVEL = value; hud.AdjustPlayerLevel(LEVEL); } }
    // Update canvas UI element when xp changes
    public int xp { get { return XP; }
        private set { XP = value; hud.AdjustHUDBarXP(lastLevelUpXp, nextLevelUpXp, XP); } }
    // Show canvas UI element when level points are gained
    public int redeemableLevelPoints { get { return REDEEMABLE_LEVEL_POINTS; }
        private set { REDEEMABLE_LEVEL_POINTS = value; hud.CheckRedeemableLevelPoints(); } }

    public int nextLevelUpXp { get; private set; }
    public int lastLevelUpXp { get; private set; }

    void Start() {
        hud = CanvasMaster.Instance.HUDCanvas.GetComponent<HUDCanvas>();
        SetDefaultStats();
    }

    /// <summary>
    /// Sets the starting values for stats.
    /// </summary>
    private void SetDefaultStats() {
        shieldRecovery = new Stat("Shield recovery", Stat.STARTING_STAT, Stat.RECOVERY_MIN_SPEED, Stat.RECOVERY_MAX_SPEED);
        staminaRecovery = new Stat("Stamina recovery", Stat.STARTING_STAT, Stat.RECOVERY_MIN_SPEED, Stat.RECOVERY_MAX_SPEED);
        ammoRecovery = new Stat("Ammo recovery", Stat.STARTING_STAT, Stat.RECOVERY_MIN_SPEED, Stat.RECOVERY_MAX_SPEED);

        dodgeRate = new Stat("Dodge rate", Stat.STARTING_STAT, Stat.DODGE_MIN_PERCENT, Stat.DODGE_MAX_PERCENT);
        criticalRate = new Stat("Critical rate", Stat.STARTING_STAT, Stat.CRITICAL_MIN_PERCENT, Stat.CRITICAL_MAX_PERCENT);
        rareItemFindRate = new Stat("Rare item find rate", Stat.STARTING_STAT, Stat.RARE_FIND_MIN_PERCENT, Stat.RARE_FIND_MAX_PERCENT);

        piercingDmg = new Stat("Piercing damage", Stat.STARTING_STAT, Stat.DAMAGE_MIN_BOOST, Stat.DAMAGE_MAX_BOOST);
        kineticDmg = new Stat("Kinetic damage", Stat.STARTING_STAT, Stat.DAMAGE_MIN_BOOST, Stat.DAMAGE_MAX_BOOST);
        energyDmg = new Stat("Energy damage", Stat.STARTING_STAT, Stat.DAMAGE_MIN_BOOST, Stat.DAMAGE_MAX_BOOST);

        piercingRes = new Stat("Piercing resistance", Stat.STARTING_STAT, Stat.RESISTANCE_MIN_PERCENT, Stat.RESISTANCE_MAX_PERCENT);
        kineticRes = new Stat("Kinetic resistance", Stat.STARTING_STAT, Stat.RESISTANCE_MIN_PERCENT, Stat.RESISTANCE_MAX_PERCENT);
        energyRes = new Stat("Energy resistance", Stat.STARTING_STAT, Stat.RESISTANCE_MIN_PERCENT, Stat.RESISTANCE_MAX_PERCENT);

        attackSpd = new Stat("Attack speed", Stat.STARTING_STAT, Stat.ATTACK_MIN_SPEED, Stat.ATTACK_MAX_SPEED);
        movementSpd = new Stat("Movement speed", Stat.STARTING_STAT, Stat.MOVEMENT_MIN_SPEED, Stat.MOVEMENT_MAX_SPEED);
        fireRate = new Stat("Fire rate", Stat.STARTING_STAT, Stat.FIRE_RATE_MIN_SPEED, Stat.FIRE_RATE_MAX_SPEED);

        level = 1;
        xp = 0;
        nextLevelUpXp = XP_MULTIPLIER;
        lastLevelUpXp = 0;
        redeemableLevelPoints = 0;
    }

    /// <summary>
    /// Saves player's stats to a save object.
    /// </summary>
    /// <param name="save">save object to save to</param>
    public void SavePlayerStats(Save save) {
        save.shieldRecovery = shieldRecovery;
        save.staminaRecovery = staminaRecovery;
        save.ammoRecovery = ammoRecovery;

        save.dodgeRate = dodgeRate;
        save.criticalRate = criticalRate;
        save.rareItemFindRate = rareItemFindRate;

        save.piercingDmg = piercingDmg;
        save.kineticDmg = kineticDmg;
        save.energyDmg = energyDmg;

        save.piercingRes = piercingRes;
        save.kineticRes = kineticRes;
        save.energyRes = energyRes;

        save.attackSpd = attackSpd;
        save.movementSpd = movementSpd;
        save.fireRate = fireRate;

        save.level = level;
        save.xp = xp;
        save.nextLevelUpXp = nextLevelUpXp;
        save.lastLevelUpXp = lastLevelUpXp;
        save.redeemableLevelPoints = redeemableLevelPoints;

        player.SavePlayer(save);
    }

    /// <summary>
    /// Loads player's stats to a save object.
    /// </summary>
    /// <param name="save">save object to load from</param>
    public void LoadPlayerStats(Save save) {
        shieldRecovery.LoadStat(save.shieldRecovery);
        staminaRecovery.LoadStat(save.staminaRecovery);
        ammoRecovery.LoadStat(save.ammoRecovery);

        dodgeRate.LoadStat(save.dodgeRate);
        criticalRate.LoadStat(save.criticalRate);
        rareItemFindRate.LoadStat(save.rareItemFindRate);

        piercingDmg.LoadStat(save.piercingDmg);
        kineticDmg.LoadStat(save.kineticDmg);
        energyDmg.LoadStat(save.energyDmg);

        piercingRes.LoadStat(save.piercingRes);
        kineticRes.LoadStat(save.kineticRes);
        energyRes.LoadStat(save.energyRes);

        attackSpd.LoadStat(save.attackSpd);
        movementSpd.LoadStat(save.movementSpd);
        fireRate.LoadStat(save.fireRate);

        level = save.level;
        xp = save.xp;
        nextLevelUpXp = save.nextLevelUpXp;
        lastLevelUpXp = save.lastLevelUpXp;
        redeemableLevelPoints = save.redeemableLevelPoints;

        // Inform player that it should load it's stuff
        loadPlayer = true;

        RefreshPlayerForStatChanges();
    }

    /// <summary>
    /// Randomizes a gained stat when leveling up.
    /// </summary>
    /// <param name="type">type of level to gain</param>
    public void RandomizeGainedStat(WordsType type) {
        Stat[] stats;
        TopInfoCanvas info = CanvasMaster.Instance.topInfoCanvas;

        // Set correct stats to the array
        switch (type) {
            case WordsType.Nurturing:
                stats = new Stat[] { shieldRecovery, staminaRecovery, ammoRecovery };
                break;
            case WordsType.Rational:
                stats = new Stat[] { dodgeRate, criticalRate, rareItemFindRate };
                break;
            case WordsType.Idealistic:
                stats = new Stat[] { piercingDmg, kineticDmg, energyDmg };
                break;
            case WordsType.Stoic:
                stats = new Stat[] { piercingRes, kineticRes, energyRes };
                break;
            case WordsType.Nihilistic:
                stats = new Stat[] { attackSpd, movementSpd, fireRate };
                break;
            default:
                stats = new Stat[3]; // Not possible, but removes error "unassigned variable"
                break;
        }

        // Increase random stat from the array
        List<int> maxedStats = new List<int>();
        while (true) {

            int index = Random.Range(0, 3);

            // If stat is already checked and maxed out, randomize new index
            if (maxedStats.Contains(index))
                continue;

            Stat stat = stats[index];
            bool increased = stat.IncreaseLevel();

            if (!increased) {
                maxedStats.Add(index);
            } else {
                info.ShowGainStatText(stat);
                break;
            }

            if (maxedStats.Count == stats.Length) {
                // Inform player that all stats are maxed out
                info.ShowStatsMaxedText(type);
                break;
            }
        }

        // Decrease level up points by 1 and check if points left
        redeemableLevelPoints --;
        hud.CheckRedeemableLevelPoints();

        RefreshPlayerForStatChanges();
    }

    private void RefreshPlayerForStatChanges() {
        if (player != null)
            player.RefreshStats();
    }

    /// <summary>
    /// Adds exp to the player.
    /// </summary>
    /// <param name="amount">amount to add</param>
    public void GainXP(int amount) {
        xp += amount;

        // Save current level to memory before leveling up
        int currentLevel = level;

        // Check if you gain level or multiple levels
        while (xp >= nextLevelUpXp)
                LevelUp();

        // If player gained a level, play the level up sound
        // (This can't be played in LevelUp(), since otherwise the
        // sound will play multiple times if player gains multiple levels)
        if (currentLevel != level) {
            CanvasSounds sounds = CanvasMaster.Instance.canvasSounds;
            sounds.PlaySound(sounds.LEVEL_UP);
        }
            

        // Adjust hud bar here too since player is able to gain multiple levels
        hud.AdjustHUDBarXP(lastLevelUpXp, nextLevelUpXp, xp);
    }

    /// <summary>
    /// Uses a toy to gain xp.
    /// </summary>
    /// <param name="toy">toy to use</param>
    public void UseToy(ConsumableSO toy) {
        // Save current level to a variable
        int currentLevel = level;

        // Calculate the percentage to gain
        float fullPercentage = nextLevelUpXp - lastLevelUpXp;
        float xpToGain = fullPercentage * (toy.expToGain / 100);
        GainXP((int)xpToGain);

        if (currentLevel != level) {
            // If player gained a level, use that level to increase stats
            RandomizeGainedStat(toy.toyWordsType);
        } else {
            // Else show info text that player gained xp
            CanvasMaster.Instance.topInfoCanvas.ShowXpPercentageGainedText(toy.expToGain);
        }
    }

    /// <summary>
    /// Gains a level for the player.
    /// </summary>
    private void LevelUp() {
        level ++; // Increase level
        redeemableLevelPoints ++; // Increase level points

        // Get next xp multiplier
        int xpToAdd = level * XP_MULTIPLIER;   // Level 5 = 500;

        // Set last level up xp to current nextlevelupxp
        lastLevelUpXp = nextLevelUpXp;

        // Calculate next level up xp
        nextLevelUpXp += xpToAdd;

        // Check if storage slot is unlocked
        Storage.Instance.CheckStorageUnlock(level);
    }
}
