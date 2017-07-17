using System;
using System.Collections.Generic;

namespace CityGenerator {
    public class Rule {
        private readonly Dictionary<IRuleResult, int> results;

        public readonly char id;
        private int normalisedMax;

        public Rule(char id) {
            this.id = id;
            results = new Dictionary<IRuleResult, int>();
        }

        public void AddRuleResult(IRuleResult result, int probability) {
            results.Add(result, probability);
            normalisedMax += probability;
        }

        // Selects a random ruleresult from the supplied results
        public IRuleResult SelectRule(Random rand) {
            int decision = rand.Next(normalisedMax);
            int current = 0;

            foreach (IRuleResult rule in results.Keys) {
                current += results[rule];
                if (decision < current) {
                    return rule;
                }
            }
            return null;

        }
    }
}
