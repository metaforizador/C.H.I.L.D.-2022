#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays the stats of the player.
/// </summary>
public class PlayerStatsCanvas : MonoBehaviour {

    [SerializeField]
    private TextMeshProUGUI shieldNum, staminaNum, ammoNum, dodgeNum, criticalNum, rareItemFindNum,
        piercingDmgNum, kineticDmgNum, energyDmgNum, piercingResNum, kineticResNum, energyResNum,
        attackNum, movementNum, fireRateNum, levelNum, xpNum, nextLevelNum;

    /// <summary>
    /// Displays the player's stats on the canvas.
    /// </summary>
    public void OnEnable() {
        PlayerStats p = PlayerStats.Instance;

        shieldNum.text = p.shieldRecovery.level.ToString();
        staminaNum.text = p.staminaRecovery.level.ToString();
        ammoNum.text = p.ammoRecovery.level.ToString();

        dodgeNum.text = p.dodgeRate.level.ToString();
        criticalNum.text = p.criticalRate.level.ToString();
        rareItemFindNum.text = p.rareItemFindRate.level.ToString();

        piercingDmgNum.text = p.piercingDmg.level.ToString();
        kineticDmgNum.text = p.kineticDmg.level.ToString();
        energyDmgNum.text = p.energyDmg.level.ToString();

        piercingResNum.text = p.piercingRes.level.ToString();
        kineticResNum.text = p.kineticRes.level.ToString();
        energyResNum.text = p.energyRes.level.ToString();

        attackNum.text = p.attackSpd.level.ToString();
        movementNum.text = p.movementSpd.level.ToString();
        fireRateNum.text = p.fireRate.level.ToString();

        levelNum.text = p.level.ToString();
        xpNum.text = p.xp.ToString();
        nextLevelNum.text = p.nextLevelUpXp.ToString();
    }
}
