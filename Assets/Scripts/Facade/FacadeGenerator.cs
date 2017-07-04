using System;
using System.Collections.Generic;

namespace CityGenerator {

    public class FacadeGenerator {
        Dictionary<char, Rule> ruleCharMap;
        public Random rand;

        public FacadeGenerator(string file) {
            rand = new Random();
            ReadRules(file);
        }

        public void ReadRules(string file) {
            RuleParser parser = new RuleParser();
            parser.ReadRuleset(file);
            ruleCharMap = parser.rules;
        }

        public void GenerateFacade() {
        }

        public IWallComponent GenerateComponent(char ruleId) {
            IRuleResult result = ruleCharMap[ruleId].SelectRule(rand);
            return result.Create(this);
        }
    }
}
