using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Camera MainCamera;
    public Camera MapCamera;

    private void Awake()
    {
        Instance = this;
    }

    public void SwitchToMainCamera()
    {
        MainCamera.tag = "MainCamera";
        MapCamera.tag = "Untagged";
    }

    public void SwitchToMapCamera()
    {
        MainCamera.tag = "Untagged";
        MapCamera.tag = "MainCamera";
    }
}
