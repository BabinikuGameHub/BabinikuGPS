using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardScript : MonoBehaviour
{
    private Camera mainCamera;

    void LateUpdate()
    {
        mainCamera = Camera.main;
        transform.rotation = mainCamera.transform.rotation;
    }
}
