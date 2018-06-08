using CubeArena.Assets.MyScripts.Logging.Survey.Models;
using NUnit.Framework;

namespace CubeArena.Assets.Editor.Tests.MyScripts.Logging.Survey.Models {
    public class WeightOptionHelpersTest {

        [TestCase (WeightOption.Effort, "Effort")]
        [TestCase (WeightOption.TemporalDemand, "Temporal Demand")]
        public void TestDeviceConfigGeneration (WeightOption weightOption, string expectedValued) {
            Assert.AreEqual (expectedValued, weightOption.ToFriendlyString ());
        }
    }
}