using Steamworks;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using NansUtils.Utils;
using System.Net.Http;
using System;

namespace NansUtils
{
    public class NansUtilsPlugin : RocketPlugin
    {
        public static string CurrentVersion { get; set; } = "1.0.4"; // Update this with each release
        public const string VersionUrl = "https://raw.githubusercontent.com/Nanaimo2013/NansUtils/main/version.txt";

        protected override void Load()
        {
            Rocket.Core.Logging.Logger.Log("NansUtils has been loaded!");
            Provider.onEnemyConnected += OnPlayerConnected;
        }

        protected override void Unload()
        {
            Rocket.Core.Logging.Logger.Log("NansUtils has been unloaded!");
            Provider.onEnemyConnected -= OnPlayerConnected;
        }

        private void OnPlayerConnected(SteamPlayer steamPlayer)
        {
            UnturnedPlayer unturnedPlayer = UnturnedPlayer.FromSteamPlayer(steamPlayer);
            SendWelcomeNotification(unturnedPlayer);
            if (unturnedPlayer.IsAdmin)
            {
                CheckForUpdates(unturnedPlayer);
            }
        }

        public async void CheckForUpdates(UnturnedPlayer player)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string latestVersion = await client.GetStringAsync(VersionUrl);
                    if (latestVersion.Trim() != CurrentVersion)
                    {
                        ChatUtils.SendMessage(player, $"A new version of NansUtils is available: {latestVersion.Trim()}. Current version: {CurrentVersion}.", Color.yellow);
                    }
                }
            }
            catch (Exception ex)
            {
                Rocket.Core.Logging.Logger.LogError($"Failed to check for updates: {ex.Message}");
            }
        }

        private void SendWelcomeNotification(UnturnedPlayer player)
        {
            string pluginName = "NansUtils";
            string authorName = "Nanaimo_2013";
            string message = $"Thank you {player.CharacterName} for using {pluginName} made by {authorName}!";
            ChatUtils.SendMessage(player, message, Color.green);
        }
    }
} 
