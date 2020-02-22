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
	if(!isObject(%client.player))
	{
		return;
	}

	CityMenu_SellItem(%client, %brick, %item, %markup);
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
