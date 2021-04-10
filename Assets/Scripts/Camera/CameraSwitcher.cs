using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{

    [SerializeField]
    private InputAction switchAction;
    [SerializeField]
    private CinemachineVirtualCamera lockCamera;

    private Animator animator;

    private bool isFreeCamera = true;

    void Awake()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(InitializeComponents());
    }

    IEnumerator InitializeComponents()
    {
        yield return new WaitUntil(() => GameManager.localPlayerNetworkInstance != null);
        GameManager.localPlayerNetworkInstance.OnLocalPlayerSpawn += OnPlayerSpawned;
        GameManager.localPlayerNetworkInstance.OnLocalPlayerDeath += OnPlayerDied;
    }

    private void OnEnable()
    {
        switchAction.Enable();
    }

    private void OnDisable()
    {
        switchAction.Disable();
    }

    private void Start()
    {    
        switchAction.performed += _ => SwitchCamera();
    }

    private void SwitchCamera()
    {
        if (isFreeCamera)
        {
            animator.Play("Lock Camera");
        }
        else
        {
            animator.Play("Free Camera");
        }
        isFreeCamera = !isFreeCamera;
    }

    public void OnPlayerDied(GameObject player)
    {
        if (!isFreeCamera)
        {
            SwitchCamera();
        }

        switchAction.Disable();

    }

    public void OnPlayerSpawned(GameObject player)
    {
        lockCamera.Follow = player.transform;
        lockCamera.LookAt = player.transform;
        switchAction.Enable();
    }

}
