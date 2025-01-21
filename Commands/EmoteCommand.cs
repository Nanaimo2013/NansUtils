using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using NansUtils.Utils;

namespace NansUtils.Commands
{
    public class EmoteCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "emote";
        public string Help => "Performs an emote.";
        public string Syntax => "/emote <action>";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "nansutils.emote" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                ChatUtils.SendMessage((UnturnedPlayer)caller, "Usage: /emote <action>", UnityEngine.Color.red);
                return;
            }

            string action = string.Join(" ", command);
            ChatUtils.SendMessage((UnturnedPlayer)caller, $"{caller.DisplayName} {action}", UnityEngine.Color.magenta);
        }
    }
} 