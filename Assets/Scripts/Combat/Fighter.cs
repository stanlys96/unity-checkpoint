using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using GameDevTV.Utils;

namespace RPG.Combat {
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
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
        LazyValue<Weapon> currentWeapon = null;

        private void Awake() {
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private Weapon SetupDefaultWeapon() {
            AttachWeapon(defaultWeapon);
            return defaultWeapon;
        }

        void Start()
        {
            mover = GetComponent<Mover>();
            animator = GetComponent<Animator>();
            actionScheduler = GetComponent<ActionScheduler>();
            currentWeapon.ForceInit();
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
            currentWeapon.value = weapon;
            AttachWeapon(weapon);
        }

        private void AttachWeapon(Weapon weapon) {
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(rightHandTransform, leftHandTransform, animator);
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

        public IEnumerable<float> GetAdditiveModifier(Stat stat) {
            if (stat == Stat.Damage) {
                yield return currentWeapon.value.GetWeaponDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifier(Stat stat) {
            if (stat == Stat.Damage) {
                yield return currentWeapon.value.GetPercentageBonus();
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
            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            if (currentWeapon.value.HasProjectile()) {
                currentWeapon.value.ShootProjectile(rightHandTransform, leftHandTransform, target.GetComponent<Health>(), gameObject, damage);
            } else {
                target.transform.GetComponent<Health>().TakeDamage(damage, gameObject);
            }
        }

        void Shoot() {
            Hit();
        }

        public object CaptureState() {
            return currentWeapon.value.name;
        }

        public void RestoreState(object state) {
            string weaponName = (string) state;
            Weapon weapon = Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
    }
}