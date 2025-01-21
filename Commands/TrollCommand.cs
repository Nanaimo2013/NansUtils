using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using NansUtils.Utils;

namespace NansUtils.Commands
{
    public class TrollCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "troll";
        public string Help => "Trolls a player.";
        public string Syntax => "/troll <player>";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "nansutils.troll" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length != 1)
            {
                ChatUtils.SendMessage((UnturnedPlayer)caller, "Usage: /troll <player>", UnityEngine.Color.red);
                return;
            }

            UnturnedPlayer target = UnturnedPlayer.FromName(command[0]);
            if (target == null)
            {
                ChatUtils.SendMessage((UnturnedPlayer)caller, "Player not found.", UnityEngine.Color.red);
                return;
            }

            // Example troll: flip the player's screen
            target.Player.look.sendPluginRotation(target.Player.look.yaw, target.Player.look.pitch + 180);
            ChatUtils.SendMessage((UnturnedPlayer)caller, $"{target.CharacterName} has been trolled!", UnityEngine.Color.magenta);
        }
    }
} 