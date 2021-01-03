using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

        //Check if the application is actually focused, so you can't Alt+Tab and still move the camera 
        if (Application.isFocused)
        {
            //Move the actual position of the camera if the player touches the edges of the screen
            if (Mouse.current.position.ReadValue().y >= Screen.height - borderThickness)
            {
                transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed, Space.World);
            }
            if (Mouse.current.position.ReadValue().y <= 0 + borderThickness)
            {
                transform.Translate(Vector3.back * Time.deltaTime * moveSpeed, Space.World);
            }
            if (Mouse.current.position.ReadValue().x >= Screen.width - borderThickness)
            {
                transform.Translate(Vector3.right * Time.deltaTime * moveSpeed, Space.World);
            }
            if (Mouse.current.position.ReadValue().x <= 0 + borderThickness)
            {
                transform.Translate(Vector3.left * Time.deltaTime * moveSpeed, Space.World);
            }

        }
    }
}
