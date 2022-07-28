using UnityEngine;

namespace AI
{
    [CreateAssetMenu(menuName = "AI/Action/Shoot", fileName = "Shoot")]
    public class Shoot : Action
    {
        public override void Act(Enemy enemyUnit, float deltaTime)
        {
            enemyUnit.Shoot();
        }
    }
}