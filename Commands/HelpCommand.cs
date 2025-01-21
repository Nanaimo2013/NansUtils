using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using NansUtils.Utils;

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
                ChatUtils.SendMessage(player, "NansUtils Help: Available commands are /update, /maxskills, /greet, /emote, /teleport.", UnityEngine.Color.cyan);
            }
        }
    }
} 