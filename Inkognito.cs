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
        public const string VERSION = "0.3.0.0";
        public bool isActive = true;
        public Dictionary<CSteamID, string> CharNames;
        private readonly System.Random random = new System.Random();

        protected override void Load()
        {
            Instance = this;
            Logger.Log("Inkognito v" + VERSION);
            CharNames = new Dictionary<CSteamID, string>();
            foreach (var sPlayer in Provider.clients)
            {
                CharNames.Add(UnturnedPlayer.FromSteamPlayer(sPlayer).CSteamID, sPlayer.playerID.characterName);
                if (isActive)
                    refreshName(sPlayer);
            }
            U.Events.OnPlayerConnected += onConnect;
            U.Events.OnPlayerDisconnected += onDisconnect;
            ChatManager.onChatted += onChat;
        }

        private void onChat(SteamPlayer player, EChatMode mode, ref Color chatted, ref bool isRich, string text, ref bool isVisible)
        {
            if (mode != EChatMode.GLOBAL)
                return;
            if (text.StartsWith("/"))
                return;
            if (isActive)
            {
                if (!Configuration.Instance.InkognitoInGlobalChat)
                {
                    isVisible = false;
                    UnturnedChat.Say(CharNames[UnturnedPlayer.FromSteamPlayer(player).CSteamID] + ": " + text, chatted, isRich);
                    return;
                }
                else
                {

                    if (UnturnedPlayer.FromSteamPlayer(player).HasPermission("inkognito.writeoriginalname"))
                    {
                        isVisible = false;
                        UnturnedChat.Say(CharNames[UnturnedPlayer.FromSteamPlayer(player).CSteamID] + ": " + text, chatted, isRich);
                        return;
                    }
                    bool viewEach = false;
                    foreach (var sPlayer in Provider.clients)
                    {
                        if (UnturnedPlayer.FromSteamPlayer(sPlayer).HasPermission("inkognito.vieworiginalname"))
                        {
                            viewEach = true;
                            break;
                        }
                    }
                    if (viewEach)
                    {
                        foreach (var sPlayer in Provider.clients)
                        {
                            isVisible = false;
                            if (UnturnedPlayer.FromSteamPlayer(sPlayer).HasPermission("inkognito.vieworiginalname"))
                            {
                                UnturnedChat.Say(UnturnedPlayer.FromSteamPlayer(sPlayer), CharNames[UnturnedPlayer.FromSteamPlayer(player).CSteamID] + ": " + text, chatted, isRich);
                            }
                            else
                            {
                                UnturnedChat.Say(UnturnedPlayer.FromSteamPlayer(sPlayer), UnturnedPlayer.FromSteamPlayer(player).CharacterName + ": " + text, chatted, isRich);
                            }
                        }
                    }
                }
            }
        }

        protected override void Unload()
        {
            foreach (var sPlayer in Provider.clients)
            {
                sPlayer.playerID.characterName = CharNames[UnturnedPlayer.FromSteamPlayer(sPlayer).CSteamID];
            }
            CharNames.Clear();
            U.Events.OnPlayerConnected -= onConnect;
            U.Events.OnPlayerDisconnected -= onDisconnect;
            ChatManager.onChatted -= onChat;
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
            {
                refreshName(player.SteamPlayer());
            }
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
                refreshName(sPlayer);
            }
        }

        public void refreshName(SteamPlayer player)
        {
            if (Configuration.Instance.InkognitoUseGroupPrefixAndSuffix)
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromSteamPlayer(player);
                string currentPrefix = "";
                int currentPrefixPri = -1;
                string currentSuffix = "";
                int currentSuffixPri = -1;
                foreach (var group in R.Permissions.GetGroups(uPlayer, false))
                {
                    if (group.Prefix != "" && group.Priority > currentPrefixPri)
                    {
                        currentPrefix = group.Prefix;
                        currentPrefixPri = group.Priority;
                    }
                    if (group.Suffix != "" && group.Priority > currentSuffixPri)
                    {
                        currentSuffix = group.Suffix;
                        currentSuffixPri = group.Priority;
                    }
                }
                player.playerID.characterName = currentPrefix + Configuration.Instance.InkognitoNames[randomNum(0, Configuration.Instance.InkognitoNames.Count)] + currentSuffix;
            }
            else
            {
                player.playerID.characterName = Configuration.Instance.InkognitoNames[randomNum(0, Configuration.Instance.InkognitoNames.Count)];
            }
        }

        public int randomNum(int min, int max)
        {
            return random.Next(min, max);
        }
    }
}