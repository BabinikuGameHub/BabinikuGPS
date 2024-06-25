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
    private CharacterSO _characterSO;


    public void InitializeWSO(CharacterSO charSO, int Num)
    {
        _characterSO = charSO;
        _numBadge.text = Num.ToString();
        _spriteRenderer.sprite = charSO.CharacterSprite;
    }

    public void ClickThisCharacter()
    {
        MapPOIManager.Instance.CreatePOI(_characterSO);

        POIPanelScript parentScript = gameObject.GetComponentInParent<POIPanelScript>();
        parentScript.TurnOffPanel();
    }
}
