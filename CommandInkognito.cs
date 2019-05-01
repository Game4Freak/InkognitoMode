using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Core;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Unturned.Player;
using UnityEngine;

namespace Game4Freak.Inkognito
{
    public class CommandInkognito : IRocketCommand
    {
        public string Name
        {
            get { return "inkognito"; }
        }
        public string Help
        {
            get { return "inkognito mode"; }
        }

        public AllowedCaller AllowedCaller
        {
            get
            {
                return AllowedCaller.Player;
            }
        }

        public string Syntax
        {
            get { return "<on|off|refresh> <all|playername>"; }
        }

        public List<string> Aliases
        {
            get { return new List<string> { "ikg" }; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "inkognito" };
            }
        }

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (command.Length == 0)
            {
                UnturnedChat.Say(caller, "Invalid! Try /inkognito " + Syntax, Color.red);
                return;
            }
            if (command[0].ToLower() == "off")
            {
                if (!player.HasPermission(Permissions[0] + ".off"))
                {
                    UnturnedChat.Say(caller, "You dont have permissions to do that!", Color.red);
                    return;
                }
                Inkognito.Instance.turnOff();
                UnturnedChat.Say(caller, "Inkognito deactivated!", Color.cyan);
                return;
            }
            else if (command[0].ToLower() == "on")
            {
                if (!player.HasPermission(Permissions[0] + ".on"))
                {
                    UnturnedChat.Say(caller, "You dont have permissions to do that!", Color.red);
                    return;
                }
                Inkognito.Instance.turnOn();
                UnturnedChat.Say(caller, "Inkognito activated!", Color.cyan);
                return;
            }
            else if (command[0].ToLower() == "refresh")
            {
                if (!Inkognito.Instance.isActive)
                {
                    UnturnedChat.Say(caller, "Inkognito is not active!", Color.red);
                    return;
                }
                if (command.Length == 1)
                {
                    if (player.HasPermission(Permissions[0] + ".refresh") || player.HasPermission(Permissions[0] + ".refresh.self"))
                    {
                        Inkognito.Instance.refreshName(player.SteamPlayer());
                        UnturnedChat.Say(caller, "Refreshed your name", Color.cyan);
                        return;
                    }
                    else
                    {
                        UnturnedChat.Say(caller, "You dont have permissions to do that!", Color.red);
                        return;
                    }
                }
                else
                {
                    if (player.HasPermission(Permissions[0] + ".refresh") || player.HasPermission(Permissions[0] + ".refresh.other"))
                    {
                        if (command[1].ToLower() == "all")
                        {
                            foreach (var sPlayer in Provider.clients)
                            {
                                Inkognito.Instance.refreshName(sPlayer);
                            }
                            UnturnedChat.Say(caller, "Refreshed all names", Color.cyan);
                            return;
                        }
                        else
                        {
                            UnturnedPlayer target = UnturnedPlayer.FromName(command[1]);
                            if (target != null)
                            {
                                Inkognito.Instance.refreshName(target.SteamPlayer());
                                UnturnedChat.Say(caller, "Refreshed the name of " + target.SteamName, Color.cyan);
                            }
                            else
                            {
                                UnturnedChat.Say(caller, "Could not find the player: " + command[1], Color.red);
                                return;
                            }
                        }
                    }
                    else
                    {
                        UnturnedChat.Say(caller, "You dont have permissions to do that!", Color.red);
                        return;
                    }
                }
            }
        }
    }
}
