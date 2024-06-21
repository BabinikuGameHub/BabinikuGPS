namespace Mapbox.Examples
{
	using UnityEngine;
	using Mapbox.Utils;
	using Mapbox.Unity.Map;
	using Mapbox.Unity.MeshGeneration.Factories;
	using Mapbox.Unity.Utilities;
	using System.Collections.Generic;
    using Mapbox.Unity.Location;
    using System.Linq;
    using System.Collections;

    public class SpawnOnMap : MonoBehaviour
	{
		[SerializeField]
		AbstractMap _map;

		[SerializeField]
		Camera _mapCamera;

		[SerializeField]
		[Geocode]
		List<string> _locationStrings;
		List<Vector2d> _locations;

		[SerializeField]
		float _spawnScale = 1f;

		[SerializeField]
		private GameObject _poiHolderObject;
		[SerializeField]
		GameObject _markerPrefab;
		[SerializeField]
		ScorePopupScript _scorePopupScript; 

		List<GameObject> _spawnedObjects;

		bool _panSequence;
		float _baseZoom;
		double _areaScore;


        ILocationProvider _locationProvider;
		Unity.Location.Location _currentLocation;

        void Start()
		{
//#if UNITY_ANDROID
//			_locationStrings = new();
//#endif

			_locations = new();

			_spawnedObjects = new List<GameObject>();
			for (int i = 0; i < _locationStrings.Count; i++)
			{
				var locationString = _locationStrings[i];
				_locations.Add(Conversions.StringToLatLon(locationString));
				GameObject instance = Instantiate(_markerPrefab, _poiHolderObject.transform);
				instance.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);	
				instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
				_spawnedObjects.Add(instance);
			}

            _locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
            _locationProvider.OnLocationUpdated += UpdateCurrentLocation;
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
		}

        void UpdateCurrentLocation(Unity.Location.Location location)
		{
			_currentLocation = location;

        }


        public void CreatePOI()
		{
			if(_spawnedObjects.Count == 3)
			{
				_scorePopupScript.PopupEvent("이미 PIN을 3곳에 찍었습니다!");
				return;
            }

			Vector2d currentLocation = LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation.LatitudeLongitude;
			string currLocString = Conversions.LatLonToString(currentLocation);
			GameObject newPOI = Instantiate(_markerPrefab, _poiHolderObject.transform);

            newPOI.transform.position = _map.GeoToWorldPosition(currentLocation, true);
            newPOI.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);

			_spawnedObjects.Add(newPOI);
			_locationStrings.Add(currLocString);
			_locations.Add(currentLocation);
        }

		public void CalculateArea()
		{
			_baseZoom = _map.Zoom;

			if(_spawnedObjects.Count < 3)
            {
                _scorePopupScript.PopupEvent("PIN이 찍힌 곳이 3곳보다 적습니다!");
                return;
            }


			Vector2d centerPosition = GetCenterPosition(_locations);

			QuadTreeCameraMovement.Instance.SetCameraPosition(new Vector3(0, 0, 0));

			_map.UpdateMap(centerPosition);

			_panSequence = true;


            _areaScore = GeoAreaCalculator.CalculateArea(_locations[0].x, _locations[0].y, _locations[1].x, _locations[1].y, _locations[2].x, _locations[2].y);

			GameManager.Instance.AddScore((int)_areaScore);

			_scorePopupScript.PopupScoreEvent((int)_areaScore);
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

			foreach(Vector2d location in locations.Skip(1))
			{
				if(location.x > maxX) maxX = location.x;
				if(location.x < minY) minY = location.x;
				if(location.y > maxY) maxY = location.y;
				if(location.y < minY) minY = location.y;
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