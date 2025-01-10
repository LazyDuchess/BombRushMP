using BombRushMP.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace BombRushMP.Common
{
    public class AuthUser
    {
        [JsonIgnore]
        public UserKinds UserKind
        {
            get
            {
                if (Enum.TryParse<UserKinds>(_userKind, out var result))
                    return result;
                return UserKinds.Player;
            }

            set
            {
                _userKind = value.ToString();
            }
        }
        [JsonProperty("UserKind")]
        private string _userKind = UserKinds.Player.ToString();
        public string[] Tags = [];
        public string Badge = "";

        public bool HasTag(string tag)
        {
            return Tags.Contains(tag);
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write((int)UserKind);
            writer.Write(Tags.Length);
            foreach(var tag in Tags)
            {
                writer.Write(tag);
            }
            writer.Write(Badge);
        }

        public void Read(BinaryReader reader)
        {
            UserKind = (UserKinds)reader.ReadInt32();
            var tags = reader.ReadInt32();
            Tags = new string[tags];
            for(var i = 0; i < tags; i++)
            {
                Tags[i] = reader.ReadString();
            }
            Badge = reader.ReadString();
        }
    }
}
