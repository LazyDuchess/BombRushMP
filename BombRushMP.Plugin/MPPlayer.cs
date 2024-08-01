using BombRushMP.Common.Packets;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class MPPlayer
    {
        public ushort ClientId = 0;
        public ClientState ClientState = null;
        public ClientVisualState ClientVisualState = null;
        public Player Player;
        public bool Teleporting = true;

        public void FrameUpdate()
        {
            var clientController = ClientController.Instance;
            if (!clientController.Connected) return;
            //if (ClientId == clientController.LocalID) return;

            var worldHandler = WorldHandler.instance;
            if (ClientState == null || ClientVisualState == null)
            {
                if (Player != null)
                {
                    GameObject.Destroy(Player.gameObject);
                    Player = null;
                }
                return;
            }
            if (Player == null)
                Player = MPUtility.CreateMultiplayerPlayer();
            if (Teleporting)
            {
                Teleporting = false;
                worldHandler.PlacePlayerAt(Player, ClientVisualState.GetUnityPosition(), ClientVisualState.GetUnityRotation(), true);
            }
            else
            {
                Player.transform.position = Vector3.Lerp(Player.transform.position, ClientVisualState.GetUnityPosition(), Time.deltaTime * ClientConstants.PlayerInterpolation);
                Player.transform.rotation = Quaternion.Lerp(Player.transform.rotation, ClientVisualState.GetUnityRotation(), Time.deltaTime * ClientConstants.PlayerInterpolation);
            }
        }

        public void Delete()
        {
            var clientController = ClientController.Instance;
            var worldHandler = WorldHandler.instance;
            ClientState = null;
            ClientVisualState = null;
            if (Player != null)
            {
                clientController.PlayerRegistry.Remove(Player);
                worldHandler.SceneObjectsRegister.players.Remove(Player);
                GameObject.Destroy(Player.gameObject);
                Player = null;
            }
        }
    }
}
