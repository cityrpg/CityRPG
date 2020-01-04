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
// Lots
// ============================================================
datablock fxDTSBrickData(CityRPGSmallLotBrickData : brick16x16FData)
{
	iconName = $City::DataPath @ "/ui/BrickIcons/16x16LotIcon";

	category = "CityRPG";
	subCategory = "Lots";

	uiName = "16x16 Lot";

	CityRPGBrickType = $CityBrick_Lot;
	CityRPGBrickAdmin = false;

	triggerDatablock = CityRPGLotTriggerData;
	triggerSize = "16 16 4800";
	trigger = 0;

	initialPrice = 500;
};

datablock fxDTSBrickData(CityRPGHalfSmallLotBrickData : brick16x32FData)
{
	iconName = $City::DataPath @ "/ui/BrickIcons/16x32LotIcon";

	category = "CityRPG";
	subCategory = "Lots";

	uiName = "16x32 Lot";

	CityRPGBrickType = $CityBrick_Lot;
	CityRPGBrickAdmin = false;

	triggerDatablock = CityRPGLotTriggerData;
	triggerSize = "16 32 4800";
	trigger = 0;

	initialPrice = 750;
};

datablock fxDTSBrickData(CityRPGMediumLotBrickData : brick32x32FData)
{
	iconName = $City::DataPath @ "/ui/BrickIcons/32x32LotIcon";

	category = "CityRPG";
	subCategory = "Lots";

	uiName = "32x32 Lot";

	CityRPGBrickType = $CityBrick_Lot;
	CityRPGBrickAdmin = false;

	triggerDatablock = CityRPGLotTriggerData;
	triggerSize = "32 32 6400";
	trigger = 0;

	initialPrice = 1500;
};

datablock fxDTSBrickData(CityRPGHalfLargeLotBrickData)
{
	brickFile = $City::DataPath @ "/bricks/32x64F.blb";
	iconName = $City::DataPath @ "/ui/BrickIcons/32x64LotIcon";

	category = "CityRPG";
	subCategory = "Lots";

	uiName = "32x64 Lot";

	CityRPGBrickType = $CityBrick_Lot;
	CityRPGBrickAdmin = false;

	triggerDatablock = CityRPGLotTriggerData;
	triggerSize = "32 64 6400";
	trigger = 0;

	initialPrice = 2000;
};

datablock fxDTSBrickData(CityRPGLargeLotBrickData : brick64x64FData)
{
	iconName = $City::DataPath @ "/ui/BrickIcons/64x64LotIcon";

	category = "CityRPG";
	subCategory = "Lots";

	uiName = "64x64 Lot";

	CityRPGBrickType = $CityBrick_Lot;
	CityRPGBrickAdmin = false;

	triggerDatablock = CityRPGLotTriggerData;
	triggerSize = "64 64 12800";
	trigger = 0;

	initialPrice = 4500;
};

// Sale Lots
//datablock fxDTSBrickData(CityRPGSmallZoneBrickData : brick16x16FData)
//{
//	iconName = $City::DataPath @ "/ui/BrickIcons/16x16ZoneIcon";
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
//	iconName = $City::DataPath @ "/ui/BrickIcons/16x32ZoneIcon";
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
//	iconName = $City::DataPath @ "/ui/BrickIcons/32x32ZoneIcon";
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
//	iconName = $City::DataPath @ "/ui/BrickIcons/64x64ZoneIcon";
//
//	category = "CityRPG";
//	subCategory = "CityRPG Zones";
//
//	uiName = "Large Zone";
//
//	CityRPGBrickAdmin = true;
//	CityRPGMatchingLot = CityRPGLargeLotBrickData;
//};

// ============================================================
// Data Bricks
// ============================================================
exec($City::ScriptPath @ "brickScripts/info/bank.cs");
exec($City::ScriptPath @ "brickScripts/info/police.cs");
exec($City::ScriptPath @ "brickScripts/info/bounty.cs");
exec($City::ScriptPath @ "brickScripts/info/labor.cs");
//exec($City::ScriptPath @ "brickScripts/info/realestate.cs");
exec($City::ScriptPath @ "brickScripts/info/criminalbank.cs");
exec($City::ScriptPath @ "brickScripts/info/atm.cs");
exec($City::ScriptPath @ "brickScripts/info/playeratm.cs");
exec($City::ScriptPath @ "brickScripts/info/education.cs");
exec($City::ScriptPath @ "brickScripts/info/job.cs");

exec($City::ScriptPath @ "brickScripts/info/vote.cs");

// ============================================================
// Spawns
// ============================================================
datablock fxDtsBrickData(CityRPGPersonalSpawnBrickData : brickSpawnPointData)
{
	category = "CityRPG";
	subCategory = "Spawns";

	uiName = "Personal Spawn";

	specialBrickType = "";

	CityRPGBrickType = $CityBrick_Spawn;
	CityRPGBrickAdmin = false;

	spawnData = "personalSpawn";
};

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
// Resources
// ============================================================
exec($City::ScriptPath @ "brickScripts/resources/tree.cs");
exec($City::ScriptPath @ "brickScripts/resources/ore.cs");
exec($City::ScriptPath @ "brickScripts/resources/smallore.cs");

// ============================================================
// Other
// ============================================================
datablock fxDTSBrickData(CityRPGPermaSpawnData : brick2x2FData)
{
	category = "CityRPG";
	subCategory = "Info Bricks";

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

// Input Events
function fxDTSBrick::OnEnterLot(%brick, %obj)
{
	$inputTarget_self = %brick;

	$inputTarget_client = %obj.client;
	$inputTarget_player = %obj.client.player;

	$inputTarget_miniGame = (isObject(getMiniGameFromObject(%obj.client))) ? getMiniGameFromObject(%obj.client) : 0;

	%brick.processInputEvent("OnEnterLot", %obj.client);
}

function fxDTSBrick::onLeaveLot(%brick, %obj)
{
	$inputTarget_self = %brick;

	$inputTarget_client = %obj.client;
	$inputTarget_player = %obj.client.player;

	$inputTarget_miniGame = (isObject(getMiniGameFromObject(%obj.client))) ? getMiniGameFromObject(%obj.client) : 0;

	%brick.processInputEvent("OnLeaveLot", %obj.client);
}

function fxDTSBrick::onTransferSuccess(%brick, %client)
{
	$inputTarget_self	= %brick;
	$inputTarget_player	= %client.player;
	$inputTarget_client	= %client;

	%brick.processInputEvent("onTransferSuccess", %client);
}

function fxDTSBrick::onTransferDecline(%brick, %client)
{
	$inputTarget_self	= %brick;
	$inputTarget_client	= %client;

	// Repeated Service Offer Hack
	for(%i = 0; %i < %brick.numEvents; %i++)
	{
		if(%brick.eventInput[%i] $= "onTransferDecline" && (%brick.eventOutput[%i] $= "requestFunds" || %brick.eventOutput[%i] $= "sellItem" || %brick.eventOutput[%i] $= "sellFood"))
		%brick.eventEnabled[%i] = false;
	}

	%brick.processInputEvent("onTransferDecline", %client);
}

function fxDTSBrick::onJobTestPass(%brick, %client)
{
	$inputTarget_self	= %brick;
	$inputTarget_player	= %client.player;
	$inputTarget_client	= %client;

	%brick.processInputEvent("onJobTestPass", %client);
}

function fxDTSBrick::onJobTestFail(%brick, %client)
{
	$inputTarget_self	= %brick;
	$inputTarget_player	= %client.player;
	$inputTarget_client	= %client;

	%brick.processInputEvent("onJobTestFail", %client);
}

// Output Events
function fxDTSBrick::doJobTest(%brick, %job, %job2, %convicts, %client)
{
	%convictStatus = getWord(CityRPGData.getData(%client.bl_id).valueJailData, 1);

	if(!%job && !%job2 && (%convicts ? (!%convictStatus ? true : false) : true))
		%brick.onJobTestPass(%client);
	else if((CityRPGData.getData(%client.bl_id).valueJobID == %job || CityRPGData.getData(%client.bl_id).valueJobID == %job2) && (%convicts ? (!%convictStatus ? true : false) : true))
		%brick.onJobTestPass(%client);
	else
		%brick.onJobTestFail(%client);
}

function fxDTSBrick::requestFunds(%brick, %serviceName, %fund, %client)
{
	if(isObject(%client.player) && !%client.player.serviceOrigin && isObject(%brick))
	{
		%client.player.serviceOrigin = %brick;
		%client.player.serviceFee = %fund;
		%client.player.serviceType = "service";

		messageClient(%client,'',"\c6Service \"\c3" @ %serviceName @ "\c6\" requests \c3$" @ %fund SPC "\c6.");
		messageClient(%client,'',"\c6Accept with \c3/yes\c6, decline with \c3/no\c6.");
	}
	else if(%client.player.serviceOrigin && %client.player.serviceOrigin != %brick)
	{
		messageClient(%client, '', "\c6You already have a charge request from another service! Type \c3/no\c6 to reject it.");
	}
}

function fxDTSBrick::sellFood(%brick, %portion, %food, %markup, %client)
{
	if(isObject(%client.player) && !%client.player.serviceOrigin  && isObject(%brick))
	{
		%client.player.serviceType = "food";
		%client.player.serviceItem = %food;
		%client.player.serviceSize = %portion;
		%client.player.serviceFee = (5 * %portion - mFloor(%portion * 0.75)) +  %markup;
		%client.player.serviceMarkup = %markup;
		%client.player.serviceOrigin = %brick;

		messageClient(%client,'','\c6A service is offering to feed you %1 \c3%2\c6 portion of \c3%3\c6 for \c3$%4\c6.', City_DetectVowel($CityRPG::portion[%portion]), strreplace($CityRPG::portion[%portion], "_", " "), %food, %client.player.serviceFee);
		messageClient(%client,'',"\c6Accept with \c3/yes\c6, decline with \c3/no\c6.");
	}
	else if(%client.player.serviceOrigin && %client.player.serviceOrigin != %brick)
		messageClient(%client, '', "\c6You already have a charge request from another service! Type \c3/no\c6 to reject it.");
}

function fxDTSBrick::sellItem(%brick, %item, %markup, %client)
{
	if(isObject(%client.player) && !%client.player.serviceOrigin  && isObject(%brick))
	{
		%name = $CityRPG::prices::weapon::name[%item].uiName;

		if(CitySO.minerals >= $CityRPG::prices::weapon::mineral[%item])
		{
			%client.player.serviceType = "item";
			%client.player.serviceItem = %item;
			%client.player.serviceFee = $CityRPG::prices::weapon::price[%item] + %markup;
			%client.player.serviceMarkup = %markup;
			%client.player.serviceOrigin = %brick;

			messageClient(%client,'',"\c6A service is offering to sell you one \c3" @ %name SPC "\c6for \c3$" @ %client.player.serviceFee SPC "\c6.");
			messageClient(%client,'',"\c6Accept with \c3/yes\c6, decline with \c3/no\c6.");
		}
		else
			messageClient(%client, '', '\c6A service is trying to offer you %1 \c3%2\c6, but the city needs \c3%3\c6 more minerals to produce it!', City_DetectVowel(%name), %name, ($CityRPG::prices::weapon::mineral[%item] - CitySO.minerals));
	}
	else if(%client.player.serviceOrigin && %client.player.serviceOrigin != %brick)
		messageClient(%client, '', "\c6You already have a charge request from another service! Type \c3/no\c6 to reject it.");
}

function fxDTSBrick::sellClothes(%brick, %item, %markup, %client)
{
	if(isObject(%client.player) && !%client.player.serviceOrigin  && isObject(%brick))
	{
		%client.player.serviceType = "clothes";
		%client.player.serviceItem = %item;
		%client.player.serviceFee = %markup;
		%client.player.serviceMarkup = %markup;
		%client.player.serviceOrigin = %brick;

		messageClient(%client,'', '\c6A clothing service is offering to dress you in %1 \c3%2 \c6for \c3$%3\c6.', City_DetectVowel(ClothesSO.sellName[%item]), ClothesSO.sellName[%item], %client.player.serviceFee);
		messageClient(%client,'', "\c6Accept with \c3/yes\c6, decline with \c3/no\c6.");
	}
	else if(%client.player.serviceOrigin && %client.player.serviceOrigin != %brick)
		messageClient(%client, '', "\c6You already have a charge request from another service! Type \c3/no\c6 to reject it.");
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
	}
}
