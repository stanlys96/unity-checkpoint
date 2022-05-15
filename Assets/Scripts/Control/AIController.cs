using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using GameDevTV.Utils;

namespace RPG.Control {
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseRadius = 6f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float dwellingTime = 5f;
        [SerializeField] float suspicionTime = 5f;
        [SerializeField] float moveSpeed = 0.5f;

        GameObject player;
        Fighter fighter;
        LazyValue<Vector3> guardPosition;
        Mover mover;
        ActionScheduler actionScheduler;
        float waypointTolerance = 0.5f;
        int currentWaypointIndex = 0;
        float timeSinceLastAtWaypoint = Mathf.Infinity;
        float timeSinceLastSawPlayer = Mathf.Infinity;

        // Start is called before the first frame update
        void Awake()
        {
            player = GameObject.FindWithTag("Player");
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
            actionScheduler = GetComponent<ActionScheduler>();
            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private Vector3 GetGuardPosition() {
            return transform.position;
        }

        private void Start() {
            guardPosition.ForceInit();
        }

        // Update is called once per frame
        void Update()
        {
            if (GetComponent<Health>().IsDead()) return;
            timeSinceLastAtWaypoint += Time.deltaTime;
            timeSinceLastSawPlayer += Time.deltaTime;
            if (Vector3.Distance(transform.position, player.transform.position) < chaseRadius) {
                timeSinceLastSawPlayer = 0;
                fighter.Attack(player.GetComponent<Health>());
            } else if (timeSinceLastAtWaypoint < dwellingTime || timeSinceLastSawPlayer < suspicionTime) {
                actionScheduler.CancelAction();
            } else {
                PatrolBehaviour();
            }
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseRadius);
        }

        private void PatrolBehaviour() {
            Vector3 nextPosition = guardPosition.value;
            if (patrolPath != null) {
                if (AtWaypoint()) {
                    timeSinceLastAtWaypoint = 0;
                    CycleWaypoint();
                }
                nextPosition = patrolPath.GetWaypoint(currentWaypointIndex);
            }
            mover.StartMoveAction(nextPosition, moveSpeed);
        }

        private bool AtWaypoint() {
            return Vector3.Distance(transform.position, patrolPath.GetWaypoint(currentWaypointIndex)) < waypointTolerance;
        }

        private void CycleWaypoint() {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }
    }
}