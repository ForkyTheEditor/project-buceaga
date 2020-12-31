using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGridSnapper : MonoBehaviour
{
    [SerializeField] private float gridStep = 0.5f;

    private void FixedUpdate()
    {
        SnapToGrid(transform);
    }

    private void SnapToGrid(Transform objectTransform)
    {
        Vector3 pos = objectTransform.position;

        //Snap it to the grid
        objectTransform.position = new Vector3(Mathf.Round(pos.x / gridStep) * gridStep, pos.y, Mathf.Round(pos.z / gridStep) * gridStep);
    }


}
