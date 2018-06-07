using System;
using System.Linq;

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
            return string.Concat (val.Select (x => Char.IsUpper (x) ? " " + x : x.ToString ())).TrimStart (' ');
        }
    }
}