using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AhabCharacterAbility : CharacterAbility
{
    // Start is called before the first frame update
    public override double CalculateXValue(double xValue)
    {
        double AdditionValue = 0;

        if (_level == 1)
        {
            AdditionValue += 2;
        }
        else if (_level == 2)
        {
            AdditionValue += 4;
        }
        else if (_level == 3)
        {
            AdditionValue += 6;
        }

        return xValue + AdditionValue;
    }
}
