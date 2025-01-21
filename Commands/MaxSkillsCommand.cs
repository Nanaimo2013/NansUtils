using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using NansUtils.Utils;

namespace NansUtils.Commands
{
    public class MaxSkillsCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "maxskills";
        public string Help => "Maxes out the player's skills.";
        public string Syntax => "/maxskills";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "nansutils.maxskills" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            player.GiveSkill(255); // Max out all skills
            ChatUtils.SendMessage(player, "Your skills have been maxed out!", UnityEngine.Color.green);
        }
    }
} 