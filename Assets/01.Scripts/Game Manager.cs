using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Camera MainCamera;
    public Camera MapCamera;


    //UI canvas panels
    public GameObject CafePanel;
    public GameObject MapPanel;
    public GameObject GachaPanel;

    private int CurrentScore;

    private void Awake()
    {
        Instance = this;

        CurrentScore = 0;
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

    public void AddScore(int addedScore)
    {
        CurrentScore += addedScore;
    }

    public void SubtractScore(int removedScore)
    {
        CurrentScore -= removedScore;
    }
}
