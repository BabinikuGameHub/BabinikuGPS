using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using System.Collections.Generic;
using Mapbox.Unity.Location;
using System.Linq;
using System.Collections;
using System;
using global::Unity.VisualScripting;
using TMPro;
using UnityEngine.Events;
using static UnityEngine.GUILayout;

namespace Mapbox.Examples
{
    [Serializable]
    public class POICharacterData
    {
        public string SOID;
        public string Name;
        public string locationString;
        public bool isDebug = false;
        [Serialize] public DateTime TimeStamp;

        public CharacterSO GetCharacterSO()
        {
            return GachaManager.Instance.GetSOByName(Name);
        }

    }

    public class MapPOIManager : MonoBehaviour
    {
        public static MapPOIManager Instance;

        public bool isDebug;
        
        private bool _poiDebug;
        public bool PoiDebug
        {
            get
            {
                return _poiDebug;
            }
            set
            {
                _poiDebug = value;

                if(_poiDebug == false)
                {
                    _debugText.SetActive(false);
                }
                else
                {
                    _debugText.SetActive(true);
                }
            }
        }

        [SerializeField]
        AbstractMap _map;

        [SerializeField]
        Camera _mapCamera;

        [SerializeField]
        [Geocode]
        List<Vector2d> _locations;

        List<POICharacterData> _locationDatas;

        [SerializeField]
        float _spawnScale = 1f;

        [SerializeField]
        GameObject _poiHolderObject;
        [SerializeField]
        GameObject _markerPrefab;

        List<GameObject> _spawnedObjects;

        bool _panSequence;
        float _baseZoom;

        //Score Calculation Variables
        double _areaScore;
        double _areaPlus = 0;
        double _multiplier = 1;

        private int _remainingPin;

        public int RemainingPin
        {
            get
            {
                return _remainingPin;
            }
            set
            {
                _remainingPin = value;
                OnPinUpdate?.Invoke();
            }
        }

        [SerializeField]private int _maxPin;

        public int MaxPin
        {
            get
            {
                return _maxPin;
            }
            set
            {
                _maxPin = value;
                OnPinUpdate?.Invoke();
            }
        }


        const int INITIALMAXPIN = 3;

        public UnityEvent OnPinUpdate;

        [Header("Debug Related")]
        public bool IsDebug = false;
        [SerializeField] public TextMeshProUGUI _currentCoordinate;
        [SerializeField] private GameObject _mapCharacterObject;
        [SerializeField] private GameObject _debugText;


        private void Awake()
        {
            Instance = this;

        }

        private void OnEnable()
        {
            if (MaxPin == 0)
            {
                MaxPin = 3;
                RemainingPin = 3;
                OnPinUpdate.Invoke();
            }
        }

        void Start()
        {
            if (IsDebug)
                _locationDatas = new();

            _locations = new();
            _spawnedObjects = new();

            if (_locationDatas.Count != 0)
                SetLocations(_locationDatas);

        }

        public List<string> GetLocationStrings()
        {
            return _locationDatas.Select(x => x.locationString).ToList();
        }

        public List<POICharacterData> GetPOIDatas()
        {
            return _locationDatas.Where(x => !x.isDebug).ToList();
        }


        public void SetLocations(List<POICharacterData> locationdatas)
        {
            _locationDatas = locationdatas;

            for (int i = 0; i < _locationDatas.Count; i++)
            {
                POICharacterData locationData = _locationDatas[i];
                _locations.Add(Conversions.StringToLatLon(locationData.locationString));

                GameObject instance = Instantiate(_markerPrefab, _poiHolderObject.transform);
                CharPrefabScript charPrefabScript = instance.GetComponent<CharPrefabScript>();

                charPrefabScript.InitializeFromPOIData(locationData);

                instance.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);
                instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
                _spawnedObjects.Add(instance);
            }

        }

        private void Update()
        {
            int count = _spawnedObjects.Count;
            for (int i = 0; i < count; i++)
            {
                var spawnedObject = _spawnedObjects[i];
                var location = _locations[i];
                spawnedObject.transform.position = _map.GeoToWorldPosition(location, true);
                //spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
            }

            if (_panSequence)
            {
                PanCameraToFitPOI();

            }

            //Debug current location

            Vector2d currentLocation = LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation.LatitudeLongitude;
            string currLocString = Conversions.LatLonToString(currentLocation);
            _currentCoordinate.text = currLocString;

            _currentCoordinate.text = UnixTimestampUtils.From(LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation.TimestampDevice).ToLocalTime().ToString();

        }


        public void ResetPOIs()
        {
            foreach(GameObject poi in _spawnedObjects)
            {
                Destroy(poi);
            }

            _spawnedObjects = new();
            _locationDatas = new();
            _locations = new();

            _remainingPin = _maxPin;
        }

        public void CreatePOI(CharacterSO characterSO)
        {
            if (_remainingPin <= 0)
            {
                GameManager.Instance.PopupMessage("이미 사용할 수 있는 PIN을 전부 소모했습니다");
                return;
            }


            GameManager.Instance.UseCharacter(characterSO);

            Vector2d currentLocation = LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation.LatitudeLongitude;
            GameObject newPOI = Instantiate(_markerPrefab, _poiHolderObject.transform);
            CharPrefabScript charPrefabScript = newPOI.GetComponent<CharPrefabScript>();

            string currLocString = Conversions.LatLonToString(currentLocation);
            DateTime createdTime = DateTime.Now;


            POICharacterData poidata = new POICharacterData
            {
                SOID = characterSO.uniqueID,
                Name = characterSO.CharacterName,
                locationString = currLocString,
                TimeStamp = createdTime,
            };

            charPrefabScript.InitializeFromPOIData(poidata);

            newPOI.transform.position = _map.GeoToWorldPosition(currentLocation, true);
            newPOI.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);


            _locationDatas.Add(poidata);

            _spawnedObjects.Add(newPOI);
            _locations.Add(currentLocation);

            _remainingPin--;

            GameManager.Instance.SaveProgress();
        }


        public void CreatePOIDebug(CharacterSO characterSO)
        {
            if (_remainingPin <= 0)
            {
                GameManager.Instance.PopupMessage("이미 사용할 수 있는 PIN을 전부 소모했습니다");
                return;
            }

            //GameManager.Instance.UseCharacter(characterSO);

            Vector2d currentPlayerLocation = LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation.LatitudeLongitude;
            Vector3 playerLocationLocal = _map.GeoToWorldPosition(currentPlayerLocation);

            Vector3 DebugPosition = new Vector3(playerLocationLocal.x -_mapCharacterObject.transform.position.x, 0, playerLocationLocal.z - _mapCharacterObject.transform.position.z);

            Vector2d currentLocation = _map.WorldToGeoPosition(DebugPosition);


            GameObject newPOI = Instantiate(_markerPrefab, _poiHolderObject.transform);
            CharPrefabScript charPrefabScript = newPOI.GetComponent<CharPrefabScript>();

            string currLocString = Conversions.LatLonToString(currentLocation);
            DateTime createdTime = DateTime.Now;


            POICharacterData poidata = new POICharacterData
            {
                SOID = characterSO.uniqueID,
                Name = characterSO.CharacterName,
                locationString = currLocString,
                TimeStamp = createdTime,
                isDebug = true,
            };

            charPrefabScript.InitializeFromPOIData(poidata);

            newPOI.transform.position = _map.GeoToWorldPosition(currentLocation, true);
            newPOI.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);


            _locationDatas.Add(poidata);

            _spawnedObjects.Add(newPOI);
            _locations.Add(currentLocation);

            //_remainingPin--;

            //GameManager.Instance.SaveProgress();
        }


        public void CalculateArea()
        {
            Vector2d centerPosition = GetCenterPosition(_locations);

            QuadTreeCameraMovement.Instance.SetCameraPosition(new Vector3(0, 0, 0));

            _map.UpdateMap(centerPosition);
            _panSequence = true;


            _areaScore = GeoAreaCalculator.CalculateArea(_locations[0].x, _locations[0].y, _locations[1].x, _locations[1].y, _locations[2].x, _locations[2].y);

        }

        public void CalculatePOIVariables()
        {
            List<string> POIName = _locationDatas.Select(x => x.Name).ToList();

            foreach(var poiName in POIName)
            {
                CharacterSO characterSO = GachaManager.Instance.GetSOByName(poiName);
            }
        }


        public int CalculateCharacterAbility()
        {
            List<CharacterSO> charSOList = new();
            List<CharacterAbility> charAbilityList = new();

            foreach (POICharacterData charData in _locationDatas)
            {
                CharacterSO dataSO = charData.GetCharacterSO();
                CharacterAbility dataAbility = charData.GetCharacterSO().CharacterPrefab.GetComponent<CharacterAbility>();

                charSOList.Add(dataSO);
                charAbilityList.Add(dataAbility);

            }

            for(int i = 0; i < charAbilityList.Count; i++)
            {
                CharacterAbility currentChar = charAbilityList[i];

                List<CharacterSO> referenceList = new(charSOList);
                referenceList.RemoveAt(i);

                currentChar.AddCharacterReferences(referenceList);


                _areaPlus = currentChar.CalculatePValue(_areaPlus);
                _multiplier = currentChar.CalculateXValue(_multiplier);

            }

            return (int)((_areaScore + _areaPlus) * _multiplier);
        }


        public void ResolvePoints()
        {
            _baseZoom = _map.Zoom;

            if (_spawnedObjects.Count < 3)
            {
                GameManager.Instance.PopupMessage("PIN이 찍힌 곳이 3곳보다 적습니다!");
                return;
            }

            if (!GameManager.Instance.CalculateResolve() && isDebug == false)
            {
                GameManager.Instance.PopupMessage("이미 오늘 하루치 결산을 했습니다!");
                return;
            }


            CalculateArea();

            int TotalScore = CalculateCharacterAbility();

            if(!_locationDatas.Any(x => x.isDebug))
            {
                GameManager.Instance.AddScore((int)TotalScore);
            }

            GameManager.Instance.PopupMessage((int)TotalScore, (int)_areaScore, (int)_areaPlus, (int)_multiplier);

            GameManager.Instance.SaveProgress();
        }

        private void PanCameraToFitPOI()
        {
            bool allInView = _spawnedObjects.All(x => IsWithinViewPort(_mapCamera.WorldToViewportPoint(x.transform.position)) == true);

            if (!allInView)
            {
                _map.UpdateMap(_map.Zoom * 0.99f);
            }
            else
            {
                _panSequence = false;
                _map.UpdateMap(_map.Zoom * 0.99f);
                StartCoroutine(ScoreUI());
            }
        }


        IEnumerator ScoreUI()
        {
            Debug.Log($"{_areaScore} 점 획득!");

            yield return new WaitForSeconds(3f);

            _map.UpdateMap(_baseZoom);

            QuadTreeCameraMovement.Instance.ResetCameraPosition();
        }

        Vector2d GetCenterPosition(List<Vector2d> locations)
        {
            double maxX = locations[0].x;
            double minX = locations[0].x;
            double minY = locations[0].y;
            double maxY = locations[0].y;

            foreach (Vector2d location in locations.Skip(1))
            {
                if (location.x > maxX) maxX = location.x;
                if (location.x < minY) minY = location.x;
                if (location.y > maxY) maxY = location.y;
                if (location.y < minY) minY = location.y;
            }

            double midX = (minX + maxX) / 2;
            double midY = (minY + maxY) / 2;

            Vector2d newValue = new Vector2d(midX, midY);

            double x = newValue.x;
            double y = newValue.y;

            return newValue;
        }

        bool IsWithinViewPort(Vector3 viewPos)
        {
            if (viewPos.x > 1 || viewPos.x < 0)
                return false;
            if (viewPos.y > 1 || viewPos.y < 0)
                return false;

            return true;
        }


    }
}