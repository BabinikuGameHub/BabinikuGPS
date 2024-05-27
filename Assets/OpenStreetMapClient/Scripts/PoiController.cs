using OSMClient;
using System;
using UnityEngine;

public class PoiController : MonoBehaviour
{
    [SerializeField] bool scaleWithMap = false;
    [SerializeField] float referenceScale = 100;
    Vector2d location;
    Vector3 initScale;

    void Start()
    {
        OSMController.Instance.MapChanged += UpdatePosition;
        Init();
    }

    public void Init()
    {
        initScale = transform.localScale;
    }

    public void SetLocation()
    {
        SetLocation(OSMController.Instance.Location);
    }

    public void SetLocation(Vector2d location)
    {
        this.location = location;
        UpdatePosition(OSMController.Instance);
    }

    private void UpdatePosition(OSMController map)
    {
        transform.localPosition = map.LocationToPosition(location.y, location.x);

        if (scaleWithMap)
        {
            var meterSize = map.GetMeterSize();
            transform.localScale = initScale * referenceScale * meterSize;
        }
    }

    private void OnDestroy()
    {
        OSMController.Instance.MapChanged -= UpdatePosition;
    }
}