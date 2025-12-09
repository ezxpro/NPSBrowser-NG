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

        [ProtoMember(2)]
        public List<Item> LocalDatabase { get; set; } = new List<Item>();

        [ProtoMember(3)]
        public List<Renascene> RenasceneCache { get; set; } = new List<Renascene>();

        public bool IsCacheValid
        {
            get
            {
                if (_cacheInvalid)
                    return false;

                TimeSpan cacheAge = DateTime.Now - UpdateDate;
                return cacheAge < TimeSpan.FromDays(4);
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

                    // Ensure lists are never null
                    _instance.LocalDatabase ??= new List<Item>();
                    _instance.RenasceneCache ??= new List<Renascene>();
                    return;
                }
                catch (ProtoException)
                {
                    // corrupted cache → rebuild
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

        // REQUIRED BY PROTOBUF
        private NPCache()
        {
            UpdateDate = DateTime.MinValue;
            LocalDatabase = new List<Item>();
            RenasceneCache = new List<Renascene>();
        }

        // Your original constructor
        public NPCache(DateTime creationDate) : this()
        {
            UpdateDate = creationDate;
        }
    }
}
