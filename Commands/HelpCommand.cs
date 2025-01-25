using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using NansUtils.Utils;
using UnityEngine;

namespace NansUtils.Commands
{
    public class HelpCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "nuhelp";
        public string Help => "Provides help information for NansUtils commands.";
        public string Syntax => "/nuhelp";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "nansutils.help" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (caller is UnturnedPlayer player)
            {
                ChatUtils.SendMessage(player, "=== NansUtils Commands ===", Color.cyan);
                ChatUtils.SendMessage(player, "/update - Check for and apply plugin updates", Color.cyan);
                ChatUtils.SendMessage(player, "/maxskills - Max out all your skills", Color.cyan);
                ChatUtils.SendMessage(player, "/greet - Get a friendly greeting", Color.cyan);
                ChatUtils.SendMessage(player, "/emote <action> - Perform an emote action", Color.cyan);
                ChatUtils.SendMessage(player, "/teleport (tp) - Various teleport commands. Use /tp for help", Color.cyan);
                ChatUtils.SendMessage(player, "/cleari [range] - Clear items from ground. Optional range 1-100", Color.cyan);
                ChatUtils.SendMessage(player, "/clearv [range] - Clear vehicles from map. Optional range 1-100", Color.cyan);
                ChatUtils.SendMessage(player, "/troll <player> <effect> - Apply various troll effects to players", Color.cyan);
                ChatUtils.SendMessage(player, "/freeze <player> - Freeze or unfreeze a player", Color.cyan);
                ChatUtils.SendMessage(player, "/repair - Repair the vehicle you are in", Color.cyan);
                ChatUtils.SendMessage(player, "/refuel - Refuel the vehicle you are in", Color.cyan);
                ChatUtils.SendMessage(player, "/jump - Teleport to where you are looking", Color.cyan);
                ChatUtils.SendMessage(player, "Use each command to see more detailed help.", Color.cyan);
            }
        }
    }
} 