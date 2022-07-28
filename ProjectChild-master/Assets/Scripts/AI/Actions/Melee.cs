using UnityEngine;

namespace AI
{
    [CreateAssetMenu(menuName = "AI/Action/Melee", fileName = "Melee")]
    public class Melee : Action
    {
        public override void Act(Enemy enemyUnit, float deltaTime)
        {
            enemyUnit.Shoot();
        }
    }
}