using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using NansUtils.Utils;

namespace NansUtils.Commands
{
    public class TeleportCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "teleport";
        public string Help => "Teleports a player to a location.";
        public string Syntax => "/teleport <player> <x> <y> <z>";
        public List<string> Aliases => new List<string> { "tp" };
        public List<string> Permissions => new List<string> { "nansutils.teleport" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length != 4 || !float.TryParse(command[1], out float x) || !float.TryParse(command[2], out float y) || !float.TryParse(command[3], out float z))
            {
                ChatUtils.SendMessage((UnturnedPlayer)caller, "Usage: /teleport <player> <x> <y> <z>", UnityEngine.Color.red);
                return;
            }

            UnturnedPlayer target = UnturnedPlayer.FromName(command[0]);
            if (target == null)
            {
                ChatUtils.SendMessage((UnturnedPlayer)caller, "Player not found.", UnityEngine.Color.red);
                return;
            }

            target.Teleport(new UnityEngine.Vector3(x, y, z), target.Rotation);
            ChatUtils.SendMessage((UnturnedPlayer)caller, $"{target.CharacterName} has been teleported.", UnityEngine.Color.green);
        }
    }
} 