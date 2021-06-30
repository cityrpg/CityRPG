// ============================================================
// CityRPG 4 Lot Registry
// ============================================================

// There are two major components at play here: McTwist's Chown tool, and McTwist's saver (Thanks, McTwist)
// Any functions that deal directly with the CityLotRegistry object are contained here.
// TODO: Ability to convert lots that are saved from other servers.
// TODO: Pref for servers that have the original save files and want to override conversion

// ============================================================
// Base Function
// ============================================================
function CityLots_TransferLot(%brick, %targetBL_ID)
{
	%lotBricks = findLotBricksLinkedByID(%brick.getCityLotID());

	if(%lotBricks == 0)
	{
		error("CityRPG Lot Registry - Failed to obtain the lot brick '" @ %brick @ "'. Aborting transfer.");
		return;
	}

	// Some lots are "linked" meaning they will have multiple corresponding bricks.
	// Loop through all and start a transfer for each.
	// In most cases, we only need to do this once, for a singular lot.
	for(%i = 0; %i <= getFieldCount(%lotBricks)-1; %i++)
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

		%targetBrick = getField(%lotBricks, %i);
		%chown.setStartBrick(%targetBrick);
	}

	%brick.setCityLotOwnerID(%targetBL_ID);
	%brick.setCityLotPreownedPrice(-1); // Take the lot off sale if it was listed.
}

// ============================================================
// Registry
// ============================================================
function CityLots_InitRegistry()
{
	if(!isObject(CityLotRegistry))
	{
		%newRegistry = new scriptObject(CityLotRegistry)
		{
			class = Saver;
			file = $City::SavePath @ "LotKeys.txt";
			defFile = $City::SavePath @ "LotDefaults.txt";
			folder = $City::SavePath @ "LotData/Lot";
			saveExt = "txt";
		};

		%newRegistry.addValue("name", "Unclaimed Lot");
		%newRegistry.addValue("ownerID", -1);
		%newRegistry.addValue("ruleStr", "This lot currently has no rules.");
		%newRegistry.addValue("transferDate", "None");
		%newRegistry.addValue("preownedSalePrice", -1);
	}
}

function CityLots_GetLotCount()
{
	%count = CityLotRegistry.countKeys;

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

// Brick::setCityLotSaveName()
// Converts a set of parameters to the lot's name.
// hostID: The BLID of the server host.
// ownerID: The BLID of the lot's owner. "-1" or "none" both indicate a lot owned by the city.
// lotID: The lot's unique ID number.
// isLinked (bool): Whether the lot is a child of another lot.
function fxDTSBrick::setCityLotSaveName(%brick, %hostID, %ownerID, %lotID, %isLinked)
{
	if(%hostID $= "")
	{
		%hostID = getNumKeyID();
	}

	if(%ownerID $= "" || %ownerID == -1)
	{
		%ownerID = "none";
	}

	if(%lotID $= "")
	{
		return -1;
	}

	if(%isLinked $= "")
	{
		%isLinked = 0;
	}

	%brick.SetNTObjectNameOverride(%hostID @ "_" @ %ownerID @ "_" @ %lotID @ "_" @ %isLinked);
}

// Brick::setCityLotSaveNameWhole()
// Sets a save name derived from a raw string obtained using getCityLotSaveName
function fxDTSBrick::setCityLotSaveNameWhole(%brick, %str)
{
	if(atof(getWord(%str, 1)) == -1)
	{
		%str = setWord(%str, 1, "none");
	}

	%brick.SetNTObjectNameOverride(%str);
}

// Determines the state of the lot and directs the corresponding init process.
function fxDTSBrick::initCityLot(%brick, %forceExisting)
{
	if(%brick.lotInitialized)
	{
		error("CityRPG Lot Registry - Attempting to re-initialize a lot! Something's gone wrong. Aborting...");
		return;
	}

	%brick.lotInitialized = 1;
	if(%brick.getCityLotID() == -1 && !%forceExisting)
	{
		%brick.initNewCityLot();
	}
	else
	{
		%brick.initExistingCityLot();
	}

	%brick.cityLotInit = 0;

	// Cache and identify the brick.
	%obj = CityLotRegistry.makeOnline(%brick.getCityLotID());
	if(%brick.getCityLotIsLinked())
	{
		// %obj.linkedBrickCount & %obj.brickLinked[i] for linked lots
		%obj.brickLinked[%obj.linkedBrickCount++] = %brick;
	}
	else
	{
		%obj.brick = %brick;
	}
}

function fxDTSBrick::initExistingCityLot(%brick)
{
	$City::RealEstate::TotalLots++;

	if(%brick.getCityLotPreownedPrice() != -1)
	{
		$City::RealEstate::LotCountSale++;
	}

	if(%brick.getCityLotOwnerID() == -1)
	{
		$City::RealEstate::UnclaimedLots++;
	}

	%nameRaw = %brick.getCityLotSaveName();

	%lotHost = getWord(%nameRaw, 0);
	//%lotSavedOwner = getWord(%nameRaw, 1);
	%lotID = getWord(%nameRaw, 2);
	%isLinked = getWord(%nameRaw, 3);

	// Legacy support for pre-0.3.0 lots
	if(%isLinked $= "")
	{
		%isLinked = 0;
	}

	// If there is a mismatch, or the lot appears to be a legacy lot.
	if(%lotHost != getNumKeyID() || getWordCount(%nameRaw) < 3)
	{
		if($LoadingBricks_Client !$= "")
		{
			if(!$City::WarningMessageDisplay)
			{
				$City::WarningMessageDisplay = 1;
				%warningMsg = "!!!! WARNING: This save appears to be from a different CityRPG server or an older version of CityRPG. Lots will be registered as Unclaimed lots.";

				warn(%warningMsg);
				messageAll('', %warningMsg);
			}

			%brick.convertCityLotOwnership();
			return;
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

	if(%lotID == -1)
	{
		warn("CityRPG 4 - Attempt to initialize existing lot " @ %brick @ ", but lot ID is blank! Aborting init.");
		return;
	}

	%brick.cityLotOverride = 1;
	// Note that for an existing lot, the owner ID is always derived from the lot registry, NOT the brick's saved name.
	// This rules out any potential error in the brick's saved name, i.e. outdated save.
	%brick.setCityLotSaveName(getNumKeyID(), %ownerID, %lotID, %isLinked);
	%brickOwnerID = getBrickGroupFromObject(%brick).bl_id;
	// Lot owner must correlate to brick owner OR unclaimed lot must correlate to host ID
	if((%ownerID != -1 && %brickOwnerID != %ownerID) || (%ownerID == -1 && %brickOwnerID != getNumKeyID()))
	{
		// Ownership mismatches are normal and can happen under a number of circumstances, including loading an old save or linking lots.
		// Transfer for the owner ID as named in the registry
		CityLots_TransferLot(%brick, %ownerID);
	}

	if(%ownerID != -1)
	{
		// Add the lot to the owner's list, initializing the list with our first value if it's blank.
		$City::Cache::LotsOwnedBy[%ownerID] = $City::Cache::LotsOwnedBy[%ownerID] $= "" ? %brick : $City::Cache::LotsOwnedBy[%ownerID] SPC %brick;
	}
}

function fxDTSBrick::initNewCityLot(%brick)
{
	if(%brick.getDataBlock().CityRPGBrickType != $CityBrick_Lot)
	{
		error("Lot registry - Attempting to initialize non-lot brick '" @ %brick @ "' as a lot! Aborting init.");
		return;
	}

	$City::RealEstate::TotalLots++;
	$City::RealEstate::UnclaimedLots++;

	if(CityLotRegistry.keyExists(%brick.getCityLotID()) != 0)
	{
		warn("Lot registry - Attempting to initialize a lot that already exists. Re-initializing as a new lot.");
		backtrace();
		%brick.cityLotOverride = 1;
		%brick.SetNTObjectNameOverride("");
	}

	%newID = CityLots_GetLotCount()+1;

	CityLotRegistry.addKey(%newID);

	%publicID = getNumKeyID();
	if(%brick.getGroup().bl_id != %publicID)
	{
		CityLots_TransferLot(%brick, %publicID);
	}

	%brick.cityLotOverride = 1;
	%brick.setCityLotSaveName(%publicID, -1, %newID, 0);

	echo("City: Registered new lot, #" @ %newID);

	return %newID;
}

function fxDTSBrick::convertCityLotOwnership(%brick)
{
	%brick.initNewCityLot();

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

	// If the lot's brick name is blank at this stage, for whatever reason, lotID will be -1 due to getWord failing.

	if(CityLotRegistry.existKey[%lotID] == 0)
	{
		// Doesn't exist in the registry
		return -1;
	}

	return %lotID;
}

function fxDTSBrick::destroyCityLot(%brick)
{
	%lotID = %brick.getCityLotID();

	%ownerID = %brick.getCityLotOwnerID();

	// Always override on remove
	%brick.cityLotOverride = 1;
	$City::RealEstate::TotalLots--;

	if(%ownerID != -1)
	{
		// Now, we have to remove this lot from the owner's cache of owned lots.
		%brick.cityLotCacheRemove();
	}
	else
	{
		$City::RealEstate::UnclaimedLots--;
	}

	if(%brick.getCityLotPreownedPrice() != -1)
	{
		$City::RealEstate::LotCountSale--;
	}

	// This lot will exist in the memory, but it will no-longer have a brick associated with it.
	// Therefore, we need to remove the brick from the cache.
	// If the lot is re-loaded later, it will "log in" on init.
	CityLotRegistry.makeOffline(%lotID);

	%brick.lotInitialized = 0;
}

// findLotBrickByID(Lot ID)
// Returns 0 if the brick does not exist.
function findLotBrickByID(%value)
{
	if(CityLotRegistry.data[%value] $= "")
	{
		return 0;
	}

	return CityLotRegistry.data[%value].brick;
}

// findLotLinkedBricksByID(Lot ID)
// Returns a set of fields containing the lot's base brick, and all linked lots, if any
// Returns 0 if the brick does not exist.
function findLotBricksLinkedByID(%value)
{
	if(CityLotRegistry.data[%value] $= "")
	{
		return 0;
	}

	// Init with the base brick.
	%fields = CityLotRegistry.data[%value].brick;

	for(%i = 1; %i <= CityLotRegistry.data[%value].linkedBrickCount; %i++)
	{
		%fields = %fields TAB CityLotRegistry.data[%value].brickLinked[%i];
	}

	return %fields;
}

// ## Getters

function fxDTSBrick::getCityLotName(%brick)
{
	return CityLotRegistry.get(%brick.getCityLotID(), "name");
}

function fxDTSBrick::getCityLotOwnerID(%brick)
{
	return CityLotRegistry.get(%brick.getCityLotID(), "ownerID");
}

function fxDTSBrick::getCityLotRuleStr(%brick)
{
	return CityLotRegistry.get(%brick.getCityLotID(), "ruleStr");
}

function fxDTSBrick::getCityLotTransferDate(%brick)
{
	return CityLotRegistry.get(%brick.getCityLotID(), "transferDate");
}

function fxDTSBrick::getCityLotPreownedPrice(%brick)
{
	return CityLotRegistry.get(%brick.getCityLotID(), "preownedSalePrice");
}

function fxDTSBrick::getCityLotIsLinked(%brick)
{
	return %brick.isLinkedLot;
}

function fxDTSBrick::getCityLotIsBase(%brick)
{
	return %brick.isLinkBase;
}

// ## Setters

function fxDTSBrick::setCityLotName(%brick, %value)
{
	%valueNew = CityLotRegistry.set(%brick.getCityLotID(), "name", getSubStr(%value, 0, 40));
	return %valueNew;
}

function fxDTSBrick::setCityLotOwnerID(%brick, %value)
{
	%lotID = %brick.getCityLotID();
	%valueOld = CityLotRegistry.get(%lotID, "ownerID");

	// ## Display name handling
	if(%valueOld == -1 && %value != -1)
	{
		// If transferring from the city to a player, automatically rename the lot.
		%brick.setCityLotName(%brick.getGroup().name @ "\c6's Lot");
	}

	// ## Caching
	if(%valueOld == -1)
	{
		$City::RealEstate::UnclaimedLots--;
	}
	else if(%valueOld !$= "")
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

	CityLotRegistry.set(%lotID, "ownerID", %value);

	// ## Brick name handling
	// The brick name(s) need to match the new owner ID, so we need to update it.
	%lotBricks = findLotBricksLinkedByID(%brick.getCityLotID());

	for(%i = 0; %i <= getFieldCount(%lotBricks)-1; %i++)
	{
		%targetBrick = getField(%lotBricks, %i);

		%nameRaw = %targetBrick.getCityLotSaveName();
		%lotHost = getWord(%nameRaw, 0);
		%lotID = getWord(%nameRaw, 2);
		%isLinked = getWord(%nameRaw, 3);

		%targetBrick.cityLotOverride = 1;
		%targetBrick.setCityLotSaveName(%lotHost, %value, %lotID, %isLinked);
	}

	return %value;
}

function fxDTSBrick::setCityLotTransferDate(%brick, %value)
{
	CityLotRegistry.set(%brick.getCityLotID(), "transferDate", %value);
}

function fxDTSBrick::setCityLotPreownedPrice(%brick, %value)
{
	%valueOld = CityLotRegistry.get(%brick.getCityLotID(), "preownedSalePrice");

	if(%valueOld != -1 && %value == -1)
	{
		// The value has changed from a number to -1, meaning the lot has gone off sale.
		$City::RealEstate::LotCountSale--;

		CitySO.lotListings = strreplace(CitySO.lotListings, %brick.getCityLotID() @ " ", "");
	}
	else if(%valueOld == -1 && %value != -1)
	{
		// The value has changed from -1 to a number, meaning the lot has been listed for sale.
		$City::RealEstate::LotCountSale++;

		CitySO.lotListings = CitySO.lotListings = CitySO.lotListings @ %brick.getCityLotID() @ " ";
	}

	CityLotRegistry.set(%brick.getCityLotID(), "preownedSalePrice", %value);
}

// ============================================================
// Linking
// ============================================================

function fxDTSBrick::linkCityLot(%brickLinkBase, %brickLinkTarget)
{	
	// Initialize the lots as linked
	// For future proofing, these are called upon using the functions getCityLotIsLinked & getCityLotIsBase
	%brickLinkBase.isLinkBase = true;
	%brickLinkBase.isLinkedLot = true;

	%brickLinkTarget.isLinkedLot = true;

	%name = %brickLinkBase.getCityLotSaveName();

	// Now the fun part. Destroy the link target, and re-initialize it as a clone of the base lot.
	// Linked lot names are tagged as such with the value set from "0" to "1"
	%brickLinkTarget.destroyCityLot();
	%brickLinkTarget.setCityLotSaveNameWhole(setWord(%name, 3, 1));

	// Init the new linked lot as if it is being loaded from a save.
	%brickLinkTarget.initCityLot(true);
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

	function fxDTSBrick::SetNTObjectNameOverride(%obj, %name)
	{
		%obj.cityNameOverride = 1;
		%obj.setNTObjectName(%name);
		%obj.cityNameOverride = 0;
	}

	// Brick rename blocking
	// TODO: See if we can package this only for fxDTSBrick to make it less error prone?
	function SimObject::SetNTObjectName(%obj, %name)
	{
		%override = %obj.cityNameOverride;

		// Special override to handle lots when they are loaded from a save.
		// We're packaging SetNTObjectName because this isn't called until after the loading tick.
		if(%obj.cityLotInit && !%override)
		{
			Parent::SetNTObjectName(%obj, %name, 1);

			%obj.initCityLot();

			return;
		}

		if(!%override && !%obj.cityLotOverrideReset && %obj.dataBlock !$= "" && %obj.dataBlock.CityRPGBrickType == $CityBrick_Lot)
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
		// Check that the brick actually exists, is planted, etc.
		// Also verify that is has a lot ID. If it doesn't, the brick likely never fully initialized.
		// This can happen in certain edge cases, such as while loading bricks that already exist (onRemove is called on the brick after it fails to plant)
		if(%brick.isPlanted && %brick.getDataBlock().CityRPGBrickType == $CityBrick_Lot && %brick.getCityLotID() != -1)
		{
			%brick.destroyCityLot();
		}

		Parent::onRemove(%brick);
	}

	function disconnect(%a)
	{
		CityLotRegistry.delete();
		return parent::disconnect(%a);
	}

	function City_Init()
	{
		CityLots_InitRegistry();
		Parent::City_Init();
	}

	// Saver functions for sanity
	function Saver::keyExists(%this)
	{
		return %this.existKey[%this];
	}
};

deactivatePackage(CityRPG_LotRegistry);
activatepackage(CityRPG_LotRegistry);
