using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat {
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] float attackRange = 1.5f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] float moveSpeed = 1f;

        GameObject target;
        Mover mover;
        Animator animator;
        ActionScheduler actionScheduler;
        float timeSinceLastAttack = Mathf.Infinity;

        // Start is called before the first frame update
        void Start()
        {
            mover = GetComponent<Mover>();
            animator = GetComponent<Animator>();
            actionScheduler = GetComponent<ActionScheduler>();
        }

        // Update is called once per frame
        void Update()
        {
            if (GetComponent<Health>().IsDead()) return;
            timeSinceLastAttack += Time.deltaTime;
            if (target == null) return;
            if (target.GetComponent<Health>().IsDead()) return;

            if (Vector3.Distance(transform.position, target.transform.position) > attackRange) {
                mover.MoveTo(target.transform.position, moveSpeed);
            } else {
                mover.Cancel();
                AttackBehaviour();
            }
        }

        public void Attack(GameObject combatTarget) {
            actionScheduler.StartAction(this);
            target = combatTarget;
        }

        private void AttackBehaviour() {
            if (timeSinceLastAttack > timeBetweenAttacks) {
                transform.LookAt(target.transform.position);
                timeSinceLastAttack = 0;
                animator.ResetTrigger("stopAttack");
                animator.SetTrigger("attack");
            }
        }

        public void Cancel() {
            animator.ResetTrigger("attack");
            animator.SetTrigger("stopAttack");
            target = null;
        }

        void Hit() {
            if (target == null) return;
            target.transform.GetComponent<Health>().TakeDamage(weaponDamage);
        }
    }
}