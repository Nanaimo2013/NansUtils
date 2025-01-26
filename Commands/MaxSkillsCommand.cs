using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using NansUtils.Utils;
using SDG.Unturned;
using UnityEngine;
using System.Linq;

namespace NansUtils.Commands
{
    public class MaxSkillsCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "maxskills";
        public string Help => "Set to max level all of your/player skills. Use /maxskills [overpower] [player|*]";
        public string Syntax => "/maxskills [overpower] [player|*]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { 
            "nansutils.maxskills",
            "nansutils.maxskills.overpower",
            "nansutils.maxskills.other",
            "nansutils.maxskills.all"
        };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            bool overpower = false;
            UnturnedPlayer targetPlayer = null;

            // Default case: /maxskills
            if (command.Length == 0)
            {
                if (!(caller is UnturnedPlayer player))
                {
                    ChatUtils.SendMessage(null, "Console must specify a player. Use: maxskills <overpower> <player|*>", Color.red);
                    return;
                }
                GiveMaxSkills(player, false);
                return;
            }

            // Parse overpower parameter
            if (command.Length >= 1)
            {
                if (!bool.TryParse(command[0], out overpower))
                {
                    ChatUtils.SendMessage(caller as UnturnedPlayer, "Invalid overpower value. Use 'true' or 'false'.", Color.red);
                    return;
                }

                if (overpower && !caller.HasPermission("nansutils.maxskills.overpower"))
                {
                    ChatUtils.SendMessage(caller as UnturnedPlayer, "You don't have permission to use overpower mode.", Color.red);
                    return;
                }
            }

            // Parse target player parameter
            if (command.Length >= 2)
            {
                if (command[1] == "*")
                {
                    if (!caller.HasPermission("nansutils.maxskills.all"))
                    {
                        ChatUtils.SendMessage(caller as UnturnedPlayer, "You don't have permission to max skills for all players.", Color.red);
                        return;
                    }

                    // Max skills for all players
                    int count = 0;
                    foreach (var steamPlayer in Provider.clients)
                    {
                        var player = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                        GiveMaxSkills(player, overpower);
                        count++;
                    }
                    ChatUtils.SendMessage(caller as UnturnedPlayer, $"Maxed skills for {count} players.", Color.green);
                    return;
                }
                else
                {
                    if (!caller.HasPermission("nansutils.maxskills.other"))
                    {
                        ChatUtils.SendMessage(caller as UnturnedPlayer, "You don't have permission to max skills for other players.", Color.red);
                        return;
                    }

                    targetPlayer = UnturnedPlayer.FromName(command[1]);
                    if (targetPlayer == null)
                    {
                        ChatUtils.SendMessage(caller as UnturnedPlayer, "Player not found.", Color.red);
                        return;
                    }

                    GiveMaxSkills(targetPlayer, overpower);
                    ChatUtils.SendMessage(caller as UnturnedPlayer, $"Maxed skills for {targetPlayer.CharacterName}.", Color.green);
                    return;
                }
            }

            // If we got here with just overpower parameter, apply to self
            if (caller is UnturnedPlayer player2)
            {
                GiveMaxSkills(player2, overpower);
            }
        }

        private void GiveMaxSkills(UnturnedPlayer player, bool overpower)
        {
            var pSkills = player.Player.skills;

            foreach (var skill in pSkills.skills.SelectMany(skArr => skArr)) {
                skill.level = overpower ? byte.MaxValue : skill.max;
            }

            pSkills.askSkills(player.CSteamID);
            ChatUtils.SendMessage(player, overpower ? "Your skills have been overpowered!" : "Your skills have been maxed out!", Color.green);
        }
    }
}