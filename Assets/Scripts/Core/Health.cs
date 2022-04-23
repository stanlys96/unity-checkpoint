using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;

namespace RPG.Core {
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float healthPoints = 100f;

        private bool isDead = false;
        Animator animator;
        NavMeshAgent navMeshAgent;

        void Start()
        {
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public void TakeDamage(float damage) {
            healthPoints = Mathf.Max(healthPoints - damage, 0);
            if (healthPoints <= 0) {
                Die();
            }
        }

        private void Die() {
            if (isDead) return;
            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<NavMeshAgent>().enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
        }

        public bool IsDead() {
            return isDead;
        }

        public object CaptureState() {
            return healthPoints;
        }

        public void RestoreState(object state) {
            float data = (float)state;
            healthPoints = data;
            if (healthPoints <= 0) {
                Die();
            }
        }
    }
}