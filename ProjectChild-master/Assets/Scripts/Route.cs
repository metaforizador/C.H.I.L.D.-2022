using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class Route : MonoBehaviour
    {
        public Enemy occupyingEnemy;
        public List<Vector3> waypoints;

        private void Start()
        {
            foreach(Transform waypoint in transform)
            {
                waypoints.Add(waypoint.position);
            }
        }
    }
}
