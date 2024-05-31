namespace Mapbox.Examples
{
	using UnityEngine;
	using Mapbox.Utils;
	using Mapbox.Unity.Map;
	using Mapbox.Unity.MeshGeneration.Factories;
	using Mapbox.Unity.Utilities;
	using System.Collections.Generic;
    using Mapbox.Unity.Location;

	public class SpawnOnMap : MonoBehaviour
	{
		[SerializeField]
		AbstractMap _map;

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

		List<GameObject> _spawnedObjects;

		void Awake()
		{
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
		}

		public void CreatePOI()
		{
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
			double area = 0;

			if(_spawnedObjects.Count < 3)
			{
                Debug.Log($"Not Enough Points");
                return;
            }

			area = GeoAreaCalculator.CalculateArea(_locations[0].x, _locations[0].y, _locations[1].x, _locations[1].y, _locations[2].x, _locations[2].y);

			Debug.Log($"Calculated Area is {area}");
        }
	}
}