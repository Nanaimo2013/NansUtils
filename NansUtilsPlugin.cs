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
using System.Reflection;

namespace NansUtils
{
    public class NansUtilsPlugin : RocketPlugin
    {
        public static NansUtilsPlugin Instance { get; private set; }
        public static string CurrentVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        protected override void Load()
        {
            Instance = this;
            Rocket.Core.Logging.Logger.Log($"NansUtils v{CurrentVersion} has been loaded!");
            Provider.onEnemyConnected += OnPlayerConnected;
            PlayerLife.onPlayerDied += OnPlayerDied;
        }

        protected override void Unload()
        {
            Instance = null;
            Rocket.Core.Logging.Logger.Log("NansUtils has been unloaded!");
            Provider.onEnemyConnected -= OnPlayerConnected;
            PlayerLife.onPlayerDied -= OnPlayerDied;
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

        private void OnPlayerDied(PlayerLife sender, EDeathCause cause, ELimb limb, CSteamID instigator)
        {
            var unturnedPlayer = UnturnedPlayer.FromPlayer(sender.player);
            if (unturnedPlayer != null)
            {
                Commands.BackCommand.SaveDeathLocation(unturnedPlayer);
            }
        }

        public async void CheckForUpdates(UnturnedPlayer player)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("NansUtils");
                    string latestVersion = await client.GetStringAsync("https://api.github.com/repos/Nanaimo2013/NansUtils/releases/latest");
                    var json = Newtonsoft.Json.Linq.JObject.Parse(latestVersion);
                    string latest = json["tag_name"].ToString().TrimStart('v');

                    System.Version currentVer = Assembly.GetExecutingAssembly().GetName().Version;
                    System.Version latestVer = System.Version.Parse(latest);

                    if (latestVer > currentVer)
                    {
                        ChatUtils.SendMessage(player, $"A new version of NansUtils is available: v{latest}. Current version: v{CurrentVersion}.", Color.yellow);
                        ChatUtils.SendMessage(player, "Use /update to download and install the update.", Color.yellow);
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
            string message = $"Thank you {player.CharacterName} for using {pluginName} v{CurrentVersion} made by {authorName}!";
            ChatUtils.SendMessage(player, message, Color.green);
        }
    }
} 
