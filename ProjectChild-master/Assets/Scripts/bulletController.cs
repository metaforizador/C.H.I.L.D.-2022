using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all necessary values that bullets need to carry.
/// </summary>
public class bulletController : MonoBehaviour {

    public CharacterParent.CharacterType shooter { get; private set; }
    public float damage { get; private set; }
    public float criticalRate { get; private set; }
    public DamageType damageType { get; private set; }

    public float secondsAlive = 10;
    private float aliveCounter = 0;

    /// <summary>
    /// Initializes the bullet with necessary values.
    /// </summary>
    /// <param name="shooter">was the shooter player or enemy</param>
    /// <param name="damage">damage the bullet deals</param>
    /// <param name="criticalRate">current calculated critical rate</param>
    /// <param name="damageType">type of the damage</param>
    public void Initialize(CharacterParent.CharacterType shooter, float damage, float criticalRate, DamageType damageType) {
        this.shooter = shooter;
        this.damage = damage;
        this.criticalRate = criticalRate;
        this.damageType = damageType;
    }

    // Update is called once per frame
    void Update()
    {
        aliveCounter += Time.deltaTime;

        if(aliveCounter > secondsAlive)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }
}
