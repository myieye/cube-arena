using System.Collections.Generic;

namespace CubeArena.Assets.MyScripts.Logging.Survey.Models {
    public class WeightQuestion : Question {

        public WeightQuestion (int id, WeightOption optionOne, WeightOption optionTwo) {
            this.Id = id;
            this.OptionOne = optionOne;
            this.OptionTwo = optionTwo;
        }

        public int Id { get; private set; }
        public WeightOption OptionOne { get; private set; }
        public WeightOption OptionTwo { get; private set; }
    }
}