using Mapbox.Examples;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPOIScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _numBadge;
    [SerializeField] private Image _spriteRenderer;
    private int count;
    private CharacterSO _characterSO;


    public void InitializeWSO(CharacterSO charSO, int Num)
    {
        _characterSO = charSO;
        _numBadge.text = Num.ToString();
        count = Num;
        _spriteRenderer.sprite = charSO.CharacterSprite;
    }

    public void ClickThisCharacter()
    {
        if (count <= 0)
            return;

        if ( MapPOIManager.Instance.PoiDebug == true)
        {
            MapPOIManager.Instance.CreatePOIDebug(_characterSO);
        }
        else
        {
            MapPOIManager.Instance.CreatePOI(_characterSO);
        }

        POIPanelScript parentScript = gameObject.GetComponentInParent<POIPanelScript>();
        parentScript.TurnOffPanel();
    }
}
