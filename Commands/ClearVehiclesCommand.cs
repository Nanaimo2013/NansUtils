using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using NansUtils.Utils;
using UnityEngine;
using SDG.Unturned;
using System.Linq;

namespace NansUtils.Commands
{
    public class ClearVehiclesCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "clearv";
        public string Help => "Clears vehicles. Use /clearv for all vehicles or /clearv <range> for vehicles within range.";
        public string Syntax => "/clearv [range]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "nansutils.clearvehicles" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (!(caller is UnturnedPlayer player))
            {
                ChatUtils.SendMessage(null, "This command can only be used by players.", Color.red);
                return;
            }

            float range = -1; // -1 means clear all vehicles
            if (command.Length > 0)
            {
                if (!float.TryParse(command[0], out range))
                {
                    ChatUtils.SendMessage(player, "Invalid range. Please use a number.", Color.red);
                    return;
                }

                if (range <= 0 || range > 100)
                {
                    ChatUtils.SendMessage(player, "Range must be between 1 and 100.", Color.red);
                    return;
                }
            }

            int vehiclesCleared = ClearVehicles(player, range);
            
            if (range == -1)
            {
                ChatUtils.SendMessage(player, $"Cleared {vehiclesCleared} vehicles from the map.", Color.green);
            }
            else
            {
                ChatUtils.SendMessage(player, $"Cleared {vehiclesCleared} vehicles within {range} meters.", Color.green);
            }
        }

        private int ClearVehicles(UnturnedPlayer player, float range)
        {
            int vehiclesCleared = 0;
            var vehicles = VehicleManager.vehicles.ToList(); // Create a copy to avoid modification issues

            foreach (var vehicle in vehicles)
            {
                // Skip if vehicle is null or already destroyed
                if (vehicle == null) continue;

                // If range is -1 or vehicle is within range
                if (range == -1 || Vector3.Distance(vehicle.transform.position, player.Position) <= range)
                {
                    // Check if anyone is in the vehicle
                    if (vehicle.passengers.Any(p => p.player != null))
                    {
                        continue; // Skip vehicles with passengers
                    }

                    VehicleManager.askVehicleDestroy(vehicle);
                    vehiclesCleared++;
                }
            }

            return vehiclesCleared;
        }
    }
} 