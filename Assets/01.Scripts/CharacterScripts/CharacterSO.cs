using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "ScriptableObject/CharacterSO")]

public class CharacterSO : ScriptableObject
{
    public string Name;
    public Sprite CharacterSprite;
    public Animation CharacterAnimation;

}
