using UnityEngine;

namespace AI
{
    public abstract class Condition : ScriptableObject
    {
        public abstract bool Decide(Enemy unit);
    }
}
