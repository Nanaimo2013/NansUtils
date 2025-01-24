using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using NansUtils.Utils;
using UnityEngine;
using SDG.Unturned;
using System;

namespace NansUtils.Commands
{
    public class TrollCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "troll";
        public string Help => "Trolls a player with various effects.";
        public string Syntax => "/troll <player> <effect>";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "nansutils.troll" };

        private readonly Dictionary<string, Action<UnturnedPlayer>> trollEffects;

        public TrollCommand()
        {
            trollEffects = new Dictionary<string, Action<UnturnedPlayer>>(StringComparer.OrdinalIgnoreCase)
            {
                { "spin", player => {
                    player.Player.movement.sendPluginSpeedMultiplier(5f);
                    player.Player.movement.sendPluginGravityMultiplier(0.5f);
                }},
                { "jump", player => {
                    player.Player.movement.sendPluginJumpMultiplier(3f);
                    player.Player.movement.sendPluginGravityMultiplier(0.7f);
                }},
                { "slow", player => {
                    player.Player.movement.sendPluginSpeedMultiplier(0.3f);
                    player.Player.movement.sendPluginJumpMultiplier(0.5f);
                }},
                { "moon", player => {
                    player.Player.movement.sendPluginGravityMultiplier(0.2f);
                    player.Player.movement.sendPluginJumpMultiplier(2f);
                }},
                { "heavy", player => {
                    player.Player.movement.sendPluginGravityMultiplier(3f);
                    player.Player.movement.sendPluginJumpMultiplier(0.3f);
                }},
                { "dizzy", player => {
                    EffectManager.triggerEffect(new TriggerEffectParameters(Guid.Parse("6ff3bdbe89634bb49b3aa8ebdc71d42f"))
                    {
                        position = player.Position,
                        relevantDistance = 30f
                    });
                    player.Player.movement.sendPluginSpeedMultiplier(2f);
                }},
                { "dance", player => {
                    player.Player.animator.sendGesture((EPlayerGesture)2, true);
                    player.Player.movement.sendPluginSpeedMultiplier(1.5f);
                }},
                { "salute", player => {
                    player.Player.animator.sendGesture((EPlayerGesture)4, true);
                }},
                { "point", player => {
                    player.Player.animator.sendGesture((EPlayerGesture)3, true);
                }},
                { "wave", player => {
                    player.Player.animator.sendGesture((EPlayerGesture)5, true);
                }},
                { "facepalm", player => {
                    player.Player.animator.sendGesture((EPlayerGesture)6, true);
                }},
                { "surrender", player => {
                    player.Player.animator.sendGesture((EPlayerGesture)7, true);
                }},
                { "reset", player => {
                    player.Player.movement.sendPluginSpeedMultiplier(1f);
                    player.Player.movement.sendPluginJumpMultiplier(1f);
                    player.Player.movement.sendPluginGravityMultiplier(1f);
                    player.Player.animator.sendGesture((EPlayerGesture)0, true);
                }},
                { "crawl", player => {
                    player.Player.animator.sendGesture((EPlayerGesture)8, true);
                    player.Player.movement.sendPluginSpeedMultiplier(0.5f);
                }},
                { "sit", player => {
                    player.Player.animator.sendGesture((EPlayerGesture)9, true);
                }},
                { "backflip", player => {
                    player.Player.animator.sendGesture((EPlayerGesture)10, true);
                    player.Player.movement.sendPluginJumpMultiplier(2f);
                }},
                { "laugh", player => {
                    player.Player.animator.sendGesture((EPlayerGesture)11, true);
                }},
                { "crazy", player => {
                    player.Player.movement.sendPluginSpeedMultiplier(3f);
                    player.Player.movement.sendPluginGravityMultiplier(0.3f);
                    player.Player.animator.sendGesture((EPlayerGesture)2, true);
                    EffectManager.triggerEffect(new TriggerEffectParameters(Guid.Parse("6ff3bdbe89634bb49b3aa8ebdc71d42f"))
                    {
                        position = player.Position,
                        relevantDistance = 30f
                    });
                }}
            };
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (!(caller is UnturnedPlayer player))
            {
                ChatUtils.SendMessage(null, "This command can only be used by players.", Color.red);
                return;
            }

            if (command.Length != 2)
            {
                ShowHelp(player);
                return;
            }

            UnturnedPlayer target = UnturnedPlayer.FromName(command[0]);
            if (target == null)
            {
                ChatUtils.SendMessage(player, "Player not found.", Color.red);
                return;
            }

            string effect = command[1].ToLower();
            if (trollEffects.TryGetValue(effect, out Action<UnturnedPlayer> trollAction))
            {
                trollAction(target);
                ChatUtils.SendMessage(player, $"Applied {effect} effect to {target.CharacterName}!", Color.green);
                ChatUtils.SendMessage(target, $"You've been trolled with {effect} effect!", Color.yellow);
            }
            else
            {
                ShowHelp(player);
            }
        }

        private void ShowHelp(UnturnedPlayer player)
        {
            ChatUtils.SendMessage(player, "Troll Effects:", Color.yellow);
            ChatUtils.SendMessage(player, "/troll <player> spin - Makes player spin fast", Color.yellow);
            ChatUtils.SendMessage(player, "/troll <player> jump - Makes player jump high", Color.yellow);
            ChatUtils.SendMessage(player, "/troll <player> slow - Makes player move slowly", Color.yellow);
            ChatUtils.SendMessage(player, "/troll <player> moon - Low gravity mode", Color.yellow);
            ChatUtils.SendMessage(player, "/troll <player> heavy - High gravity mode", Color.yellow);
            ChatUtils.SendMessage(player, "/troll <player> dizzy - Makes player dizzy", Color.yellow);
            ChatUtils.SendMessage(player, "/troll <player> dance - Makes player dance", Color.yellow);
            ChatUtils.SendMessage(player, "/troll <player> salute - Makes player salute", Color.yellow);
            ChatUtils.SendMessage(player, "/troll <player> point - Makes player point", Color.yellow);
            ChatUtils.SendMessage(player, "/troll <player> wave - Makes player wave", Color.yellow);
            ChatUtils.SendMessage(player, "/troll <player> facepalm - Makes player facepalm", Color.yellow);
            ChatUtils.SendMessage(player, "/troll <player> surrender - Makes player surrender", Color.yellow);
            ChatUtils.SendMessage(player, "/troll <player> reset - Resets all effects", Color.yellow);
            ChatUtils.SendMessage(player, "/troll <player> crawl - Makes player crawl", Color.yellow);
            ChatUtils.SendMessage(player, "/troll <player> sit - Makes player sit", Color.yellow);
            ChatUtils.SendMessage(player, "/troll <player> backflip - Makes player do a backflip", Color.yellow);
            ChatUtils.SendMessage(player, "/troll <player> laugh - Makes player laugh", Color.yellow);
            ChatUtils.SendMessage(player, "/troll <player> crazy - Goes completely crazy", Color.yellow);
        }
    }
} 