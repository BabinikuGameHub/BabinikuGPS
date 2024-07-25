using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DictContentPanelScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _name;
    [SerializeField] TextMeshProUGUI _description;
    [SerializeField] TextMeshProUGUI _abilityDescription;
    [SerializeField] Image _icon;

    CharacterSO _currentCharacterSO;

    public void InitializeWSO(CharacterSO characterSO)
    {
        gameObject.SetActive(true);

        _currentCharacterSO = characterSO;

        _name.text = _currentCharacterSO.CharacterName;
        _description.text = _currentCharacterSO.CharacterDescription;
        _abilityDescription.text = _currentCharacterSO.AbilityDescription;
        _icon.sprite = _currentCharacterSO.CharacterSprite;
    }
}
