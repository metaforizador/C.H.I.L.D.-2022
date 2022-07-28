using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object which holds all the values that an weapon has.
/// </summary>
[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class WeaponSO : PickableSO {

    public AudioClip shootingSound;
    [Header("Only enemies reload their guns")]
    public AudioClip reloadingSound;

    [Header("(Note: starting health and armor is 100, total is 200)")]
    public float damagePerBullet;
    public DamageType weaponType;
    [Header("Meters in second")]
    public float bulletSpeed;
    [Header("Amount of bullets in 1 clip")]
    public int ammoSize;
    [Header("Shoot bullet every {value} second")]
    public float rateOfFire;
    [Header("In seconds, only affects enemies")]
    public float reloadTime;
}
