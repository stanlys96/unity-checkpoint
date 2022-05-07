using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat {
    using UnityEngine;
    
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject {
        [SerializeField] float attackRange = 1.5f;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] AnimatorOverrideController weaponOverride = null;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] WeaponProjectile projectile = null;

        const string weaponName = "WEAPON";

        public void Spawn(Transform rightHand, Transform leftHand, Animator animator) {
            DestroyOldWeapon(rightHand, leftHand);
            if (weaponPrefab != null) {
                GameObject weapon = Instantiate(weaponPrefab, GetHandTransform(rightHand, leftHand));
                weapon.name = weaponName;
            }
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (weaponOverride != null) {
                animator.runtimeAnimatorController = weaponOverride;
            } else if (overrideController != null) {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
        }

        public float GetAttackRange() {
            return attackRange;
        }

        public float GetWeaponDamage() {
            return weaponDamage;
        }

        public Transform GetHandTransform(Transform rightHand, Transform leftHand) {
            return isRightHanded ? rightHand : leftHand;
        }

        public bool HasProjectile() {
            return projectile != null;
        }

        public void ShootProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator) {
            WeaponProjectile projectileInstance = Instantiate(projectile, GetHandTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, weaponDamage, instigator);
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand) {
            Transform oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon == null) {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null) return;

            oldWeapon.name = "DESTROYED";

            Destroy(oldWeapon.gameObject);
        }
    }
}