using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using NansUtils.Utils;
using UnityEngine;
using SDG.Unturned;
using System.Linq;

namespace NansUtils.Commands
{
    public class ClearItemsCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "cleari";
        public string Help => "Clears items from the ground. Use /cleari for all items or /cleari <range> for items within range.";
        public string Syntax => "/cleari [range]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "nansutils.clearitems" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (!(caller is UnturnedPlayer player))
            {
                ChatUtils.SendMessage(null, "This command can only be used by players.", Color.red);
                return;
            }

            float range = -1; // -1 means clear all items
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

            int itemsCleared = ClearItems(player, range);
            
            if (range == -1)
            {
                ChatUtils.SendMessage(player, $"Cleared {itemsCleared} items from the ground.", Color.green);
            }
            else
            {
                ChatUtils.SendMessage(player, $"Cleared {itemsCleared} items within {range} meters.", Color.green);
            }
        }

        private int ClearItems(UnturnedPlayer player, float range)
        {
            int itemsCleared = 0;
            
            // Get all regions in the level
            var regions = ItemManager.regions;
            if (regions == null) return 0;

            for (byte x = 0; x < regions.GetLength(0); x++)
            {
                for (byte y = 0; y < regions.GetLength(1); y++)
                {
                    var items = regions[x, y].items;
                    if (items == null) continue;

                    // Check each item in the region
                    foreach (var item in items.ToList())
                    {
                        if (range == -1 || Vector3.Distance(item.point, player.Position) <= range)
                        {
                            ItemManager.askClearRegionItems(x, y);
                            itemsCleared++;
                        }
                    }
                }
            }

            return itemsCleared;
        }
    }
} 