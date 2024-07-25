using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelScript : MonoBehaviour
{
    // Start is called before the first frame update
    public virtual void TurnOnPanel() { _panelOn = true; }
    public virtual void TurnOffPanel() { _panelOn = false; }

    public void TogglePanel() 
    {
        if (_panelOn)
        {
            TurnOffPanel();
            _panelOn = false;
        }
        else
        {
            TurnOnPanel();
            _panelOn = true;
        }
    }

    public bool _panelOn;
}
