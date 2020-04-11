using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed = 20f;

    [SerializeField]
    private int borderThickness = 1;

    // Update is called once per frame
    void Update()
    {
        //MOBA style camera

        //Move the actual position of the camera if the player touches the edges of the screen
        if(Input.mousePosition.y >= Screen.height - borderThickness)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed, Space.World);
        }
        if (Input.mousePosition.y <= 0 + borderThickness)
        {
            transform.Translate(Vector3.back * Time.deltaTime * moveSpeed, Space.World);
        }
        if (Input.mousePosition.x >= Screen.width - borderThickness)
        {
            transform.Translate(Vector3.right * Time.deltaTime * moveSpeed, Space.World);
        }
        if (Input.mousePosition.x <= 0 + borderThickness)
        {
            transform.Translate(Vector3.left * Time.deltaTime * moveSpeed, Space.World);
        }

    }
}
