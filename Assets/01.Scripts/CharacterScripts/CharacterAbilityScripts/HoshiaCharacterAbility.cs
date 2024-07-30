using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoshiaCharacterAbility : CharacterAbility
{
    // Start is called before the first frame update
    public override double CalculateXValue(double xValue)
    {
        double AdditionValue = 0;

        foreach(CharacterSO characterSO in otherCharacterReference)
        {
            AdditionValue += characterSO.Level;
        }

        if(_level == 2)
        {
            AdditionValue += 1;
        }
        else if(_level == 3)
        {
            AdditionValue += 2;
        }

        return xValue + AdditionValue;
    }


}
