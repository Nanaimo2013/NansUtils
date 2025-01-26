using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using NansUtils.Utils;
using UnityEngine;
using SDG.Unturned;
using System.Collections;
using System;

namespace NansUtils.Commands
{
    public class VehicleBoostCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "vboost";
        public string Help => "Boosts vehicle speed temporarily. Usage: /vboost <multiplier> <duration>";
        public string Syntax => "/vboost <multiplier> <duration>";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "nansutils.vboost" };

        private static Dictionary<InteractableVehicle, Coroutine> activeBoosts = new Dictionary<InteractableVehicle, Coroutine>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            
            if (!player.Player.movement.getVehicle())
            {
                ChatUtils.SendMessage(player, "You must be in a vehicle to use this command.", Color.red);
                return;
            }

            if (command.Length != 2)
            {
                ChatUtils.SendMessage(player, "Usage: /vboost <multiplier> <duration>", Color.red);
                return;
            }

            if (!float.TryParse(command[0], out float multiplier))
            {
                ChatUtils.SendMessage(player, "Invalid multiplier. Please use a number (e.g. 1.5, 2.0).", Color.red);
                return;
            }

            if (!float.TryParse(command[1], out float duration))
            {
                ChatUtils.SendMessage(player, "Invalid duration. Please use a number in seconds.", Color.red);
                return;
            }

            // Limit the multiplier and duration for balance
            multiplier = Mathf.Clamp(multiplier, 1f, 5f);
            duration = Mathf.Clamp(duration, 1f, 300f); // Max 5 minutes

            InteractableVehicle vehicle = player.Player.movement.getVehicle();

            // Cancel existing boost if there is one
            if (activeBoosts.ContainsKey(vehicle))
            {
                player.Player.movement.sendPluginSpeedMultiplier(1f);
                activeBoosts.Remove(vehicle);
            }

            // Apply the boost
            player.Player.movement.sendPluginSpeedMultiplier(multiplier);

            // Start coroutine to reset after duration
            activeBoosts[vehicle] = NansUtilsPlugin.Instance.StartCoroutine(ResetBoostAfterDelay(vehicle, player, duration));

            ChatUtils.SendMessage(player, $"Vehicle boosted to {multiplier}x speed for {duration} seconds!", Color.green);
        }

        private static IEnumerator ResetBoostAfterDelay(InteractableVehicle vehicle, UnturnedPlayer player, float duration)
        {
            yield return new WaitForSeconds(duration);

            if (vehicle != null && !vehicle.isDead)
            {
                player.Player.movement.sendPluginSpeedMultiplier(1f);

                // Notify any players in the vehicle
                foreach (var passenger in vehicle.passengers)
                {
                    if (passenger?.player != null)
                    {
                        var steamPlayer = passenger.player;
                        var unturnedPlayer = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                        ChatUtils.SendMessage(unturnedPlayer, "Vehicle boost has expired.", Color.yellow);
                    }
                }
            }

            if (activeBoosts.ContainsKey(vehicle))
            {
                activeBoosts.Remove(vehicle);
            }
        }
    }
} 