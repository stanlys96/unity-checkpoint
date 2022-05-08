using System.Collections.Generic;

namespace RPG.Stats {
    public interface IModifierProvider {
        public IEnumerable<float> GetAdditiveModifier(Stat stat);
        public IEnumerable<float> GetPercentageModifier(Stat stat);
    }
}