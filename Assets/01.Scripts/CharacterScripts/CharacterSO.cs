using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

[CreateAssetMenu( menuName = "ScriptableObject/CharacterSO")]

public class CharacterSO : ScriptableObject
{
    public string uniqueID;
    public string CharacterName;
    public string CharacterDescription = "설명 채워 넣어야됨";
    public string AbilityDescription = "설명 추가하기";
    public int Level = 1;
    public PinType Type;
    public Sprite CharacterSprite;
    public GameObject CharacterPrefab;

    

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(uniqueID))
        {
            uniqueID = System.Guid.NewGuid().ToString();
            Debug.Log($"Generated new unique ID for {name}: {uniqueID}");
        }
    }


}

public enum PinType
{
    ADD,
    MULTIPLY,
    NONE,
}
