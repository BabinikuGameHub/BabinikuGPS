using Mapbox.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharPrefabScript : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private POICharacterData _poiData;

    //[SerializeField] private POIData _data;

    // Start is called before the first frame update
    void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void InitializeFromPOIData(POICharacterData POIData)
    {
        _poiData = POIData;

        _spriteRenderer.sprite = GachaManager.Instance.GetSObyID(POIData.SOID).CharacterSprite;
    }

}
