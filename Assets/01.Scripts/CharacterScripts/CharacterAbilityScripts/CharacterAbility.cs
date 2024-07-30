using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAbility : MonoBehaviour
{
    [SerializeField] protected CharacterSO _thisCharacterSO;
    protected int _level;
    protected List<CharacterSO> otherCharacterReference = new List<CharacterSO>();

    // Start is called before the first frame update

    private void Start()
    {
        _level = _thisCharacterSO.Level;
    }

    public virtual void AddCharacterReferences(List<CharacterSO> otherCharacterReference)
    {
        foreach(CharacterSO character in otherCharacterReference)
        {
            if (!character.Equals(_thisCharacterSO))
            {
                otherCharacterReference.Add(character);
            }
        }
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
