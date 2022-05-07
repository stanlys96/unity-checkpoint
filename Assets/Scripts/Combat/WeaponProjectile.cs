using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat {
    public class WeaponProjectile : MonoBehaviour
    {
        [SerializeField] float speed = 2f;
        [SerializeField] bool isHoming = true;
        [SerializeField] float maxLifeTime = 10f;
        [SerializeField] GameObject impactEffect = null;

        Health target = null;
        GameObject instigator = null;
        float damage = 5f;

        // Start is called before the first frame update
        void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        // Update is called once per frame
        void Update()
        {
            if (target == null) return;
            if (!target.IsDead() && isHoming) {
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }

        private Vector3 GetAimLocation() {
            CapsuleCollider capsule = target.GetComponent<CapsuleCollider>();
            if (capsule == null) {
                return target.transform.position;
            }
            return target.transform.position + Vector3.up * capsule.height / 2;
        }

        public void SetTarget(Health target, float damage, GameObject instigator) {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;
            Destroy(gameObject, maxLifeTime);
        }

        private void OnTriggerEnter(Collider other) {
            if (target.IsDead()) return;
            if (other.GetComponent<Health>() == target) {
                Instantiate(impactEffect, GetAimLocation(), transform.rotation);
                target.TakeDamage(damage, instigator);
                Destroy(gameObject);
            }
        }
    }
}