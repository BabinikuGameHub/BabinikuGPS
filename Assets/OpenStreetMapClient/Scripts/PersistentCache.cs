using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace OSMClient
{
    /// <summary>
    /// Provides access to content caches.
    /// Stores data in persistent data storage.
    /// </summary>
    public static class PersistentCache
    {
        public static bool Initialized = false;
        private static string persistentDataPath;
        public static string cacheDataPath;

        public static void Init()
        {
            Initialized = true;
            persistentDataPath = Application.persistentDataPath;

            cacheDataPath = Path.Combine(persistentDataPath, "PersistentCache");

            if (!Directory.Exists(cacheDataPath))
                Directory.CreateDirectory(cacheDataPath);
        }

        /// <summary>
        /// Clean cache directory
        /// </summary>
        public static void ClearPersistentStorage()
        {
            if (!Initialized)
                Init();

            if (Directory.Exists(cacheDataPath))
            {
                Directory.Delete(cacheDataPath, true);
                Directory.CreateDirectory(cacheDataPath);
            }
        }

        public static byte[] TryLoad(string key, TimeSpan outOfDatePeriod)
        {
            if (!Initialized)
                Init();

            var fullPath = GetPath(key);
            if (File.Exists(fullPath))
                try
                {
                    var lastWrite = File.GetLastWriteTime(fullPath);

                    if (DateTime.Now - lastWrite < outOfDatePeriod)
                    {
                        return File.ReadAllBytes(fullPath);
                    }
                    else
                    {
                        File.Delete(fullPath);
                    }
                }
                catch { }

            return null;
        }

        public static void TryLoadAsync(string key, TimeSpan outOfDatePeriod, ConcurrentQueue<(byte[] bytes, Action<byte[]> act)> queue, Action<byte[]> onCompleted)
        {
            if (!Initialized)
                Init();

            ThreadPool.QueueUserWorkItem((o) =>
            {
                var bytes = TryLoad(key, outOfDatePeriod);
                if (onCompleted != null)
                    queue.Enqueue((bytes, onCompleted));
            });
        }

        public static string Save(string key, byte[] bytes)
        {
            if (!Initialized)
                Init();

            var fullPath = GetPath(key);
            File.WriteAllBytes(fullPath, bytes);

            return fullPath;
        }

        public static string SaveAsync(string key, byte[] bytes, Action onCompleted = null)
        {
            if (!Initialized)
                Init();

            var fullPath = GetPath(key);
            ThreadPool.QueueUserWorkItem((w) =>
            {
                Save(key, bytes);
                onCompleted?.Invoke();
            });

            return fullPath;
        }

        public static string GetPath(string key)
        {
            return Path.Combine(cacheDataPath, UnityWebRequest.EscapeURL(key));
        }

        public static bool Remove(string key)
        {
            var fullName = GetPath(key);
            if (File.Exists(fullName))
            {
                File.Delete(fullName);
                return true;
            }

            return false;
        }
    }
}