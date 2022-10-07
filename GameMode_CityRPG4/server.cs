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

$City::Version = "1.1.0";
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

// Weapon_Rocket_Launcher
%error = ForceRequiredAddOn("Weapon_Rocket_Launcher");
if(%error == $Error::AddOn_NotFound)
{
   error("ERROR: GameMode_CityRPG4 - required add-on Weapon_Rocket_Launcher not found");
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

%error = ForceRequiredAddOn("Player_No_Jet");
if(%error == $Error::AddOn_NotFound)
{
  error("ERROR: GameMode_CityRPG4 - required add-on Player_No_Jet not found");
  return;
}

if(%error == $Error::AddOn_Disabled)
{
  playerNoJet.uiName = "";
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

  // Event_Bot_Relay (Optional)
  if(isFile("Add-Ons/Event_Bot_Relay/server.cs"))
  {
    exec("Add-Ons/Event_Bot_Relay/server.cs");
  }

  // Weapon_Shotgun (Optional)
  if(isFile("Add-Ons/Weapon_Shotgun/server.cs"))
  {
    exec("Add-Ons/Weapon_Shotgun/server.cs");
  }

  // Weapon_Sniper_Rifle (Optional)
  if(isFile("Add-Ons/Weapon_Sniper_Rifle/server.cs"))
  {
    exec("Add-Ons/Weapon_Sniper_Rifle/server.cs");
  }

}
else
{
  // Optionals that only need to load in a Custom configuration for compatibility
  // These are only loaded if they are enabled.

  // Brick_Checkpoint (Optional)
  // If enabled, we would like checkpoints to execute first.
  if($AddOn__Brick_Checkpoint)
  {
    %error = ForceRequiredAddOn("Brick_Checkpoint");
    
    if(%error == $Error::None)
    {
      $City::CheckpointIsActive = 1;
      deactivatepackage(CheckpointPackage);
      // We don't want the checkpoint package loading.
      // The necessary functions will be rewritten later to fix spawn compatibility.
    }
  }

  // Event_doPlayerTeleport (Optional)
  // If doPlayerTeleport is enabled, re-register it without the "relative" option.
  // This prevents players from exploiting doPlayerTeleport to move through walls.
  if($AddOn__Event_doPlayerTeleport)
  {
    %error = ForceRequiredAddOn("Event_doPlayerTeleport");

    if(%error == $Error::None)
    {
      unregisterOutputEvent("fxDTSBrick","doPlayerTeleport");
      registerOutputEvent("fxDTSBrick","doPlayerTeleport","string 200 90\tlist Relative 0 North 1 East 2 South 3 West 4\tbool",1);
    }
    // See package.cs for the function arg fix
  }

  // Event_Zones (Optional)
  // If Event_Zones is enabled, disable events that may be abused.
  if($AddOn__Event_Zones)
  {
    %error = ForceRequiredAddOn("Event_Zones");

    if(%error == $Error::None)
    {
      unregisterOutputEvent("fxDTSBrick","setZone");
      unregisterOutputEvent("fxDTSBrick","setZoneVelocityMod");
    }
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
exec($City::ScriptPath @ "jobs.cs");
exec($City::ScriptPath @ "core.cs");
exec($City::ScriptPath @ "player.cs");
exec($City::ScriptPath @ "commands.cs");
exec($City::ScriptPath @ "admin.cs");

// Modules to preload
exec($City::ScriptPath @ "support/Support_CenterprintMenuSystem.cs");

// Core packages (Order-dependent)
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
exec($City::ScriptPath @ "cityModules/lotRegistryMenu.cs");
exec($City::ScriptPath @ "cityModules/cash.cs");
exec($City::ScriptPath @ "cityModules/voteImpeach.cs");
exec($City::ScriptPath @ "cityModules/mayor.cs");

exec($City::ScriptPath @ "support/spacecasts.cs");
exec($City::ScriptPath @ "support/extraResources.cs");
exec($City::ScriptPath @ "support/formatNumber.cs");
exec($City::ScriptPath @ "support/saver.cs");

// Global saving
exec($City::ScriptPath @ "globalSaving/mayorSaving.cs");

// ============================================================
// Restricted Events
// ============================================================
// Remove events that can be abused
// Non-default event restrictions are defined above in optional add-on loading.
echo("*** De-registering events for CityRPG... ***");
unRegisterOutputEvent("fxDTSBrick", "RadiusImpulse");
unRegisterOutputEvent("fxDTSBrick", "SetItem");
unRegisterOutputEvent("fxDTSBrick", "SetItemDirection");
unRegisterOutputEvent("fxDTSBrick", "SetItemPosition");
unRegisterOutputEvent("fxDTSBrick", "SetVehicle");
unRegisterOutputEvent("fxDTSBrick", "SpawnItem");
unregisterOutputEvent("fxDTSBrick","spawnProjectile");
unregisterOutputEvent("fxDTSBrick","spawnExplosion");

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

unRegisterOutputEvent("Bot", "AddHealth");
unRegisterOutputEvent("Bot", "BurnPlayer");
unRegisterOutputEvent("Bot", "ChangeDatablock");
unRegisterOutputEvent("Bot", "ClearBurn");
unRegisterOutputEvent("Bot", "ClearTools");
unRegisterOutputEvent("Bot", "SetHealth");
unRegisterOutputEvent("Bot", "SpawnExplosion");
unRegisterOutputEvent("Bot", "SpawnProjectile");

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
addExtraResource($City::DataPath @ "ui/hunger.png");

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
