// ============================================================
// Brick Types
// ============================================================
$CityBrick_Lot = 1;
$CityBrick_Info = 2;
$CityBrick_Spawn = 3;
$CityBrick_ResourceLumber = 4;
$CityBrick_ResourceOre = 5;

// ============================================================
// Error Types
// ============================================================
$Error::Lot::OutOfBounds = -1;
$Error::Lot::Overlap = -2;

// ============================================================
// Handling Script Start
// ============================================================
datablock triggerData(CityRPGLotTriggerData)
{
	tickPeriodMS = 500;
	parent = 0;
};

datablock triggerData(CityRPGInputTriggerData)
{
	tickPeriodMS = 500;
	parent = 0;
};

// ============================================================
// Bricks
// ============================================================
// Player info bricks
exec($City::ScriptPath @ "brickScripts/info/atm.cs");

// Personal spawns
datablock fxDtsBrickData(CityRPGPersonalSpawnBrickData : brickSpawnPointData)
{
	category = "CityRPG";
	subCategory = "Personal";

	uiName = "Personal Spawn";

	specialBrickType = "";

	CityRPGBrickType = $CityBrick_Spawn;
	CityRPGBrickAdmin = false;

	spawnData = "personalSpawn";
};

// Resources
exec($City::ScriptPath @ "brickScripts/resources/tree.cs");
exec($City::ScriptPath @ "brickScripts/resources/ore.cs");
exec($City::ScriptPath @ "brickScripts/resources/smallore.cs");

// City info bricks
exec($City::ScriptPath @ "brickScripts/info/bank.cs");
exec($City::ScriptPath @ "brickScripts/info/police.cs");
exec($City::ScriptPath @ "brickScripts/info/bounty.cs");
exec($City::ScriptPath @ "brickScripts/info/labor.cs");
exec($City::ScriptPath @ "brickScripts/info/realestate.cs");
exec($City::ScriptPath @ "brickScripts/info/criminalbank.cs");
exec($City::ScriptPath @ "brickScripts/info/education.cs");
exec($City::ScriptPath @ "brickScripts/info/job.cs");
exec($City::ScriptPath @ "brickScripts/info/vote.cs");

// ============================================================
// Lots
// ============================================================
datablock fxDTSBrickData(CityRPGSmallLotBrickData : brick16x16FData)
{
	iconName = $City::DataPath @ "ui/BrickIcons/16x16LotIcon";

	category = "Baseplates";
	subCategory = "CityRPG Lots";

	uiName = "16x16 Lot";

	CityRPGBrickType = $CityBrick_Lot;
	CityRPGBrickAdmin = true;

	triggerDatablock = CityRPGLotTriggerData;
	triggerSize = "16 16 4800";
	trigger = 0;
};

datablock fxDTSBrickData(CityRPGHalfSmallLotBrickData : brick16x32FData)
{
	iconName = $City::DataPath @ "ui/BrickIcons/16x32LotIcon";

	category = "Baseplates";
	subCategory = "CityRPG Lots";

	uiName = "16x32 Lot";

	CityRPGBrickType = $CityBrick_Lot;
	CityRPGBrickAdmin = true;

	triggerDatablock = CityRPGLotTriggerData;
	triggerSize = "16 32 4800";
	trigger = 0;
};

datablock fxDTSBrickData(CityRPGMediumLotBrickData : brick32x32FData)
{
	iconName = $City::DataPath @ "ui/BrickIcons/32x32LotIcon";

	category = "Baseplates";
	subCategory = "CityRPG Lots";

	uiName = "32x32 Lot";

	CityRPGBrickType = $CityBrick_Lot;
	CityRPGBrickAdmin = true;

	triggerDatablock = CityRPGLotTriggerData;
	triggerSize = "32 32 6400";
	trigger = 0;
};

datablock fxDTSBrickData(CityRPGHalfLargeLotBrickData)
{
	brickFile = $City::DataPath @ "bricks/32x64F.blb";
	iconName = $City::DataPath @ "ui/BrickIcons/32x64LotIcon";

	category = "Baseplates";
	subCategory = "CityRPG Lots";

	uiName = "32x64 Lot";

	CityRPGBrickType = $CityBrick_Lot;
	CityRPGBrickAdmin = true;

	triggerDatablock = CityRPGLotTriggerData;
	triggerSize = "32 64 6400";
	trigger = 0;
};

datablock fxDTSBrickData(CityRPGLargeLotBrickData : brick64x64FData)
{
	iconName = $City::DataPath @ "ui/BrickIcons/64x64LotIcon";

	category = "Baseplates";
	subCategory = "CityRPG Lots";

	uiName = "64x64 Lot";

	CityRPGBrickType = $CityBrick_Lot;
	CityRPGBrickAdmin = true;

	triggerDatablock = CityRPGLotTriggerData;
	triggerSize = "64 64 12800";
	trigger = 0;
};

// Sale Lots
//datablock fxDTSBrickData(CityRPGSmallZoneBrickData : brick16x16FData)
//{
//	iconName = $City::DataPath @ "ui/BrickIcons/16x16ZoneIcon";
//
//	category = "CityRPG";
//	subCategory = "CityRPG Zones";
//
//	uiName = "Small Zone";
//
//	CityRPGBrickAdmin = true;
//	CityRPGMatchingLot = CityRPGSmallLotBrickData;
//};
//
//datablock fxDTSBrickData(CityRPGHalfSmallZoneBrickData : brick16x32FData)
//{
//	iconName = $City::DataPath @ "ui/BrickIcons/16x32ZoneIcon";
//
//	category = "CityRPG";
//	subCategory = "CityRPG Zones";
//
//	uiName = "Half-Small Zone";
//
//	CityRPGBrickAdmin = true;
//	CityRPGMatchingLot = CityRPGHalfSmallLotBrickData;
//};
//
//datablock fxDTSBrickData(CityRPGMediumZoneBrickData : brick32x32FData)
//{
//	iconName = $City::DataPath @ "ui/BrickIcons/32x32ZoneIcon";
//
//	category = "CityRPG";
//	subCategory = "CityRPG Zones";
//
//	uiName = "Medium Zone";
//
//	CityRPGBrickAdmin = true;
//	CityRPGMatchingLot = CityRPGMediumLotBrickData;
//};
//
//datablock fxDTSBrickData(CityRPGLargeZoneBrickData : brick64x64FData)
//{
//	iconName = $City::DataPath @ "ui/BrickIcons/64x64ZoneIcon";
//
//	category = "CityRPG";
//	subCategory = "CityRPG Zones";
//
//	uiName = "Large Zone";
//
//	CityRPGBrickAdmin = true;
//	CityRPGMatchingLot = CityRPGLargeLotBrickData;
//};

// Jail spawn
datablock fxDtsBrickData(CityRPGJailSpawnBrickData : brickSpawnPointData)
{
	category = "CityRPG";
	subCategory = "Spawns";

	uiName = "Jail Spawn";

	specialBrickType = "";

	CityRPGBrickType = $CityBrick_Spawn;
	CityRPGBrickAdmin = true;

	spawnData = "jailSpawn";
};

// ============================================================
// Other
// ============================================================
datablock fxDTSBrickData(CityRPGPermaSpawnData : brick2x2FData)
{
	category = "CityRPG";
	subCategory = "City Info Bricks";

	uiName = "Item Spawn Brick";

	CityRPGBrickAdmin = true;
	CityRPGPermaspawn = 1;
};

datablock fxDTSBrickData(CityRPGPoliceVehicleData : brickVehicleSpawnData)
{
	category = "CityRPG";
	subCategory = "Spawns";
	uiName = "Police Vehicle Spawn";
	CityRPGBrickAdmin = true;
};

datablock fxDTSBrickData(CityRPGCrimeVehicleData : brickVehicleSpawnData)
{
	category = "CityRPG";
	subCategory = "Spawns";
	uiName = "Crime Vehicle Spawn";
	CityRPGBrickAdmin = true;
};

// ============================================================
// Functions
// ============================================================
function fxDTSBrick::createCityTrigger(%brick, %data)
{
	if(isObject(%brick.trigger))
	{
		return;
	}
	
	%datablock = %brick.getDatablock();

	%trigX = getWord(%datablock.triggerSize, 0);
	%trigY = getWord(%datablock.triggerSize, 1);
	%trigZ = getWord(%datablock.triggerSize, 2);

	if(mFloor(getWord(%brick.rotation, 3)) == 90)
		%scale = (%trigY / 2) SPC (%trigX / 2) SPC (%trigZ / 2);
	else
		%scale = (%trigX / 2) SPC (%trigY / 2) SPC (%trigZ / 2);

	%brick.trigger = new trigger()
	{
		datablock = %datablock.triggerDatablock;
		position = getWords(%brick.getWorldBoxCenter(), 0, 1) SPC getWord(%brick.getWorldBox(), 2) + ((getWord(%datablock.triggerSize, 2) / 4));
		rotation = "1 0 0 0";
		scale = %scale;
		polyhedron = "-0.5 -0.5 -0.5 1 0 0 0 1 0 0 0 1";
		parent = %brick;
	};

	%boxSize = getWord(%scale, 0) / 2.5 SPC getWord(%scale, 1) / 2.5 SPC getWord(%scale, 2) / 2.5;

	if(%brick.getDatablock().CityRPGBrickType == $CityBrick_Lot)
	{
		getBrickGroupFromObject(%brick).lotsOwned++;

		if(isObject(getBrickGroupFromObject(%brick).client))
			getBrickGroupFromObject(%brick).client.SetInfo();
	}
}

function fxDTSBrick::cityBrickInit(%brick)
{
	%client = %brick.getGroup().client;

	if(!%brick.isPlanted || !isObject(%brick))
		return;

	switch(%brick.getDatablock().CityRPGBrickType)
	{
		case $CityBrick_Lot:
			%brick.schedule(1, "createCityTrigger");
		case $CityBrick_Info:
			%brick.schedule(1, "createCityTrigger");
		case $CityBrick_Spawn:
			$CityRPG::temp::spawnPoints = ($CityRPG::temp::spawnPoints $= "") ? %brick : $CityRPG::temp::spawnPoints SPC %brick;
		case $CityBrick_ResourceLumber:
			%seed = getRandom(1, ResourceSO.treeCount);
			%brick.id = ResourceSO.tree[%seed].id;
			%brick.BPH = ResourceSO.tree[%seed].BPH;
			%brick.name = ResourceSO.tree[%seed].name;
			%brick.totalHits = ResourceSO.tree[%seed].totalHits;
			%brick.color = getClosestPaintColor(ResourceSO.tree[%seed].color);
			%brick.setColor(%brick.color);
		case $CityBrick_ResourceOre:
			%seed = getRandom(1, ResourceSO.mineralCount);
			%brick.id = ResourceSO.mineral[%seed].id;
			%brick.BPH = ResourceSO.mineral[%seed].BPH;
			%brick.name = ResourceSO.mineral[%seed].name;
			%brick.totalHits = ResourceSO.mineral[%seed].totalHits;
			%brick.color = getClosestPaintColor(ResourceSO.mineral[%seed].color);
			%brick.setColor(%brick.color);
		default:
			// TODO wtf, move this!
			if($LoadingBricks_Client $= "" && %brick.getDatablock().getID() == brickVehicleSpawnData.getID() && !%client.isCityAdmin())
			{
				commandToClient(%client, 'centerPrint', "\c6You have paid " @ $c_p @ "$" @ mFloor($CityRPG::prices::vehicleSpawn) @ "\c6 to plant this vehicle spawn.", 3);

				City.subtract(%client.bl_id, "money", mFloor($CityRPG::prices::vehicleSpawn));
				%client.setInfo();
			}
	}
}

// Brick::getCityLotTrigger(this/brick)
// Returns the lot trigger containing the brick. Caches the value on %brick.cityLotTrigger.
// If the brick overlaps in multiple lots, the first trigger found is returned.
function fxDTSBrick::cityLotTriggerCheck(%brick)
{
	if(%brick.isPlanted && %brick.cityLotTrigger !$= "")
	{
		return %brick.cityLotTrigger;
	}

	// If not already cached, determine the lot trigger.
	if(mFloor(getWord(%brick.rotation, 3)) == 90)
		%boxSize = getWord(%brick.getDatablock().brickSizeY, 1) / 2.5 SPC getWord(%brick.getDatablock().brickSizeX, 0) / 2.5 SPC getWord(%brick.getDatablock().brickSizeZ, 2) / 2.5;
	else
		%boxSize = getWord(%brick.getDatablock().brickSizeX, 1) / 2.5 SPC getWord(%brick.getDatablock().brickSizeY, 0) / 2.5 SPC getWord(%brick.getDatablock().brickSizeZ, 2) / 2.5;

	initContainerBoxSearch(%brick.getWorldBoxCenter(), %boxSize, $typeMasks::triggerObjectType);

	while(isObject(%trigger = containerSearchNext()))
	{
		if(%trigger.getDatablock() == CityRPGLotTriggerData.getID())
		{
			%brick.cityLotTrigger = %trigger;
			return %trigger;
		}
	}
}

// Brick::getCityBrickUnstable(this/brick, lotTrigger)
function fxDTSBrick::getCityBrickUnstable(%brick, %lotTrigger)
{
	%lotTriggerMinX = getWord(%lotTrigger.getWorldBox(), 0);
	%lotTriggerMinY = getWord(%lotTrigger.getWorldBox(), 1);
	%lotTriggerMinZ = getWord(%lotTrigger.getWorldBox(), 2);

	%lotTriggerMaxX = getWord(%lotTrigger.getWorldBox(), 3);
	%lotTriggerMaxY = getWord(%lotTrigger.getWorldBox(), 4);
	%lotTriggerMaxZ = getWord(%lotTrigger.getWorldBox(), 5);

	%brickMinX = getWord(%brick.getWorldBox(), 0) + 0.0016;
	%brickMinY = getWord(%brick.getWorldBox(), 1) + 0.0013;
	%brickMinZ = getWord(%brick.getWorldBox(), 2) + 0.00126;

	%brickMaxX = getWord(%brick.getWorldBox(), 3) - 0.0016;
	%brickMaxY = getWord(%brick.getWorldBox(), 4) - 0.0013;
	%brickMaxZ = getWord(%brick.getWorldBox(), 5) - 0.00126;

	if(%brickMinX < %lotTriggerMinX || %brickMinY < %lotTriggerMinY || %brickMinZ < %lotTriggerMinZ || %brickMaxX > %lotTriggerMaxX || %brickMaxY > %lotTriggerMaxY || %brickMaxZ > %lotTriggerMaxZ)
	{
		return 1;
	}

	return 0;
}

// Brick::cityBrickCheck(this/brick)
// Checks if the current brick can be planted by the client that owns it.
// Typically called on a client's temp brick, except when using the duplicator.
// Displays an error and returns -1 if there are any problems.
function fxDTSBrick::cityBrickCheck(%brick)
{
	%client = %brick.getGroup().client;

	if(!isObject(%client))
	{
		// City brick checks are never called while loading.
		// Therefore, if the client doesn't exist, something went wrong. Refuse to plant for security.
		return 0;
	}

	// Set %brickType and check it.
	%brickType = %brick.getDataBlock().CityRPGBrickType;

	if(%brickType !$= "" && isObject(%brick.client))
	{
		// Log if it's a CityRPG brick
		%brick.client.cityLog("Attempt to plant " @ %brick.getDatablock().getName());
	}

	if(%client.isCityAdmin())
	{
		return 1;
	}

	%brickData = %brick.getDatablock();

	if(%brickData.CityRPGBrickType == $CityBrick_Lot)
	{
		commandToClient(%client, 'centerPrint', "\c6You cannot place new lot bricks.<br>\c6To purchase a lot, find an unclaimed lot and type /lot while standing on it.", 5);
		return 0;
	}

	if(%brickData.CityRPGBrickAdmin)
	{
		commandToClient(%client, 'centerPrint', "\c6You cannot place this type of brick.", 3);
		return 0;
	}

	// Lot zone check
	%lotTrigger = %brick.cityLotTriggerCheck();

	if(!%lotTrigger && %brickData.CityRPGBrickType != $CityBrick_Lot)
	{
		commandToClient(%client, 'centerPrint', "You cannot plant a brick outside of a lot.\n\c6Use a lot brick to start your build!", 3);
		return 0;
	}

	%price = $Pref::Server::City::lotCost[%brick.getDatablock().getName()];

	if(City.get(%client.bl_id, "money") < mFloor(price))
	{
		commandToClient(%client, 'centerPrint', "\c6You need at least \c3$" @ %price SPC "\c6in order to plant this brick!", 3);
		return 0;
	}

	if(%brick.getDatablock().CityRPGBrickType != $CityBrick_Lot && %brick.getCityBrickUnstable(%lotTrigger))
	{
		commandToClient(%client, 'ServerMessage', 'MsgPlantError_Unstable');
		return 0;
	}

	if(%lotTrigger && %brickData.getID() == brickVehicleSpawnData.getID() && City.get(%client.bl_id, "money") < mFloor($CityRPG::prices::vehicleSpawn))
	{
		commandToClient(%client, 'centerPrint', "\c6You need at least " @ $c_p @ "$" @ mFloor($CityRPG::prices::vehicleSpawn) SPC "\c6in order to plant this vehicle spawn!", 3);
		return 0;
	}

	if(%brick.getDatablock().CityRPGBrickType && isObject(%brick.client)) {
		return 1;
		%brick.client.cityLog("---- Passed CityRPG checks", 1);
	}
}

function fxDTSBrick::onCityBrickRemove(%brick, %data)
{
	if(!isObject(%brick.trigger))
	{
		return;
	}
	
	for(%a = 0; %a < clientGroup.getCount(); %a++)
	{
		%subClient = ClientGroup.getObject(%a);
		if(isObject(%subClient.player) && %subClient.CityRPGTrigger == %brick.trigger)
			%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, clientGroup.getObject(%a).player, true);
	}

	%boxSize = getWord(%brick.trigger.scale, 0) / 2.5 SPC getWord(%brick.trigger.scale, 1) / 2.5 SPC getWord(%brick.trigger.scale, 2) / 2.5;

	initContainerBoxSearch(%brick.trigger.getWorldBoxCenter(), %boxSize, $typeMasks::playerObjectType);
	while(isObject(%player = containerSearchNext()))
		%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, %player);
	%brick.trigger.delete();
}

// ============================================================
// Trigger Functions
// ============================================================
function CityRPGLotTriggerData::onEnterTrigger(%this, %trigger, %obj)
{
	parent::onEnterTrigger(%this, %trigger, %obj);

	%lotID = %trigger.parent.getCityLotID();

	if(!isObject(%obj.client))
	{
		if(isObject(%obj.getControllingClient()))
			%client = %obj.getControllingClient();
		else
			return;
	}
	else
		%client = %obj.client;

	%trigger.parent.onLotEntered(%obj);

	%client.CityRPGTrigger = %trigger;
	%client.CityLotBrick = %trigger.parent;

	%client.cityLotDisplay(%trigger.parent);

	// Realtime tracking of lot occupants - Add to the index.
	%trigger.parent.lotOccupants = %trigger.parent.lotOccupants $= "" ? %client TAB "" : %trigger.parent.lotOccupants @ %client TAB "";

	// Lot visit tracking
	%lotsVisited = City.get(%client.bl_id, "lotsVisited");
	%visited = 0;

	if(%visited !$= "")
	{
		// Loop through the lots this player has visited.
		for(%i = 0; %i <= getWordCount(%lotsVisited); %i++)
		{
			%visited = %lotID == getWord(%lotsVisited, %i);

			if(%visited)
			{
				// We've found it -- search is done.
				break;
			}
		}
	}

	// This is the player's first visit. Record the visit to this lot
	if(!%visited)
	{
		// Trigger the event
		%trigger.parent.onLotFirstEntered(%obj);
		
		// Initialize if blank
		if(%lotsVisited == -1)
		{
			City.set(%client.bl_id, "lotsVisited", %lotID);
		}
		else
		{
			// Push to the beginning, listing the lots in reverse order of when first visited.
			City.set(%client.bl_id, "lotsVisited", %lotID SPC %lotsVisited);
		}
	}
}

function CityRPGLotTriggerData::onLeaveTrigger(%this, %trigger, %obj)
{
	if(!isObject(%obj.client))
	{
		if(isObject(%obj.getControllingClient()))
			%client = %obj.getControllingClient();
		else
			return;
	}
	else
		%client = %obj.client;

	%client.cityMenuClose();
	%trigger.parent.onLotLeft(%obj);

	if(%trigger.parent!=%client.CityLotBrick)
		return;

	// Realtime tracking of lot occupants - Remove from the index.
	%trigger.parent.lotOccupants = strreplace(%trigger.parent.lotOccupants, %client TAB "", "");

	%client.CityRPGTrigger = "";
	%client.CityLotBrick = "";

	//%client.SetInfo();
}

function CityRPGInputTriggerData::onEnterTrigger(%this, %trigger, %obj)
{
	if(!isObject(%obj.client))
	{
		return;
	}

	if(%obj.client.cityMenuOpen)
	{
		%obj.client.cityMenuClose();
	}

	%obj.client.cityLog(%trigger.parent.getDatablock().getName() SPC "enter");

	%obj.client.CityRPGTrigger = %trigger;
	%trigger.parent.getDatablock().parseData(%trigger.parent, %obj.client, true, "");
}

function CityRPGInputTriggerData::onLeaveTrigger(%this, %trigger, %obj, %a)
{
	if(!isObject(%obj.client))
	{
		return;
	}

	%obj.client.cityLog(%trigger.parent.getDatablock().getName() SPC "leave");

	if(%obj.client.CityRPGTrigger == %trigger)
	{
		%trigger.parent.getDatablock().parseData(%trigger.parent, %obj.client, false, "");
		%obj.client.CityRPGTrigger = "";

		if(%obj.client.cityMenuID == %trigger.parent.getID() || %obj.client.cityMenuBack == %trigger.parent.getID())
		{
			%obj.client.cityMenuClose();
		}
	}
}
