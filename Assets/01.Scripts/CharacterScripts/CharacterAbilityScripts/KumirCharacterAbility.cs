using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KumirCharacterAbility : CharacterAbility
{
    public override double CalculatePValue(double pValue)
    {
        double AdditionValue = 0;

        if (_level == 1)
        {
            AdditionValue += 200;
        }
        else if (_level == 2)
        {
            AdditionValue += 350;
        }
        else if (_level == 3)
        {
            AdditionValue += 500;
        }

        return pValue + AdditionValue;
    }
}
