using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Combat {
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float moveSpeed = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Weapon defaultWeapon = null;

        Health target;
        Mover mover;
        Animator animator;
        ActionScheduler actionScheduler;
        float timeSinceLastAttack = Mathf.Infinity;
        Weapon currentWeapon = null;

        // Start is called before the first frame update
        void Start()
        {
            mover = GetComponent<Mover>();
            animator = GetComponent<Animator>();
            actionScheduler = GetComponent<ActionScheduler>();
            if (currentWeapon == null) {
                EquipWeapon(defaultWeapon);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (GetComponent<Health>().IsDead()) return;
            timeSinceLastAttack += Time.deltaTime;
            if (target == null) return;
            if (target.GetComponent<Health>().IsDead()) return;

            if (Vector3.Distance(transform.position, target.transform.position) > currentWeapon.GetAttackRange()) {
                mover.MoveTo(target.transform.position, moveSpeed);
            } else {
                mover.Cancel();
                AttackBehaviour();
            }
        }

        public void EquipWeapon(Weapon weapon) {
            currentWeapon = weapon;
            if (currentWeapon != null) {
                currentWeapon.Spawn(rightHandTransform, leftHandTransform, GetComponent<Animator>());
            }
        }

        public void Attack(Health combatTarget) {
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

        public Health GetTarget() {
            return target;
        }

        void Hit() {
            if (target == null) return;
            if (currentWeapon.HasProjectile()) {
                currentWeapon.ShootProjectile(rightHandTransform, leftHandTransform, target.GetComponent<Health>(), gameObject);
            } else {
                target.transform.GetComponent<Health>().TakeDamage(currentWeapon.GetWeaponDamage(), gameObject);
            }
        }

        void Shoot() {
            Hit();
        }

        public object CaptureState() {
            if (gameObject.tag == "Player") {
                print(currentWeapon.name);
            }
            return currentWeapon.name;
        }

        public void RestoreState(object state) {
            string weaponName = (string) state;
            Weapon weapon = Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
    }
}