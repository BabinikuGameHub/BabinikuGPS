using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

namespace OSMClient
{
    /// <summary>
    /// Applies GPS location to OSM client.
    /// </summary>
    public class GpsProvider : MonoBehaviour
    {
        #region Public fields

        public OSMController OSMController;
        [Tooltip("Automatically reset map location to GPS location, if user moves map more then given value (meters)")]
        public float AutoResetLocationDistance = 200;
        public bool CalculateViewDirection = true;
        public TextMeshProUGUI currentCoord;

        #endregion

        #region Private fields

        double lastAssignedLat;
        double lastAssignedLon;
        Queue<Vector2d> lastLocations = new Queue<Vector2d>();
        const float directionSensitivity = 1;
        bool ServiceIsRunning => Input.location.status == LocationServiceStatus.Running;
        Vector2d gpsLocation => new Vector2d(Input.location.lastData.longitude, Input.location.lastData.latitude);

        #endregion

        #region Public methods

        /// <summary>
        /// Forcibly set map location to GPS location
        /// </summary>
        public void SetLocationToMap()
        {
            if (ServiceIsRunning)
            if (OSMController != null)
            {
                var loc = gpsLocation;
                lastAssignedLat = loc.y;
                lastAssignedLon = loc.x;
                //
                lastLocations.Enqueue(new Vector2d(lastAssignedLon, lastAssignedLat));
                while (lastLocations.Count > 3)
                    lastLocations.Dequeue();
                //
                OSMController.Latitude = lastAssignedLat;
                OSMController.Longitude = lastAssignedLon;
                if (CalculateViewDirection)
                    SetViewDirection();
            }
        }

#endregion

        #region Private methods

        void OnEnable()
        {
            StartCoroutine(StartLocationService());

            //if (Input.location.isEnabledByUser)
            //    Input.location.Start();
        }

        IEnumerator StartLocationService()
        {
            // Request location permissions at runtime
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Permission.RequestUserPermission(Permission.FineLocation);
                yield return new WaitUntil(() => Permission.HasUserAuthorizedPermission(Permission.FineLocation));
            }

            // Ensure location service is enabled
            if (!Input.location.isEnabledByUser)
            {
                Debug.Log("Location services are not enabled by the user.");
                yield break;
            }

            // Start the location service
            Input.location.Start();

            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                Debug.Log("Location service initializing... wait time: " + maxWait);
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            if (maxWait < 1)
            {
                Debug.Log("Location service initialization timed out.");
                yield break;
            }

            if (Input.location.status == LocationServiceStatus.Failed)
            {
                Debug.Log("Unable to determine device location.");
                yield break;
            }

            Debug.Log("Location service running. Latitude: " + Input.location.lastData.latitude + " Longitude: " + Input.location.lastData.longitude);
        }

        void OnDisable()
        {
            Input.location.Stop();
        }

        void Update()
        {
            if (!ServiceIsRunning)
            {
                Debug.Log("Service Not Running");
                currentCoord.text = $"Service Not Running";
            }
            else if (OSMController != null)
            {
                if (LocationWasChangedByUser())
                {
                    var dist = LocationHelper.GetDistanceInMeters(OSMController.Location, gpsLocation);
                    if (dist > AutoResetLocationDistance * 2)
                    {
                        SimpleGestures.Instance?.StopMoving();
                        SetLocationToMap();
                    }
                    else
                    if (dist > AutoResetLocationDistance)
                    {
                        SimpleGestures.Instance?.StopMoving();
                        StopAllCoroutines();
                        StartCoroutine(MoveToGPSLocation());
                    }
                }
                else
                {
                    SetLocationToMap();
                }


                //currentCoord.text = $"X: BLANK \n Y: BLANK \n";
                currentCoord.text = $"X: {gpsLocation.x} \n Y: {gpsLocation.y} \n";
            }

        }

        IEnumerator MoveToGPSLocation()
        {
            var start = OSMController.Location;

            for (float k = 0.05f; k <= 1; k += 0.05f)
            if (ServiceIsRunning)
            if (OSMController != null)
            {
                OSMController.Location = Vector2d.Lerp(start, gpsLocation, k);
                lastAssignedLat = OSMController.Location.y;
                lastAssignedLon = OSMController.Location.x;
                yield return null;
            }

            SetLocationToMap();
        }

        private void SetViewDirection()
        {
            if (lastLocations.Count >= 3)
            {
                var points = lastLocations.ToArray();
                var dir1 = points[2] - points[1];
                var dir2 = points[1] - points[0];
                var l1 = dir1.magnitude;
                var l2 = dir2.magnitude;
                const double epsilon = 0.0000001 / directionSensitivity;
                if (l1 > epsilon && l2 > epsilon)
                {
                    dir1 = dir1 / l1;
                    dir2 = dir2 / l2;

                    var dot = Vector2d.Dot(dir1, dir2);
                    if (dot > 0.9)
                    {
                        //straight direction => set view dir on map
                        if (OSMController != null)
                            OSMController.SetViewDirection(points[1], points[2]);
                    }
                }
            }
        }

        bool LocationWasChangedByUser()
        {
            return OSMController != null &&
                   (Math.Abs(OSMController.Latitude - lastAssignedLat) > 2 * double.Epsilon ||
                   Math.Abs(OSMController.Longitude - lastAssignedLon) > 2 * double.Epsilon);
        }

#endregion
    }
}