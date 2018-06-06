using System.Linq;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.Helpers {
    public class EnabledComponent<T> where T : Behaviour {

        private GameObject parent;
        private T instance;

        public EnabledComponent (GameObject parent) {
            this.parent = parent;
        }
        
        public T Get {
            get {
                if (!(instance && instance.enabled) && parent) {
                    instance = parent.GetComponents<T> ().FirstOrDefault (comp => comp.enabled);
                }
                return instance;
            }
        }
    }
}