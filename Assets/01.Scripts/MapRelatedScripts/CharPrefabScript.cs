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

    private void Start()
    {
        QuadTreeCameraMovement.Instance.OnZoomChange.AddListener(AdjustCharacterSize);
    }

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

    public void AdjustCharacterSize()
    {
        float newScaleValue = QuadTreeCameraMovement.Instance.Zoom / 17;

        transform.localScale = new Vector3(newScaleValue, newScaleValue, 1);
    }

}
