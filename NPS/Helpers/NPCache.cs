using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace NPS.Helpers
{
    [Serializable]
    public class NPCache
    {
        private const string Path = "nps.cache";
        private static NPCache _instance;
        private bool _cacheInvalid;

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

        public DateTime UpdateDate { get; set; }
        public List<Item> localDatabase = new List<Item>();
        public List<Renascene> renasceneCache = new List<Renascene>();

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

        public static void Load()
        {
            if (File.Exists(Path))
            {
                try
                {
                    var json = File.ReadAllText(Path);
                    _instance = JsonConvert.DeserializeObject<NPCache>(json)
                                ?? new NPCache(DateTime.MinValue);

                    _instance.renasceneCache ??= new List<Renascene>();
                    _instance.localDatabase ??= new List<Item>();
                    return;
                }
                catch (JsonException)
                {
                    // bad cache → drop it
                }
            }

            _instance = new NPCache(DateTime.MinValue);
        }

        public void InvalidateCache()
        {
            _cacheInvalid = true;
        }

        public void Save(DateTime updateDate)
        {
            UpdateDate = updateDate;
            Save();
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(this, Formatting.None);
            File.WriteAllText(Path, json);
        }

        public NPCache(DateTime creationDate)
        {
            UpdateDate = creationDate;
        }
    }
}
