using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using NansUtils.Utils;

namespace NansUtils.Commands
{
    public class UpdateCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "update";
        public string Help => "Checks for updates to the NansUtils plugin.";
        public string Syntax => "/update";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "nansutils.update" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            NansUtilsPlugin plugin = (NansUtilsPlugin)Rocket.Core.R.Plugins.GetPlugin("NansUtils");
            plugin.CheckForUpdates(player);
        }
    }
} 