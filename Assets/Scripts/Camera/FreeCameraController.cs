using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class FreeCameraController : MonoBehaviour
{
    //TODO: Add zoom function

    [SerializeField]
    private float panSpeed = 20f;
    [SerializeField]
    private float zoomSpeed = 10f;
    [SerializeField]
    private float zoomInMax = 5f;

   [SerializeField]
    private int borderThickness = 1;
  

    private CinemachineInputProvider inputProvider = null;
    private CinemachineVirtualCamera freeCamera = null;
    private Transform camTransform = null;



    private void Awake()
    {

        inputProvider = GetComponent<CinemachineInputProvider>();
        freeCamera = GetComponent<CinemachineVirtualCamera>();
        camTransform = freeCamera.VirtualCameraGameObject.transform;

    }

    // Update is called once per frame
    void Update()
    {
        float x = inputProvider.GetAxisValue(0);
        float y = inputProvider.GetAxisValue(1);
        float z = inputProvider.GetAxisValue(2);

        if (x != 0 | y != 0)
        {
            PanScreen(x, y);
        }

    }

    public Vector2 FindPanDirection(float x, float y)
    {
        Vector2 retVect = Vector2.zero;
        if (y >= Screen.height - borderThickness)
        {
            retVect.y += 1;
        }
        else if (y <= 0 + borderThickness)
        {
            retVect.y -= 1;
        }
        if (x >= Screen.width - borderThickness)
        {
            retVect.x += 1;
        }
        else if (x <= 0 + borderThickness)
        {
            retVect.x -= 1;
        }

        return retVect;
    }

    public void PanScreen(float x, float y)
    {
        
        if (!Application.isFocused)
        {
            return;
        }

        Vector2 dir = FindPanDirection(x, y);
        //Make sure you cast the dir to a vector3 so that you move on the X and Z axes.
        camTransform.position = Vector3.MoveTowards(camTransform.position, camTransform.position + new Vector3(dir.x, 0, dir.y), panSpeed * Time.deltaTime);
        
    }

   

}
