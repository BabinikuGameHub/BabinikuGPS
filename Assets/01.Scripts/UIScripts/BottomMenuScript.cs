using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomMenuScript : MonoBehaviour
{
    [SerializeField] GameObject _cafeField;
    [SerializeField] GameObject _mapField;
    [SerializeField] GameObject _gachaField;


    [SerializeField] GameObject _cafeUI;
    [SerializeField] GameObject _mapUI;
    [SerializeField] GameObject _gachaUI;


    private void Start()
    {
        OnMapButtonClick();

        //GameManager.Instance.SwitchToMainCamera();
        //_cafeField.SetActive(true);
        //_cafeUI.SetActive(true);
        //_mapField.SetActive(false);
        //_mapUI.SetActive(false);
        //_gachaField.SetActive(false);
        //_gachaUI.SetActive(false);
    }

    //가챠화면으로 전환
    public void OnGachaButtonClick()
    {
        GameManager.Instance.SwitchToMainCamera();
        _cafeField.SetActive(false);
        _cafeUI.SetActive(false);
        _mapField.SetActive(false);
        _mapUI.SetActive(false);
        _gachaField.SetActive(true);
        _gachaUI.SetActive(true);
    }

    //지도 화면으로 전환
    public void OnMapButtonClick()
    {
        GameManager.Instance.SwitchToMapCamera();
        _cafeField.SetActive(false);
        _cafeUI.SetActive(false);
        _mapField.SetActive(true);
        _mapUI.SetActive(true);
        _gachaField.SetActive(false);
        _gachaUI.SetActive(false);
    }

    //카페 화면으로 전환
    public void OnCafeButtonClick()
    {
        GameManager.Instance.SwitchToMainCamera();
        _cafeField.SetActive(true);
        _cafeUI.SetActive(true);
        _mapField.SetActive(false);
        _mapUI.SetActive(false);
        _gachaField.SetActive(false);
        _gachaUI.SetActive(false);
    }    
}
