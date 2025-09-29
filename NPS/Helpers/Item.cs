using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using ProtoBuf;

namespace NPS
{
    [ProtoContract]
    public class Item : IEquatable<Item>
    {
        [ProtoMember(1)]
        public string TitleId;
        [ProtoMember(2)]
        public string Region;
        [ProtoMember(3)]
        public string TitleName;
        [ProtoMember(4)]
        public string zRif;
        [ProtoMember(5)]
        public string pkg;
        
        [ProtoMember(6)]
        public DateTime lastModifyDate = DateTime.MinValue;

        public int DLCs => DlcItm?.Count ?? 0;
        [NonSerialized] [CanBeNull] public List<Item> DlcItm;

        public string extension => ItsCompPack ? ".ppk" : ".pkg";

        [ProtoMember(7)]
        public bool ItsPsx = false;
        [ProtoMember(8)]
        public bool ItsPsp = false;
        [ProtoMember(9)]
        public bool ItsPS3 = false;
        [ProtoMember(10)]
        public bool ItsPS4 = false;

        [ProtoMember(11)]
        public bool ItsCompPack = false;
        [ProtoMember(12)]
        public bool IsAvatar = false;
        [ProtoMember(13)]
        public bool IsDLC = false;
        [ProtoMember(14)]
        public bool IsTheme = false;
        [ProtoMember(15)]
        public bool IsUpdate = false;

        [ProtoMember(16)]
        public string ParentGameTitle = string.Empty;
        [ProtoMember(17)]
        public string ContentId = null;
        [ProtoMember(18)]
        public string offset = "";
        [ProtoMember(19)]
        public string contentType = "";

        public string DownloadFileName
        {
            get
            {
                string res = "";
                if (ItsPS3 || ItsCompPack) res = TitleName;
                else if (string.IsNullOrEmpty(ContentId)) res = TitleId;
                else res = ContentId;

                if (!string.IsNullOrEmpty(offset)) res += "_" + offset;

                string regexSearch =
                    new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
                return r.Replace(res, "");
            }
        }

        public bool CompareName(string name)
        {
            if (TitleId.Contains(name, StringComparison.OrdinalIgnoreCase)) return true;
            if (TitleName.Contains(name, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        public bool Equals(Item other)
        {
            if (other == null) return false;

            //return this.TitleId == other.TitleId && this.Region == other.Region && this.TitleName == other.TitleName && this.zRif == other.zRif && this.pkg == other.pkg;
            return TitleId == other.TitleId && Region == other.Region && DownloadFileName == other.DownloadFileName;
        }
    }
}