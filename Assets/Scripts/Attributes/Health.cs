using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;

namespace RPG.Attributes {
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float regenerationPercentage = 80f;

        float healthPoints = -1f;
        private bool isDead = false;
        Animator animator;
        NavMeshAgent navMeshAgent;

        void Start()
        {
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
            if (healthPoints < 0) {
                healthPoints = GetComponent<BaseStats>().GetStat(Stat.Health);
            }
        }

        public void TakeDamage(float damage, GameObject instigator) {
            healthPoints = Mathf.Max(healthPoints - damage, 0);
            if (healthPoints <= 0) {
                AwardExperience(instigator);
                Die();
            }
        }

        private void AwardExperience(GameObject instigator) {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience != null) {
                experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
            }
        }

        public float GetPercentage() {
            return 100 * (healthPoints / GetComponent<BaseStats>().GetStat(Stat.Health));
        }

        private void RegenerateHealth() {
            float healthRegen = GetComponent<BaseStats>().GetStat(Stat.Health) * (regenerationPercentage / 100);
            healthPoints = Mathf.Max(healthPoints, healthRegen);
        }

        private void Die() {
            if (isDead) return;
            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelAction();
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