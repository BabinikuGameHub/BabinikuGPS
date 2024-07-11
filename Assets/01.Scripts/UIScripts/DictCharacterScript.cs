using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DictCharacterScript : MonoBehaviour
{
    private Image _characterImage;

    [SerializeField] CharacterSO _characterSO;

    private void Awake()
    {
        _characterImage = GetComponent<Image>();
    }

    public void InitializeWithSO(CharacterSO characterSO)
    {
        _characterSO = characterSO;

        _characterImage.sprite = _characterSO.CharacterSprite;
    }

    public void OpenPanel()
    {
        DictPanelScript.Instance.TurnOnSpecific(_characterSO);
    }
}
