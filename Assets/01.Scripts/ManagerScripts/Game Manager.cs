using Mapbox.Examples;
using Mapbox.Json;
using System;
using System.Collections;
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
    [SerializeField] GameObject _popupPrefab;

    //UI canvas panels
    public GameObject CafePanel;
    public GameObject MapPanel;
    public GameObject GachaPanel;
    public GameObject CommonPanel;


    [SerializeField] private GameObject _charHolder;
    [SerializeField] private int _currentScore;

    [SerializeField] private Dictionary<CharacterSO, int> _currentCharacters = new();

    public UnityEvent ScoreUpdate;

    public System.Random Rand = new();

    private PlayerData _playerData;
    private string saveFilePath;

    private bool _hasReset = false;
    private DateTime _lastResetTime;

    public DateTime LastResetTime
    {
        get
        {
            if(_lastResetTime == null)
            {
                return DateTime.Now;
            }
            else
            {
                return _lastResetTime;
            }
        }

        set
        {
            _lastResetTime = value;
        }
    }

    private int _resolveNum = 1;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "playerData.json");
        LoadProgress();


        InvokeRepeating("CheckResetTime", 0f, 60f);
    }

    //초기화 관련
    //일일 초기화 매커니즘
    // 일단 초기화 -> 초기화시간 기록 및 초기화한 사실(_hasReset) 기록 -> 게임 매 분(update나 invoke repeating 활용) 마다 체크 -> 아직 하루가 안 지났으면 그냥 return -> 4시가 지났다면 _hasReset을 false로 지정
    // -> _hasReset == false 면 초기화 


    void CheckResetTime()
    {

        if (IsResetTime(LastResetTime))
        {
            LastResetTime = DateTime.Now;
            newDayReset();
        }

    }

    bool IsResetTime(DateTime lastTime)
    {
        DateTime currentTime = System.DateTime.Now;


        DateTime fourAmToday = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 4, 0, 0);

        if (currentTime > lastTime && currentTime > fourAmToday && lastTime < fourAmToday)
        {
            return true;
        }

        return false;
    }

    void ResetResolveNum()
    {
        _resolveNum = 1;
    }

    void ResetPOI()
    {
        _playerData.ResetPOI();
    }

    public void newDayReset()
    {
        ResetResolveNum();
        ResetPOI();

        SaveProgress();


        PopupMessage("초기화 완료!");
    }

    //세이브 로드 관련
    public void SaveProgress()
    {
        PlayerData data = new PlayerData
        {
            points = _currentScore,
            currentCharacters = _currentCharacters.Values.ToArray(),
            poiData = MapPOIManager.Instance.GetPOIDatas(),
            ResetTime = LastResetTime,
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

    public void SetLastResetTime(DateTime lastResetTime) { LastResetTime = lastResetTime;}

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

    public bool CalculateResolve()
    {
        if(_resolveNum > 0)
        {
            _resolveNum--;
            return true;
        }
        else
        {
            return false;
        }
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

    //공용

    public void PopupMessage(string message)
    {
        GameObject popup = Instantiate(_popupPrefab, CommonPanel.transform);
        ScorePopupScript scorePopupScript = popup.GetComponent<ScorePopupScript>();
        scorePopupScript.PopupEvent(message);
    }

    public void PopupMessage(int score)
    {
        GameObject popup = Instantiate(_popupPrefab, CommonPanel.transform);
        ScorePopupScript scorePopupScript = popup.GetComponent<ScorePopupScript>();
        scorePopupScript.PopupScoreEvent(score);
    }
}

[System.Serializable]
public class PlayerData
{
    public int points;
    public int[] currentCharacters;
    public List<POICharacterData> poiData;
    public DateTime ResetTime;

    public void ApplyData()
    {
        GameManager.Instance.SetPoints(points);
        GameManager.Instance.SetCharacters(currentCharacters);
        GameManager.Instance.SetLastResetTime(ResetTime);
        MapPOIManager.Instance.SetLocations(poiData);
    }

    public void ResetPOI()
    {
        poiData = new();
        MapPOIManager.Instance.ResetPOIs();
    }
}