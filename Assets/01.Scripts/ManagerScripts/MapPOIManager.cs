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

namespace Mapbox.Examples
{
    [Serializable]
    public class POICharacterData
    {
        public string SOID;
        public string locationString;
        [Serialize] public DateTime TimeStamp;

    }

    public class MapPOIManager : MonoBehaviour
    {
        public static MapPOIManager Instance;

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
        double _areaScore;

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

        [Header("Debug Related")]
        public bool IsDebug = false;
        [SerializeField] public TextMeshProUGUI _currentCoordinate;

        public UnityEvent OnPinUpdate;

        private void Awake()
        {
            Instance = this;
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
            return _locationDatas;
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
                spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
            }

            if (_panSequence)
            {
                PanCameraToFitPOI();

            }

            //Debug current location

            Vector2d currentLocation = LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation.LatitudeLongitude;
            string currLocString = Conversions.LatLonToString(currentLocation);
            _currentCoordinate.text = currLocString;

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


        public void CalculateArea()
        {
            _baseZoom = _map.Zoom;

            if (_spawnedObjects.Count < 3)
            {
                GameManager.Instance.PopupMessage("PIN이 찍힌 곳이 3곳보다 적습니다!");
                return;
            }

            if (!GameManager.Instance.CalculateResolve())
            {
                GameManager.Instance.PopupMessage("이미 오늘 하루치 결산을 했습니다!");
                return;
            }


            Vector2d centerPosition = GetCenterPosition(_locations);

            QuadTreeCameraMovement.Instance.SetCameraPosition(new Vector3(0, 0, 0));

            _map.UpdateMap(centerPosition);

            _panSequence = true;


            _areaScore = GeoAreaCalculator.CalculateArea(_locations[0].x, _locations[0].y, _locations[1].x, _locations[1].y, _locations[2].x, _locations[2].y);

            GameManager.Instance.AddScore((int)_areaScore);

            GameManager.Instance.PopupMessage((int)_areaScore);

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