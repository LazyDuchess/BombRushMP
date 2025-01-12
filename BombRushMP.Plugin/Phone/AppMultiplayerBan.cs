using BombRushMP.Common.Packets;
using CommonAPI;
using CommonAPI.Phone;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin.Phone
{
    public class AppMultiplayerBan : PlayerPickerApp
    {
        public override bool Available
        {
            get
            {
                var clientController = ClientController.Instance;
                if (clientController == null) return false;
                if (!clientController.Connected) return false;
                var user = clientController.GetLocalUser();
                if (user == null) return false;
                return user.IsModerator;
            }
        }
        public static void Initialize()
        {
            var txtFile = File.ReadAllBytes(Path.Combine(MPSettings.Instance.Directory, "acn_icon.png"));
            var texture = new Texture2D(1, 1);
            texture.LoadImage(txtFile);
            texture.wrapMode = TextureWrapMode.Clamp;
            var icon = TextureUtility.CreateSpriteFromTexture(texture);
            PhoneAPI.RegisterApp<AppMultiplayerBan>("ban players", icon);
        }

        public override void OnAppInit()
        {
            base.OnAppInit();
            CreateIconlessTitleBar("Ban");
            AllowEveryone = false;
        }

        public override void PlayerChosen(ushort[] playerIds)
        {
            base.PlayerChosen(playerIds);
            var clientController = ClientController.Instance;
            foreach (var player in playerIds)
            {
                ClientController.Instance.SendPacket(new ClientChat($"/banid {player}"), Common.Networking.IMessage.SendModes.ReliableUnordered, Common.Networking.NetChannels.Chat);
            }
        }
    }
}
