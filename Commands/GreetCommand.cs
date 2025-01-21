using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using NansUtils.Utils;

namespace NansUtils.Commands
{
    public class GreetCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "greet";
        public string Help => "Greets the player.";
        public string Syntax => "/greet";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "nansutils.greet" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            ChatUtils.SendMessage(player, $"Hello, {player.CharacterName}!", UnityEngine.Color.yellow);
        }
    }
} 