using UnityEngine;

namespace AI
{
    [CreateAssetMenu(menuName = "AI/Action/Patrol", fileName = "Patrol")]
    public class Patrol : Action
    {
        public override void Act(Enemy enemyUnit, float deltaTime)
        {
            enemyUnit.Patrol();
        }
    }
}