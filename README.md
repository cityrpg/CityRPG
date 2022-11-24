# ![CityRPG 4](https://lakeys.net/cityrpg/logo.png)
Live, build, and thrive in a city made of blocks, inhabited by players like you.

CityRPG is a mod for the game Blockland that simulates the life of a blocky city. Each city is built, run, and populated by the players. It’s a sandbox-RPG-simulation blend with limitless potential.

In CityRPG 4, you can explore a range of jobs and opportunities as you work your way to the top! You can join the fun on an existing server, or create your own server with any theme, rules, and style that you like

[Download CityRPG 4](https://github.com/cityrpg/CityRPG-4/releases) | [Join us on Discord](https://discord.gg/dHcnHb3) | [View update progress on Trello](https://lakeys.net/cityrpg/roadmap/)
------------ | ------------- | ------------- |

![Banner](https://lakeys.net/cityrpg/media/banner.png)

## Created by the CityRPG 4 Team
Development: Lake

Directors: PunisherLex, Qauk, and Sargeras

### Special thanks
The CityRPG 4 testers: Alphabite, Dglider, Jasa, Luigi609, Mega Bear, Remurr, Snack Nsack Chocolate, Stargatefan

The previous CityRPG developers: Jookia/PeopleMods, Iban, Diggy/Wentworth, McTwist, Gadgethm, Aoki, /Ty, and many more.

Dglider for bugfixes and tips, Sentry for replacing missing assets.

# Commands

## Gameplay
- **/help** - Displays in-game help information for playing CityRPG.
- **/lot** - Opens the lot management menu for the lot you are on.
- **/reincarnate** - For players in the endgame.

## Admin Commands
- **/adminMode** - Toggle Admin Mode, enabling jets and disabling most gameplay effects for administration.
- **/updateScore** - Updates scores in the player list.
- **/setMinerals** - Sets the mineral count for the city. This affects item sales.
- **/setLumber** - Sets the total lumber for the city. This affects item sales.
- **/editEducation** [level] [player] - Changes a player's education level.
- **/gMoney** [amount] [player] - Grant money to a player.
- **/dMoney** [amount] [player] - Remove money from a player's inventory.
- **/clearMoney** - Clear all cash from your character. Does not take from your bank.
- **/dBank** [amount] [player] - Remove money from a player's bank.
- **/cleanse** - Removes your wanted level.
- **/edithunger** [level 1-10] - Changes hunger to a certain level.
- **/resetAllJobs** - Resets the job for every profile on the server. Used for changing custom job configurations.

# Optional Integrations
CityRPG 4 has optional support for the following add-ons. In order to use them, enable both GameMode_CityRPG4 and the desired add-on under Custom in the game-modes menu.

* [Mail System](https://blocklandglass.com/addons/addon.php?id=670) - Players can buy and sell notes, letters, and cards in stores. Selling these items does not require a licensed job like the Arms Dealer.
* Tier+Tactical Weapons: [Tier 1](https://blocklandglass.com/addons/addon.php?id=1206), [Tier 1A](https://blocklandglass.com/addons/addon.php?id=1207), [Tier 2](https://blocklandglass.com/addons/addon.php?id=1209), [Tier 2A](https://blocklandglass.com/addons/addon.php?id=1210)  - Most items can be sold in stores as weapons, including some of the easter eggs if they are enabled. Ammo is currently not supported.

## Abuse Prevention
* [doPlayerTeleport Event](https://forum.blockland.us/index.php?topic=253312.0) - The "Relative" checkbox is disabled to prevent abuse (i.e. teleporting into unwanted areas of players' builds)
* [Zone Events](https://blocklandglass.com/addons/rtb/view.php?id=119) - Events prone to abuse are disabled.

# Developer Documentation
CityRPG 4 is being built fully open source, and we encourage you to fork the repository and make your own changes. We would love to see your contributions!

Below is an incomplete documentation of key functions in CityRPG 4.

Note that this documentation is applicable to the unstable version. For the latest release docs, see here:
https://github.com/cityrpg/CityRPG-4/tree/0.2.0

## Client/GameConnection

### GameConnection::cityMenuOpen(names, functions, exitMsg, autoClose, canOverride)
Displays a generic menu, currently using a chat-based approach. Returns true if the menu opens successfully. No eval or script object handling is necessary.

While this currently resembles the mechanics of classic CityRPG menus (chat-based, type a number), the method of displaying menus is subject to change. For best results, make sure to check client.cityMenuOpen before attempting. Take note of the default 8-line chat limit.

**Args:**
- exitMsg (str): The message to display when the menu closes
- menu (str, fields): A set of fields containing names for each menu item.
- functions (str, fields): The function that will be called corresponding with each name option. The following are passed to this function, in order: %client (Client object), %input (User input that triggered this menu option), %id (The active ID for the current menu).
- menuID (str): A unique identifier for the menu, for reference elsewhere. Generally set to the ID of the brick that triggered it.
- autoClose (Bool): If set to 'true', the menu will close as soon as the function executes.
- canOverride (Bool): If set to 'true', the menu will close automatically if another menu is opened. If false, other menus will be blocked from opening.

**Example:**
```
%menu = "Option 1" TAB "Option 2";
%functions = "announce" TAB "talk";
findClientByName(Blockhead).cityMenuOpen(%menu, %functions, "", "", true, false);
```

Result:
Typing "1" will show the client's ID in yellow text. ("announce()")
Typing "2" will show the client's ID with a "CONSOLE: " chat message. ("talk()")

To prompt the user for text, direct client.cityMenuFunction to a new function with the arguments `client` and `input`. The user's next raw text input will be passed to the function as `input`.

Check `server/cityModules/lotRegistry.cs` and `server/brickScripts/info/jobs.cs` for complex utilizations of the cityMenuOpen function.

#### Pre-existing generic functions
This is a list of existing functions for menus. These functions should be used where possible to avoid redundancy in creating extra menu functions.

- CityMenu_Close - Closes the menu.
- CityMenu_Placeholder - Displays the text "Sorry, this feature is currently not available. Please try again later."

### GameConnection::isCityAdmin()
Returns true or false whether the target client is in Admin Mode. For most uses, it is recommended that you use this in place of checking isAdmin.

## Tools

### CityRPGBatonImage::onCityPlayerHit(obj, slot, col, pos, normal)
Allows the creation of additional checks when players are batoned. MUST return `true` if a check is passed, otherwise baton actions will overlap each other.

Shares the same arguments as the CityRPGBatonImage::onHitObject function.

### CityRPGLBImage::onCityPlayerHit(obj, slot, col, pos, normal)
See CityRPGBatonImage::onCityPlayerHit

### gameConnection::arrest(client, cop)
**Deprecated**

Called when `client` is arrested by `cop`.

## Misc

### City_illegalAttackTest(atkr, victim)
**Deprecated**

Called when players are attacked, determines if an attack is to incur demerits as an assault. Returns `false `

## JobSO and Custom Jobs

### JobSO::loadJobFiles(so)
This is the function that populates the job list with jobs. If desired, this function can be packaged and/or overridden to update the game-mode's job list. Jobs can be executed by running `JobSO.createJob`

### JobSO::createJob(so, file)
Adds a job from a script file. For a reference of what a job script file looks like, refer to the files contained within `GameMode_CityRPG4/server/jobs`.

The path will check for a script filename first in `GameMode_CityRPG4/jobs`, and if none is found there, it will check for a direct path.

This can be used to create custom jobs without directly editing the game-mode files.

#### Examples:
`JobSO.createJob("PdOfficer");` - Adds the job found in `GameMode_CityRPG4/jobs/PdOfficer.cs`

`JobSO.createJob("Add-Ons/CityRPG_Jobs_MyCustomJobs/PdOfficer.cs");` - Adds the job found in `Add-Ons/Suport_CityRPG_MyCustomJobs/PdOfficer.cs`
