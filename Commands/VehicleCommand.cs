using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using NansUtils.Utils;
using UnityEngine;
using SDG.Unturned;

namespace NansUtils.Commands
{
    public class RepairCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "repair";
        public string Help => "Repairs the vehicle you are currently in.";
        public string Syntax => "/repair";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "nansutils.repair" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            
            if (!player.Player.movement.getVehicle())
            {
                ChatUtils.SendMessage(player, "You must be in a vehicle to use this command.", Color.red);
                return;
            }

            InteractableVehicle vehicle = player.Player.movement.getVehicle();
            vehicle.health = vehicle.asset.health;
            VehicleManager.sendVehicleHealth(vehicle, vehicle.health);
            
            ChatUtils.SendMessage(player, "Vehicle has been repaired.", Color.green);
        }
    }

    public class RefuelCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "refuel";
        public string Help => "Refuels the vehicle you are currently in.";
        public string Syntax => "/refuel";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "nansutils.refuel" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            
            if (!player.Player.movement.getVehicle())
            {
                ChatUtils.SendMessage(player, "You must be in a vehicle to use this command.", Color.red);
                return;
            }

            InteractableVehicle vehicle = player.Player.movement.getVehicle();
            vehicle.fuel = vehicle.asset.fuel;
            VehicleManager.sendVehicleFuel(vehicle, vehicle.fuel);
            
            ChatUtils.SendMessage(player, "Vehicle has been refueled.", Color.green);
        }
    }
} 