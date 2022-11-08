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
		messageClient(%client, '', "\c6You already have a charge request from another service! Type " @ $c_p @ "/no\c6 to reject it.");
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
		messageClient(%client, '', "\c6You already have a charge request from another service! Type " @ $c_p @ "/no\c6 to reject it.");
}

function fxDTSBrick::sellItem(%brick, %item, %markup, %client)
{
	if(isObject(%client.player) && !%client.player.serviceOrigin  && isObject(%brick))
	{
		%name = $City::Item::name[%item].uiName;
		%lotTrigger = %brick.cityLotTriggerCheck();
		if(isObject(%lotTrigger))
			%lotName = %lotTrigger.parent.getCityLotName();
		else
			%lotName = "\c0(Unknown lot)";
		%vendorBLID = %brick.getGroup().bl_id;

		if(CitySO.minerals < $City::Item::mineral[%item])
		{
			messageClient(%client, '', '\c6A service is trying to offer you %2 %1%3\c6, but the city needs %1%4\c6 more minerals to produce it!', $c_p, City_DetectVowel(%name), %name, ($City::Item::mineral[%item] - CitySO.minerals));
			return;
		}

		%sellerLevel = JobSO.job[City.get(%vendorBLID, "jobid")].sellRestrictedItemsLevel;
		%itemLicenseLevel = $City::Item::restrictionLevel[%item];

		if(%sellerLevel < %itemLicenseLevel)
		{
			%vowel = City_DetectVowel(%name);
			if(%sellerLevel == 0 && %itemLicenseLevel == 1)
				messageClient(%client, '', '\c6This vendor cannot sell you %2 %1%3\c6 because they are not licensed to sell weapon-class items.', $c_p, %vowel, %name);
			else //if(%sellerLevel >= 1 && %itemLicenseLevel > 0)
				messageClient(%client, '', '\c6This vendor cannot sell you %2 %1%3\c6 because they are not licensed to sell this type of item.', $c_p, %vowel, %name);

			// Warn the vendor if they are online
			%vendorClient = findClientByBL_ID(%vendorBLID);
			if(isObject(%vendorClient))
			{
				messageClient(%vendorClient, '', '%1%2\c6 tried to buy %3 %1%4\c6 from %1%5\c6, but you are not licensed to sell it.', $c_p, %client.name, %vowel, %name, %lotName);
			}

			return;
		}

		%client.player.serviceType = "item";
		%client.player.serviceItem = %item;
		%client.player.serviceFee = $City::Item::price[%item] + %markup;
		%client.player.serviceMarkup = %markup;
		%client.player.serviceOrigin = %brick;

		%str = "This service is offering to sell you one " @ %name SPC "for $" @ %client.player.serviceFee @ ".";
		commandToClient(%client, 'MessageBoxYesNo', "Purchase", %str, 'yes');
	}
	else if(%client.player.serviceOrigin && %client.player.serviceOrigin != %brick)
		messageClient(%client, '', "\c6You already have a charge request from another service! Type " @ $c_p @ "/no\c6 to reject it.");
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
		messageClient(%client, '', "\c6You already have a charge request from another service! Type " @ $c_p @ "/no\c6 to reject it.");
}
