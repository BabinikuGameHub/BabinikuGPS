using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OSMClient
{
    public class StressTest : MonoBehaviour
    {
        [SerializeField] PoiController prefab;
        bool built;

        public void Build()
        {
            if (built)
                return;
            built = true;
            var loc = OSMController.Instance.Location;
            for (int i = 0; i < 1000; i++)
            {
                var poi = Instantiate(prefab);
                poi.Init();
                poi.SetLocation(new Vector2d(loc.x + Random.Range(-0.15f, 0.15f), loc.y + Random.Range(-0.15f, 0.15f)));
            }
        }
    }
}
