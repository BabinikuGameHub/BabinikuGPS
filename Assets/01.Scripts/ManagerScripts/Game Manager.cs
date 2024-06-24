using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Camera MainCamera;
    public Camera MapCamera;
    [SerializeField] GameObject _charPrefab;

    //UI canvas panels
    public GameObject CafePanel;
    public GameObject MapPanel;
    public GameObject GachaPanel;

    [SerializeField] private GameObject _charHolder;
    [SerializeField] private int CurrentScore;

    [SerializeField] private Dictionary<CharacterSO, List<GameObject>> _currentCharacters = new();

    public UnityEvent ScoreUpdate;

    public System.Random Rand = new();

    private void Awake()
    {
        Instance = this;
    }

    public void SwitchToMainCamera()
    {
        MainCamera.tag = "MainCamera";
        MapCamera.tag = "Untagged";
    }

    public void SwitchToMapCamera()
    {
        MainCamera.tag = "Untagged";
        MapCamera.tag = "MainCamera";
    }

    public void AddScore(int addedScore)
    {
        CurrentScore += addedScore;
        ScoreUpdate.Invoke();
    }

    public void SubtractScore(int removedScore)
    {
        CurrentScore -= removedScore;
        ScoreUpdate.Invoke();
    }

    public int GetScore()
    {
        return CurrentScore;
    }

    public Dictionary<CharacterSO, List<GameObject>> GetCurrentCharacterDict()
    {
        return _currentCharacters;
    }

    public void UseCharacter(CharacterSO charSO)
    {
        if(!_currentCharacters.ContainsKey(charSO))
        {
            return;
        }
        else
        {
            List<GameObject> charList = _currentCharacters[charSO];
            charList.RemoveAt(0);

            _currentCharacters[charSO] = charList;
        }
    }

    public void AddObtainedCharacters(List<CharacterSO> CharacterList)
    {
        foreach(CharacterSO addingCharacter in CharacterList)
        {
            GameObject newChar = Instantiate(_charPrefab, _charHolder.transform);
            CharPrefabScript charPrefabScript = newChar.GetComponent<CharPrefabScript>();
            charPrefabScript.IntializeFromSO(addingCharacter);
            SpriteRenderer charSpriteRenderer = newChar.GetComponentInChildren<SpriteRenderer>();
            charSpriteRenderer.enabled = false;

            if (_currentCharacters.ContainsKey(addingCharacter))
            {
                List<GameObject> charList = _currentCharacters[addingCharacter];

                charList.Add(newChar);

                _currentCharacters[addingCharacter] = charList;

            }
            else
            {
                List<GameObject> charList = new();
                charList.Add(newChar);

                _currentCharacters.Add(addingCharacter, charList);
            }
        }
    }
}
