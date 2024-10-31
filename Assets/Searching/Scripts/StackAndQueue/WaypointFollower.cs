using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

namespace StackAndQueue
{
    public class WaypointFollower : MonoBehaviour
    {
        public Queue<Vector3> waypoints;
        public float speed = 1.0f;
        private Vector3? currentTarget = null;

        void Start()
        {
            var pos1 = new Vector3(0, 0, 0);
            var pos2 = new Vector3(0, 0, 5);
            var pos3 = new Vector3(5, 0, 5);
            var pos4 = new Vector3(5, 0, 0);

            // 1. add pos1, pos2, pos3, pos4 to the waypoints queue

            // 2. set the currentTarget to the first waypoint in the queue
        }

        void Update()
        {
            MoveTowardTarget();
        }

        void MoveTowardTarget()
        {
            // Only move if there is a valid target
            if (currentTarget == null)
            {
                return;
            }

            // 3. Move object toward the target position

            // 4. Check if the object has reached the target position
            // 5. If there are more waypoints, get the next target from the queue
            // 6. No more waypoints; stop moving
        }

        void OnDrawGizmos()
        {
            if (waypoints == null)
            {
                return;
            }

            Gizmos.color = Color.red;

            List<Vector3> paths = new List<Vector3>
            {
                transform.position
            };
            if (currentTarget != null)
            {
                paths.Add(currentTarget.Value);
                foreach (var w in waypoints)
                {
                    paths.Add(w);
                }
            }
            for (int i = 0; i < paths.Count; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(paths[i], 0.5f);
                if (i + 1 < paths.Count)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(paths[i], paths[i + 1]);
                }
            }
        }
    }
}