using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.TextCore.Text;

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

    public override bool Equals(object obj)
    {
        // Check for null and compare run-time types.
        if (obj == null || GetType() != obj.GetType()) return false;

        CharacterSO other = (CharacterSO)obj;
        return CharacterName == other.CharacterName && Level == other.Level;
    }

    public override int GetHashCode()
    {
        return CharacterName.GetHashCode() ^ Level.GetHashCode();
    }

}

public enum PinType
{
    ADD,
    MULTIPLY,
    NONE,
}
