using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Interaction.HHD {
    public class TouchGestures_6DOF : MonoBehaviour {

        private enum GestureState { Idle }

        private Vector2 touchOnePrev;
        private Vector2 touchTwoPrev;
        
        void Start () {
            
        }
        
        void Update()
        {
            if (Input.touchCount > 0)
            {
                // Get movement of the finger since last frame
                Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

                // Move object across XY plane
                //transform.Translate(-touchDeltaPosition.x * speed, -touchDeltaPosition.y * speed, 0);
            }
        }
    }
}