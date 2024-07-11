using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictPanelScript : MonoBehaviour
{
    public static DictPanelScript Instance;

    [SerializeField] GameObject DictList;
    [SerializeField] GameObject DictSpecific;
    [SerializeField] DictContentPanelScript dictSpecificScript;
    [SerializeField] GameObject PanelHolder;

    private List<GameObject> list = new();

    [Header("Prefab")]
    [SerializeField] GameObject DictListPrefab;

    // Start is called before the first frame update

    private void Awake()
    {
        Instance = this;
    }

    public void TurnOnList()
    {
        if (DictList.activeSelf == true)
            return;

        DictList.SetActive(true);
        CreateDictList();
    }

    public void TurnOffList()
    {
        DictList.SetActive(false);
        DictSpecific.SetActive(false);
    }

    public void TurnOnSpecific(CharacterSO characterSO)
    {
        DictSpecific.SetActive(true);
        dictSpecificScript.InitializeWSO(characterSO);
    }

    public void TurnOffSpecific()
    {
        DictSpecific.SetActive(false);
    }

    private void CreateDictList()
    {
        EmptyList();

        List<CharacterSO> characterList = GachaManager.Instance.GetCharacterList();

        foreach(CharacterSO character in characterList)
        {
            GameObject panelObject = Instantiate(DictListPrefab, PanelHolder.transform);
            DictCharacterScript dictCharacterScript = panelObject.GetComponent<DictCharacterScript>();

            dictCharacterScript.InitializeWithSO(character);

            list.Add(panelObject);
        }

    }

    private void EmptyList()
    {
        if(list.Count > 0)
        {
            foreach(GameObject character in list)
            {
                character.Destroy();
            }
        }

        list = new();
    }
}

