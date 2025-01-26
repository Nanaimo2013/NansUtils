using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using NansUtils.Utils;
using UnityEngine;
using System;
using NansUtils.Models;
using NansUtils.Services;
using System.Linq;

namespace NansUtils.Commands
{
    public class KitCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "kit";
        public string Help => "Create and use kits. Use /kit help for more information.";
        public string Syntax => "/kit <name|create|help|list> [options]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { 
            "nansutils.kit",
            "nansutils.kit.create",
            "nansutils.kit.admin",
            "nansutils.kit.other"
        };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length == 0)
            {
                ShowHelp(player);
                return;
            }

            switch (command[0].ToLower())
            {
                case "help":
                    ShowHelp(player);
                    break;

                case "list":
                    ShowKitList(player);
                    break;

                case "create":
                    if (!player.HasPermission("nansutils.kit.create"))
                    {
                        ChatUtils.SendMessage(player, "You don't have permission to create kits.", Color.red);
                        return;
                    }
                    HandleKitCreation(player, command.Skip(1).ToArray());
                    break;

                default:
                    if (command.Length == 1)
                    {
                        // Using a kit on self
                        UseKit(player, command[0], player);
                    }
                    else if (command.Length == 2 && player.HasPermission("nansutils.kit.other"))
                    {
                        // Using a kit on another player
                        var targetPlayer = UnturnedPlayer.FromName(command[1]);
                        if (targetPlayer == null)
                        {
                            ChatUtils.SendMessage(player, "Player not found.", Color.red);
                            return;
                        }
                        UseKit(player, command[0], targetPlayer);
                    }
                    else
                    {
                        ShowHelp(player);
                    }
                    break;
            }
        }

        private void HandleKitCreation(UnturnedPlayer player, string[] args)
        {
            if (args.Length < 4)
            {
                ChatUtils.SendMessage(player, "Usage: /kit create <name> <description> <usage> <duration>", Color.red);
                ChatUtils.SendMessage(player, "Usage types: once, infinite, daily, weekly, monthly", Color.red);
                ChatUtils.SendMessage(player, "Duration format: 1s, 1m, 1h, 1d (seconds, minutes, hours, days)", Color.red);
                return;
            }

            string name = args[0];
            string description = args[1];
            string usageStr = args[2].ToLower();
            string durationStr = args[3];

            // Parse usage
            if (!Enum.TryParse(usageStr, true, out KitUsage usage))
            {
                ChatUtils.SendMessage(player, "Invalid usage type. Use: once, infinite, daily, weekly, monthly", Color.red);
                return;
            }

            // Parse duration
            TimeSpan duration;
            if (!TryParseTimeSpan(durationStr, out duration))
            {
                ChatUtils.SendMessage(player, "Invalid duration format. Use: 1s, 1m, 1h, 1d (seconds, minutes, hours, days)", Color.red);
                return;
            }

            if (KitService.CreateKit(name, description, player.CharacterName, usage, duration, player))
            {
                ChatUtils.SendMessage(player, $"Kit '{name}' created successfully!", Color.green);
            }
            else
            {
                ChatUtils.SendMessage(player, $"A kit with the name '{name}' already exists.", Color.red);
            }
        }

        private void UseKit(UnturnedPlayer caller, string kitName, UnturnedPlayer target)
        {
            var kit = KitService.GetKit(kitName);
            if (kit == null)
            {
                ChatUtils.SendMessage(caller, $"Kit '{kitName}' not found.", Color.red);
                return;
            }

            if (!KitService.CanUseKit(kit, target.CSteamID.ToString()))
            {
                var cooldown = KitService.GetRemainingCooldown(kit, target.CSteamID.ToString());
                if (cooldown.HasValue)
                {
                    string timeLeft = FormatTimeSpan(cooldown.Value);
                    ChatUtils.SendMessage(caller, $"This kit is on cooldown. Time remaining: {timeLeft}", Color.red);
                }
                else
                {
                    ChatUtils.SendMessage(caller, "This kit can only be used once.", Color.red);
                }
                return;
            }

            if (KitService.GiveKit(kit, target))
            {
                if (caller.CSteamID != target.CSteamID)
                {
                    ChatUtils.SendMessage(caller, $"Given kit '{kitName}' to {target.CharacterName}.", Color.green);
                    ChatUtils.SendMessage(target, $"You received kit '{kitName}' from {caller.CharacterName}.", Color.green);
                }
                else
                {
                    ChatUtils.SendMessage(caller, $"You received kit '{kitName}'.", Color.green);
                }
            }
            else
            {
                ChatUtils.SendMessage(caller, "Failed to give kit. Check logs for details.", Color.red);
            }
        }

        private void ShowKitList(UnturnedPlayer player)
        {
            var kits = KitService.GetAllKits().ToList();
            if (!kits.Any())
            {
                ChatUtils.SendMessage(player, "No kits available.", Color.yellow);
                return;
            }

            ChatUtils.SendMessage(player, "=== Available Kits ===", Color.cyan);
            foreach (var kit in kits)
            {
                var cooldown = KitService.GetRemainingCooldown(kit, player.CSteamID.ToString());
                string status = cooldown.HasValue ? $"[Cooldown: {FormatTimeSpan(cooldown.Value)}]" : "[Available]";
                ChatUtils.SendMessage(player, $"{kit.Name} - {kit.Description} {status}", Color.cyan);
            }
        }

        private void ShowHelp(UnturnedPlayer player)
        {
            ChatUtils.SendMessage(player, "=== Kit Command Help ===", Color.cyan);
            ChatUtils.SendMessage(player, "/kit <name> - Use a kit", Color.cyan);
            ChatUtils.SendMessage(player, "/kit list - Show available kits", Color.cyan);
            if (player.HasPermission("nansutils.kit.create"))
            {
                ChatUtils.SendMessage(player, "/kit create <name> <description> <usage> <duration> - Create a new kit", Color.cyan);
                ChatUtils.SendMessage(player, "Usage types: once, infinite, daily, weekly, monthly", Color.cyan);
                ChatUtils.SendMessage(player, "Duration format: 1s, 1m, 1h, 1d (seconds, minutes, hours, days)", Color.cyan);
            }
            if (player.HasPermission("nansutils.kit.other"))
            {
                ChatUtils.SendMessage(player, "/kit <name> <player> - Give a kit to another player", Color.cyan);
            }
        }

        private bool TryParseTimeSpan(string input, out TimeSpan result)
        {
            result = TimeSpan.Zero;
            if (string.IsNullOrEmpty(input)) return false;

            try
            {
                string value = input.Substring(0, input.Length - 1);
                char unit = input[input.Length - 1];
                int number;

                if (!int.TryParse(value, out number))
                    return false;

                switch (char.ToLower(unit))
                {
                    case 's':
                        result = TimeSpan.FromSeconds(number);
                        return true;
                    case 'm':
                        result = TimeSpan.FromMinutes(number);
                        return true;
                    case 'h':
                        result = TimeSpan.FromHours(number);
                        return true;
                    case 'd':
                        result = TimeSpan.FromDays(number);
                        return true;
                    default:
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays >= 1)
                return $"{timeSpan.TotalDays:0.#} days";
            if (timeSpan.TotalHours >= 1)
                return $"{timeSpan.TotalHours:0.#} hours";
            if (timeSpan.TotalMinutes >= 1)
                return $"{timeSpan.TotalMinutes:0.#} minutes";
            return $"{timeSpan.TotalSeconds:0.#} seconds";
        }
    }
} 