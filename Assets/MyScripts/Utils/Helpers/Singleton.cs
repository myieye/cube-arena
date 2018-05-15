using System;
using System.Reflection;

namespace CubeArena.Assets.MyScripts.Utils.Helpers {
    
    public abstract class Singleton<T> where T : Singleton<T> {
        protected Singleton () {
            if (_instance != null) {
                throw new InvalidOperationException ("A singleton of " + typeof(T).Name + " has already been created.");
            }
        }
        private static T _instance;
        public static T Instance {
            get {
                if (_instance == null) {
                    var constructor = typeof (T).GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)[0];
                    // Method GetConstructor not found when building VS Project
                    // var constructor = typeof (T).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                    _instance = (T) constructor.Invoke (null);
                }
                return _instance;
            }
        }
    }
}