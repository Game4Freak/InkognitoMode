using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Enumerations;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Game4Freak.Inkognito
{
    public class Inkognito : RocketPlugin<InkognitoConfiguration>
    {
        public static Inkognito Instance;
        public const string VERSION = "0.1.0.0";
        public bool isActive = true;
        public Dictionary<CSteamID, string> CharNames;

        protected override void Load()
        {
            Instance = this;
            Logger.Log("Inkognito v" + VERSION);
            CharNames = new Dictionary<CSteamID, string>();
            /*foreach (var sPlayer in Provider.clients)
            {
                CharNames.Add(UnturnedPlayer.FromSteamPlayer(sPlayer).CSteamID, sPlayer.playerID.characterName);
                if (isActive)
                    onConnect(UnturnedPlayer.FromSteamPlayer(sPlayer));
            }*/

            U.Events.OnPlayerConnected += onConnect;
            U.Events.OnPlayerDisconnected += onDisconnect;
        }

        protected override void Unload()
        {
            CharNames.Clear();
            U.Events.OnPlayerConnected -= onConnect;
            U.Events.OnPlayerDisconnected -= onDisconnect;
        }

        private void onDisconnect(UnturnedPlayer player)
        {
            if (isActive && CharNames.ContainsKey(player.CSteamID))
                player.SteamPlayer().playerID.characterName = CharNames[player.CSteamID];

            CharNames.Remove(player.CSteamID);
        }

        private void onConnect(UnturnedPlayer player)
        {
            CharNames.Add(player.CSteamID, player.SteamPlayer().playerID.characterName);
            if (isActive)
                player.SteamPlayer().playerID.characterName = Configuration.Instance.InkognitoNames[randomNum(0, Configuration.Instance.InkognitoNames.Count)];
        }

        public void turnOff()
        {
            isActive = false;
            foreach (var sPlayer in Provider.clients)
            {
                sPlayer.playerID.characterName = CharNames[UnturnedPlayer.FromSteamPlayer(sPlayer).CSteamID];
            }
        }

        public void turnOn()
        {
            isActive = true;
            foreach (var sPlayer in Provider.clients)
            {
                sPlayer.playerID.characterName = Configuration.Instance.InkognitoNames[randomNum(0, Configuration.Instance.InkognitoNames.Count)];
            }
        }

        public int randomNum(int min, int max)
        {
            System.Random random = new System.Random();
            return random.Next(min, max);
        }
    }
}