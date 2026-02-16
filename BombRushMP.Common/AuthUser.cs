using BombRushMP.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common
{
    public class AuthUser
    {
        [JsonProperty("Description")]
        private string _description = "";
        [JsonIgnore]
        public bool CanLurk => IsModerator || HasTag(SpecialPlayerUtils.SpecialPlayerTag);
        [JsonIgnore]
        public bool IsAdmin => UserKind == UserKinds.Admin;
        [JsonIgnore]
        public bool IsModerator => UserKind == UserKinds.Mod || UserKind == UserKinds.Admin;
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
        public int[] Badges = [];

        public AuthUser(UserKinds userKind = UserKinds.Player, string[] tags = null, int[] badges = null, string description = "")
        {
            UserKind = userKind;
            Tags = tags;
            if (Tags == null)
                Tags = [];
            Badges = badges;
            if (Badges == null)
                Badges = [];
            _description = description;
        }

        public AuthUser()
        {

        }

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
            writer.Write(Badges.Length);
            foreach (var badge in Badges)
            {
                writer.Write(badge);
            }
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
            var badges = reader.ReadInt32();
            Badges = new int[badges];
            for (var i = 0; i < badges; i++)
            {
                Badges[i] = reader.ReadInt32();
            }
        }

        public static string HashPassword(string password, string salt)
        {
            var keyBytes = Encoding.UTF8.GetBytes(salt);
            var messageBytes = Encoding.UTF8.GetBytes(password);

            using var hmac = new HMACSHA256(keyBytes);
            var hash = hmac.ComputeHash(messageBytes);

            return Convert.ToBase64String(hash);
        }
    }
}
