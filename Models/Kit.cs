using System;
using System.Collections.Generic;
using SDG.Unturned;
using Newtonsoft.Json;

namespace NansUtils.Models
{
    public class Kit
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedAt { get; set; }
        public KitUsage Usage { get; set; }
        public TimeSpan Cooldown { get; set; }
        public List<KitItem> Items { get; set; }
        public List<KitClothing> Clothing { get; set; }
        public Dictionary<string, DateTime> LastUsed { get; set; }

        public Kit()
        {
            Items = new List<KitItem>();
            Clothing = new List<KitClothing>();
            LastUsed = new Dictionary<string, DateTime>();
            CreatedAt = DateTime.UtcNow;
        }
    }

    public class KitItem
    {
        public ushort ItemId { get; set; }
        public byte Amount { get; set; }
        public byte Quality { get; set; }
        public byte Page { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }
        public byte Rotation { get; set; }
    }

    public class KitClothing
    {
        public ushort ItemId { get; set; }
        public byte Quality { get; set; }
        public ClothingType Type { get; set; }
    }

    public enum KitUsage
    {
        Once,
        Infinite,
        Daily,
        Weekly,
        Monthly
    }

    public enum ClothingType
    {
        Hat,
        Mask,
        Glasses,
        Shirt,
        Pants,
        Backpack,
        Vest
    }
} 