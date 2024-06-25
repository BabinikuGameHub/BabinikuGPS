using Mapbox.Examples;
using Mapbox.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;

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
    [SerializeField] private int _currentScore;

    [SerializeField] private Dictionary<CharacterSO, int> _currentCharacters = new();

    public UnityEvent ScoreUpdate;

    public System.Random Rand = new();

    private PlayerData _playerData;
    private string saveFilePath;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "playerData.json");
        LoadProgress();
    }

    //세이브 로드 관련
    public void SaveProgress()
    {
        PlayerData data = new PlayerData
        {
            points = _currentScore,
            currentCharacters = _currentCharacters.Values.ToArray(),
            poiData = MapPOIManager.Instance.GetPOIDatas(),
        };


        string json = JsonConvert.SerializeObject(data);
        File.WriteAllText(saveFilePath, json);

        //PlayerPrefs.SetString("Progress", json);
        //PlayerPrefs.Save();
    }

    public void LoadProgress()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            //string json = PlayerPrefs.GetString("Progress", "");

            _playerData = JsonConvert.DeserializeObject<PlayerData>(json);


            _playerData.ApplyData();
        }
        else
        {
            _playerData = new PlayerData
            {
                points = _currentScore,
                currentCharacters = new int[GachaManager.Instance.GetCharacterList().Count],
                poiData = new List<POICharacterData>(),
            };

            _playerData.ApplyData();
        }
    }
    
    public void SetPoints(int points)
    {
        _currentScore = points;
    }

    public void SetCharacters(int[] characters)
    {
        Dictionary<CharacterSO, int> newDict = new();

        List<CharacterSO> characterList = GachaManager.Instance.GetCharacterList();

        for(int i = 0; i < characterList.Count; i++)
        {
            newDict.Add(characterList[i], characters[i]);
        }

        _currentCharacters = newDict;
    }

    private void OnApplicationQuit()
    {
        SaveProgress();
    }

    //카메라 전환 관련
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


    //점수 계산 관련
    public void AddScore(int addedScore)
    {
        _currentScore += addedScore;
        ScoreUpdate.Invoke();
    }

    public void SubtractScore(int removedScore)
    {
        _currentScore -= removedScore;
        ScoreUpdate.Invoke();
    }

    public int GetScore()
    {
        return _currentScore;
    }

    public Dictionary<CharacterSO, int> GetCurrentCharacterDict()
    {
        return _currentCharacters;
    }

    //캐릭터 습득,손실 관련
    public void UseCharacter(CharacterSO charSO)
    {
        if(!_currentCharacters.ContainsKey(charSO))
        {
            return;
        }
        else
        {
            int charNum = _currentCharacters[charSO];
            charNum--;

            _currentCharacters[charSO] = charNum;
        }
    }

    public void AddObtainedCharacters(List<CharacterSO> CharacterList)
    {
        foreach(CharacterSO addingCharacter in CharacterList)
        {

            if (_currentCharacters.ContainsKey(addingCharacter))
            {
                _currentCharacters[addingCharacter]++;

            }
            else
            {
                _currentCharacters.Add(addingCharacter, 1);
            }
        }
    }
}

[System.Serializable]
public class PlayerData
{
    public int points;
    public int[] currentCharacters;
    public List<POICharacterData> poiData;

    public void ApplyData()
    {
        GameManager.Instance.SetPoints(points);
        GameManager.Instance.SetCharacters(currentCharacters);
        MapPOIManager.Instance.SetLocations(poiData);
    }
}