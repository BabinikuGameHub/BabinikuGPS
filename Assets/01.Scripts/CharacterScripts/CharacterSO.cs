using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

[CreateAssetMenu( menuName = "ScriptableObject/CharacterSO")]

public class CharacterSO : ScriptableObject
{
    public string uniqueID;
    public string Name;
    public Sprite CharacterSprite;
    public Animation CharacterAnimation;


    private void OnValidate()
    {
        if (string.IsNullOrEmpty(uniqueID))
        {
            uniqueID = System.Guid.NewGuid().ToString();
            Debug.Log($"Generated new unique ID for {name}: {uniqueID}");
        }
    }
}
