using CubeArena.Assets.MyPrefabs.Cubes;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Interaction.State {
    public class CubeStatePair {
        public CubeStatePair (GameObject cube) {
            Cube = cube;
            StateManager = cube.GetComponent<CubeStateManager> ();
        }

        public GameObject Cube { get; private set; }
        public CubeStateManager StateManager { get; private set; }
    }
}