using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    [CreateAssetMenu(menuName = "AI/State", fileName = "State")]
    public class State : ScriptableObject
    {
        [SerializeField]
        private Action[] actions = null;

        [SerializeField]
        private Transition[] transitions = null;

        public void UpdateState(Enemy enemyUnit, float deltaTime)
        {
            DoActions(enemyUnit, deltaTime);
            CheckTransitions(enemyUnit);
        }

        private void DoActions(Enemy enemyUnit, float deltaTime)
        {
            foreach (Action action in actions)
            {
                action.Act(enemyUnit, deltaTime);
            }
        }

        private void CheckTransitions(Enemy unit)
        {
            foreach (Transition transition in transitions)
            {
                if (transition.condition.Decide(unit))
                {
                    if (unit.TransitionToState(transition.trueState))
                    {
                        break;
                    }
                }
                else
                {
                    if (unit.TransitionToState(transition.falseState))
                    {
                        break;
                    }
                }
            }
        }
    }
}