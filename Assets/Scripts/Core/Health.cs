using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Core {
    public class Health : MonoBehaviour
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
            if (healthPoints <= 0 && !isDead) {
                isDead = true;
                animator.SetTrigger("die");
                navMeshAgent.enabled = false;
            }
        }

        public bool IsDead() {
            return isDead;
        }
    }
}