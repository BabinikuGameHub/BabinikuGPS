using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POIPanelScript : MonoBehaviour
{
    [SerializeField] private GameObject _characterHolder;
    [SerializeField] private GameObject _poiPrefab;

    public void TurnOnPanel()
    {
        gameObject.SetActive(true);
        InitializeCharHolder();
    }

    public void TurnOffPanel()
    {
        gameObject.SetActive(false);
    }
    
    private void InitializeCharHolder()
    {
        ClearPanel();

        Dictionary<CharacterSO, int> charDict = GameManager.Instance.GetCurrentCharacterDict();

        foreach(KeyValuePair<CharacterSO, int> pairs in charDict)
        {
            int num = pairs.Value;
            GameObject newPOI = Instantiate(_poiPrefab, _characterHolder.transform);
            CharacterPOIScript cpoiScript = newPOI.GetComponent<CharacterPOIScript>();
            cpoiScript.InitializeWSO(pairs.Key, num);
        }
    }

    private void ClearPanel()
    {
        foreach (Transform child in _characterHolder.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
