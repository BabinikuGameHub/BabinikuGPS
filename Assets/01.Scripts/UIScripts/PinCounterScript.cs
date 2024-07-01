using Mapbox.Examples;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PinCounterScript : MonoBehaviour
{
    int _remainingPins;
    int _maxPins;

    [SerializeField] TextMeshProUGUI _pinTextScript;

    // Start is called before the first frame update
    //private void Start()
    //{
    //    UpdatePinCount();
    //}

    void OnEnable()
    {
        UpdatePinCount();
        MapPOIManager.Instance.OnPinUpdate.AddListener(UpdatePinCount);

    }

    private void OnDisable()
    {
        MapPOIManager.Instance.OnPinUpdate.RemoveAllListeners();
    }


    void UpdatePinCount()
    {
        _remainingPins = MapPOIManager.Instance.RemainingPin;
        _maxPins = MapPOIManager.Instance.MaxPin;

        _pinTextScript.text = $"{_remainingPins}/{_maxPins}";
    }
}
