using BombRushMP.Common;
using BombRushMP.Server;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.ServerApp
{
    public class AppAuthKeys : AuthKeys
    {
        [JsonIgnore]
        public string SQLConnectionString;
        [JsonIgnore]
        private bool _usesSQL = false;

        public void SetSQLConnection(string connectionString)
        {
            _usesSQL = true;
            SQLConnectionString = connectionString;
        }

        public override AuthUser GetUser(string key, string challenge)
        {
            if (_usesSQL)
            {
                using var db = new NpgsqlConnection(SQLConnectionString);
                db.Open();

                using var tokenCmd = new NpgsqlCommand("SELECT discord_id, game_token FROM users", db);

                using var tokenResult = tokenCmd.ExecuteReader();

                Guid finalGuid = Guid.Empty;
                var finalId = string.Empty;

                while (tokenResult.Read())
                {
                    var id = tokenResult.GetString(0);
                    var guid = tokenResult.GetGuid(1);

                    var otherHashed = AuthUser.HashPassword(guid.ToString(), challenge);
                    if (otherHashed == key)
                    {
                        finalGuid = guid;
                        finalId = id;
                        break;
                    }
                }

                tokenResult.Close();

                if (finalGuid != Guid.Empty)
                {
                    using var userCmd = new NpgsqlCommand("SELECT role, badges FROM users WHERE discord_id = @id", db);
                    userCmd.Parameters.AddWithValue("id", finalId);

                    using var userResult = userCmd.ExecuteReader();
                    if (userResult.Read())
                    {
                        var role = userResult.GetString(0);
                        var badges = userResult.GetFieldValue<int[]>(1);
                        userResult.Close();

                        var parsedRole = UserKinds.Player;

                        switch (role)
                        {
                            case "Player":
                                parsedRole = UserKinds.Player;
                                break;

                            case "Mod":
                                parsedRole = UserKinds.Mod;
                                break;

                            case "Admin":
                                parsedRole = UserKinds.Admin;
                                break;
                        }

                        if (parsedRole == UserKinds.Mod || parsedRole == UserKinds.Admin)
                        {
                            var badgeList = badges.ToList();
                            badgeList.Add(0);
                            badges = badgeList.ToArray();
                        }

                        return new AuthUser(parsedRole, null, badges, finalId);
                    }
                }
            }
            return base.GetUser(key, challenge);
        }
    }
}
