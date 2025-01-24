using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using NansUtils.Utils;
using UnityEngine;
using SDG.Unturned;
using System.Linq;
using SDG.Framework.Devkit;

namespace NansUtils.Commands
{
    public class TeleportCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "teleport";
        public string Help => "Teleports players. Use: /tp <player|coords|location>, /tp <player> <player>, /tphere <player|*>";
        public string Syntax => "/tp [player|coords|location], /tphere <player|*>";
        public List<string> Aliases => new List<string> { "tp" };
        public List<string> Permissions => new List<string> 
        { 
            "nansutils.teleport",
            "nansutils.teleport.coords",
            "nansutils.teleport.location",
            "nansutils.teleport.here",
            "nansutils.teleport.all"
        };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (!(caller is UnturnedPlayer player))
            {
                ChatUtils.SendMessage(caller as UnturnedPlayer, "This command can only be used by players.", Color.red);
                return;
            }

            if (!player.HasPermission("nansutils.teleport"))
            {
                ChatUtils.SendMessage(player, "You don't have permission to use teleport commands.", Color.red);
                return;
            }

            if (command.Length == 0)
            {
                ShowHelp(player);
                return;
            }

            // Handle /tphere command
            if (command[0].ToLower() == "here")
            {
                if (!player.HasPermission("nansutils.teleport.here"))
                {
                    ChatUtils.SendMessage(player, "You don't have permission to use /tp here.", Color.red);
                    return;
                }

                if (command.Length != 2)
                {
                    ChatUtils.SendMessage(player, "Usage: /tp here <player|*>", Color.red);
                    return;
                }

                if (command[1] == "*")
                {
                    if (!player.HasPermission("nansutils.teleport.all"))
                    {
                        ChatUtils.SendMessage(player, "You don't have permission to teleport all players.", Color.red);
                        return;
                    }
                    TeleportAllToPlayer(player);
                }
                else
                {
                    var targetPlayer = UnturnedPlayer.FromName(command[1]);
                    if (targetPlayer == null)
                    {
                        ChatUtils.SendMessage(player, "Player not found.", Color.red);
                        return;
                    }
                    TeleportPlayerToPlayer(targetPlayer, player);
                }
                return;
            }

            // Handle coordinate teleport
            if (command.Length == 3)
            {
                if (!player.HasPermission("nansutils.teleport.coords"))
                {
                    ChatUtils.SendMessage(player, "You don't have permission to teleport to coordinates.", Color.red);
                    return;
                }

                if (float.TryParse(command[0], out float x) && 
                    float.TryParse(command[1], out float y) && 
                    float.TryParse(command[2], out float z))
                {
                    if (y < 0)
                    {
                        ChatUtils.SendMessage(player, "Cannot teleport below ground level.", Color.red);
                        return;
                    }
                    TeleportToCoordinates(player, new Vector3(x, y, z));
                }
                else
                {
                    ChatUtils.SendMessage(player, "Invalid coordinates. Use numbers only.", Color.red);
                }
                return;
            }

            // Handle player to player teleport
            if (command.Length == 2)
            {
                UnturnedPlayer target1 = UnturnedPlayer.FromName(command[0]);
                UnturnedPlayer target2 = UnturnedPlayer.FromName(command[1]);

                if (target1 == null || target2 == null)
                {
                    ChatUtils.SendMessage(player, "One or both players not found.", Color.red);
                    return;
                }

                TeleportPlayerToPlayer(target1, target2);
                ChatUtils.SendMessage(player, $"Teleported {target1.CharacterName} to {target2.CharacterName}.", Color.green);
                return;
            }

            // Handle teleport to player or location
            if (command.Length == 1)
            {
                UnturnedPlayer target = UnturnedPlayer.FromName(command[0]);
                if (target != null)
                {
                    TeleportPlayerToPlayer(player, target);
                    return;
                }

                if (!player.HasPermission("nansutils.teleport.location"))
                {
                    ChatUtils.SendMessage(player, "You don't have permission to teleport to locations.", Color.red);
                    return;
                }

                // Try to find location in the map's locations
                var searchName = command[0].ToLower();
                var nodes = FindLocationsOnMap();
                var location = nodes.FirstOrDefault(l => 
                    l.locationName.ToLower().Contains(searchName));
                if (location != null)
                {
                    Vector3 spawnPoint = location.transform.position;
                    TeleportToCoordinates(player, spawnPoint);
                    ChatUtils.SendMessage(player, $"Teleported to location: {location.locationName}", Color.green);
                    return;
                }

                ChatUtils.SendMessage(player, "Player or location not found.", Color.red);
            }
        }

        private void ShowHelp(UnturnedPlayer player)
        {
            ChatUtils.SendMessage(player, "Teleport Commands:", Color.yellow);
            ChatUtils.SendMessage(player, "/tp <player> - Teleport to a player", Color.yellow);
            if (player.HasPermission("nansutils.teleport.coords"))
                ChatUtils.SendMessage(player, "/tp <x> <y> <z> - Teleport to coordinates", Color.yellow);
            if (player.HasPermission("nansutils.teleport.location"))
                ChatUtils.SendMessage(player, "/tp <location> - Teleport to a location", Color.yellow);
            ChatUtils.SendMessage(player, "/tp <player1> <player2> - Teleport player1 to player2", Color.yellow);
            if (player.HasPermission("nansutils.teleport.here"))
                ChatUtils.SendMessage(player, "/tp here <player> - Teleport a player to you", Color.yellow);
            if (player.HasPermission("nansutils.teleport.all"))
                ChatUtils.SendMessage(player, "/tp here * - Teleport all players to you", Color.yellow);
        }

        private void TeleportPlayerToPlayer(UnturnedPlayer player, UnturnedPlayer target)
        {
            if (player == null || target == null)
            {
                ChatUtils.SendMessage(player ?? target, "Player not found.", Color.red);
                return;
            }

            player.Teleport(target.Position, target.Rotation);
            ChatUtils.SendMessage(player, $"Teleported to {target.CharacterName}.", Color.green);
            ChatUtils.SendMessage(target, $"{player.CharacterName} has teleported to you.", Color.yellow);
        }

        private void TeleportToCoordinates(UnturnedPlayer player, Vector3 position)
        {
            player.Teleport(position, player.Rotation);
            ChatUtils.SendMessage(player, $"Teleported to {position.x}, {position.y}, {position.z}.", Color.green);
        }

        private void TeleportAllToPlayer(UnturnedPlayer target)
        {
            int count = 0;
            foreach (var player in Provider.clients.Select(c => UnturnedPlayer.FromSteamPlayer(c)))
            {
                if (player.CSteamID != target.CSteamID)
                {
                    TeleportPlayerToPlayer(player, target);
                    count++;
                }
            }
            ChatUtils.SendMessage(target, $"Teleported {count} players to you.", Color.green);
        }

        private IEnumerable<LocationDevkitNode> FindLocationsOnMap()
        {
            return LocationDevkitNodeSystem.Get().GetAllNodes();
        }
    }
} 