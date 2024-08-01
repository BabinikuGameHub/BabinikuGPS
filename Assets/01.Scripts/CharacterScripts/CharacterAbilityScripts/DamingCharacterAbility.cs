using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamingCharacterAbility : CharacterAbility
{
    public override double CalculatePValue(double pValue)
    {
        double AdditionValue = 0;

        if (otherCharacterReference.All(x => x.Type == PinType.ADD))
        {
            if (_level == 1)
            {
                AdditionValue += 800;
            }
            else if (_level == 2)
            {
                AdditionValue += 1800;
            }
            else if (_level == 3)
            {
                AdditionValue += 3000;
            }
        }

        return pValue + AdditionValue;
    }
}
