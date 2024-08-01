using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LemomiCharacterAbility : CharacterAbility
{
    public override double CalculateXValue(double xValue)
    {
        double AdditionValue = 0;

        if(otherCharacterReference.All(x => x.Type == PinType.ADD))
        {
            if (_level == 1)
            {
                AdditionValue += 3;
            }
            else if (_level == 2)
            {
                AdditionValue += 5;
            }
            else if (_level == 3)
            {
                AdditionValue += 10;
            }

        }

        return xValue + AdditionValue;
    }

}
