using UnityEngine;

namespace AI
{
    public abstract class Action : ScriptableObject
    {
        public abstract void Act(Enemy enemyUnit, float deltaTime);
    }

}