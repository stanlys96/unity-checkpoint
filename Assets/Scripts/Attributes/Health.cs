using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using GameDevTV.Utils;

namespace RPG.Attributes {
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float regenerationPercentage = 80f;

        LazyValue<float> healthPoints;
        private bool isDead = false;
        Animator animator;
        NavMeshAgent navMeshAgent;

        private void Awake() {
            healthPoints = new LazyValue<float>(GetHealth);
        }

        void Start()
        {
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
            healthPoints.ForceInit();
        }

        private float GetHealth() {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public void TakeDamage(float damage, GameObject instigator) {
            print(gameObject.name + " took damage: " + damage);
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            if (healthPoints.value <= 0) {
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
            return 100 * (healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health));
        }

        private void RegenerateHealth() {
            float healthRegen = GetComponent<BaseStats>().GetStat(Stat.Health) * (regenerationPercentage / 100);
            healthPoints.value = Mathf.Max(healthPoints.value, healthRegen);
        }

        public float GetHealthPoints() {
            return healthPoints.value;
        }

        public float GetMaxHealthPoints() {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
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
            return healthPoints.value;
        }

        public void RestoreState(object state) {
            float data = (float)state;
            healthPoints.value = data;
            if (healthPoints.value <= 0) {
                Die();
            }
        }
    }
}