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
		%client.cityMenuMessage("\c6You are currently not on a lot.");
		return;
	}

	// ## Initial display ## //
	%brick = %client.CityRPGLotBrick;
	%price = %brick.dataBlock.initialPrice;

	if(%brick.getCityLotID() == -1)
	{
		error("Attempting to access a blank lot! Re-initializing it...");

		%brick.initNewCityLot();
	}

	if(!%notitle)
	{
		%client.cityMenuMessage("\c3Lot Management\c6 for: " @ %brick.getCityLotName() @ "\c6 - " @ %brick.getDataBlock().uiName);
	}

	// ## Options for all lots ## //
	%menu = "View lot rules.";
			//TAB "View warning log."

	%functions =	"CityMenu_LotRules";
						//TAB "CityMenu_Placeholder"

	// ## Options for unclaimed lots ## //
	if(%brick.getCityLotOwnerID() == -1)
	{
		%client.cityMenuMessage("\c6This lot is for sale! It can be purchased for \c3$" @ %price @ "\c6.");

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
	%client.cityMenuMessage("\c3Code enforcement requires following restrictions on this lot:");

	%lotRules = $Pref::Server::City::LotRules;
	%client.cityMenuMessage("\c6" @ %lotRules);
}

// ## Functions for unclaimed lots ## //
function CityMenu_LotPurchasePrompt(%client)
{
	%lot = %client.cityMenuID;

	%client.cityLog("Lot " @ %lot.getCityLotID() @ " purchase prompt");

	if(CityRPGData.getData(%client.bl_id).valueMoney >= %lot.dataBlock.initialPrice)
	{
		%client.cityMenuMessage("\c6You are purchasing this lot for \c3$" @ %lot.dataBlock.initialPrice @ "\c6. Make sure you have read the lot rules. Lot sales are final!");
		%client.cityMenuMessage("\c6Type \c31\c6 to confirm, or leave the lot to cancel.");

		%client.cityMenuFunction = CityLots_PurchaseLot;
		%client.cityMenuID = %lot;
	}
	else
	{
		%client.cityMenuMessage("\c6You need \c3$" @ %lot.dataBlock.initialPrice @ "\c6 on hand to purchase this lot.");
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
		%client.cityMenuMessage("\c0Lot purchase cancelled.");
		%client.cityMenuClose();
	}
	else if(%lot.getCityLotOwnerID() != -1 || CityRPGData.getData(%client.bl_id).valueMoney < %lot.dataBlock.initialPrice)
	{
		%client.cityLog("Lot " @ %lot.getCityLotID() @ " purchase fell through", 0, 1);

		// Security check falls through
		%client.cityMenuMessage("\c0Sorry, you are no-longer able to purchase this lot at this time.");
		%client.cityMenuClose();
	}
	else if(CityRPGData.getData(%client.bl_id).valueMoney >= %lot.dataBlock.initialPrice)
	{
		%client.cityLog("Lot " @ %lot.getCityLotID() @ " purchase success");

		CityRPGData.getData(%client.bl_id).valueMoney -= %lot.dataBlock.initialPrice;
		%client.cityMenuMessage("\c6You have purchased this lot for \c3$" @ %lot.dataBlock.initialPrice @ "\c6!");

		%client.setInfo();

		CityLots_TransferLot(%client.cityMenuID, %client.bl_id); // The menu ID is the lot brick ID
		%client.cityMenuID.setCityLotTransferDate(getDateTime());

		// Open the menu for the new lot
		%client.cityMenuClose(1);
		CityMenu_Lot(%client);
	}
}

// ## Functions for lot owners ## //
function CityMenu_LotSetNamePrompt(%client)
{
	%client.cityLog("Lot " @ %client.cityMenuID.getCityLotID() @ " rename prompt");

	%client.cityMenuMessage("\c6Enter a new name for your lot.");
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
		%client.cityMenuMessage("\c6Sorry, that name exceeds the length limit. Please try again.");
		return;
	}

	%name = StripMLControlChars(%input);

	%brick.setCityLotName(%name);
	%client.cityMenuMessage("\c6Lot name changed to \c3" @ %brick.getCityLotName() @ "\c6.");

	%client.cityMenuClose();
}

// ## Functions for admins ## //
function CityMenu_LotAdmin(%client)
{
	%client.cityMenuClose(true);
	%brick = %client.CityRPGLotBrick;
	%ownerID = %brick.getCityLotOwnerID();

	%client.cityMenuMessage("\c3Lot Admin\c6 for: \c3" @ %brick.getCityLotName() @ "\c6 - Lot ID: \c3" @ %brick.getCityLotID() @ "\c6 - Brick ID: \c3" @ %brick.getID() @ "\c6 - Lot purchase date: \c3" @ %brick.getCityLotTransferDate());

	if(%ownerID != -1)
	{
		%owner = CityRPGData.getData(%ownerID);
		%client.cityMenuMessage("\c6Owner: \c3" @ %owner.valueName @ "\c6 (ID \c3" @ %brick.getCityLotOwnerID() @ "\c6)");
	}
	else
	{
		%client.cityMenuMessage("\c6Lot is owned by the city.");
	}

	%menu = "Force rename."
			TAB "Transfer lot to the city."
			TAB "Transfer lot to a player.";

	%functions =	"CityMenu_LotAdmin_SetNamePrompt"
						TAB "CityMenu_LotAdmin_TransferCity"
						TAB "CityMenu_LotAdmin_TransferPlayerPrompt";

	%client.cityMenuOpen(%menu, %functions, %brick, "\c3Lot management menu closed.");
}

function CityMenu_LotAdmin_SetNamePrompt(%client)
{
	%client.cityLog("Lot MOD " @ %client.cityMenuID.getCityLotID() @ " rename prompt");
	%client.cityMenuMessage("\c6Enter a new name for the lot \c3" @ %client.cityMenuID.getCityLotName() @ "\c6. ML tags are allowed.");
	%client.cityMenuFunction = CityMenu_LotAdmin_SetName;
}

function CityMenu_LotAdmin_SetName(%client, %input)
{
	%client.cityLog("Lot MOD " @ %client.cityMenuID.getCityLotID() @ " rename '" @ %input @ "'");

	if(strlen(%input) > 40)
	{
		%client.cityMenuMessage("\c6Sorry, that name exceeds the length limit. Please try again.");
		return;
	}

	%client.cityMenuID.setCityLotName(%name);
	%client.cityMenuMessage("\c6Lot name changed to \c3" @ %client.cityMenuID.getCityLotName() @ "\c6.");

	%client.cityMenuClose();
}

function CityMenu_LotAdmin_TransferCity(%client)
{
	%hostID = getNumKeyID();

	%brick = %client.cityMenuID;

	%client.cityLog("Lot MOD " @ %brick.getCityLotID() @ " transfer city");

	CityLots_TransferLot(%brick, %hostID);
	%brick.setCityLotTransferDate(getDateTime());

	%brick.setCityLotName("Unclaimed Lot");
	%brick.setCityLotOwnerID(-1);

	%client.cityMenuMessage("\c6Lot transferred to the city successfully.");
	%client.cityMenuClose();
}

function CityMenu_LotAdmin_TransferPlayerPrompt(%client)
{
	%client.cityLog("Lot MOD " @ %client.cityMenuID.getCityLotID() @ " transfer pl prompt");

	%client.cityMenuMessage("\c6Enter a Blockland ID of the player to transfer the lot to.");
	%client.cityMenuFunction = CityMenu_LotAdmin_TransferPlayer;
}

function CityMenu_LotAdmin_TransferPlayer(%client, %input)
{
	%client.cityLog("Lot MOD " @ %client.cityMenuID.getCityLotID() @ " transfer pl '" @ %input @ "'");

	%target = findClientByBL_ID(%input);

	// Hacky workaround to detect if a non-number is passed to avoid pain.
	if(%input == 0 && %input !$= "0")
	{
		%client.cityMenuMessage("\c3" @ %input @ "\c6 is not a valid Blockland ID. Please try again.");
		return;
	}

	CityLots_TransferLot(%client.cityMenuID, %input);
	%client.cityMenuID.setCityLotTransferDate(getDateTime());

	%client.cityMenuClose();
}

// ============================================================
// Registry
// ============================================================
// TODO We are currently using the included Sassy saver, however this is planned to be replaced in the future.

function CityLots_InitRegistry()
{
	if(!isObject(CityRPGLotRegistry))
	{
		new scriptObject(CityRPGLotRegistry)
		{
			class = Sassy;
			dataFile = $City::SavePath @ "CityLots.dat";
		};

		// Also use valueCount as a fallback check.
		// Externally deleted files still return 1 for isFile (and thus loadedSaveFile) until the game restarts.
		// This fixes the registry breaking if the file is deleted via external method mid-game, or if any other trickery occurs.
		if(!CityRPGLotRegistry.loadedSaveFile || CityRPGLotRegistry.valueCount != 5)
		{
			CityRPGLotRegistry.addValue("name", "Unclaimed Lot");
			CityRPGLotRegistry.addValue("ownerID", -1);
			CityRPGLotRegistry.addValue("ruleStr", "This lot currently has no rules.");
			CityRPGLotRegistry.addValue("transferDate", "None");
			CityRPGLotRegistry.addValue("preownedSalePrice", -1);

			// Instead of using the getData function directly, these values are generally called upon using setter and getter functions.
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

// Brick::getCityLotSaveName()
// Returns the brick's NT Object name in a readable format.
function fxDTSBrick::getCityLotSaveName(%brick)
{
	%nameRaw = %brick.getName();
	%nameRaw = getSubStr(%nameRaw, 1, strlen(%nameRaw));
	%nameRaw = strreplace(%nameRaw, "_", " ");

	return %nameRaw;
}

// Determines the state of the lot and directs the corresponding init process.
function fxDTSBrick::initCityLot(%brick)
{
	if(%brick.getCityLotID() == -1)
	{
		%brick.initNewCityLot();
	}
	else
	{
		%brick.initExistingCityLot();
	}

	// Cache the brick.
	CityRPGLotRegistry.getData(%brick.getCityLotID()).brick = %brick;
}

function fxDTSBrick::initExistingCityLot(%brick)
{
	%nameRaw = %brick.getCityLotSaveName();

	%lotHost = getWord(%nameRaw, 0);
	//%lotSavedOwner = getWord(%nameRaw, 1);
	%lotID = getWord(%nameRaw, 2);

	// If there is a mismatch, or the lot appears to be a legacy lot.
	if(%lotHost != getNumKeyID() || getWordCount(%nameRaw) < 3)
	{
		if($LoadingBricks_Client !$= "")
		{
			if(!$City::WarningMessageDisplay)
			{
				$City::WarningMessageDisplay = 1;
				// TODO Clarify "See the prefs panel"
				%warningMsg = "!!!! WARNING: This save appears to be from a different CityRPG server, or an older version. Lot data (names, etc.) may not carry over, but ownership will be converted. If you would like to override this (i.e. you have the CityRPG data files from the original server), see the prefs panel.";

				warn(%warningMsg);
				messageAll('', %warningMsg);
			}

			%brick.convertCityLotOwnership();
		}
		else
		{
			// We have a host mismatch, but mysteriously, we're not loading bricks.
			// In this case, something has gone terribly wrong.
			error("CityRPG Lot Registry - Lot host mismatch outside of loading bricks! ('" @ %nameRaw @ "'). Aborting...");
			return;
		}
	}

	%ownerID = %brick.getCityLotOwnerID();

	$City::RealEstate::TotalLots++;
	// Count it as a pre-owned lot for sale if applicable.
	if(%brick.setCityLotPreownedPrice() == -1)
	{
		$City::RealEstate::LotCountSale++;
	}

	if(%lotID == -1)
	{
		warn("CityRPG 4 - Attempt to initialize existing lot " @ %brick @ ", but lot ID is blank! Aborting init.");
		return;
	}

	%brick.cityLotInit = 0;
	%brick.cityLotOverride = 1;
	// Note that for an existing lot, the owner ID is always derived from the lot registry, NOT the brick's saved name.
	// This rules out any potential error in the brick's saved name.
	%brick.setNTObjectName(getNumKeyID() @ "_" @ %ownerID @ "_" @ %lotID);

	if(%ownerID != -1)
	{
		// Add the lot to the owner's list, initializing the list with our first value if it's blank.
		$City::Cache::LotsOwnedBy[%ownerID] = $City::Cache::LotsOwnedBy[%ownerID] $= "" ? %brick : $City::Cache::LotsOwnedBy[%ownerID] SPC %brick;
	}
	else
	{
		$City::RealEstate::UnclaimedLots++;
	}
}

function fxDTSBrick::initNewCityLot(%brick)
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

	%newID = CityLots_GetLotCount()+1;

	if(CityRPGLotRegistry.getData(%newID) != 0)
	{
		error("CityRPG Lot Registry - Attempting to initialize the lot '" @ %newID @ "' but the ID already exists! This lot may not have registered correctly.");
	}

	CityRPGLotRegistry.addData(%newID);

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

	%brick.cityLotOverride = 1;
	%brick.setNTObjectName(%publicID @ "_" @ "none" @ "_" @ %newID);

	$City::RealEstate::UnclaimedLots++;

	echo("City: Registered new lot, #" @ %newID);

	return %newID;
}

function fxDTSBrick::convertCityLotOwnership(%brick)
{
	talk("TODO: Lot ownership conversion not implemented");
	// 1. Check the lot's brick name for the original owner. Assign.
	// 2. Initialize the lot as a new lot to give it an ID on the current server, flushing out the old one.
	// 3. Call a transfer of the lot's ownership via CityLots_TransferLot to the original owner.
}

// Removes the lot from the owner's cached list of "owned lots".
function fxDTSBrick::cityLotCacheRemove(%brick)
{
	%ownerID = %brick.getCityLotOwnerID();

	for(%i = 0; %i <= getWordCount($City::Cache::LotsOwnedBy[%ownerID]); %i++)
	{
		%brickCheck = getWord($City::Cache::LotsOwnedBy[%ownerID], %i);
		if(%brickCheck == %brick)
		{
			$City::Cache::LotsOwnedBy[%ownerID] = removeWord($City::Cache::LotsOwnedBy[%ownerID], %i);
			%removed = 1;
			break;
		}

	}

	if(!%removed)
		error("CityRPG 4 - Attempted to remove the lot '" @ %brick.getCityLotID() @ "' from the ownership cache of BLID '" @ %ownerID @ "' but the value is missing from the cache.");
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

	%nameRaw = %brick.getCityLotSaveName();

	%lotID = getWord(%nameRaw, 2);

	// If the lot's brick name is blank at this stage, for whatever reason, lotID will  be -1 due to getWord failing.

	if(CityRPGLotRegistry.getData(%lotID) == 0)
	{
		// Doesn't exist in the registry
		return -1;
	}

	return %lotID;
}

function findLotBrickByID(%value)
{
	return CityRPGLotRegistry.getData(%value).brick;
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

function fxDTSBrick::getCityLotTransferDate(%brick)
{
	return CityRPGLotRegistry.getData(%brick.getCityLotID()).valueTransferDate;
}

function fxDTSBrick::getCityLotPreownedPrice(%brick)
{
	return CityRPGLotRegistry.getData(%brick.getCityLotID()).valuePreownedSalePrice;
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
	%lotID = %brick.getCityLotID();
	%data = CityRPGLotRegistry.getData(%lotID);
	%valueOld = %data.valueOwnerID;

	// ## Display name handling
	if(%valueOld == -1 && %value != -1)
	{
		// If transferring from the city to a player, automatically rename the lot.
		%brick.setCityLotName(%brick.getGroup().name @ "\c6's Lot");

		$City::RealEstate::UnclaimedLots--;
	}

	// ## Caching
	if(%valueOld != -1)
	{
		// If transferring from a player, clear the cache.
		%brick.cityLotCacheRemove();
	}

	if(%value == -1)
	{
		$City::RealEstate::UnclaimedLots++;
	}
	else
	{
		// If transferring to a player, add it to their cache.
		// Initialize if the cache is blank.
		$City::Cache::LotsOwnedBy[%value] = $City::Cache::LotsOwnedBy[%value] $= "" ? %brick : $City::Cache::LotsOwnedBy[%value] SPC %brick;
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

	// ## Brick name handling
	// The brick's name needs to match the new owner ID, so we need to update it.
	%nameRaw = %brick.getCityLotSaveName();
	%lotHost = getWord(%nameRaw, 0);
	%lotID = getWord(%nameRaw, 2);

	%brick.cityLotOverride = 1;
	%brick.setNTObjectName(%lotHost @ "_" @ (%valueNew == -1?"none":%valueNew) @ "_" @ %lotID);

	return %valueNew;
}

function fxDTSBrick::setCityLotTransferDate(%brick, %value)
{
	CityRPGLotRegistry.getData(%brick.getCityLotID()).valueTransferDate = %value;
}

function fxDTSBrick::setCityLotPreownedPrice(%brick, %value)
{
	CityRPGLotRegistry.getData(%brick.getCityLotID()).valuePreownedSalePrice = %value;
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

			// Init value will be set back to 0 from initCityLot()
			%obj.initCityLot();

			return;
		}

		if(!%obj.cityLotOverride && !%obj.cityLotOverrideReset && %obj.dataBlock !$= "" && %obj.dataBlock.CityRPGBrickType == $CityBrick_Lot)
		{
			%client = %obj.cityLastWrench;

			// If the brick changes but the name is unchanged, we'll still block the check without messaging the client.
			if(isObject(%client) && "_" @ %name !$= %obj.getName())
			{
				%client.cityMenuMessage("\c6You cannot rename lot bricks. Please name your lot using the lot menu instead.");
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
			warn("CityRPG 4 - Attempt to call AddNTName on a lot brick!");
			backtrace();
		}

		Parent::AddNTName(%obj, %name);
	}

	function fxDTSBrick::onPlant(%brick)
	{
		Parent::onPlant(%brick);

		if(%brick.dataBlock !$= "" && %brick.dataBlock.CityRPGBrickType == $CityBrick_Lot)
		{
			%brick.schedule(0,initNewCityLot);
		}
	}

	// Hack to work around wonky packaging issues
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

	function fxDTSBrick::onRemove(%brick, %client)
	{
		%lotID = %brick.getCityLotID();

		// Check that the brick actually exists, is planted, etc.
		// Also verify that is has a lot ID. If it doesn't, the brick likely never fully initialized.
		// This can happen in certain edge cases, such as while loading bricks that already exist (onRemove is called on the brick after it fails to plant)
		if(%brick.isPlanted && %brick.getDataBlock().CityRPGBrickType == $CityBrick_Lot && %lotID != -1)
		{
			%ownerID = %brick.getCityLotOwnerID();

			// Always override on remove
			%brick.cityLotOverride = 1;

			$City::RealEstate::TotalLots--;
			if(%ownerID == -1)
				$City::RealEstate::UnclaimedLots--;
			else
			{
				// Now, we have to remove this lot from the owner's cache of owned lots.
				%brick.cityLotCacheRemove();
			}

			if(%brick.getCityLotPreownedPrice() == -1)
			{
				$City::RealEstate::LotCountSale--;
			}

			// This lot will exist in the memory, but it will no-longer have a brick associated with it.
			// Therefore, we need to remove the brick from the cache.
			// If the lot is re-loaded later, it will be restored on init.
			CityRPGLotRegistry.getData(%lotID).brick = -1;
		}

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
