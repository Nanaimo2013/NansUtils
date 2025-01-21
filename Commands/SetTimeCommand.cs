using Rocket.API;
using SDG.Unturned;
using System.Collections.Generic;
using NansUtils.Utils;

namespace NansUtils.Commands
{
    public class SetTimeCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "settime";
        public string Help => "Sets the time of day.";
        public string Syntax => "/settime <hour>";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "nansutils.settime" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length != 1 || !ushort.TryParse(command[0], out ushort hour) || hour > 23)
            {
                ChatUtils.SendMessage((UnturnedPlayer)caller, "Usage: /settime <hour> (0-23)", UnityEngine.Color.red);
                return;
            }

            LightingManager.time = hour * 3600; // Convert hours to seconds
            ChatUtils.SendMessage((UnturnedPlayer)caller, $"Time set to {hour}:00", UnityEngine.Color.green);
        }
    }
} 