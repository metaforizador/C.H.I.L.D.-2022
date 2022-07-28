using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the stats of the characters.
/// </summary>
[System.Serializable]
public class Stat {
    // Level of the stat is the only value which needs to be saved
    public int level { get; private set; }

    // Constant default values
    public const int STARTING_STAT = 0, MAX_BASE_STAT_VALUES = 15;
    public const float RECOVERY_DELAY = 0.2f;
    public const float CRITICAL_HIT_MULTIPLIER = 2; // Doubles the damage
    public const float BASE_MOVEMENT_SPEED = 6f;

    // Real min and max values for stats
    public const float RECOVERY_MIN_SPEED = 1, RECOVERY_MAX_SPEED = 4;
    public const float RESISTANCE_MIN_PERCENT = 0, RESISTANCE_MAX_PERCENT = 60;
    public const float DAMAGE_MIN_BOOST = 1, DAMAGE_MAX_BOOST = 2.5f;
    public const float DODGE_MIN_PERCENT = 10, DODGE_MAX_PERCENT = 50;
    public const float CRITICAL_MIN_PERCENT = 10, CRITICAL_MAX_PERCENT = 75;
    public const float RARE_FIND_MIN_PERCENT = 0, RARE_FIND_MAX_PERCENT = 80;
    public const float ATTACK_MIN_SPEED = 1, ATTACK_MAX_SPEED = 2.5f;
    public const float FIRE_RATE_MIN_SPEED = 1, FIRE_RATE_MAX_SPEED = 2.5f;
    public const float MOVEMENT_MIN_SPEED = 1, MOVEMENT_MAX_SPEED = 2f;

    // Name of the stat
    [System.NonSerialized]
    public string name;

    // Current value of the stat which gets affected by the level
    [System.NonSerialized]
    private float CurrentValue;
    public float currentValue { get { return CurrentValue; } }

    // Used for calculating current value of the stat
    [System.NonSerialized]
    private float minValue, maxValue, valuePerLevel;

    public Stat(string name, int level, float minValue, float maxValue) {
        this.name = name;
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.valuePerLevel = (maxValue - minValue) / MAX_BASE_STAT_VALUES;
        SetLevel(level);
    }

    /// <summary>
    /// Loads necessary values from the serialized stat.
    /// </summary>
    /// <param name="loadFrom">Loaded stat</param>
    public void LoadStat(Stat loadFrom) {
        this.level = loadFrom.level;
    }

    /// <summary>
    /// Sets current level and calculates currentValue based on level.
    /// </summary>
    /// <param name="level">Level to set</param>
    private void SetLevel(int level) {
        this.level = level;
        this.CurrentValue = CalculateValue(this.minValue, this.maxValue, this.level);
    }

    /// <summary>
    /// Calculates the current value of stat.
    /// </summary>
    /// <param name="min">minimum allowed value</param>
    /// <param name="max">maximum allowed value</param>
    /// <param name="level">current level of the stat</param>
    /// <returns></returns>
    public static float CalculateValue(float min, float max, int level) {
        float valuePerLevel = (max - min) / MAX_BASE_STAT_VALUES;
        return min + (valuePerLevel * level);
    }

    /// <summary>
    /// Increases level of the stat if it is not full.
    /// </summary>
    /// <returns>True if level was not full before increasing</returns>
    public bool IncreaseLevel() {
        if (this.level == MAX_BASE_STAT_VALUES) {
            return false;
        }

        SetLevel(this.level + 1);
        return true;
    }
}
