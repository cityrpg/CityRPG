// ============================================================
// Base Function
// ============================================================
function CityLots_TransferLot(%brick, %targetBL_ID)
{
	// Create Chown
	// We're using the CityRPGHostClient to bypass the trust check used by Chown.
	if(!isObject(CityRPGHostClient.chown))
	{
		%chown = Chown(CityRPGHostClient);
	}
	else
	{
		%chown = CityRPGHostClient.chown;
	}

	%chown.isCityTransfer = 1;

	%chown.bl_id = %targetBL_ID;
	%chown.target_group = "BrickGroup_" @ %targetBL_ID;
	%chown.setStartBrick(%brick);

	%brick.setCityLotOwnerID(%targetBL_ID);
}

// ============================================================
// Menu Functions
// ============================================================
function CityMenu_Lot(%client, %notitle)
{
	if(%client.CityRPGLotBrick $= "")
	{
		messageClient(%client, '', "\c6You are currently not on a lot.");
		return;
	}

	// ## Initial display ## //
	%brick = %client.CityRPGLotBrick;
	%price = %brick.dataBlock.initialPrice;

	if(%brick.getCityLotID() == -1)
	{
		error("Attempting to access a blank lot! Re-initializing it...");

		%brick.initializeCityLot();
		%brick.assignCityLotName();
	}

	if(!%notitle)
	{
		messageClient(%client, '', "\c3Lot Management\c6 for: " @ %brick.getCityLotName() @ "\c6 - " @ %brick.getDataBlock().uiName);
	}

	// ## Options for all lots ## //
	%menu = "View lot rules.";
			//TAB "View warning log."

	%functions =	"CityMenu_LotRules";
						//TAB "CityMenu_Placeholder"

	// ## Options for unclaimed lots ## //
	if(%brick.getCityLotOwnerID() == -1)
	{
		messageClient(%client, '', "\c6This lot is for sale! It can be purchased for \c3$" @ %price @ "\c6.");

		// Place these options first.
		%menu = "Purchase this lot. " TAB %menu;
		%functions = "CityMenu_LotPurchasePrompt" TAB %functions;
	}

	// ## Options for lot owners ## //
	if(%brick.getCityLotOwnerID() == %client.bl_id)
	{
		%menu = %menu
				TAB "Rename lot.";

		%functions = %functions
				TAB "CityMenu_LotSetNamePrompt";
	}

	// ## Options for admins ## //
	if(%client.isAdmin)
	{
		%menu = %menu TAB "\c4Open admin menu.";
		%functions = %functions TAB "CityMenu_LotAdmin";
	}

	// ## Finalization ## //
	%menu = %menu TAB "Close menu.";
	%functions = %functions TAB "CityMenu_Close";

	%client.cityMenuOpen(%menu, %functions, %client.CityRPGLotBrick, "\c3Lot management menu closed.");
}

// ## Functions for all lots ## //
function CityMenu_LotRules(%client)
{
	messageClient(%client, '', "\c3Code enforcement requires following restrictions on this lot:");

	%lotRules = $Pref::Server::City::LotRules;
	messageClient(%client, '', "\c6" @ %lotRules);
}

// ## Functions for unclaimed lots ## //
function CityMenu_LotPurchasePrompt(%client)
{
	%lot = %client.cityMenuID;

	%client.cityLog("Lot " @ %lot.getCityLotID() @ " purchase prompt");

	if(CityRPGData.getData(%client.bl_id).valueMoney >= %lot.dataBlock.initialPrice)
	{
		messageClient(%client, '', "\c6You are purchasing this lot for \c3$" @ %lot.dataBlock.initialPrice @ "\c6. Make sure you have read the lot rules. Lot sales are final!");
		messageClient(%client, '', "\c6Type \c31\c6 to confirm, or leave the lot to cancel.");

		%client.cityMenuFunction = CityLots_PurchaseLot;
		%client.cityMenuID = %lot;
	}
	else
	{
		messageClient(%client, '', "\c6You need \c3$" @ %lot.dataBlock.initialPrice @ "\c6 on hand to purchase this lot.");
		%client.cityMenuClose();
	}
}

function CityLots_PurchaseLot(%client, %input, %lot)
{
	if(%lot $= "")
	{
		%lot = %client.cityMenuID;
	}

	if(%input !$= "1")
	{
		messageClient(%client, '', "\c0Lot purchase cancelled.");
		%client.cityMenuClose();
	}
	else if(%lot.getCityLotOwnerID() != -1 || CityRPGData.getData(%client.bl_id).valueMoney < %lot.dataBlock.initialPrice)
	{
		%client.cityLog("(!!!) Lot " @ %lot.getCityLotID() @ " purchase fell through");

		// Security check falls through
		messageClient(%client, '', "\c0Sorry, you are no-longer able to purchase this lot at this time.");
		%client.cityMenuClose();
	}
	else if(CityRPGData.getData(%client.bl_id).valueMoney >= %lot.dataBlock.initialPrice)
	{
		%client.cityLog("Lot " @ %lot.getCityLotID() @ " purchase success");

		CityRPGData.getData(%client.bl_id).valueMoney -= %lot.dataBlock.initialPrice;
		messageClient(%client, '', "\c6You have purchased this lot for \c3$" @ %lot.dataBlock.initialPrice @ "\c6!");

		%client.setInfo();

		CityLots_TransferLot(%client.cityMenuID, %client.bl_id); // The menu ID is the lot brick ID

		// Open the menu for the new lot
		%client.cityMenuClose(1);
		CityMenu_Lot(%client);
	}
}

// ## Functions for lot owners ## //
function CityMenu_LotSetNamePrompt(%client)
{
	%client.cityLog("Lot " @ %client.cityMenuID.getCityLotID() @ " rename prompt");

	messageClient(%client, '', "\c6Enter a new name for your lot.");
	%client.cityMenuFunction = CityMenu_LotSetName;
}

function CityMenu_LotSetName(%client, %input)
{
	%brick = %client.cityMenuID;

	%client.cityLog("Lot " @ %brick.getCityLotID() @ " rename '" @ %input @ "'");

	if(%brick.getCityLotOwnerID() != %client.bl_id)
	{
		return;
	}

	if(strlen(%input) > 40)
	{
		messageClient(%client, '', "\c6Sorry, that name exceeds the length limit. Please try again.");
		return;
	}

	%name = StripMLControlChars(%input);

	%brick.setCityLotName(%name);
	messageClient(%client, '', "\c6Lot name changed to \c3" @ %brick.getCityLotName() @ "\c6.");

	%client.cityMenuClose();
}

// ## Functions for admins ## //
function CityMenu_LotAdmin(%client)
{
	%client.cityMenuClose(true);

	messageClient(%client, '', "\c3Lot Admin\c6 for: \c3" @ %client.CityRPGLotBrick.getCityLotName() @ "\c6 - Lot ID: \c3" @ %client.CityRPGLotBrick.getCityLotID());

	%menu = "Force rename."
			TAB "Transfer lot to the city."
			TAB "Transfer lot to a player.";

	%functions =	"CityMenu_LotAdmin_SetNamePrompt"
						TAB "CityMenu_LotAdmin_TransferCity"
						TAB "CityMenu_LotAdmin_TransferPlayerPrompt";

	%client.cityMenuOpen(%menu, %functions, %client.CityRPGLotBrick, "\c3Lot management menu closed.");
}

function CityMenu_LotAdmin_SetNamePrompt(%client)
{
	%client.cityLog("Lot MOD " @ %client.cityMenuID.getCityLotID() @ " rename prompt");
	messageClient(%client, '', "\c6Enter a new name for the lot \c3" @ %client.cityMenuID.getCityLotName() @ "\c6.");
	%client.cityMenuFunction = CityMenu_LotAdmin_SetName;
}

function CityMenu_LotAdmin_SetName(%client, %input)
{
	%client.cityLog("Lot MOD " @ %client.cityMenuID.getCityLotID() @ " rename '" @ %input @ "'");

	if(strlen(%input) > 40)
	{
		messageClient(%client, '', "\c6Sorry, that name exceeds the length limit. Please try again.");
		return;
	}

	%name = StripMLControlChars(%input);

	%client.cityMenuID.setCityLotName(%name);
	messageClient(%client, '', "\c6Lot name changed to \c3" @ %client.cityMenuID.getCityLotName() @ "\c6.");

	%client.cityMenuClose();
}

function CityMenu_LotAdmin_TransferCity(%client)
{
	%hostID = getNumKeyID();

	%brick = %client.cityMenuID;

	%client.cityLog("Lot MOD " @ %brick.getCityLotID() @ " transfer city");

	CityLots_TransferLot(%brick, %hostID);

	%brick.setCityLotName("Unclaimed Lot");
	%brick.setCityLotOwnerID(-1);

	messageClient(%client, '', "\c6Lot transferred to the city successfully.");
	%client.cityMenuClose();
}

function CityMenu_LotAdmin_TransferPlayerPrompt(%client)
{
	%client.cityLog("Lot MOD " @ %client.cityMenuID.getCityLotID() @ " transfer pl prompt");

	messageClient(%client, '', "\c6Enter a Blockland ID of the player to transfer the lot to.");
	%client.cityMenuFunction = CityMenu_LotAdmin_TransferPlayer;
}

function CityMenu_LotAdmin_TransferPlayer(%client, %input)
{
	%client.cityLog("Lot MOD " @ %client.cityMenuID.getCityLotID() @ " transfer pl '" @ %input @ "'");

	%target = findClientByBL_ID(%input);

	// Hacky workaround to detect if a non-number is passed to avoid pain.
	if(%input == 0 && %input !$= "0")
	{
		messageClient(%client, '', "\c3" @ %input @ "\c6 is not a valid Blockland ID. Please try again.");
		return;
	}

	CityLots_TransferLot(%client.cityMenuID, %input);

	%client.cityMenuClose();
}

// ============================================================
// Database
// ============================================================
// TODO We are currently using the included Sassy saver, however this is planned to be replaced in the future.

function CityLots_InitRegistry()
{
	if(!isObject(CityRPGLotRegistry))
	{
		new scriptObject(CityRPGLotRegistry)
		{
			class = Sassy;
			dataFile = "config/server/CityRPG/CityLots.dat";
		};

		// Also use valueCount as a fallback check.
		// Externally deleted files still return 1 for isFile (and thus loadedSaveFile) until the game restarts.
		// This fixes the registry breaking if the file is deleted via external method mid-game, or if any other trickery occurs.
		if(!CityRPGLotRegistry.loadedSaveFile || CityRPGLotRegistry.valueCount != 4)
		{
			CityRPGLotRegistry.addValue("name", "Unclaimed Lot");
			CityRPGLotRegistry.addValue("ownerID", -1);
			CityRPGLotRegistry.addValue("ruleStr", "This lot currently has no rules.");
		}
	}
}

function CityLots_GetLotCount()
{
	%count = CityRPGLotRegistry.dataCount;

	if(%count $= "")
	{
		%count = 0;
	}

	return %count;
}

function fxDTSBrick::initializeCityLot(%brick)
{
	if(CityRPGLotRegistry.getData(%brick.getCityLotID()) != 0)
	{
		warn("Lot registry - Attempting to initialize a lot that already exists. Re-initializing as a new lot.");
		backtrace();
		%brick.cityLotOverride = 1;
		%brick.setNTObjectName("");
	}

	// 1. Initialize lot data with default values
	// 2. Increment the lot index

	%newIndex = CityLots_GetLotCount()+1;

	if(CityRPGLotRegistry.getData(%newIndex) != 0)
	{
		error("CityRPG Lot Registry - Attempting to initialize the lot '" @ %newIndex @ "' but the ID already exists! This lot may not have registered correctly.");
	}

	CityRPGLotRegistry.addData(%newIndex);

	if(CityRPGLotRegistry.dataCount > 0)
	{
		CityRPGLotRegistry.saveData();
	}
	else
	{
		error("Lot registry is blank or missing! Will not export.");
	}

	%publicID = getNumKeyID();

	if(%brick.getGroup().bl_id != %publicID)
	{
		CityLots_TransferLot(%brick, %publicID);
	}

	return %newIndex;
}

// Returns the lot's ID number.
function fxDTSBrick::getCityLotID(%brick)
{
	%nameRaw = %brick.getName();

	if(%nameRaw $= "")
	{
		// No name
		return -1;
	}

	%lotID = getSubStr(%nameRaw, 1, strlen(%nameRaw));

	if(CityRPGLotRegistry.getData(%lotID) == 0)
	{
		// Doesn't exist in the registry
		return -1;
	}

	return %lotID;
}

// ## Getters

function fxDTSBrick::getCityLotName(%brick)
{
	return CityRPGLotRegistry.getData(%brick.getCityLotID()).valueName;
}

function fxDTSBrick::getCityLotOwnerID(%brick)
{
	return CityRPGLotRegistry.getData(%brick.getCityLotID()).valueOwnerID;
}

function fxDTSBrick::getCityLotRuleStr(%brick)
{
	return CityRPGLotRegistry.getData(%brick.getCityLotID()).valueRuleStr;
}

// ## Setters

function fxDTSBrick::setCityLotName(%brick, %value)
{
	%valueNew = CityRPGLotRegistry.getData(%brick.getCityLotID()).valueName = getSubStr(%value, 0, 40);

	if(CityRPGLotRegistry.dataCount > 0)
	{
		CityRPGLotRegistry.saveData();
	}
	else
	{
		error("Lot registry is blank or missing! Will not export.");
	}

	return %valueNew;
}

function fxDTSBrick::setCityLotOwnerID(%brick, %value)
{
	%data = CityRPGLotRegistry.getData(%brick.getCityLotID());
	%valueOld = %data.valueOwnerID;

	if(%valueOld == -1 && %value != -1)
	{
		// If transferring from the city to a player, automatically rename the lot.
		%brick.setCityLotName(%brick.getGroup().name @ "\c6's Lot");
	}

	%valueNew = %data.valueOwnerID = %value;

	if(CityRPGLotRegistry.dataCount > 0)
	{
		CityRPGLotRegistry.saveData();
	}
	else
	{
		error("Lot registry is blank or missing! Will not export.");
	}

	return %valueNew;
}

// ============================================================
// Package
// ============================================================
package CityRPG_LotRegistry
{
	// Chown add-on hook
	function Chown::tickTransfer(%this, %brick_i)
	{
		%brick = %this.brick;
		if(isObject(%brick) && %brick.dataBlock.CityRPGBrickType == $CityBrick_Lot)
		{
			%isLot = 1;

			// Special override that doesn't reset after ClearNTObjectName
			%brick.cityLotOverrideReset = 1;
		}

		// Handling for the lot registry
		// If this isn't a marked city transfer but it's a lot brick...
		if(!%this.isCityTransfer)
		{
			%brick.setCityLotOwnerID(%this.target_group.bl_id);
		}

		parent::tickTransfer(%this, %brick_i);
	}

	function serverCmdSetWrenchData(%client, %fields)
	{
		%brick = %client.wrenchBrick;
		%brick.cityLastWrench = %client;

		Parent::serverCmdSetWrenchData(%client, %fields);
	}

	// Brick rename blocking
	function SimObject::SetNTObjectName(%obj, %name)
	{
		// Special override to handle lots when they are loaded from a save.
		if(%obj.cityLotInit)
		{
			Parent::SetNTObjectName(%obj, %name);

			// assignCityLot will reset the init value
			%obj.assignCityLotName();

			return;
		}

		if(!%obj.cityLotOverride && !%obj.cityLotOverrideReset && %obj.dataBlock !$= "" && %obj.dataBlock.CityRPGBrickType == $CityBrick_Lot)
		{
			%client = %obj.cityLastWrench;

			// If the brick changes but the name is unchanged, we'll still block the check without messaging the client.
			if(isObject(%client) && "_" @ %name !$= %obj.getName())
			{
				messageClient(%client, '', "\c6You cannot rename lot bricks. Please name your lot using the lot menu instead.");
			}

			return;
		}

		Parent::SetNTObjectName(%obj, %name);
		%obj.cityLotOverride = 0;
		%obj.cityLotOverrideReset = 0;
	}

	// In case the above check falls through, we'll also package these with a warning.
	function SimObject::ClearNTObjectName(%obj)
	{
		// Throw an error if the following conditions are met:
		// %obj.cityLotOverride and cityLotOverrideReset are not true, the datablock exists and is a lot, and the name isn't already empty.
		if(!%obj.cityLotOverride && !%obj.cityLotOverrideReset && %obj.dataBlock !$= "" && %obj.dataBlock.CityRPGBrickType == $CityBrick_Lot && %obj.getName() !$= "")
		{
			warn("CityRPG 4 - Attempt to call ClearNTObjectName on a lot brick!");
			backtrace();
			return;
		}

		Parent::ClearNTObjectName(%obj);
		%obj.cityLotOverride = 0;
	}

	function SimObject::AddNTName(%obj, %name)
	{
		if(%obj.dataBlock !$= "" && %obj.dataBlock.CityRPGBrickType == $CityBrick_Lot)
		{
			warn("CityRPG 4 - Attempt to call ClearNTObjectName on a lot brick!");
			backtrace();
		}

		Parent::AddNTName(%obj, %name);
	}

	// Assigns the name for a city lot brick.
	// Initializes the lot if it doesn't already exist.
	function fxDTSBrick::assignCityLotName(%brick)
	{
		%lotID = %brick.getCityLotID();

		if(%lotID == -1)
		{
			%lotID = %brick.initializeCityLot();
		}

		%brick.cityLotInit = 0;
		%brick.cityLotOverride = 1;
		%brick.setNTObjectName(%lotID);

		// If this falls through, the lot is an existing lot and can be left alone.
	}

	function fxDTSBrick::onPlant(%brick)
	{
		Parent::onPlant(%brick);

		if(%brick.dataBlock !$= "" && %brick.dataBlock.CityRPGBrickType == $CityBrick_Lot)
		{
			%brick.schedule(0,assignCityLotName);
		}
	}

	// Hack to work around wonky packaging issues
	// Called after City_OnPlant
	function fxDTSBrick::onCityLoadPlant(%this, %brick)
	{
		Parent::onCityLoadPlant(%this, %brick);

		if(%brick.dataBlock !$= "" && %brick.dataBlock.CityRPGBrickType == $CityBrick_Lot)
		{
			// In the loading tick, names are assigned after the brick is planted.
			// We need to set a special override so the name isn't caught by setNTObjectName
			%brick.cityLotInit = 1;
		}
	}

	function fxDTSBrick::onRemove(%brick,%client)
	{
		// Always override on remove
		%brick.cityLotOverride = 1;
		Parent::onRemove(%brick);
	}

	function disconnect(%a)
	{
		CityRPGLotRegistry.delete();
		return parent::disconnect(%a);
	}

	function City_Init()
	{
		CityLots_InitRegistry();
		Parent::City_Init();
	}
};

deactivatePackage(CityRPG_LotRegistry);
activatepackage(CityRPG_LotRegistry);
