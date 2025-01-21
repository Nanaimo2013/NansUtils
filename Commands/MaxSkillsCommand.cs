using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using NansUtils.Utils;
using SDG.Unturned;

namespace NansUtils.Commands
{
    public class MaxSkillsCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "maxskills";
        public string Help => "Maxes out all skills for the player.";
        public string Syntax => "/maxskills";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "nansutils.maxskills" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (caller is UnturnedPlayer player)
            {
                foreach (var skillSet in player.Player.skills.skills)
                {
                    foreach (var skill in skillSet)
                    {
                        skill.level = skill.max; // Set each skill to its maximum level
                    }
                }
                ChatUtils.SendMessage(player, "Your skills have been maxed out!", UnityEngine.Color.green);
            }
            else
            {
                ChatUtils.SendMessage(caller as UnturnedPlayer, "This command can only be used by players.", UnityEngine.Color.red);
            }
        }
    }
}