using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using NansUtils.Utils;
using UnityEngine;
using SDG.Unturned;

namespace NansUtils.Commands
{
    public class JumpCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "jump";
        public string Help => "Teleports you to where you are looking.";
        public string Syntax => "/jump";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "nansutils.jump" };

        private const float MAX_DISTANCE = 100f;

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            // Get player's look direction and perform raycast
            RaycastInfo ray = DamageTool.raycast(new Ray(player.Player.look.aim.position, player.Player.look.aim.forward), MAX_DISTANCE, RayMasks.GROUND);

            if (ray.point != Vector3.zero)
            {
                // Add a small offset to prevent getting stuck in the ground
                Vector3 teleportPosition = ray.point + new Vector3(0, 0.5f, 0);
                player.Teleport(teleportPosition, player.Rotation);
                ChatUtils.SendMessage(player, "Jumped to target location!", Color.green);
            }
            else
            {
                ChatUtils.SendMessage(player, "No valid location to jump to. Make sure you're looking at the ground.", Color.red);
            }
        }
    }
} 