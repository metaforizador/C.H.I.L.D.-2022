using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the enemy values.
/// </summary>
public class Enemy : CharacterParent {

    [SerializeField]
    private EnemySO scriptableObject = null;

    private Player player;

    public Animator animator;

    //public enum State {Patrolling, Shooting, Dying};
    private AI.State curState;
    public AI.State startState;

    public int level { get; private set; }

    public float turnSpeed;
    private float turnSmoothVelocity;
    private float turnSmoothTime = 0.1f;

    public AI.Route route;
    private Vector3 nextPoint;

    public override void Start() {
        //transitions to startState
        TransitionToState(startState);

        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        characterType = CharacterType.Enemy;

        animator = GetComponentInChildren<Animator>();

        // Calculate stats from scriptableObject values
        shieldRecovery = Stat.CalculateValue(Stat.RECOVERY_MIN_SPEED, Stat.RECOVERY_MAX_SPEED, scriptableObject.shieldRecovery);
        staminaRecovery = Stat.CalculateValue(Stat.RECOVERY_MIN_SPEED, Stat.RECOVERY_MAX_SPEED, scriptableObject.staminaRecovery);

        dodgeRate = Stat.CalculateValue(Stat.DODGE_MIN_PERCENT, Stat.DODGE_MAX_PERCENT, scriptableObject.dodgeRate);
        criticalRate = Stat.CalculateValue(Stat.CRITICAL_MIN_PERCENT, Stat.CRITICAL_MAX_PERCENT, scriptableObject.criticalRate);

        piercingDmg = Stat.CalculateValue(Stat.DAMAGE_MIN_BOOST, Stat.DAMAGE_MAX_BOOST, scriptableObject.piercingDmg);
        kineticDmg = Stat.CalculateValue(Stat.DAMAGE_MIN_BOOST, Stat.DAMAGE_MAX_BOOST, scriptableObject.kineticDmg);
        energyDmg = Stat.CalculateValue(Stat.DAMAGE_MIN_BOOST, Stat.DAMAGE_MAX_BOOST, scriptableObject.energyDmg);

        piercingRes = Stat.CalculateValue(Stat.RESISTANCE_MIN_PERCENT, Stat.RESISTANCE_MAX_PERCENT, scriptableObject.piercingRes);
        kineticRes = Stat.CalculateValue(Stat.RESISTANCE_MIN_PERCENT, Stat.RESISTANCE_MAX_PERCENT, scriptableObject.kineticRes);
        energyRes = Stat.CalculateValue(Stat.RESISTANCE_MIN_PERCENT, Stat.RESISTANCE_MAX_PERCENT, scriptableObject.energyRes);

        attackSpd = Stat.CalculateValue(Stat.ATTACK_MIN_SPEED, Stat.ATTACK_MAX_SPEED, scriptableObject.attackSpd);
        movementSpd = Stat.CalculateValue(Stat.MOVEMENT_MIN_SPEED, Stat.MOVEMENT_MAX_SPEED, scriptableObject.movementSpd);
        fireRate = Stat.CalculateValue(Stat.FIRE_RATE_MIN_SPEED, Stat.FIRE_RATE_MAX_SPEED, scriptableObject.fireRate);

        level = scriptableObject.level;

        base.Start();

        // If weapon or armor is not assigned and EnemySO values are not null
        if (GetWeapon() == null && scriptableObject.startingWeapon != null)
            ChangeWeapon(scriptableObject.startingWeapon);
        if (GetArmor() == null && scriptableObject.startingArmor != null)
            ChangeArmor(scriptableObject.startingArmor);
    }

    /// <summary>
    /// Checks for collision with player bullets.
    /// </summary>
    /// <param name="collision">happened collision</param>
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Bullet")) {
            bulletController bullet = collision.gameObject.GetComponent<bulletController>();
            if (bullet.shooter == CharacterType.Player) {
                TakeDamage(bullet.damageType, bullet.damage, bullet.criticalRate);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    /// <summary>
    /// Shows enemy stats on hud when taking damage.
    /// </summary>
    /// <param name="type">type of damage</param>
    /// <param name="amount">damage amount</param>
    /// <param name="criticalRate">critical rate of damage</param>
    protected override void TakeDamage(DamageType type, float amount, float criticalRate) {
        // Call the base method
        base.TakeDamage(type, amount, criticalRate);

        // Show stats in hud
        hud.ShowEnemyStats(this);
    }

    protected override void Update() {

        base.Update();
        // Don't run update if enemy is not alive
        if (!alive)
        {
            return;
        }

        curState.UpdateState(this, Time.deltaTime);
    }

    public bool TransitionToState(AI.State newState)
    {
        if (curState != newState)
        {
            Debug.Log("State transition " + curState + " -> " + newState);

            OnStateExit(curState);
            curState = newState;
            OnStateEnter(newState);

            return true;
        }

        return false;
    }

    private void OnStateEnter(AI.State newState)
    {
        
    }

    private void OnStateExit(AI.State curState)
    {
        shooting = false;
        animator.SetBool("Shooting", false);
        animator.SetBool("Moving", false);
    }

    public void Shoot()
    {
        shooting = true;
        animator.SetBool("Shooting", true);

        //enemy turns towards player while shooting
        Vector3 targetDirection = GameObject.Find("Player").transform.position - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, turnSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    public void Patrol()
    {
        setRoute();

        if(this.route == null)
        {
            return;
        }

        if(nextPoint == new Vector3(0, 0, 0))
        {
            nextPoint = FindClosestPoint(this.route);
        }

        nextPoint = checkIfPointReached(nextPoint);

        MoveTowards(nextPoint);
    }

    private Vector3 checkIfPointReached(Vector3 nextPoint)
    {
        Vector3 newPoint = nextPoint;

        Transform rt = this.route.transform;

        if (Vector3.Magnitude(nextPoint - transform.position) < 10)
        {
            for (int i = 0; i < rt.childCount; i++)
            {
                if (rt.GetChild(i).position == nextPoint)
                {
                    if (i + 1 < rt.childCount - 1)
                    {
                        newPoint = rt.GetChild(i + 1).position;
                        break;
                    }
                    else
                    {
                        newPoint = rt.GetChild(0).position;
                        break;
                    }
                }
            }
        }

        return newPoint;
    }

    private void setRoute()
    {
        if(this.route == null)
        {
            return;
        }

        AI.Route[] routes = GameObject.FindObjectsOfType<AI.Route>();
        Vector3 closestPoint = new Vector3(0, 0, 0);
        AI.Route closestRoute = null;

        foreach (AI.Route newRoute in routes)
        {
            if (newRoute.occupyingEnemy == null)
            {
                Vector3 newPoint = FindClosestPoint(newRoute);
                float newDist = Vector3.Magnitude(newPoint - transform.position);
                float closestDist = Vector3.Magnitude(closestPoint - transform.position);

                if (closestPoint == new Vector3(0, 0, 0) || newDist < closestDist)
                {
                    closestPoint = newPoint;
                    closestRoute = newRoute;

                    newRoute.occupyingEnemy = this;
                    this.route = newRoute;
                }
            }
        }
    }

    private Vector3 FindClosestPoint(AI.Route route)
    {
        Vector3 closestPoint = new Vector3();

        foreach (Vector3 waypoint in route.waypoints)
        {
            float closestDist = Vector3.Magnitude(closestPoint - transform.position);
            float distance = Vector3.Magnitude(waypoint - transform.position);

            if (closestDist == 0 || closestDist > distance)
            {
                closestPoint = waypoint;
            }
        }

        return closestPoint;
    }

    public void MoveTowards(Vector3 targetPoint)
    {
        animator.SetBool("Moving", true);

        Vector3 targetDirection = targetPoint - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, turnSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);

        transform.position += transform.forward * movementSpd * 40 * Time.deltaTime; //movementSpd should be multiplied with base movement speed somewhere else
    }

    /// <summary>
    /// Give xp to the player when the enemy dies.
    /// </summary>
    protected override void Die() {
        PlayerStats.Instance.GainXP(scriptableObject.xp);
        base.Die();
    }
}
