using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using NansUtils.Utils;
using UnityEngine;
using System;
using System.Collections.Concurrent;

namespace NansUtils.Commands
{
    public class BackCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "back";
        public string Help => "Return to your previous death location.";
        public string Syntax => "/back";
        public List<string> Aliases => new List<string> { "return" };
        public List<string> Permissions => new List<string> { "nansutils.back", "nansutils.back.bypass" };

        private static readonly ConcurrentDictionary<string, DeathLocation> _deathLocations = new ConcurrentDictionary<string, DeathLocation>();
        private const int DEFAULT_DELAY_SECONDS = 30;

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var player = (UnturnedPlayer)caller;

            if (command.Length > 0 && command[0].ToLower() == "help")
            {
                ShowHelp(player);
                return;
            }

            if (!_deathLocations.TryGetValue(player.CSteamID.ToString(), out DeathLocation deathLoc))
            {
                ChatUtils.SendMessage(player, "You haven't died yet!", Color.red);
                return;
            }

            // Check delay if player doesn't have bypass permission
            if (!player.HasPermission("nansutils.back.bypass"))
            {
                var timeSinceDeath = DateTime.UtcNow - deathLoc.DeathTime;
                var remainingDelay = DEFAULT_DELAY_SECONDS - timeSinceDeath.TotalSeconds;

                if (remainingDelay > 0)
                {
                    ChatUtils.SendMessage(player, $"You must wait {remainingDelay:F0} seconds before using /back", Color.red);
                    return;
                }
            }

            // Teleport player back to death location
            player.Player.teleportToLocation(deathLoc.Position, player.Rotation);
            ChatUtils.SendMessage(player, "Teleported to your death location.", Color.green);

            // Remove the death location after use
            _deathLocations.TryRemove(player.CSteamID.ToString(), out _);
        }

        private void ShowHelp(UnturnedPlayer player)
        {
            ChatUtils.SendMessage(player, "=== Back Command Help ===", Color.cyan);
            ChatUtils.SendMessage(player, "/back - Return to your death location", Color.cyan);
            ChatUtils.SendMessage(player, $"There is a {DEFAULT_DELAY_SECONDS} second delay unless you have the bypass permission", Color.cyan);
        }

        public static void SaveDeathLocation(UnturnedPlayer player)
        {
            var deathLoc = new DeathLocation
            {
                Position = player.Position,
                DeathTime = DateTime.UtcNow
            };

            _deathLocations.AddOrUpdate(player.CSteamID.ToString(), deathLoc, (key, old) => deathLoc);
            ChatUtils.SendMessage(player, "Use /back to return to your death location", Color.yellow);
        }

        private class DeathLocation
        {
            public Vector3 Position { get; set; }
            public DateTime DeathTime { get; set; }
        }
    }
} 