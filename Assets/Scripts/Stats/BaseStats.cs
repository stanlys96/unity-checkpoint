using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RPG.Stats {
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression;
        [SerializeField] GameObject levelUpParticle;
        [SerializeField] bool shouldAddModifier = false;

        int currentLevel = 0;

        public event Action onLevelUp;

        private void Start() {
            currentLevel = CalculateLevel();
            Experience experience = GetComponent<Experience>();
            if (experience != null) {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        public float GetStat(Stat stat) {
            return (progression.GetStat(stat, characterClass, GetLevel()) + GetAdditiveModifiers(stat)) * (1 + GetPercentageModifiers(stat) / 100);
        }

        private float GetAdditiveModifiers(Stat stat) {
            if (!shouldAddModifier) return 0;
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float value in provider.GetAdditiveModifier(stat))
                {
                    total += value;
                }
            }
            return total;
        }

        private float GetPercentageModifiers(Stat stat) {
            if (!shouldAddModifier) return 0;
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float value in provider.GetPercentageModifier(stat))
                {
                    total += value;
                }
            }
            return total;
        }

        private void UpdateLevel() {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel) {
                currentLevel = newLevel;
                Instantiate(levelUpParticle, transform);
                onLevelUp();
            }
        }

        public int GetLevel() {
            if (currentLevel < 1) {
                currentLevel = CalculateLevel();
            }
            return currentLevel;
        }

        private int CalculateLevel() {
            Experience experience = GetComponent<Experience>();
            if (experience == null) return startingLevel;
            float currentXP = experience.GetExperience();
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
            for (int level = 1; level <= penultimateLevel; level++)
            {
                if (currentXP < progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level)) {
                    return level;
                }
            }
            return penultimateLevel + 1;
        }
    }
}