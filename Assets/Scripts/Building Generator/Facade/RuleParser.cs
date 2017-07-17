using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace CityGenerator {
    public class RuleParser {
        public Dictionary<char, Rule> rules;

        public RuleParser() {
            rules = new Dictionary<char, Rule>();
        }

        public void ReadRuleset(String filename) {
            StreamReader sr = new StreamReader(filename);

            string line;
            while ((line = sr.ReadLine()) != null) {
                if(line[0] == '#') {
                    continue;
                }                                
                ReadRuleLine(line);
            }
            sr.Close();
        }

        // Reads a line in the format: IDChar Percentage Type ParamA ParamB etc.
        public void ReadRuleLine(string line) {
            string[] tokens = line.Split(' ');
            char idChar = tokens[0][0];
            int chance = int.Parse(tokens[1]);
            string ruleType = tokens[2];
            int paramNum = tokens.Length - 3;
            string[] ruleParams = new string[paramNum];

            for(int i = 0; i< paramNum; i++) {
                ruleParams[i] = tokens[i + 3];
            }
            IRuleResult result = CreateRule(ruleType, ruleParams);

            AddRuleResult(idChar, chance, result);
        }

        private void AddRuleResult(char idChar, int chance, IRuleResult result) {
            if(rules.ContainsKey(idChar)) {
                rules[idChar].AddRuleResult(result, chance);
            } else {
                Rule newRule = new Rule(idChar);
                newRule.AddRuleResult(result, chance);
                rules[idChar] = newRule;
            }
        }

        private IRuleResult CreateRule(string type, string[] ruleParams) {
            type = type.ToUpper();
            switch(type) {
                case "SPLIT":
                    return new SplitRule(ruleParams[0][0], ruleParams[1][0]);
                case "REPEAT":
                    return new RepeatRule(ruleParams);
                case "MIRROR":
                    return new MirrorRule(ruleParams[0][0]);
                case "EXTRUDE":
                    return new ExtrudeRule(ruleParams);
                case "BORDER":
                    return new BorderRule(ruleParams);
                case "PANEL":
                    return new PanelRule();
                case "WINDOW":
                    return new WindowRule(ruleParams);
                case "DOOR":
                    return new DoorRule();
                default:
                    return new NestRule(type[0]);
            }
        }
    }
}
