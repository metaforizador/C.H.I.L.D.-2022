using UnityEngine;

namespace AI
{
    [CreateAssetMenu(menuName = "AI/Action/Follow", fileName = "Follow")]
    public class Follow : Action
    {
        public override void Act(Enemy enemyUnit, float deltaTime)
        {
            enemyUnit.MoveTowards(GameObject.Find("Player").transform.position);
        }
    }
}