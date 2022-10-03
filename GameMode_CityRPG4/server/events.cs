// See City_Init_AssembleEvents() in init.cs for registration
// Input Events
function fxDTSBrick::onLotEntered(%brick, %obj)
{
	$inputTarget_self = %brick;

	$inputTarget_client = %obj.client;
	$inputTarget_player = %obj.client.player;

	%brick.processInputEvent("onLotEntered", %obj.client);
}

function fxDTSBrick::onLotLeft(%brick, %obj)
{
	$inputTarget_self = %brick;

	$inputTarget_client = %obj.client;
	$inputTarget_player = %obj.client.player;

	%brick.processInputEvent("onLotLeft", %obj.client);
}

function fxDTSBrick::onLotFirstEntered(%brick, %obj)
{
	$inputTarget_self = %brick;

	$inputTarget_client = %obj.client;
	$inputTarget_player = %obj.client.player;

	%brick.processInputEvent("onLotFirstEntered", %obj.client);
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

function fxDTSBrick::onMenuOpen(%brick, %client)
{
	$inputTarget_self	= %brick;
	$inputTarget_player	= %client.player;
	$inputTarget_client	= %client;

	%brick.processInputEvent("onMenuOpen", %client);
}

function fxDTSBrick::onMenuClose(%brick, %client)
{
	$inputTarget_self	= %brick;
	$inputTarget_player	= %client.player;
	$inputTarget_client	= %client;

	%brick.processInputEvent("onMenuClose", %client);
}

function fxDTSBrick::onMenuInput(%brick, %client)
{
	$inputTarget_self	= %brick;
	$inputTarget_player	= %client.player;
	$inputTarget_client	= %client;

	%brick.processInputEvent("onMenuInput", %client);
}

// Output Events
function fxDTSBrick::doJobTest(%brick, %job, %job2, %convicts, %client)
{
	%convictStatus = getWord(City.get(%client.bl_id, "jaildata"), 1);

	if(!%job && !%job2 && (%convicts ? (!%convictStatus ? true : false) : true))
		%brick.onJobTestPass(%client);
	else if((City.get(%client.bl_id, "jobid") $= %job || City.get(%client.bl_id, "jobid") $= %job2) && (%convicts ? (!%convictStatus ? true : false) : true))
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

		commandToClient(%client, 'MessageBoxYesNo', "Purchase", "Service \"" @ %serviceName @ "\" requests $" @ %fund @ ". Would you like to accept?",'yes');
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

		%portionVowel = City_DetectVowel($CityRPG::portion[%portion]);
		%portion = strreplace($CityRPG::portion[%portion], "_", " ");
		%fee = %client.player.serviceFee;
		%str = "This service is offering " @ %portionVowel SPC %portion @ " portion of " @ %food @ " for $" @ %fee @ ".";
		commandToClient(%client, 'MessageBoxYesNo', "Purchase", %str, 'yes');
	}
	else if(%client.player.serviceOrigin && %client.player.serviceOrigin != %brick)
		messageClient(%client, '', "\c6You already have a charge request from another service! Type \c3/no\c6 to reject it.");
}

function fxDTSBrick::sellItem(%brick, %item, %markup, %client)
{
	if(isObject(%client.player) && !%client.player.serviceOrigin  && isObject(%brick))
	{
		%name = $City::Item::name[%item].uiName;

		if(CitySO.minerals >= $City::Item::mineral[%item])
		{
			%client.player.serviceType = "item";
			%client.player.serviceItem = %item;
			%client.player.serviceFee = $City::Item::price[%item] + %markup;
			%client.player.serviceMarkup = %markup;
			%client.player.serviceOrigin = %brick;

			%str = "This service is offering to sell you one " @ %name SPC "for $" @ %client.player.serviceFee @ ".";
			commandToClient(%client, 'MessageBoxYesNo', "Purchase", %str, 'yes');
		}
		else
			messageClient(%client, '', '\c6A service is trying to offer you %1 \c3%2\c6, but the city needs \c3%3\c6 more minerals to produce it!', City_DetectVowel(%name), %name, ($City::Item::mineral[%item] - CitySO.minerals));
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

		%vowel = City_DetectVowel(ClothesSO.sellName[%item]);
		%clothingName = ClothesSO.sellName[%item];
		%fee = %client.player.serviceFee;
		%str = "This service is offering to dress you in " @ %vowel SPC %clothingName @ " for $" @ %fee @ ".";
		commandToClient(%client, 'MessageBoxYesNo', "Purchase", %str, 'yes');
	}
	else if(%client.player.serviceOrigin && %client.player.serviceOrigin != %brick)
		messageClient(%client, '', "\c6You already have a charge request from another service! Type \c3/no\c6 to reject it.");
}
