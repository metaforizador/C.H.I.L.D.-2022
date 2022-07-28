using System;
using UnityEngine;

namespace AI
{
    [CreateAssetMenu(menuName = "AI/Condition/Is Player Dead", fileName = "IsPlayerDead")]
    public class IsPlayerDead : Condition
    {
        public override bool Decide(Enemy unit)
        {
            GameObject player = GameObject.Find("Player");

            return player == null; /* || player.hp <= 0 */
        }
    }
}
