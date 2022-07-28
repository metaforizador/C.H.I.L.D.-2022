using System;

namespace AI
{
    [Serializable]
    public struct Transition
    {
        public Condition condition;
        public State trueState;
        public State falseState;
    }
}
