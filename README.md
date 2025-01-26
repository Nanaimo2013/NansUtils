# NansUtils Plugin for Unturned

A comprehensive utility plugin for Unturned servers that adds various quality of life commands and features.

## Features

- Player Commands (Teleport, Emotes, Skills)
- Vehicle Management (Repair, Refuel, Speed Boost)
- Kit System (Create and Share Loadouts)
- Death Management (Return to Death Location)
- Admin Tools (Clear Items, Clear Vehicles, Freeze Players)
- Auto-Update System

## Installation

1. Download the latest release from the [Releases](https://github.com/Nanaimo2013/NansUtils/releases) page
2. Place `NansUtils.dll` in your server's `Rocket/Plugins` folder
3. Start/Restart your server
4. Configure permissions in your `Permissions.config.xml` file

## Commands

### General Commands
- `/nuhelp [page]` - View available commands (Permission: `nansutils.help`)
- `/greet` - Get a friendly greeting (Permission: `nansutils.greet`)
- `/emote <action>` - Perform an emote action (Permission: `nansutils.emote`)
- `/maxskills` - Max out all your skills (Permission: `nansutils.maxskills`)

### Teleport Commands
- `/teleport` or `/tp` - Various teleport commands (Permission: `nansutils.teleport`)
- `/jump` - Teleport to where you are looking (Permission: `nansutils.jump`)
- `/back` - Return to your death location (Permission: `nansutils.back`)
  - `/back help` - Show back command help
  - 30-second cooldown (Bypass: `nansutils.back.bypass`)

### Vehicle Commands
- `/repair` - Repair the vehicle you are in (Permission: `nansutils.vehicle`)
- `/refuel` - Refuel the vehicle you are in (Permission: `nansutils.vehicle`)
- `/vboost <multiplier> <duration>` - Temporarily boost vehicle speed (Permission: `nansutils.vehicle`)
  - Example: `/vboost 1.5 30` - 50% speed boost for 30 seconds

### Kit System
- `/kit help` - Show kit command help (Permission: `nansutils.kit`)
- `/kit list` - List available kits
- `/kit <name>` - Use a kit
- `/kit create <name> <description> <usage> <duration>` - Create a new kit (Permission: `nansutils.kit.create`)
  - Usage types: once, infinite, daily, weekly, monthly
  - Duration format: 1s, 1m, 1h, 1d (seconds, minutes, hours, days)
- `/kit <name> <player>` - Give a kit to another player (Permission: `nansutils.kit.other`)

### Admin Commands
- `/cleari [range]` - Clear items from ground (Permission: `nansutils.clear.items`)
  - Optional range: 1-100 meters
- `/clearv [range]` - Clear vehicles from map (Permission: `nansutils.clear.vehicles`)
  - Optional range: 1-100 meters
- `/freeze <player>` - Freeze or unfreeze a player (Permission: `nansutils.freeze`)
- `/troll <player> <effect>` - Apply various troll effects (Permission: `nansutils.troll`)
- `/update` - Check for and apply plugin updates (Permission: `nansutils.update`)

## Permissions

```xml
<Group>
  <!-- Basic Permissions -->
  <Permission>nansutils.help</Permission>
  <Permission>nansutils.greet</Permission>
  <Permission>nansutils.emote</Permission>
  <Permission>nansutils.maxskills</Permission>
  <Permission>nansutils.jump</Permission>
  <Permission>nansutils.back</Permission>
  
  <!-- Vehicle Permissions -->
  <Permission>nansutils.vehicle</Permission>
  
  <!-- Kit Permissions -->
  <Permission>nansutils.kit</Permission>
  <Permission>nansutils.kit.create</Permission>
  <Permission>nansutils.kit.other</Permission>
  
  <!-- Admin Permissions -->
  <Permission>nansutils.clear.items</Permission>
  <Permission>nansutils.clear.vehicles</Permission>
  <Permission>nansutils.freeze</Permission>
  <Permission>nansutils.troll</Permission>
  <Permission>nansutils.update</Permission>
  <Permission>nansutils.back.bypass</Permission>
</Group>
```

## Features in Detail

### Kit System
- Save and share loadouts including inventory and clothing
- Multiple usage types (once, infinite, daily, weekly, monthly)
- Customizable cooldowns
- Permission-based access
- Admin ability to give kits to other players

### Back Command
- Automatically saves death locations
- 30-second cooldown (configurable)
- Bypass permission for instant use
- Clear feedback messages
- One-time use per death

### Vehicle Management
- Instant vehicle repairs
- Automatic refueling
- Temporary speed boosts with custom multipliers
- Duration-based effects

### Admin Tools
- Range-based item and vehicle clearing
- Player freezing capability
- Troll effects for moderation
- Automatic update checking for admins

## Support

For support, bug reports, or feature requests:
1. Open an issue on our [GitHub repository](https://github.com/Nanaimo2013/NansUtils/issues)
2. Contact the developer: Nanaimo_2013

## Version History

- v1.0.5 - Added back command, improved help system, kit system, jump command
- v1.0.4 - Added Vehicle Commands
- v1.0.3 - Added Clear Items and Clear Vehicles commands
- v1.0.2 - Added Teleport Commands
- v1.0.1 - Initial release

## Credits

- Developer: Nanaimo_2013
- Special thanks to the Unturned modding community

## License

This project is licensed under the MIT License - see the LICENSE file for details. 