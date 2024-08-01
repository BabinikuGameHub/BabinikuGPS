using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAbility : MonoBehaviour
{
    [SerializeField] protected CharacterSO _thisCharacterSO;
    protected int _level => _thisCharacterSO.Level;
    protected List<CharacterSO> otherCharacterReference = new List<CharacterSO>();

    // Start is called before the first frame update

    public virtual void AddCharacterReferences(List<CharacterSO> otherCharactersList)
    {
        otherCharacterReference = new();

        otherCharacterReference.AddRange(otherCharactersList);

        //foreach (CharacterSO character in otherCharactersList)
        //{
        //    otherCharacterReference.Add(character);
        //}
    }

    public virtual double CalculatePValue(double pValue)
    {
        return pValue;
    }

    public virtual double CalculateXValue(double xValue)
    {
        return xValue;
    }
}
