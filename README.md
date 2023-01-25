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

## Preferences

### Game
* **City name** - The name of the city that will be shown in various in-game messages.
* **Drop Cash on Death** - Whether players drop all of their cash when they die.
* **Disable Default Weapons** - Restart required. Disables default weapons from being sold in shops. Recommended if you are using a custom weapons pack.
* **Max Lots** - The max number of lots that a player can own.
* **Account Reset Cost (/reset)** - Cost to reset a player's account. This is to deter players from resetting their account repeatedly as a way of cheesing the game.
* **Tick Length (minutes)** - *Use caution*. How much time passes between ticks. Paychecks and other gameplay elements will *not* be balanced for different lengths of time. The day/night cycle will also not sync unless you do so manually.
* **Lot Rules** - A short text input for what your server's building rules are. Players can read this in the menu when viewing a lot. This can be a brief description or an instruction on how to read more i.e. "See the rules board at spawn for building rules."
* **Min Bounty** - Minimum amount players can place on other players as a bounty. Players can only access this if you or an admin places a bounty brick in the city.
* **Max Bounty** - Maximum amount players can place on other players as a bounty.  Players can only access this if you or an admin places a bounty brick in the city.
* **Disable tumble on starve** - Disable the tumble effect for starving players.

### Styling
* Text Color - Primary
* Clock on HUD (Experimental)

### Server Management
* Logging Enabled
* Always show hidden chat to admins

### Economy
* Economy Relay
* Max Economy Percentage
* Economy Cap
* Lot Cost - For each lot

### Mayor
* Election Active
* Mayor Run Cost
* Election Time (minutes)
* Mayor Removal Cost

# Optional Integrations
CityRPG 4 has optional support for the following add-ons. In order to use them, enable both GameMode_CityRPG4 and the desired add-on under Custom in the game-modes menu.

* [Mail System](https://blocklandglass.com/addons/addon.php?id=670) - Players can buy and sell notes, letters, and cards in stores. Selling these items does not require a licensed job like the Arms Dealer.
* Tier+Tactical Weapons: [Tier 1](https://blocklandglass.com/addons/addon.php?id=1206), [Tier 1A](https://blocklandglass.com/addons/addon.php?id=1207), [Tier 2](https://blocklandglass.com/addons/addon.php?id=1209), [Tier 2A](https://blocklandglass.com/addons/addon.php?id=1210)  - Most items can be sold in stores as weapons, including some of the easter eggs if they are enabled. Ammo is currently not supported, so it is recommended that you disable it in prefs.

## Abuse Prevention
* [doPlayerTeleport Event](https://forum.blockland.us/index.php?topic=253312.0) - The "Relative" checkbox is disabled to prevent abuse (i.e. teleporting into unwanted areas of players' builds)
* [Zone Events](https://blocklandglass.com/addons/rtb/view.php?id=119) - Events prone to abuse are disabled.

# Developer Documentation
CityRPG 4 is being built fully open source, and we encourage you to fork the repository and make your own changes. We would love to see your contributions!

Below is an incomplete documentation of key functions in CityRPG 4.

## Quick Start Guides

### General dev tips - Running from a folder

You can develop the game-mode and/or extensions for it straight out of your add-ons folder. Simply extract the desired add-on for development into a folder, and *delete* the .zip. You should be left with a folder named after the add-on, i.e. `GameMode_CityRPG4`. The game will see it as an add-on just like it would a normal .zip.

### General dev tips - Reloading the game-mode
For developing and testing features, you can make changes and then re-load the game-mode through the "Game-Mode >>" button in the admin menu. This allows you to restart only the server, and not your whole game.

This does not work if you are adding completely new files.

### General dev tips - Hot-reloading
You can reload a script to change things without closing the game at all by executing the desired script. A couple ways you can do this:
- Run `exec(...);` in your console with the path to the file. For example: `exec("add-ons/gamemode_cityrpg4/")
- An easier alternative: Use [Jincux's Development Tools](https://blocklandglass.com/addons/addon.php?id=252) add-on. See ["devtools.cs"](https://github.com/cityrpg/CityRPG/blob/master/devtools.cs) in the root folder of this repository. It covers most of CityRPG's important scripts, and you can add your own in-game.

Note - If the script contains a package, you may encounter unexplained issues due to the package load order changing. If this happens, you may still need to reload from the game-mode menu or restart the game.

### How updates are handled
Semantic versioning is used here. A major version increase (i.e. 1.x.x -> 2.x.x) means something important has changed that is likely to break some mods. Key changes are outlined under "Developer info" in the changelog for each update.

### How to add jobs
There is an example add-on in the CityRPG_Jobs_Template folder. This is an add-on that you can drop into your game, rename, and edit as you like.

From scratch, the short version: Package the JobSO::loadJobFiles function. You can overwrite it or simply add onto it (`Parent::JobSO::loadJobFiles(%this)`). Add JobSO::createJob(%file) calls leading to your job files. Your job files shold follow the same format as jobs/ or CityRPG_Jobs_Template/

See also: [JobSO and Custom Jobs](#JobSO-and-Custom-Jobs)

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
Called when `client` is arrested by `cop`.

## Misc

### City_illegalAttackTest(atkr, victim)
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
