// ============================================================
// Init, LAN test, file paths
// ============================================================

// A small hack. Change the display name so the game doesn't show up as "Custom" in the list.
$GameModeDisplayName = "CityRPG 4";

if($server::lan)
{
  schedule(1,0,messageAll,'', "Sorry, CityRPG currently does not support LAN or singleplayer. Please run CityRPG using an Internet server instead.");
  error("Sorry, CityRPG currently does not support LAN or singleplayer. Please run CityRPG using an Internet server instead.");
  return;
}

$City::ScriptPath = "Add-Ons/GameMode_CityRPG4/server/";
$City::DataPath = "Add-Ons/GameMode_CityRPG4/data/";
$City::SavePath = "config/server/CityRPG4_A2/";

$City::Version = "0.2.0";
$City::VersionTitle = "Alpha 2";
$City::isGitBuild = !isFile("Add-Ons/GameMode_CityRPG4/README.md");

$City::VersionWarning = "!!!!! WARNING: You are using save data from a different version of CityRPG. You are likely to encounter compatibility issues. To fix this, move or delete the save file located in your Blockland folder:" SPC $City::SavePath;
// ============================================================
// Required Add-on loading
// =============================================================

// Weapon_Guns_Akimbo (The Guns Akimbo forces the Gun to load on its own.
// Therefore, we can load the Guns Akimbo only.)
%error = ForceRequiredAddOn("Weapon_Guns_Akimbo");
if(%error == $Error::AddOn_NotFound)
{
  error("ERROR: GameMode_CityRPG4 - required add-on Weapon_Guns_Akimbo not found");
  return;
}

// Weapon_Shotgun
%error = ForceRequiredAddOn("Weapon_Shotgun");
if(%error == $Error::AddOn_NotFound)
{
  error("ERROR: GameMode_CityRPG4 - required add-on Weapon_Shotgun not found");
  return;
}

// Weapon_Rocket_Launcher
%error = ForceRequiredAddOn("Weapon_Rocket_Launcher");
if(%error == $Error::AddOn_NotFound)
{
   error("ERROR: GameMode_CityRPG4 - required add-on Weapon_Rocket_Launcher not found");
   return;
}

// Weapon_Sniper_Rifle
%error = ForceRequiredAddOn("Weapon_Sniper_Rifle");
if(%error == $Error::AddOn_NotFound)
{
  error("ERROR: GameMode_CityRPG4 - required add-on Weapon_Sniper_Rifle not found");
  return;
}

// Weapon_Sword
%error = ForceRequiredAddOn("Weapon_Sword");
if(%error == $Error::AddOn_NotFound)
{
  error("ERROR: GameMode_CityRPG4 - required add-on Weapon_Sword not found");
  return;
}

// Projectile_Radio_Wave
%error = ForceRequiredAddOn("Projectile_Radio_Wave");
if(%error == $Error::AddOn_NotFound)
{
  error("ERROR: GameMode_CityRPG4 - required add-on Projectile_Radio_Wave not found");
  return;
}

// Player_DifferentSlotPlayers
%error = ForceRequiredAddOn("Player_DifferentSlotPlayers");
if(%error == $Error::AddOn_NotFound)
{
  error("ERROR: GameMode_CityRPG4 - required add-on Player_DifferentSlotPlayers not found");
  return;
}

// Item_Skis
%error = ForceRequiredAddOn("Item_Skis");
if(%error == $Error::AddOn_NotFound)
{
  error("ERROR: GameMode_CityRPG4 - required add-on Item_Skis not found");
  return;
}

if(%error == $Error::AddOn_Disabled)
{
  // Skis are "disabled", remove them from the item list.
  SkiItem.uiName = "";
}

// Tool_ChangeOwnership
%error = ForceRequiredAddOn("Tool_ChangeOwnership");
if(%error == $Error::AddOn_NotFound)
{
  error("ERROR: GameMode_CityRPG4 - required add-on Tool_ChangeOwnership not found");
  return;
}

// Event_DayNightCycle
%error = ForceRequiredAddOn("Event_DayNightCycle");
if(%error == $Error::AddOn_NotFound)
{
  error("ERROR: GameMode_CityRPG4 - required add-on Event_DayNightCycle not found");
  return;
}

if($GameModeArg $= "Add-Ons/GameMode_CityRPG4/gamemode.txt")
{
  // Optionals to always load on a vanilla configuration if they exist:

  // Tool_NewDuplicator (Optional)
  if(isFile("Add-Ons/Tool_NewDuplicator/server.cs"))
  {
    exec("Add-Ons/Tool_NewDuplicator/server.cs");
  }

  // Server_NewBrickTool (Optional)
  if(isFile("Add-Ons/Server_NewBrickTool/server.cs"))
  {
    exec("Add-Ons/Server_NewBrickTool/server.cs");
  }
}
else
{
  // Optionals that only need to load in a Custom configuration for compatibility

  // Brick_Checkpoint (Optional)
  // If enabled, we would like checkpoints to execute first.
  if($AddOn__Brick_Checkpoint)
  {
    ForceRequiredAddOn("Brick_Checkpoint");

    deactivatepackage(CheckpointPackage);
    // We don't want the checkpoint package loading.
    // The necessary functions will be rewritten later to fix spawn compatibility.
  }

  // Event_doPlayerTeleport (Optional)
  // If doPlayerTeleport is enabled, re-register it without the "relative" option.
  // This prevents players from exploiting doPlayerTeleport to move through walls.

  if($AddOn__Event_doPlayerTeleport)
  {
    ForceRequiredAddOn("Event_doPlayerTeleport");

    unregisterOutputEvent("fxDTSBrick","doPlayerTeleport");
    registerOutputEvent("fxDTSBrick","doPlayerTeleport","string 200 90\tlist Relative 0 North 1 East 2 South 3 West 4\tbool",1);
  }
}



// ============================================================
// File Execution
// ============================================================

// Core Files
exec($City::ScriptPath @ "prefs.cs");
exec($City::ScriptPath @ "bricks.cs");
exec($City::ScriptPath @ "events.cs");
exec($City::ScriptPath @ "scriptobject.cs");
exec($City::ScriptPath @ "init.cs");
exec($City::ScriptPath @ "core.cs");
exec($City::ScriptPath @ "player.cs");
exec($City::ScriptPath @ "commands.cs");
exec($City::ScriptPath @ "admin.cs");
exec($City::ScriptPath @ "package.cs");
exec($City::ScriptPath @ "overrides.cs");

exec($City::ScriptPath @ "saving.cs");

// Tools
exec($City::ScriptPath @ "items/tools/pickaxe.cs");
exec($City::ScriptPath @ "items/tools/axe.cs");
exec($City::ScriptPath @ "items/tools/knife.cs");

// Weapons
exec($City::ScriptPath @ "items/weapons/taser.cs");
exec($City::ScriptPath @ "items/weapons/baton.cs");
exec($City::ScriptPath @ "items/weapons/limitedbaton.cs");

// Modules
exec($City::ScriptPath @ "cityModules/lotRegistry.cs");
exec($City::ScriptPath @ "cityModules/cash.cs");
exec($City::ScriptPath @ "cityModules/voteImpeach.cs");
exec($City::ScriptPath @ "cityModules/rep.cs");
//exec($City::ScriptPath @ "cityModules/trade.cs");
exec($City::ScriptPath @ "cityModules/mayor.cs");
exec($City::ScriptPath @ "cityModules/security.cs");

exec($City::ScriptPath @ "support/spacecasts.cs");
exec($City::ScriptPath @ "support/extraResources.cs");
exec($City::ScriptPath @ "support/formatNumber.cs");

// Global saving
exec($City::ScriptPath @ "globalSaving/mayorSaving.cs");

// ============================================================
// Restricted Events
// ============================================================
// Remove events that can be abused
echo("*** De-registering events for CityRPG... ***");
unRegisterOutputEvent("fxDTSBrick", "RadiusImpulse");
unRegisterOutputEvent("fxDTSBrick", "SetItem");
unRegisterOutputEvent("fxDTSBrick", "SetItemDirection");
unRegisterOutputEvent("fxDTSBrick", "SetItemPosition");
unRegisterOutputEvent("fxDTSBrick", "SetVehicle");
unRegisterOutputEvent("fxDTSBrick", "SpawnItem");

unRegisterOutputEvent("Player", "AddHealth");
unRegisterOutputEvent("Player", "AddVelocity");
unRegisterOutputEvent("Player", "BurnPlayer");
unRegisterOutputEvent("Player", "ChangeDatablock");
unRegisterOutputEvent("Player", "ClearBurn");
unRegisterOutputEvent("Player", "ClearTools");
unRegisterOutputEvent("Player", "InstantRespawn");
unRegisterOutputEvent("Player", "Kill");
unRegisterOutputEvent("Player", "SetHealth");
unRegisterOutputEvent("Player", "SetPlayerScale");
unRegisterOutputEvent("Player", "SetVelocity");
unRegisterOutputEvent("Player", "SpawnExplosion");
unRegisterOutputEvent("Player", "SpawnProjectile");

unRegisterOutputEvent("GameConnection", "ChatMessage");
unRegisterOutputEvent("GameConnection", "IncScore");

unRegisterOutputEvent("MiniGame", "BottomPrintAll");
unRegisterOutputEvent("MiniGame", "CenterPrintAll");
unRegisterOutputEvent("MiniGame", "ChatMsgAll");
unRegisterOutputEvent("MiniGame", "Reset");
unRegisterOutputEvent("MiniGame", "RespawnAll");

// ============================================================
// Additional Requirements
// ============================================================
addExtraResource($City::DataPath @ "ui/cash.png");
addExtraResource($City::DataPath @ "ui/health.png");
addExtraResource($City::DataPath @ "ui/location.png");
addExtraResource($City::DataPath @ "ui/time.png");

// Support_CityRPG_Plus (Optional)
// This needs to load *after* CityRPG for it to be compatible.
if($GameModeArg $= "Add-Ons/GameMode_CityRPG4/gamemode.txt")
{
  if(isFile("Add-Ons/Support_CityRPG_Plus/server.cs"))
  {
  exec("Add-Ons/Support_CityRPG_Plus/server.cs");
  }
}

$City::Loaded = 1;
