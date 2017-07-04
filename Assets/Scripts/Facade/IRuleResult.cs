using System;
using System.Collections.Generic;

namespace CityGenerator {
    // This is the definition of a rule in a ruleset, related to a component
    public abstract class IRuleResult {
        public virtual IWallComponent Create(FacadeGenerator generator) {
            return new Panel();
        }
    }

    public class SplitRule : IRuleResult {
        readonly char firstChildRule;
        readonly char secondChildRule;

        // The characters represent the ruleset from which to get the wall
        public SplitRule(char first, char second) {
            firstChildRule = first;
            secondChildRule = second;
        }

        public override IWallComponent Create(FacadeGenerator generator) {
            IWallComponent firstChild = generator.GenerateComponent(firstChildRule);
            IWallComponent secondChild = generator.GenerateComponent(secondChildRule);
            return new Split(RandomSplitType(generator.rand), firstChild, secondChild);
        }

        private Split.SplitType RandomSplitType(Random rand) {
            Array values = Enum.GetValues(typeof(Split.SplitType));
            return (Split.SplitType)values.GetValue(rand.Next(values.Length));
        }
    }

    public class RepeatRule : IRuleResult {
        readonly char childRule;
        readonly int repeatNumberLowerBound;
        readonly int repeatNumberUpperBound;

        // Expecting char int int
        public RepeatRule(string[] paramArray) {
            childRule = paramArray[0][0];
            repeatNumberLowerBound = int.Parse(paramArray[1]);
            repeatNumberUpperBound = int.Parse(paramArray[2]);
        }

        public override IWallComponent Create(FacadeGenerator generator) {
            IWallComponent childWall = generator.GenerateComponent(childRule);
            int count = generator.rand.Next(repeatNumberLowerBound, repeatNumberUpperBound);
            return new Repeat(childWall, count);
        }
    }

    public class MirrorRule : IRuleResult {
        readonly char childRule;
        public MirrorRule(char child) {
            childRule = child;
        }

        public override IWallComponent Create(FacadeGenerator generator) {
            IWallComponent childWall = generator.GenerateComponent(childRule);
            return new Mirror(childWall);
        }
    }

    /*
     * Simply derives to another rule. Defined by simply 'char % childchar'
     */
    public class NestRule : IRuleResult {
        readonly char childRule;
        public NestRule(char child) {
            childRule = child;
        }

        public override IWallComponent Create(FacadeGenerator generator) {
            IWallComponent childWall = generator.GenerateComponent(childRule);
            return childWall;
        }
    }

    public class ExtrudeRule : IRuleResult {
        readonly char childRule;
        int extrusionLowerBound;
        int extrusionUpperBound;

        public ExtrudeRule(char child, int elb, int eub) {
            childRule = child;
            extrusionLowerBound = elb;
            extrusionUpperBound = eub;
        }

        public ExtrudeRule(string[] paramArray) {
            childRule = paramArray[0][0];
            extrusionLowerBound = int.Parse(paramArray[1]);
            extrusionUpperBound = int.Parse(paramArray[2]);
        }

         public override IWallComponent Create(FacadeGenerator generator) {
            IWallComponent child = generator.GenerateComponent(childRule);
            float extrusion = generator.rand.Next(extrusionLowerBound, extrusionUpperBound);
            return new Extrude(child, extrusion);
        }
    }

    public class BorderRule : IRuleResult {
        readonly char childRule;
        int verticalLowerBound;
        int verticalUpperBound;
        int horizontalLowerBound;
        int horizontalUpperBound;

        public BorderRule(char child, int vlb, int vub, int hlb, int hub) {
            this.childRule = child;
            verticalLowerBound = vlb;
            verticalUpperBound = vub;
            horizontalLowerBound = hlb;
            horizontalUpperBound = hub;
        }

        public BorderRule(string[] paramArray) {
            childRule = paramArray[0][0];
            verticalLowerBound = int.Parse(paramArray[1]);
            verticalUpperBound = int.Parse(paramArray[2]);
            horizontalLowerBound = int.Parse(paramArray[3]);
            horizontalUpperBound = int.Parse(paramArray[4]);
        }

        public override IWallComponent Create(FacadeGenerator generator) {
            IWallComponent child = generator.GenerateComponent(childRule);
            float vert = generator.rand.Next(verticalLowerBound, verticalUpperBound);
            float hoz = generator.rand.Next(horizontalLowerBound, horizontalUpperBound);
            return new Border(child, vert, hoz);
        }

    }

    public class PanelRule : IRuleResult {
        public override IWallComponent Create(FacadeGenerator generator) {
            return new Panel();
        }
    }

    public class WindowRule : IRuleResult {
        public int sillMaxLength;
        public int maxVerticalBars;
        public int maxHorizontalBars;
        public WindowRule(int sml, int mvb, int mhb) {
            sillMaxLength = sml;
            maxVerticalBars = mvb;
            maxHorizontalBars = mhb;
        }
        public WindowRule(string[] ruleParams) {
            sillMaxLength = int.Parse(ruleParams[0]);
            maxVerticalBars = int.Parse(ruleParams[1]);
            maxHorizontalBars = int.Parse(ruleParams[2]);
        }

        public override IWallComponent Create(FacadeGenerator generator) {
            int hozBar = generator.rand.Next(maxHorizontalBars + 1);
            return new Window(generator.rand.Next(sillMaxLength),
                generator.rand.Next(maxVerticalBars + 1),
                hozBar,
                hozBar + generator.rand.Next(1, 5)
                );
        }
    }

    public class DoorRule : IRuleResult {
        public override IWallComponent Create(FacadeGenerator generator) {
            return new Door();
        }
    }
}
