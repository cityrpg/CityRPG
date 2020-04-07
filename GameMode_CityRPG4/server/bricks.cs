// ============================================================
// Brick Types
// ============================================================
$CityBrick_Lot = 1;
$CityBrick_Info = 2;
$CityBrick_Spawn = 3;
$CityBrick_ResourceLumber = 4;
$CityBrick_ResourceOre = 5;

// ============================================================
// Handling Script Start
// ============================================================
$CityRPG::temp::brickError = forceRequiredAddOn("player_no_jet");

if($CityRPG::temp::brickError)
{
	if($CityRPG::temp::brickError == $error::addOn_disabled)
		playerNoJet.uiName = "";

	if($CityRPG::temp::brickError == $error::addOn_notFound)
		return;
}

$CityRPG::loadedDatablocks = true;

if(!$CityRPG::loadedDatablocks)
{
	return;
}

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
exec($City::ScriptPath @ "brickScripts/info/playeratm.cs");
exec($City::ScriptPath @ "brickScripts/info/education.cs");
exec($City::ScriptPath @ "brickScripts/info/job.cs");
exec($City::ScriptPath @ "brickScripts/info/vote.cs");

// ============================================================
// Lots
// ============================================================
datablock fxDTSBrickData(CityRPGSmallLotBrickData : brick16x16FData)
{
	iconName = $City::DataPath @ "ui/BrickIcons/16x16LotIcon";

	category = "CityRPG";
	subCategory = "Lots";

	uiName = "16x16 Lot";

	CityRPGBrickType = $CityBrick_Lot;
	CityRPGBrickAdmin = true;

	triggerDatablock = CityRPGLotTriggerData;
	triggerSize = "16 16 4800";
	trigger = 0;

	initialPrice = 500;
};

datablock fxDTSBrickData(CityRPGHalfSmallLotBrickData : brick16x32FData)
{
	iconName = $City::DataPath @ "ui/BrickIcons/16x32LotIcon";

	category = "CityRPG";
	subCategory = "Lots";

	uiName = "16x32 Lot";

	CityRPGBrickType = $CityBrick_Lot;
	CityRPGBrickAdmin = true;

	triggerDatablock = CityRPGLotTriggerData;
	triggerSize = "16 32 4800";
	trigger = 0;

	initialPrice = 750;
};

datablock fxDTSBrickData(CityRPGMediumLotBrickData : brick32x32FData)
{
	iconName = $City::DataPath @ "ui/BrickIcons/32x32LotIcon";

	category = "CityRPG";
	subCategory = "Lots";

	uiName = "32x32 Lot";

	CityRPGBrickType = $CityBrick_Lot;
	CityRPGBrickAdmin = true;

	triggerDatablock = CityRPGLotTriggerData;
	triggerSize = "32 32 6400";
	trigger = 0;

	initialPrice = 1500;
};

datablock fxDTSBrickData(CityRPGHalfLargeLotBrickData)
{
	brickFile = $City::DataPath @ "bricks/32x64F.blb";
	iconName = $City::DataPath @ "ui/BrickIcons/32x64LotIcon";

	category = "CityRPG";
	subCategory = "Lots";

	uiName = "32x64 Lot";

	CityRPGBrickType = $CityBrick_Lot;
	CityRPGBrickAdmin = true;

	triggerDatablock = CityRPGLotTriggerData;
	triggerSize = "32 64 6400";
	trigger = 0;

	initialPrice = 2000;
};

datablock fxDTSBrickData(CityRPGLargeLotBrickData : brick64x64FData)
{
	iconName = $City::DataPath @ "ui/BrickIcons/64x64LotIcon";

	category = "CityRPG";
	subCategory = "Lots";

	uiName = "64x64 Lot";

	CityRPGBrickType = $CityBrick_Lot;
	CityRPGBrickAdmin = true;

	triggerDatablock = CityRPGLotTriggerData;
	triggerSize = "64 64 12800";
	trigger = 0;

	initialPrice = 4500;
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
	if(!isObject(%brick.trigger))
	{
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
			position = getWords(%brick.getWorldBoxCenter(), 0, 1) SPC getWord(%brick.getWorldBoxCenter(), 2) + ((getWord(%datablock.triggerSize, 2) / 4) + (%datablock.brickSizeZ * 0.1));
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
}

function fxDTSBrick::handleCityRPGBrickDelete(%brick, %data)
{
	if(isObject(%brick.trigger))
	{
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
}

// ============================================================
// Trigger Functions
// ============================================================
function CityRPGLotTriggerData::onEnterTrigger(%this, %trigger, %obj)
{
	parent::onEnterTrigger(%this, %trigger, %obj);

	if(!isObject(%obj.client))
	{
		if(isObject(%obj.getControllingClient()))
		%client = %obj.getControllingClient();
		else
			return;
	}
	else
		%client = %obj.client;

	%trigger.parent.onEnterLot(%obj);

	%client.CityRPGTrigger = %trigger;
	%client.CityRPGLotBrick = %trigger.parent;

	%lotStr = "<just:right><font:palatino linotype:24>\c6" @ %trigger.parent.getCityLotName();

	if(%trigger.parent.getCityLotOwnerID() == -1)
	{
		%lotStr = %lotStr @ "<br>\c3For sale! \c6Type /lot for more info";
	}

	%client.centerPrint(%lotStr, 2);

	//%client.SetInfo();
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
	%trigger.parent.onLeaveLot(%obj);

	if(%trigger.parent!=%client.CityRPGLotBrick)
		return;

	%client.CityRPGTrigger = "";
	%client.CityRPGLotBrick = "";

	//%client.SetInfo();
}

function CityRPGInputTriggerData::onEnterTrigger(%this, %trigger, %obj)
{
	if(!isObject(%obj.client))
	{
		return;
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
		%obj.client.cityMenuClose();
	}
}
