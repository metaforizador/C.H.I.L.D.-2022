using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all the variables and methods which both the
/// player and the enemies share.
/// </summary>
public class CharacterParent : MonoBehaviour {

    public enum CharacterType { Player, Enemy };

    // Temporary stats
    private float hp, shield, stamina, ammo;

    /********* ADJUST DIFFERENT HUD BAR COMPONENTS WHEN VALUES CHANGE *********/
    public float HP { get { return hp; }
        protected set {
            hp = value;
            if (characterType == CharacterType.Player)
                hud.AdjustHUDBar(hud.hpBar, hp);
        }
    }

    public float SHIELD {
        get { return shield; }
        protected set {
            shield = value;
            if (characterType == CharacterType.Player)
                hud.AdjustHUDBarShield(hud.shieldBar, maxShield, SHIELD);
        }
    }

    public float STAMINA {
        get { return stamina; }
        protected set {
            stamina = value;
            if (characterType == CharacterType.Player)
                hud.AdjustHUDBar(hud.staminaBar, stamina);
        }
    }

    public float AMMO {
        get { return ammo; }
        protected set {
            ammo = value;
            if (characterType == CharacterType.Player)
                hud.AdjustAmmoAmount(weapon.ammoSize, ammo);
        }
    }
    /************************************************************************/

    protected float shieldRecovery, staminaRecovery, ammoRecovery, dodgeRate, criticalRate,
        rareItemFindRate, piercingDmg, kineticDmg, energyDmg, piercingRes, kineticRes, energyRes,
        attackSpd, movementSpd, fireRate;

    protected bool alive;
    public bool shooting;

    private const float MAX_VALUE = 500;
    public float maxShield { get; private set; }

    // Movement speed when taking armor values to account
    public float movementSpeedMultiplier { get; private set; }

    // Delay values
    private const float RECOVERY_DELAY_TIME = 1.5f;
    private const float MELEE_DELAY_TIME = 3f;
    private const int D_SHIELD = 0, D_STAMINA = 1, D_MELEE = 2;
    private float[] delays = new float[] { 0, 0, 0 };

    // Stamina and ammo boosts
    private const int DEFAULT_BOOST = 1;
    protected const int B_STAMINA = 0, B_AMMO = 1;
    private float[] boostMultiPlier = new float[] { DEFAULT_BOOST, DEFAULT_BOOST };
    private float[] boostTime = new float[] { 0, 0 };

    protected CharacterType characterType;

    protected HUDCanvas hud;

    // Playing audio
    [SerializeField]
    private AudioSource audioSource;

    // Weapon and armor
    [SerializeField]
    private WeaponSO weapon = null;
    [SerializeField]
    private ArmorSO armor = null;

    public WeaponSO GetWeapon() => weapon;
    public ArmorSO GetArmor() => armor;

    // Weapon values
    private AudioClip weaponShootingSound;
    private AudioClip weaponReloadSound;
    private float weaponDamage;
    private DamageType weaponType;
    private float weaponBulletSpeed;
    private float weaponBulletConsumption;
    private float weaponRateOfFire;
    private float weaponReloadTime;
    // Weapon prefab stuff
    public GameObject weaponBullet;
    private List<GameObject> bulletPoints = new List<GameObject>();

    // Armor values
    private float armorDecreaseShieldRecoveryDelay;
    private float armorDecreaseOpponentCriticalRate;
    private float armorDecreaseOpponentCriticalMultiplier;
    private float armorIncreaseStaminaRecoveryDelay;

    public virtual void Start() {
        hud = CanvasMaster.Instance.HUDCanvas.GetComponent<HUDCanvas>();
        // Create audio component and add preset
        audioSource = gameObject.GetComponent<AudioSource>();
        // Retrieve bullet points
        foreach (GameObject bulletPoint in GameObject.FindGameObjectsWithTag("BulletPoint"))
        {
            if (bulletPoint.transform.IsChildOf(this.transform))
            {
                bulletPoints.Add(bulletPoint);
            }
        }

        RetrieveWeaponValues();
        RetrieveArmorValues();
        ResetValues();
    }

    /// <summary>
    /// Retrieves values from weapon.
    /// 
    /// These variables could possibly be removed and just access the values
    /// through the weapon.value instead when needed.
    /// </summary>
    private void RetrieveWeaponValues() {
        weaponShootingSound = weapon.shootingSound;
        weaponReloadSound = weapon.reloadingSound;
        weaponDamage = weapon.damagePerBullet;
        weaponType = weapon.weaponType;
        weaponBulletSpeed = weapon.bulletSpeed;
        weaponBulletConsumption = 100 / weapon.ammoSize; // Gets percentage
        weaponRateOfFire = weapon.rateOfFire;
        weaponReloadTime = weapon.reloadTime;
    }

    /// <summary>
    /// Retrieves values from armor.
    /// 
    /// These variables are good to have, since that way you don't
    /// have to do a null check over every value. You could also
    /// make an empty armor object, which has default values.
    /// </summary>
    protected void RetrieveArmorValues() {
        maxShield = MAX_VALUE;
        movementSpeedMultiplier = movementSpd;

        if (armor != null) {
            armorDecreaseShieldRecoveryDelay = armor.decreaseShieldRecoveryDelay / 100;
            maxShield += armor.increaseShield;
            armorDecreaseOpponentCriticalRate = armor.decreaseOpponentCriticalRate;
            armorDecreaseOpponentCriticalMultiplier = armor.decreaseOpponentCriticalMultiplier / 100;
            // Divide by 100 to make for example 80% to 0.8, then multiply by base value
            movementSpeedMultiplier = movementSpd - ((armor.reduceMovementSpeed / 100) * movementSpd);
            // Divide by 100 to make for example 80% to 0.8, then multiply by base value
            armorIncreaseStaminaRecoveryDelay = (armor.increaseStaminaRecoveryDelay / 100) * RECOVERY_DELAY_TIME;
        }
    }

    /// <summary>
    /// Resets necessary values when spawning.
    /// </summary>
    private void ResetValues() {
        alive = true;

        HP = MAX_VALUE;
        SHIELD = maxShield;
        STAMINA = MAX_VALUE;
        AMMO = MAX_VALUE;

        StartCoroutine(RestoreRecoveries());
        StartCoroutine(Shooting());
    }

    /// <summary>
    /// Changes the weapon.
    /// </summary>
    /// <param name="weapon">weapon to change to</param>
    /// <returns>the old weapon</returns>
    public virtual WeaponSO ChangeWeapon(WeaponSO weapon) {
        WeaponSO oldWeapon = this.weapon;
        this.weapon = weapon;
        RetrieveWeaponValues();

        return oldWeapon;
    }

    /// <summary>
    /// Changes the armor.
    /// </summary>
    /// <param name="armor">armor to change to</param>
    /// <returns>the old armor</returns>
    public virtual ArmorSO ChangeArmor(ArmorSO armor) {
        ArmorSO oldArmor = this.armor;
        this.armor = armor;
        RetrieveArmorValues();

        return oldArmor;
    }

    /// <summary>
    /// Boosts a recovery value for a certain amount and time.
    /// </summary>
    /// <param name="boostType">recovery to boost</param>
    /// <param name="multiplier">amount to boost</param>
    /// <param name="time">time that the boost lasts</param>
    protected void BoostRecovery(int boostType, float multiplier, float time) {
        boostMultiPlier[boostType] = multiplier;
        boostTime[boostType] = time;
    }

    protected virtual void Update() {
        // Count down different delay times
        for (int i = 0; i < delays.Length; i++) {
            if (delays[i] > 0)
                delays[i] -= Time.deltaTime;
        }

        // Count down stamina and ammo recovery boost times
        for (int i = 0; i < boostTime.Length; i++) {
            if (boostTime[i] > 0) {
                boostTime[i] -= Time.deltaTime;

                // Reset multiplier if boost time ends
                if (boostTime[i] <= 0)
                    boostMultiPlier[i] = DEFAULT_BOOST;
            }
        }
    }

    /// <summary>
    /// Restores recoverable value according to provided time delay.
    /// </summary>
    /// <returns>IEnumerator</returns>
    IEnumerator RestoreRecoveries() {
        while (alive) {
            // Recover shield
            if (SHIELD < maxShield && delays[D_SHIELD] <= 0) {
                SHIELD += shieldRecovery;

                if (SHIELD > maxShield)
                    SHIELD = maxShield;
            }

            // Recover stamina
            if (STAMINA < MAX_VALUE && delays[D_STAMINA] <= 0) {
                STAMINA += staminaRecovery * boostMultiPlier[B_STAMINA];

                if (STAMINA > MAX_VALUE)
                    STAMINA = MAX_VALUE;
            }

            // Recover ammo
            if (AMMO < MAX_VALUE && characterType == CharacterType.Player) {
                AMMO += ammoRecovery * boostMultiPlier[B_AMMO];

                if (AMMO > MAX_VALUE)
                    AMMO = MAX_VALUE;
            }

            yield return new WaitForSeconds(Stat.RECOVERY_DELAY);
        }
    }

    IEnumerator Shooting() {
        while (alive) {
            if (shooting && (AMMO >= weaponBulletConsumption)) {
                // Decrease ammo by bullet consumption amount
                AMMO -= weaponBulletConsumption;

                // Make a shooting sound
                audioSource.PlayOneShot(weaponShootingSound);

                // Create the bullet, calculate damage and initialize necessary values
                GameObject thisBullet = Instantiate(weaponBullet);
                float damage = CalculateBulletDamage();
                thisBullet.GetComponent<bulletController>().Initialize(characterType, damage, criticalRate, weaponType);

                // Set bulletDirection towards crosshair point for the player / towards player for enemies
                Vector3 bulletDirection = transform.forward;

                GameObject bulletPoint = bulletPoints[0];

                if (bulletPoints.Count > 1)
                {
                    for(int i = 0; i < bulletPoints.Count; i++)
                    {
                        if (bulletPoints[i].GetComponent<bulletPoint>().lastShot)
                        {
                            bulletPoints[i].GetComponent<bulletPoint>().lastShot = false;

                            if((i + 1) < bulletPoints.Count)
                            {
                                bulletPoint = bulletPoints[i + 1];
                                bulletPoints[i + 1].GetComponent<bulletPoint>().lastShot = true;
                            }
                            else
                            {
                                bulletPoint = bulletPoints[0];
                                bulletPoints[0].GetComponent<bulletPoint>().lastShot = true;
                            }

                            break;
                        }
                    }
                }
                 
                if (characterType == CharacterType.Player)
                {
                    Vector3 crosshairPoint = new Vector3(0, 0, 0);

                    RaycastHit hit;
                    Ray ray = GameObject.Find("Main Camera").GetComponent<Camera>().ScreenPointToRay(GameObject.Find("Crosshair").transform.position);

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~9))
                    {
                        crosshairPoint = hit.point;
                        bulletDirection = crosshairPoint - bulletPoint.transform.position;
                        Debug.Log(hit.collider.gameObject.name);
                    }
                    else
                    {
                        crosshairPoint = ray.GetPoint(1000);
                        bulletDirection = crosshairPoint - bulletPoint.transform.position;
                    }
                }
                else if (characterType == CharacterType.Enemy)
                {
                    //bulletDirection = GameObject.Find("Player").transform.position - bulletPoint.transform.position;
                    bulletDirection = transform.forward;
                }

                // Set bullets position and speed
                thisBullet.transform.position = bulletPoint.transform.position;
                thisBullet.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, bulletDirection, 100, 0.0f));
                thisBullet.GetComponent<Rigidbody>().velocity = bulletDirection.normalized * weaponBulletSpeed;

                // Enemies reload weapons when they run out of ammo
                if (characterType == CharacterType.Enemy && AMMO < weaponBulletConsumption) {
                    audioSource.PlayOneShot(weaponReloadSound);
                    Invoke("reloadAmmo", weaponReloadTime);
                }

                yield return new WaitForSeconds(weaponRateOfFire / fireRate); // Shorten delay by fire rate
            }
            yield return 0;
        }
    }

    /// <summary>
    /// Needed for enemy to invoke ammo reload.
    /// </summary>
    private void reloadAmmo() {
        AMMO = 100;
    }

    /// <summary>
    /// Calculates if character has enough stamina to do the action.
    /// 
    /// If there is enough stamina, it will be used.
    /// </summary>
    /// <param name="amount">amount of stamina the action uses</param>
    /// <returns>true if there is enough</returns>
    public bool IsEnoughStamina(float amount) {
        if (STAMINA >= amount) {
            STAMINA -= amount;
            // Add delay to shield recovery
            delays[D_STAMINA] = RECOVERY_DELAY_TIME + armorIncreaseStaminaRecoveryDelay;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if last melee hit delay has ended.
    /// </summary>
    /// <returns>true if character is able to melee</returns>
    public bool IsAbleToMelee() {
        if (delays[D_MELEE] <= 0) {
            delays[D_MELEE] = MELEE_DELAY_TIME / attackSpd;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Calculates the bullet damage based on type.
    /// </summary>
    /// <returns>calculated bullet damage</returns>
    private float CalculateBulletDamage() {
        float damageToCause = weaponDamage;

        // Add percentage to damage based on damage stats
        switch (weaponType) {
            case DamageType.Piercing:
                damageToCause *= piercingDmg;
                break;
            case DamageType.Kinetic:
                damageToCause *= kineticDmg;
                break;
            case DamageType.Energy:
                damageToCause *= energyDmg;
                break;
        }

        return damageToCause;
    }

    /// <summary>
    /// Takes damage to character.
    /// </summary>
    /// <param name="type">type of damage to take</param>
    /// <param name="amount">amount of damage to take</param>
    /// <param name="criticalRate">critical rate that the damage has</param>
    protected virtual void TakeDamage(DamageType type, float amount, float criticalRate) {
        // Check if damage got dodged
        if (Helper.CheckPercentage(dodgeRate)) {
            Debug.Log("Dodged");
            return;
        }

        // Check if it was a critical hit, take account the armor's critical modifiers
        if (Helper.CheckPercentage(criticalRate - armorDecreaseOpponentCriticalRate)) {
            Debug.Log("Critical hit");
            amount *= (Stat.CRITICAL_HIT_MULTIPLIER - armorDecreaseOpponentCriticalMultiplier);
        }

        // Add delay to shield recovery
        delays[D_SHIELD] = RECOVERY_DELAY_TIME - (RECOVERY_DELAY_TIME * armorDecreaseShieldRecoveryDelay);

        // Calculate resistance to given damage type
        switch (type) {
            case DamageType.Piercing:
                amount -= amount * (piercingRes / 100);
                break;
            case DamageType.Kinetic:
                amount -= amount * (kineticRes / 100);
                break;
            case DamageType.Energy:
                amount -= amount * (energyRes / 100);
                break;
        }

        // If shield is left, take damage to shield
        if (SHIELD > 0) {
            SHIELD -= amount;

            // If shield went under 0, add the left over damage to hp
            if (SHIELD < 0) {
                amount = Mathf.Abs(SHIELD);
                SHIELD = 0;
            } else {
                amount = 0;
            }
        }

        HP -= amount;

        if (HP <= 0)
            Die();
    }

    public void meleeHit(float damageAmount, float criticalRate)
    {
        TakeDamage(DamageType.Kinetic, damageAmount, criticalRate);
    }

    /// <summary>
    /// Kills the character.
    /// </summary>
    protected virtual void Die() {
        // Reset necessary values
        alive = false;
        HP = 0;
        SHIELD = 0;

        Destroy(gameObject); // Destroy for now
    }
}
