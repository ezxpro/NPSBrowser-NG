using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace NPS.Helpers
{
    [System.Serializable]
    public class NPCache
    {
        private const string Path = "nps.cache";

        public static NPCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    Load();
                }

                return _instance;
            }
        }

        private bool _cacheInvalid;

        public bool IsCacheValid
        {
            get
            {
                if (_cacheInvalid)
                {
                    return false;
                }

                TimeSpan cacheAge = System.DateTime.Now - UpdateDate;
                bool isValid = cacheAge < TimeSpan.FromDays(4); // Valid if not older than 4 days
                
                return isValid;
            }
        }

        private static NPCache _instance;

        public DateTime UpdateDate { get; private set; }
        public List<Item> localDatabase = new List<Item>();
        public List<Renascene> renasceneCache = new List<Renascene>();

        public static void Load()
        {
            if (File.Exists(Path))
            {
                try
                {
                    using var stream = File.OpenRead(Path);
                    var formatter = new BinaryFormatter();
                    _instance = (NPCache)formatter.Deserialize(stream);
                    _instance.renasceneCache ??= new List<Renascene>();
                    return;
                }
                catch (SerializationException)
                {
                    // Nada.
                }
            }

            _instance = new NPCache(System.DateTime.MinValue);
        }

        public void InvalidateCache()
        {
            _cacheInvalid = true;
        }

        public void Save(System.DateTime updateDate)
        {
            UpdateDate = updateDate;
            Save();
        }

        public void Save()
        {
            using var fileStream = File.Create(Path);
            var formatter = new BinaryFormatter();
            formatter.Serialize(fileStream, this);
        }

        public NPCache(System.DateTime creationDate)
        {
            UpdateDate = creationDate;
        }
    }
}