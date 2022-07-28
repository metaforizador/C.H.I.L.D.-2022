using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all the values which needs to be saved.
/// </summary>
[System.Serializable]
public class Save {
    //////// Global ////////
    public string sceneName;
    public SerializableVector3 playerPosition;
    public SerializableQuaternion playerQuaternion;

    //////// Player stats ////////
    // Nurturing
    public Stat shieldRecovery, staminaRecovery, ammoRecovery;
    // Rational
    public Stat dodgeRate, criticalRate, rareItemFindRate;
    // Idealistic
    public Stat piercingDmg, kineticDmg, energyDmg;
    // Stoic
    public Stat piercingRes, kineticRes, energyRes;
    // Nihilistic
    public Stat attackSpd, movementSpd, fireRate;
    // Other stats
    public int level, xp, nextLevelUpXp, lastLevelUpXp, redeemableLevelPoints;

    //////// Questions and answers ////////
    // Mood = mood for question, List<string> = questions
    public Dictionary<Mood, List<string>> askedQuestions;
    // WordsType = type of reply, List<string> = replies
    public Dictionary<WordsType, List<string>> givenReplies;

    //////// Inventory ////////
    public SerializablePickableSO equippedWeapon;
    public SerializablePickableSO equippedArmor;
    public List<SerializablePickableSO> inventoryConsumables;

    //////// Hotbar ////////
    public SerializablePickableSO[] hotbarConsumables;

    //////// Storage ////////
    public int unlockedStorageSlotsAmount;
    public List<SerializablePickableSO> storageContent;
}
