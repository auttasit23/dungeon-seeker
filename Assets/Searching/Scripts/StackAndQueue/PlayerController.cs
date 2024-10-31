using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StackAndQueue
{
    public class PlayerController : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                transform.position += Vector3.up;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                transform.position += Vector3.down;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                transform.position += Vector3.left;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                transform.position += Vector3.right;
            }
        }
    }

}