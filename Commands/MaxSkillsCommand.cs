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
                MaxOutSkills(player);
                ChatUtils.SendMessage(player, "Your skills have been maxed out!", UnityEngine.Color.green);
            }
            else
            {
                ChatUtils.SendMessage(caller as UnturnedPlayer, "This command can only be used by players.", UnityEngine.Color.red);
            }
        }

        private void MaxOutSkills(UnturnedPlayer player)
        {
            PlayerSkills skills = player.Player.skills;

            for (byte specialtyIndex = 0; specialtyIndex < skills.skills.Length; specialtyIndex++)
            {
                for (byte skillIndex = 0; skillIndex < skills.skills[specialtyIndex].Length; skillIndex++)
                {
                    skills.ServerSetSkillLevel(specialtyIndex, skillIndex, skills.skills[specialtyIndex][skillIndex].max);
                }
            }

            skills.askSkills(player.CSteamID); // Refresh skills UI
        }
    }
}