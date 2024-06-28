using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaResultPopupScript : MonoBehaviour
{
    [SerializeField] GameObject _resultPrefab;
    [SerializeField] GameObject _resultGroup;


    // Start is called before the first frame update

    public void TurnOffPanel()
    {
        Destroy(gameObject);
    }

    public void InitializeWithSO(List<CharacterSO> list)
    {
        foreach (Transform child in _resultGroup.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < list.Count; i++)
        {
            GameObject resultPrefab = Instantiate(_resultPrefab, _resultGroup.transform);
            Image resultImage = resultPrefab.GetComponent<Image>();
            resultImage.sprite = list[i].CharacterSprite;
        }

    }
}
