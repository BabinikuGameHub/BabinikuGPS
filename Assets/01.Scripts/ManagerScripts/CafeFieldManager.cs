using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CafeFieldManager : MonoBehaviour
{
    [SerializeField] Camera cafeCamera;

    [SerializeField] Transform charactersParent;
    private List<CafeFieldCharacter> characters = new();
    // Start is called before the first frame update
    private void Awake()
    {
        characters.AddRange(charactersParent.GetComponentsInChildren<CafeFieldCharacter>());
    }
    private void Start()
    {
        foreach (var character in characters)
        {
            character.Init(cafeCamera.transform);
        }
    }
}
