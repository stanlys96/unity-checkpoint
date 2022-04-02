using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;

namespace RPG.Control {
    public class PlayerController : MonoBehaviour
    {
        Mover mover;
        Fighter fighter;
        Health health;

        // Start is called before the first frame update
        void Start()
        {
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            if (health.IsDead()) return;
            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;
        }

        private bool InteractWithMovement() {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetRaycast(), out hit);
            if (hasHit) {
                if (Input.GetMouseButton(1)) {
                    mover.StartMoveAction(hit.point, 1f);
                }
            }
            return hasHit;
        }

        private bool InteractWithCombat() {
            RaycastHit[] hits = Physics.RaycastAll(GetRaycast());
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.GetComponent<CombatTarget>() == null) continue;
                if (hit.transform.GetComponent<Health>().IsDead()) continue;
                if (Input.GetMouseButton(1)) {
                    fighter.Attack(hit.transform.gameObject);
                }
                return true;
            }

            return false;
        }

        private Ray GetRaycast() {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}