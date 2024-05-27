using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

namespace OSMClient
{
    /// <summary>
    /// Open Street Map Client
    /// </summary>
    [RequireComponent(typeof(PoolController))]
    [RequireComponent(typeof(SimpleGestures))]
    public class OSMController : MonoBehaviour
    {
        public static OSMController Instance { get; private set; }

        #region Public fields and properties
        [Header("General")]
        [SerializeField]
        private string tileServerURL = "http://b.tile.openstreetmap.org/{2}/{0}/{1}.png";
        [SerializeField]
        [Range(1, 5)]
        private int tilesAroundLocation = 2;
        [SerializeField]
        private double latitude = 50.449547;
        [SerializeField]
        private double longitude = 30.525394;
        [SerializeField]
        private float zoom = 17;
        public float MinZoom = 2f;
        public float MaxZoom = 19.9f;
        [SerializeField]
        private float tileSize = 2;

        public string TileServerURL { get => tileServerURL; set { if (tileServerURL != value) needRebuild = true; tileServerURL = value; } }
        public int TilesAroundLocation { get => tilesAroundLocation; set { if (tilesAroundLocation != value) needRebuild = true; tilesAroundLocation = value; } }

        /// <summary>Current Latitude of map center</summary>
        public double Latitude
        {
            get => latitude;
            set
            {
                if (Math.Abs(lastBuildLocation.y - value) > epsilon) needRebuild = true;
                if (value > 80) value = 80;
                if (value < -80) value = -80;
                latitude = value;
            }
        }

        /// <summary>Current Longitude of map center</summary>
        public double Longitude
        {
            get => longitude;
            set
            {
                if (Math.Abs(lastBuildLocation.x - value) > epsilon) needRebuild = true;
                while (value > 180) value -= 360;
                while (value <= -180) value += 360;
                longitude = value;
            }
        }
        /// <summary>Current location of map center (Longitude, Latitude)</summary>
        public Vector2d Location { get { return new Vector2d(Longitude, Latitude); } set { Longitude = value.x; Latitude = value.y; } }
        /// <summary>Zoom of map (power of two)</summary>
        public float Zoom { get => zoom; set { if (zoom != value) needRebuild = true; zoom = value; } }
        /// <summary>Absolute scale of the map</summary>
        public float Scale { get => (1 << (int)zoom) * (1 + zoom - (int)zoom); }

        public float TileSize { get => tileSize; set { if (tileSize != value) needRebuild = true; tileSize = value; } }

        public CachingMode Caching = CachingMode.Asynchronous;

        [Header("Gestures")]
        public bool EnableGestures = true;
        public RotatingMode RotatingMode = RotatingMode.RotatePlayer;

        [Header("Objects")]
        [SerializeField]
        GameObject tilePrefab;
        [SerializeField]
        Texture defaultBackground;
        [SerializeField]
        Camera camera;
        [SerializeField]
        Transform player;
        #endregion

        #region Public events
        /// <summary>
        /// Fires when map was rebuilt
        /// </summary>
        public event Action<OSMController> MapChanged = delegate { };
        #endregion

        #region Private fields
        const double epsilon = 0.0000009d;
        const float tileY = -0.001f;
        const string OpenStreetMapLink = "https://www.openstreetmap.org/copyright";
        PoolController Pool;
        ConcurrentQueue<(byte[] bytes, Action<byte[]> act)> loadTilesQueue = new ConcurrentQueue<(byte[] bytes, Action<byte[]> act)>();
        Dictionary<Vector2Int, Tile> shownTiles = new Dictionary<Vector2Int, Tile>();
        Queue<Texture2D> texturesPool = new Queue<Texture2D>();
        Queue<(Material mat, Texture2D tex, byte[] bytes)> updateTextureQueue = new Queue<(Material, Texture2D, byte[])>();
        TimeSpan expPeriod = new TimeSpan(15, 0, 0, 0);
        int shownIntZoom;
        bool needRebuild = true;
        int intZoom { get => (int)Zoom; }
        float actualTileSize { get => tileSize * (1 + (Zoom - (int)Zoom)); }
        Vector2d lastBuildLocation;
        #endregion

        #region Public methods
        /// <summary>
        /// Builds map for current location
        /// </summary>
        public void Build()
        {
            var twoPowZoom = 1 << intZoom;

            //remember last build location
            lastBuildLocation = Location;

            //central tile
            var centerPos = LocationHelper.WorldToTilePos(Longitude, Latitude, intZoom);
            if (centerPos.x >= twoPowZoom - epsilon)
                centerPos.x -= twoPowZoom;

            var center = new Vector2Int((int)centerPos.x, (int)centerPos.y);

            //remove unused tiles
            if (shownIntZoom != intZoom)
            {
                RemoveAllTiles();
            }
            else
            {
                foreach (var coord in shownTiles.Keys.ToArray())
                    if (!IsTileInsideBounds(coord, center, twoPowZoom))
                        RemoveTile(shownTiles[coord]);
            }

            //create tiles, update position
            foreach(var p in MathHelper.GetSpiralFromCenter(center.x - TilesAroundLocation, center.y - TilesAroundLocation, TilesAroundLocation * 2 + 1, TilesAroundLocation * 2 + 1))
            {
                //lonigtude 180 correction
                var xx = p.x;
                if (xx < 0) xx += twoPowZoom;
                if (xx >= twoPowZoom) xx -= twoPowZoom;

                //
                var coord = new Vector2Int(xx, p.y);

                //download tile if not presented
                Tile tile;
                if (!shownTiles.TryGetValue(coord, out tile))
                {
                    tile = CreateTile(coord);
                }

                //set position
                var dx = (float)(tile.Coord.x - centerPos.x);
                var dy = (float)(centerPos.y - tile.Coord.y);
                //lonigtude 180 correction
                if (p.x < 0) dx -= twoPowZoom;
                if (p.x >= twoPowZoom) dx += twoPowZoom;
                //
                tile.Holder.transform.localScale = new Vector3(actualTileSize, actualTileSize, 1);
                tile.Holder.transform.localRotation = Quaternion.AngleAxis(90, Vector3.right);
                tile.Holder.transform.localPosition = new Vector3(dx + 0.5f, tileY, dy - 0.5f) * actualTileSize;
            }

            //
            shownIntZoom = intZoom;
            needRebuild = false;
            MapChanged?.Invoke(this);
        }

        public void SetViewDirection(Vector2d fromLocation, Vector2d toLocation)
        {
            var p1 = LocationToPosition(fromLocation.y, fromLocation.x);
            var p2 = LocationToPosition(toLocation.y, toLocation.x);
            var dir = (p2 - p1).normalized;
            switch (RotatingMode)
            {
                case RotatingMode.RotateCamera:
                    camera.transform.LookAt(camera.transform.position + Vector3.down, dir);
                    var a = camera.transform.rotation.eulerAngles.y;
                    player.transform.rotation = Quaternion.Euler(90, a, 0);
                    break;

                case RotatingMode.RotatePlayer:
                default:
                    player.transform.LookAt(player.transform.position + Vector3.down, dir);
                    camera.transform.rotation = Quaternion.Euler(90, 0, 0);
                    break;
            }
        }
        #endregion

        #region Public utils
        /// <summary>
        /// Returns local position (relative to Map GameObject) for passed Latitude and Longitude
        /// </summary>
        public Vector3 LocationToPosition(double lat, double lon)
        {
            var centerPos = LocationHelper.WorldToTilePos(Longitude, Latitude, intZoom);
            var pos = LocationHelper.WorldToTilePos(lon, lat, intZoom);
            var dx = (float)(pos.x - centerPos.x);
            var dy = (float)(centerPos.y - pos.y);
            return new Vector3(dx, 0f, dy) * actualTileSize;
        }


        /// <summary>
        /// Translate local position (relative to Map GameObject) to Latitude and Longitude
        /// </summary>
        public Vector2d PositionToLocation(Vector3 pos)
        {
            var centerPos = LocationHelper.WorldToTilePos(Longitude, Latitude, intZoom);
            var x = pos.x / actualTileSize + centerPos.x;
            var y = centerPos.y - pos.z / actualTileSize;

            return LocationHelper.TileToWorldPos(x, y, intZoom);
        }

        /// <summary>Returns size of one meter relatively to current map scale and location</summary>
        public float GetMeterSize()
        {
            var meterInLongLat = LocationHelper.MeterToLongLat(Location);
            var pos = LocationToPosition(Location.y + meterInLongLat.y, Location.x + meterInLongLat.x);
            return pos.z;//take to attention only meter size by longitude
        }
        #endregion

        #region Private methods

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            Pool = GetComponent<PoolController>();
            SimpleGestures.OnPan += Gestures_OnPan;
            SimpleGestures.OnScale += Gestures_OnScale;
            SimpleGestures.OnRotate += Gestures_OnRotate;
        }

        private void Gestures_OnPan(Vector3 delta)
        {
            if (!EnableGestures) return;

#if UNITY_STANDALONE || UNITY_EDITOR
            if (Input.GetMouseButton(1))
            {
                //right mouse buttion => rotate
                delta = camera.transform.InverseTransformDirection(delta);
                Gestures_OnRotate(delta.x * 50);
                return;
            }
#endif

            var d = PositionToLocation(-delta);
            Longitude = d.x;
            Latitude = d.y;
        }

        private void Gestures_OnRotate(float dAng)
        {
            if (!EnableGestures) return;

            switch (RotatingMode)
            {
                case RotatingMode.RotateCamera:
                    camera.transform.Rotate(new Vector3(0, dAng, 0), Space.World);
                    var a = camera.transform.rotation.eulerAngles.y;
                    player.transform.rotation = Quaternion.Euler(90, a, 0);
                    break;
                case RotatingMode.RotatePlayer:
                    player.transform.Rotate(new Vector3(0, -dAng, 0), Space.World);
                    camera.transform.rotation = Quaternion.Euler(90, 0, 0);
                    break;
            }
        }

        private void Gestures_OnScale(float d)
        {
            if (!EnableGestures) return;

            var zoom = Zoom - 1 + d;
            if (zoom < MinZoom) zoom = MinZoom;
            if (zoom > MaxZoom) zoom = MaxZoom;
            Zoom = zoom;
        }

        void Update()
        {
            //some properties were changed
            if (needRebuild)
                Build();

            if (SimpleGestures.Instance && camera)
                SimpleGestures.Instance.ZPos = camera.transform.position.y;

            //execute delayed actions
            while (loadTilesQueue.TryDequeue(out var tuple))
                tuple.act(tuple.bytes);

            //flush delayed textures
            const int maxTexturesPerFrame = 4;

            for (int i = 0; i < maxTexturesPerFrame; i++)
            if (updateTextureQueue.Count > 0)
            {
                var tuple = updateTextureQueue.Dequeue();
                if (tuple.mat.mainTexture && tuple.mat.mainTexture != defaultBackground && tuple.tex != tuple.mat.mainTexture)
                    texturesPool.Enqueue((Texture2D)tuple.mat.mainTexture);
                tuple.tex.LoadImage(tuple.bytes);
                tuple.mat.mainTexture = tuple.tex;
            }
        }

        private bool IsPointerOverUIObject(Vector2 pos)
        {
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = pos;
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        private void OnValidate()
        {
            //some properties were changed (in editor)
            needRebuild = true;
        }

        private Tile CreateTile(Vector2Int coord)
        {
            var tile = new Tile() { Coord = coord, Holder = Pool.CreateFromPool(tilePrefab) };
            shownTiles[coord] = tile;
            var key = $"OSM_tile_{intZoom}_{coord.x}_{coord.y}";
            tile.Holder.name = key;

            switch (Caching)
            {
                case CachingMode.Disabled:
                    StartCoroutine(LoadTile(tile, intZoom, key));
                    break;
                case CachingMode.Asynchronous:
                    PersistentCache.TryLoadAsync(key, expPeriod, loadTilesQueue, (bytes) =>
                    {
                        if (bytes != null)
                            InitTile(tile, bytes);
                        else
                            StartCoroutine(LoadTile(tile, intZoom, key));
                    });
                    break;
                case CachingMode.Synchronous:
                {
                    var bytes = PersistentCache.TryLoad(key, expPeriod);
                    if (bytes != null)
                        InitTile(tile, bytes);
                    else
                        StartCoroutine(LoadTile(tile, intZoom, key));
                }
                break;
            }

            return tile;
        }

        private IEnumerator LoadTile(Tile tile, int zoom, string key)
        {
            var url = string.Format(TileServerURL, tile.Coord.x, tile.Coord.y, zoom);
            var loaded = new UnityWebRequest(url, "GET", new DownloadHandlerBuffer(), null);
            yield return loaded.SendWebRequest();

            byte[] bytes = null;

            if (string.IsNullOrEmpty(loaded.error))
            {
                bytes = loaded.downloadHandler.data;
                switch (Caching)
                {
                    case CachingMode.Disabled:
                        break;
                    case CachingMode.Synchronous:
                    case CachingMode.Asynchronous:
                        PersistentCache.SaveAsync(key, bytes);
                        break;
                }
            }

            if (!tile.IsDropped)
                InitTile(tile, bytes);
        }

        private void InitTile(Tile tile, byte[] bytes)
        {
            var mat = tile.Holder.GetComponent<Renderer>().material;
            //get or create texture
            var tex = (Texture2D)mat.mainTexture;
            if (!tex || mat.mainTexture == defaultBackground)
            {
                if (texturesPool.Count > 0)
                    tex = texturesPool.Dequeue();
                else
                    tex = new Texture2D(2, 2);
            }

            //flush image data to texture
            updateTextureQueue.Enqueue((mat, tex, bytes));
            mat.mainTexture = defaultBackground;
        }

        private void RemoveAllTiles()
        {
            foreach (var tile in shownTiles.Values.ToArray())
                RemoveTile(tile);
        }

        private void RemoveTile(Tile tile)
        {
            tile.IsDropped = true;
            Pool.ReturnToPool(tile.Holder);
            shownTiles.Remove(tile.Coord);
            var mat = tile.Holder.GetComponent<Renderer>().material;
            //save unused texture in pool
            if (mat.mainTexture && mat.mainTexture != defaultBackground)
                texturesPool.Enqueue((Texture2D)mat.mainTexture);
            //assign default texture
            mat.mainTexture = defaultBackground;
        }

        private bool IsTileInsideBounds(Vector2Int tile, Vector2Int center, int twoPowZoom)
        {
            if (Math.Abs(tile.y - center.y) > TilesAroundLocation) return false;
            var dx = Math.Abs(tile.x - center.x);
            if (dx <= TilesAroundLocation) return true;
            if (dx >= twoPowZoom - TilesAroundLocation) return true;
            return false;
        }

        public void OnCopyrightClicked()
        {
            Application.OpenURL(OpenStreetMapLink);
        }
        #endregion

        #region Private classes
        class Tile
        {
            public GameObject Holder;
            public Vector2Int Coord;
            public bool IsDropped;
        }
        #endregion
    }

    public enum CachingMode
    {
        Disabled,
        Synchronous,
        Asynchronous
    }

    public enum RotatingMode
    {
        Disabled,
        RotateCamera,
        RotatePlayer
    }
}