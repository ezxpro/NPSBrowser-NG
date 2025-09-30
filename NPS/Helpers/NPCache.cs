using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;

namespace NPS.Helpers
{
    [ProtoContract]
    public class NPCache
    {
        private const string Path = "npsCache.bin";
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

        [ProtoMember(1)]
        public DateTime UpdateDate { get; set; }

        [field: ProtoMember(2)] 
        public List<Item> LocalDatabase { get; set; } = new List<Item>();

        [field: ProtoMember(3)] public List<Renascene> RenasceneCache { get; set; } = new List<Renascene>();

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
                    using (var file = File.OpenRead(Path))
                    {
                        _instance = Serializer.Deserialize<NPCache>(file);
                    }
                    _instance.RenasceneCache ??= new List<Renascene>();
                    _instance.LocalDatabase ??= new List<Item>();
                    return;
                }
                catch (ProtoException)
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
            using (var file = File.Create(Path))
            {
                Serializer.Serialize(file, this);
            }
        }

        public NPCache(DateTime creationDate)
        {
            UpdateDate = creationDate;
        }
    }
}
