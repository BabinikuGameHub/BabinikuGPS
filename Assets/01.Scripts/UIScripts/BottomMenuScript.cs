using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BottomMenuScript : MonoBehaviour
{
    [SerializeField] GameObject _cafeField;
    [SerializeField] GameObject _mapField;
    [SerializeField] GameObject _gachaField;


    [SerializeField] GameObject _cafeUI;
    [SerializeField] GameObject _mapUI;
    [SerializeField] GameObject _gachaUI;

    [SerializeField] GameObject _cafeButton;
    [SerializeField] GameObject _mapButton;
    [SerializeField] GameObject _gachaButton;

    public UnityEvent OnPanelChange;

    public UnityEvent OnMapPanelEnter;
    public UnityEvent OnGachaPanelEnter;
    public UnityEvent OnCafePanelEnter;

    private void Start()
    {
        OnGachaButtonClick();

    }

    //가챠화면으로 전환
    public void OnGachaButtonClick()
    {
        OnPanelChange?.Invoke();
        OnGachaPanelEnter?.Invoke();

        GameManager.Instance.SwitchToMainCamera();
        _cafeField.SetActive(false);
        _cafeUI.SetActive(false);
        _mapField.SetActive(false);
        _mapUI.SetActive(false);
        _gachaField.SetActive(true);
        _gachaUI.SetActive(true);

        _gachaButton.SetActive(true);
        _mapButton.SetActive(false);
        _cafeButton.SetActive(false);

    }

    //지도 화면으로 전환
    public void OnMapButtonClick()
    {
        OnPanelChange?.Invoke();
        OnMapPanelEnter?.Invoke();

        GameManager.Instance.SwitchToMapCamera();
        _cafeField.SetActive(false);
        _cafeUI.SetActive(false);
        _mapField.SetActive(true);
        _mapUI.SetActive(true);
        _gachaField.SetActive(false);
        _gachaUI.SetActive(false);

        _gachaButton.SetActive(false);
        _mapButton.SetActive(true);
        _cafeButton.SetActive(false);

    }

    //카페 화면으로 전환
    public void OnCafeButtonClick()
    {
        OnPanelChange?.Invoke();
        OnCafePanelEnter?.Invoke();

        GameManager.Instance.SwitchToMainCamera();
        _cafeField.SetActive(true);
        _cafeUI.SetActive(true);
        _mapField.SetActive(false);
        _mapUI.SetActive(false);
        _gachaField.SetActive(false);
        _gachaUI.SetActive(false);

        _gachaButton.SetActive(false);
        _mapButton.SetActive(false);
        _cafeButton.SetActive(true);

    }    
}
