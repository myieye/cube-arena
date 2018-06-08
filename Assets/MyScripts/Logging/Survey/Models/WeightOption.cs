using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace CubeArena.Assets.MyScripts.Logging.Survey.Models {
    public enum WeightOption {
        MentalDemand,
        PhysicalDemand,
        TemporalDemand,
        Performance,
        Effort,
        Frustration,
    }

    public static class WeightOptionHelpers {

        /**
        Adds spaces before capital letters
         */
        public static string ToFriendlyString (this WeightOption weightOption) {
            var val = weightOption.ToString ();
            return Regex.Replace(val, "([a-z])([A-Z])", "$1 $2");
        }
    }
}