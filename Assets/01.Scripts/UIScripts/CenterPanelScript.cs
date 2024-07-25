using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterPanelScript : MonoBehaviour
{
    [SerializeField] private PanelScript CafePanelScript;
    [SerializeField] private PanelScript GachaPanelScript;
    [SerializeField] private PanelScript MapPanelScript;

    [SerializeField] private GameObject FrameButton;

    private PanelScript _currentPanelScript;
    
    // Start is called before the first frame update
    public void ToggleMenu()
    {
        _currentPanelScript.TogglePanel();

        if(FrameButton.activeSelf == true)
        {
            FrameButton.SetActive(false);
        }
        else
        {
            FrameButton.SetActive(true);
        }
    }

    public void TurnOffMenu()   
    {
        if(_currentPanelScript != null)
            _currentPanelScript.TurnOffPanel();

        FrameButton.SetActive(true);
    }

    public void UpdateCurrentPanel(CurrentPanel currentPanel)
    {
        TurnOffMenu();

        if (currentPanel == CurrentPanel.GACHA)
        {
            _currentPanelScript = GachaPanelScript.GetComponent<PanelScript>();
        }
        else if(currentPanel == CurrentPanel.CAFE)
        {
            _currentPanelScript = CafePanelScript.GetComponent<PanelScript>();
        }
        else if (currentPanel == CurrentPanel.MAP)
        {
            _currentPanelScript = MapPanelScript.GetComponent<PanelScript>();
        }

    }
}
