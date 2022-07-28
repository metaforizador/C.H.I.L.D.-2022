using UnityEngine;

namespace AI
{
    [CreateAssetMenu(menuName = "AI/Condition/Is Player In Melee Distance", fileName = "IsPlayerInMeleeDistance")]
    public class IsPlayerInMeleeDistance : Condition
    {
        [SerializeField]
        private float distance = 10;

        private float sqrDistance;

        private void OnEnable()
        {
            sqrDistance = distance * distance;
        }

        public override bool Decide(Enemy unit)
        {
            GameObject player = GameObject.Find("Player");

            if (player == null)
            {
                return false;
            }

            Vector3 unitPosition = unit.transform.position;
            Vector3 playerPosition = player.transform.position;
            float sqrMagnitude = Vector3.SqrMagnitude(playerPosition - unitPosition);

            return sqrMagnitude <= sqrDistance;
        }
    }
}
