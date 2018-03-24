using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Test
{
  public class Rotate : MonoBehaviour
  {

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      if (Input.GetMouseButton(0))
      {
        RaycastAndRotate(Input.mousePosition);
      } else if ((Input.touchCount > 0) /*&& (Input.GetTouch(0).phase == TouchPhase.Began)*/)
      {
        RaycastAndRotate(Input.GetTouch(0).position);
      }
    }

    private void RaycastAndRotate(Vector2 origin)
    {
      Ray raycast = Camera.main.ScreenPointToRay(origin);
      RaycastHit hit;
      if (Physics.Raycast(raycast, out hit))
      {
        Debug.Log("Name: " + hit.collider.name);
        Debug.Log("Tag: " + hit.collider.tag);

        if (hit.collider.name == "Soccer")
        {
          Debug.Log("Soccer Ball clicked");
        }

        //OR with Tag

        if (hit.collider.CompareTag("BigBoy"))
        {
          Debug.Log("BigBoy clicked");
          hit.transform.Rotate(Vector3.up * 5);
        }
      }
    }
  }
}