using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaButtonScript : MonoBehaviour
{
    [SerializeField] GachaResultPopupScript _resultScript;

    private int _currentGachaCost = 10000;

    public void RollGacha()
    {
        int currentPoint = GameManager.Instance.GetScore();

        if (currentPoint < _currentGachaCost)
            return;

        GameManager.Instance.SubtractScore(_currentGachaCost);

        List<CharacterSO> list = GachaManager.Instance.RollMultiCharacter(10);

        _resultScript.gameObject.SetActive(true);

        _resultScript.InitializeWithSO(list);

        GameManager.Instance.AddObtainedCharacters(list);

    }
}
