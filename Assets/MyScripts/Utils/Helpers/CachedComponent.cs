using System;
using System.Linq;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.Helpers {
    public class CachedComponent<T> where T : Behaviour {

        private T instance;
        private Func<T> searchFunction;

        public CachedComponent (Func<T> searchFunction = null) {
            this.searchFunction = searchFunction;
            Refresh ();
        }

        public T Get {
            get {
                if (!instance) {
                    Refresh ();
                }
                return instance;
            }
        }

        public bool Is (Func<T,bool> test) {
            return Get ? test (Get) : false;
        }

        public void Try (Action<T> action) {
            if (Get) action (Get);
        }

        public void Try (Action action) {
            if (Get) action ();
        }

        private void Refresh () {
            if (searchFunction != null) {
                instance = searchFunction ();
            } else {
                instance = GameObject.FindObjectOfType<T> ();
            }
        }
    }
}