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
        public string Syntax => "/jump [distance]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "nansutils.jump" };

        private const float DEFAULT_DISTANCE = 1000f;
        private const float HEIGHT_OFFSET = 6f;

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            float distance = DEFAULT_DISTANCE;

            if (command.Length > 0)
            {
                if (!float.TryParse(command[0], out distance))
                {
                    ChatUtils.SendMessage(player, "Invalid distance. Please use a number.", Color.red);
                    return;
                }
            }

            // Get player's look position
            Vector3? targetPoint = GetTargetPosition(player, distance);

            if (!targetPoint.HasValue)
            {
                ChatUtils.SendMessage(player, "No valid location to jump to. Make sure you're looking at a valid position.", Color.red);
                return;
            }

            // Add height offset to prevent getting stuck and ensure safe landing
            Vector3 teleportPosition = targetPoint.Value + new Vector3(0, HEIGHT_OFFSET, 0);
            player.Teleport(teleportPosition, player.Rotation);
            ChatUtils.SendMessage(player, $"Jumped to position: {teleportPosition.x:F1}, {teleportPosition.y:F1}, {teleportPosition.z:F1}", Color.green);
        }

        private Vector3? GetTargetPosition(UnturnedPlayer player, float maxDistance)
        {
            var camera = player.Player.look.aim;
            var ray = new Ray(camera.position, camera.forward);

            // First try to hit something directly
            RaycastInfo hit = DamageTool.raycast(ray, maxDistance, RayMasks.BLOCK_COLLISION);
            if (hit.point != Vector3.zero)
            {
                return hit.point;
            }

            // If no direct hit, try to find ground below the end point
            var endPosition = camera.position + (camera.forward * maxDistance);
            hit = DamageTool.raycast(new Ray(endPosition + new Vector3(0, 100f, 0), Vector3.down), 200f, RayMasks.GROUND);
            if (hit.point != Vector3.zero)
            {
                return hit.point;
            }

            return null;
        }
    }
} 