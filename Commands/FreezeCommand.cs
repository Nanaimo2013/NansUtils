using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using NansUtils.Utils;

namespace NansUtils.Commands
{
    public class FreezeCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "freeze";
        public string Help => "Freezes or unfreezes a player.";
        public string Syntax => "/freeze <player>";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "nansutils.freeze" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length != 1)
            {
                ChatUtils.SendMessage((UnturnedPlayer)caller, "Usage: /freeze <player>", UnityEngine.Color.red);
                return;
            }

            UnturnedPlayer target = UnturnedPlayer.FromName(command[0]);
            if (target == null)
            {
                ChatUtils.SendMessage((UnturnedPlayer)caller, "Player not found.", UnityEngine.Color.red);
                return;
            }

            target.Player.movement.sendPluginSpeedMultiplier(target.Player.movement.pluginSpeedMultiplier == 0 ? 1 : 0);
            ChatUtils.SendMessage((UnturnedPlayer)caller, $"{target.CharacterName} has been {(target.Player.movement.pluginSpeedMultiplier == 0 ? "frozen" : "unfrozen")}.", UnityEngine.Color.cyan);
        }
    }
} 