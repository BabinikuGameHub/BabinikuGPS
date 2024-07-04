using Mapbox.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharPrefabScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private POICharacterData _poiData;


    public void InitializeFromPOIData(POICharacterData POIData)
    {
        _poiData = POIData;

        CharacterSO characterSO = GachaManager.Instance.GetSOByName(POIData.Name);

        if (characterSO == null)
        {
            return;
        }
        else
        {
            _spriteRenderer.sprite = characterSO.CharacterSprite;
        }

    }

}
