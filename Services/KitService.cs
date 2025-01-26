using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NansUtils.Models;
using SDG.Unturned;
using Rocket.Unturned.Player;

namespace NansUtils.Services
{
    public class KitService
    {
        private static readonly string KitsFile = Path.Combine("Plugins", "NansUtils", "kits.json");
        private static Dictionary<string, Kit> _kits = new Dictionary<string, Kit>(StringComparer.OrdinalIgnoreCase);

        public static void Initialize()
        {
            LoadKits();
        }

        public static void LoadKits()
        {
            try
            {
                if (File.Exists(KitsFile))
                {
                    string json = File.ReadAllText(KitsFile);
                    _kits = JsonConvert.DeserializeObject<Dictionary<string, Kit>>(json) ?? new Dictionary<string, Kit>(StringComparer.OrdinalIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                Rocket.Core.Logging.Logger.LogError($"Failed to load kits: {ex.Message}");
                _kits = new Dictionary<string, Kit>(StringComparer.OrdinalIgnoreCase);
            }
        }

        public static void SaveKits()
        {
            try
            {
                string directory = Path.GetDirectoryName(KitsFile);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                string json = JsonConvert.SerializeObject(_kits, Formatting.Indented);
                File.WriteAllText(KitsFile, json);
            }
            catch (Exception ex)
            {
                Rocket.Core.Logging.Logger.LogError($"Failed to save kits: {ex.Message}");
            }
        }

        public static bool CreateKit(string name, string description, string creator, KitUsage usage, TimeSpan cooldown, UnturnedPlayer player)
        {
            if (_kits.ContainsKey(name))
                return false;

            var kit = new Kit
            {
                Name = name,
                Description = description,
                Creator = creator,
                Usage = usage,
                Cooldown = cooldown
            };

            // Save inventory items
            var inventory = player.Player.inventory;
            for (byte page = 0; page < PlayerInventory.PAGES; page++)
            {
                var items = inventory.items[page];
                if (items == null) continue;

                foreach (var item in items.items)
                {
                    if (item == null) continue;
                    kit.Items.Add(new KitItem
                    {
                        ItemId = item.item.id,
                        Amount = item.item.amount,
                        Quality = item.item.quality,
                        Page = page,
                        X = item.x,
                        Y = item.y,
                        Rotation = item.rot
                    });
                }
            }

            // Save clothing
            var clothing = player.Player.clothing;
            if (clothing.backpack != 0)
                kit.Clothing.Add(new KitClothing { ItemId = clothing.backpack, Quality = clothing.backpackQuality, Type = ClothingType.Backpack });
            if (clothing.glasses != 0)
                kit.Clothing.Add(new KitClothing { ItemId = clothing.glasses, Quality = clothing.glassesQuality, Type = ClothingType.Glasses });
            if (clothing.hat != 0)
                kit.Clothing.Add(new KitClothing { ItemId = clothing.hat, Quality = clothing.hatQuality, Type = ClothingType.Hat });
            if (clothing.mask != 0)
                kit.Clothing.Add(new KitClothing { ItemId = clothing.mask, Quality = clothing.maskQuality, Type = ClothingType.Mask });
            if (clothing.pants != 0)
                kit.Clothing.Add(new KitClothing { ItemId = clothing.pants, Quality = clothing.pantsQuality, Type = ClothingType.Pants });
            if (clothing.shirt != 0)
                kit.Clothing.Add(new KitClothing { ItemId = clothing.shirt, Quality = clothing.shirtQuality, Type = ClothingType.Shirt });
            if (clothing.vest != 0)
                kit.Clothing.Add(new KitClothing { ItemId = clothing.vest, Quality = clothing.vestQuality, Type = ClothingType.Vest });

            _kits[name] = kit;
            SaveKits();
            return true;
        }

        public static bool CanUseKit(Kit kit, string steamId)
        {
            if (kit.Usage == KitUsage.Infinite)
                return true;

            if (!kit.LastUsed.TryGetValue(steamId, out DateTime lastUsed))
                return true;

            var now = DateTime.UtcNow;
            var timeSinceLastUse = now - lastUsed;

            switch (kit.Usage)
            {
                case KitUsage.Once:
                    return false;
                case KitUsage.Daily:
                    return timeSinceLastUse.TotalDays >= 1;
                case KitUsage.Weekly:
                    return timeSinceLastUse.TotalDays >= 7;
                case KitUsage.Monthly:
                    return timeSinceLastUse.TotalDays >= 30;
                default:
                    return timeSinceLastUse >= kit.Cooldown;
            }
        }

        public static TimeSpan? GetRemainingCooldown(Kit kit, string steamId)
        {
            if (kit.Usage == KitUsage.Infinite)
                return null;

            if (!kit.LastUsed.TryGetValue(steamId, out DateTime lastUsed))
                return null;

            var now = DateTime.UtcNow;
            TimeSpan cooldown;

            switch (kit.Usage)
            {
                case KitUsage.Once:
                    return TimeSpan.MaxValue;
                case KitUsage.Daily:
                    cooldown = TimeSpan.FromDays(1);
                    break;
                case KitUsage.Weekly:
                    cooldown = TimeSpan.FromDays(7);
                    break;
                case KitUsage.Monthly:
                    cooldown = TimeSpan.FromDays(30);
                    break;
                default:
                    cooldown = kit.Cooldown;
                    break;
            }

            var timeUntilAvailable = (lastUsed + cooldown) - now;
            return timeUntilAvailable > TimeSpan.Zero ? timeUntilAvailable : (TimeSpan?)null;
        }

        public static bool GiveKit(Kit kit, UnturnedPlayer player)
        {
            try
            {
                // Clear inventory first
                for (byte page = 0; page < PlayerInventory.PAGES; page++)
                {
                    var items = player.Player.inventory.items[page];
                    if (items != null)
                    {
                        items.clear();
                    }
                }

                // Give items
                foreach (var item in kit.Items)
                {
                    var itemJar = new Item(item.ItemId, true);
                    itemJar.amount = item.Amount;
                    itemJar.quality = item.Quality;
                    player.Player.inventory.items[item.Page].tryAddItem(itemJar, true);
                }

                // Give clothing
                foreach (var clothing in kit.Clothing)
                {
                    byte[] quality = new byte[] { clothing.Quality };
                    switch (clothing.Type)
                    {
                        case ClothingType.Hat:
                            player.Player.clothing.askWearHat(clothing.ItemId, 0, quality, true);
                            break;
                        case ClothingType.Mask:
                            player.Player.clothing.askWearMask(clothing.ItemId, 0, quality, true);
                            break;
                        case ClothingType.Glasses:
                            player.Player.clothing.askWearGlasses(clothing.ItemId, 0, quality, true);
                            break;
                        case ClothingType.Shirt:
                            player.Player.clothing.askWearShirt(clothing.ItemId, 0, quality, true);
                            break;
                        case ClothingType.Pants:
                            player.Player.clothing.askWearPants(clothing.ItemId, 0, quality, true);
                            break;
                        case ClothingType.Backpack:
                            player.Player.clothing.askWearBackpack(clothing.ItemId, 0, quality, true);
                            break;
                        case ClothingType.Vest:
                            player.Player.clothing.askWearVest(clothing.ItemId, 0, quality, true);
                            break;
                    }
                }

                // Update last used time
                kit.LastUsed[player.CSteamID.ToString()] = DateTime.UtcNow;
                SaveKits();

                return true;
            }
            catch (Exception ex)
            {
                Rocket.Core.Logging.Logger.LogError($"Failed to give kit: {ex.Message}");
                return false;
            }
        }

        public static Kit GetKit(string name)
        {
            return _kits.TryGetValue(name, out Kit kit) ? kit : null;
        }

        public static IEnumerable<Kit> GetAllKits()
        {
            return _kits.Values;
        }

        public static bool DeleteKit(string name)
        {
            if (_kits.Remove(name))
            {
                SaveKits();
                return true;
            }
            return false;
        }
    }
} 