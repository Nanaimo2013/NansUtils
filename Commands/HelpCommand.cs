using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using NansUtils.Utils;
using UnityEngine;
using System.Linq;

namespace NansUtils.Commands
{
    public class HelpCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "nuhelp";
        public string Help => "Provides help information for NansUtils commands.";
        public string Syntax => "/nuhelp [page]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "nansutils.help" };

        private readonly List<CommandHelp> _commands = new List<CommandHelp>
        {
            new CommandHelp("/update", "Check for and apply plugin updates", "nansutils.update"),
            new CommandHelp("/maxskills", "Max out all your skills", "nansutils.maxskills"),
            new CommandHelp("/greet", "Get a friendly greeting", "nansutils.greet"),
            new CommandHelp("/emote <action>", "Perform an emote action", "nansutils.emote"),
            new CommandHelp("/teleport (tp)", "Various teleport commands. Use /tp for help", "nansutils.teleport"),
            new CommandHelp("/cleari [range]", "Clear items from ground. Optional range 1-100", "nansutils.clear.items"),
            new CommandHelp("/clearv [range]", "Clear vehicles from map. Optional range 1-100", "nansutils.clear.vehicles"),
            new CommandHelp("/troll <player> <effect>", "Apply various troll effects to players", "nansutils.troll"),
            new CommandHelp("/freeze <player>", "Freeze or unfreeze a player", "nansutils.freeze"),
            new CommandHelp("/repair", "Repair the vehicle you are in", "nansutils.vehicle"),
            new CommandHelp("/refuel", "Refuel the vehicle you are in", "nansutils.vehicle"),
            new CommandHelp("/vboost <multiplier> <duration>", "Boost vehicle speed temporarily", "nansutils.vehicle"),
            new CommandHelp("/jump", "Teleport to where you are looking", "nansutils.jump"),
            new CommandHelp("/back", "Return to your death location", "nansutils.back"),
            new CommandHelp("/kit help", "Show kit command help", "nansutils.kit"),
            new CommandHelp("/kit list", "List available kits", "nansutils.kit"),
            new CommandHelp("/kit <name>", "Use a kit", "nansutils.kit"),
            new CommandHelp("/kit create <name> <desc> <usage> <duration>", "Create a new kit", "nansutils.kit.create"),
            new CommandHelp("/kit <name> <player>", "Give a kit to another player", "nansutils.kit.other")
        };

        private const int COMMANDS_PER_PAGE = 4;

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (caller is UnturnedPlayer player)
            {
                int page = 1;
                if (command.Length > 0 && !int.TryParse(command[0], out page))
                {
                    page = 1;
                }

                var availableCommands = _commands
                    .Where(cmd => player.HasPermission(cmd.Permission))
                    .ToList();

                int totalPages = (availableCommands.Count + COMMANDS_PER_PAGE - 1) / COMMANDS_PER_PAGE;
                page = System.Math.Max(1, System.Math.Min(page, totalPages));

                var pageCommands = availableCommands
                    .Skip((page - 1) * COMMANDS_PER_PAGE)
                    .Take(COMMANDS_PER_PAGE)
                    .ToList();

                ChatUtils.SendMessage(player, $"=== NansUtils Commands (Page {page}/{totalPages}) ===", Color.cyan);
                foreach (var cmd in pageCommands)
                {
                    ChatUtils.SendMessage(player, $"{cmd.Command} - {cmd.Description}", Color.cyan);
                }
                ChatUtils.SendMessage(player, $"Use /nuhelp [1-{totalPages}] to view other pages", Color.cyan);
            }
        }

        private class CommandHelp
        {
            public string Command { get; }
            public string Description { get; }
            public string Permission { get; }

            public CommandHelp(string command, string description, string permission)
            {
                Command = command;
                Description = description;
                Permission = permission;
            }
        }
    }
} 