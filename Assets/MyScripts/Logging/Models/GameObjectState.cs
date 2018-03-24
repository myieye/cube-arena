using System;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Logging.Models
{
    public class GameObjectState
    {

        public GameObjectState(GameObject obj) {
            GameObject = obj;
            Position = obj.transform.position;
            Rotation = obj.transform.rotation;
            Time = DateTime.Now;
        }

        public GameObject GameObject { get; private set; }
        public Vector3 Position { get; private set; }
        public Quaternion Rotation { get; private set; }
        public DateTime Time { get; private set; }
    }
}