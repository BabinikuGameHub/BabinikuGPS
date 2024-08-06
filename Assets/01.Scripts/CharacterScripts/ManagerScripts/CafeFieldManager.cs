using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CafeFieldManager : MonoBehaviour
{
    public static CafeFieldManager Instance;

    [SerializeField] Camera cafeCamera;

    [SerializeField] Transform charactersParent;
    private List<CafeFieldCharacter> characters = new();
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        //characters.AddRange(charactersParent.GetComponentsInChildren<CafeFieldCharacter>());
    }
    //private void OnEnable()
    //{
    //    InitializeWithSaveData();


    //    foreach (var character in characters)
    //    {
    //        character.Init(cafeCamera.transform);
    //    }

    //}


    public void InitializeWithSaveData()
    {
        if(charactersParent.childCount != 0)
        {
            DestroyAllCafeChars();
        }

        Dictionary<CharacterSO, int> characterDict = GameManager.Instance.GetCurrentCharacterDict();

        foreach( KeyValuePair<CharacterSO, int> character in characterDict )
        {
            CharacterSO charSO = character.Key;
            for(int i = 0; i < character.Value; i++)
            {
                GameObject cafeCharacterObject = Instantiate(charSO.CharacterPrefab, charactersParent);
                CafeFieldCharacter charScript = cafeCharacterObject.GetComponent<CafeFieldCharacter>();
                //characters.Add(charScript);

                charScript.Init(cafeCamera.transform);
            }
        }

    }

    private void DestroyAllCafeChars()
    {
        foreach(Transform cafeCharObject in charactersParent)
        {
            cafeCharObject.gameObject.Destroy();
        }
    }
}
