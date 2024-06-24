using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharPrefabScript : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private string _locationString;

    private int _currentLevel = 1;

    [SerializeField]
    private CharacterSO _characterSO;

    //[SerializeField] private POIData _data;

    // Start is called before the first frame update
    void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void IntializeFromSO(CharacterSO SO)
    {
        if (_characterSO == null)
        {
            _characterSO = SO;
            _spriteRenderer.sprite = _characterSO.CharacterSprite;
        }
    }


}
