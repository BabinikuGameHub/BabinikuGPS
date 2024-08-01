using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SorukaCharacterAbility : CharacterAbility
{
    public override double CalculateXValue(double xValue)
    {
        double AdditionValue = 0;
        double multiplier = 1;

        multiplier = 1 + otherCharacterReference.Where(x => x.Level.Equals(_level)).ToList().Count;

        if (_level == 1)
        {
            AdditionValue += 1;
        }
        else if (_level == 2)
        {
            AdditionValue += 3;
        }
        else if (_level == 3)
        {
            AdditionValue += 5;
        }

        return xValue + (multiplier * AdditionValue);
    }
}
