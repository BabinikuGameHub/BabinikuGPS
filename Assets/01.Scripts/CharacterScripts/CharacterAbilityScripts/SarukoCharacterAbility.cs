using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SarukoCharacterAbility : CharacterAbility
{
    public override double CalculateXValue(double xValue)
    {
        double AdditionValue = 0;

        if (_level == 1)
        {
            AdditionValue += 1.3;
        }
        else if (_level == 2)
        {
            AdditionValue += 1.6;
        }
        else if (_level == 3)
        {
            AdditionValue += 2;
        }

        return xValue * AdditionValue;
    }
}
