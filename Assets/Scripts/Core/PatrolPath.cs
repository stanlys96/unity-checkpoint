using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core {
    public class PatrolPath : MonoBehaviour
    {
        void OnDrawGizmos() {
            for (int i = 0; i < transform.childCount; i++)
            {
                int nextIndex = GetNextIndex(i);
                Gizmos.DrawSphere(GetWaypoint(i), 0.5f);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(nextIndex));
            }
        }

        public Vector3 GetWaypoint(int index) {
            return transform.GetChild(index).position;
        }

        public int GetNextIndex(int index) {
            if (index == transform.childCount - 1) {
                return 0;
            }
            return index + 1;
        }
    }
}