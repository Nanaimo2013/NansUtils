using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using NansUtils.Utils;
using System.Net.Http;
using System.Threading.Tasks;

namespace NansUtils
{
    public class NansUtilsPlugin : RocketPlugin
    {
        private const string CurrentVersion = "1.0.0"; // Update this with each release
        private const string VersionUrl = "https://raw.githubusercontent.com/Nanaimo2013/NansUtils/main/version.txt";

        protected override void Load()
        {
            Logger.Log("NansUtils has been loaded!");
            U.Events.OnPlayerConnected += OnPlayerConnected;
        }

        protected override void Unload()
        {
            Logger.Log("NansUtils has been unloaded!");
            U.Events.OnPlayerConnected -= OnPlayerConnected;
        }

        private void OnPlayerConnected(UnturnedPlayer player)
        {
            SendWelcomeNotification(player);
            if (player.IsAdmin)
            {
                CheckForUpdates(player);
            }
        }

        private async void CheckForUpdates(UnturnedPlayer player)
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
                Logger.LogError($"Failed to check for updates: {ex.Message}");
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