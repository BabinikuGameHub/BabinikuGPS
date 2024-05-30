using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POIScript : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private string _locationString;

    //[SerializeField] private POIData _data;

    // Start is called before the first frame update
    void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }



}
