using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Script for playing the particle effect when the player right clicks on the ground
/// </summary>
public class GroundPoofParticleOnClick : MonoBehaviour
{

    [SerializeField] private GameObject poofParticleGO = null;
    private ParticleSystem poofParticle = null;

    private Camera cam;

    // Start is called before the first frame update
    void Awake()
    {
        cam = Camera.main;
        if(poofParticleGO != null)
        {
            poofParticle = poofParticleGO.GetComponent<ParticleSystem>();
        }

    }

    //Activate the particle effect
    private void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            //Shoot a ray to check if it hit the ground (particle effect only plays when the ground is hit)
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.transform.tag == "Ground")
                {
                    poofParticleGO.transform.position = hitInfo.point;
                    poofParticle.Play();

                }
            }
        }
    }
}
