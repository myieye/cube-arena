using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Fire
{
    public class PermanentFireSource : MonoBehaviour, FireSource {
        public bool HasSource() {
            return true;
        }
    }
}