using Rocket.API;
using Rocket.Unturned.Chat;
using System.Collections.Generic;
using NansUtils.Utils;

namespace NansUtils.Commands
{
    public class BroadcastCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "broadcast";
        public string Help => "Broadcasts a message to all players.";
        public string Syntax => "/broadcast <message>";
        public List<string> Aliases => new List<string> { "bc" };
        public List<string> Permissions => new List<string> { "nansutils.broadcast" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                ChatUtils.SendMessage((UnturnedPlayer)caller, "Usage: /broadcast <message>", UnityEngine.Color.red);
                return;
            }

            string message = string.Join(" ", command);
            UnturnedChat.Say(message, UnityEngine.Color.blue);
        }
    }
} 