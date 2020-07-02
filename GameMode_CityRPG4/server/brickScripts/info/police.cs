// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGPoliceBrickData : brick2x4FData)
{
	category = "CityRPG";
	subCategory = "City Info Bricks";

	uiName = "Police Brick";

	CityRPGBrickType = $CityBrick_Info;
	CityRPGBrickAdmin = true;

	triggerDatablock = CityRPGInputTriggerData;
	triggerSize = "2 4 1";
	trigger = 0;
};

// ============================================================
// Trigger Data
// ============================================================
function GameConnection::refreshCityDemeritCosts(%client)
{
	%data = CityRPGData.getData(%client.bl_id);

	%client.dems = %data.valueDemerits;
	%client.demsAffordableTotal = mFloor(%data.valueMoney / $Pref::Server::City::demerits::demeritCost);
	%client.demsAffordable = (%client.demsAffordableTotal > %client.dems ? %client.dems : %client.demsAffordableTotal);
	%client.demCost = mFloor(%client.demsAffordable * $Pref::Server::City::demerits::demeritCost);
}

function CityRPGPoliceBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus !$= "")
	{
		if(%triggerStatus == true)
		{
			%client.refreshCityDemeritCosts();

			messageClient(%client, '', "\c3" @ $Pref::Server::City::name @ " Police Department");
			messageClient(%client, '', "\c6Type a number in chat:");

			if(CityRPGData.getData(%client.bl_id).valueDemerits > 0)
			{
				messageClient(%client, '', "\c6You have \c3" @ %dems SPC "\c6demerits.");
			}

			messageClient(%client, '', "\c31 \c6- View Online Criminals");

			%cost = %client.getCityRecordClearCost();
			messageClient(%client, '', "\c32 \c6- Clear your record! (\c3$" @ %cost @ "\c6)");

			if(CityRPGData.getData(%client.bl_id).valueDemerits)
			{
				if(%demsYouCanBuy >= %yourDemerits)
				{
					messageClient(%client, '', "\c33 \c6- Pay off Demerits (\c3$" @ %client.demCost @ "\c6)");
				}
				else
				{
					messageClient(%client, '', "\c33 \c6- Pay Partial Demerits (\c3" @ %client.demsAffordable @ "\c6 out of \c3" @ %client.dems @ "\c6 for \c3$" @ %client.demCost @ "\c6)");
				}
			}
			if(CityRPGData.getData(%client.bl_id).valueevidence)
			{
				if(CityRPGData.getData(%client.bl_id).valueDemerits)
				{
					messageClient(%client,'',"\c34 \c6- Turn in evidence");
				}
				else
				{
					messageClient(%client,'',"\c33 \c6- Turn in evidence");
				}
			}
		}

		if(%triggerStatus == false)
		{
			messageClient(%client, '', "\c6Thanks, come again.");
		}

		return;
	}

	%input = strLwr(%text);

	if(strReplace(%input, "1", "") !$= %input || strReplace(%input, "one", "") !$= %input)
	{
		%noCriminals = true;

		for(%a = 0; %a < clientGroup.getCount(); %a++)
		{
			%criminal = clientGroup.getObject(%a);

			if(CityRPGData.getData(%criminal.bl_id).valueDemerits >= $Pref::Server::City::demerits::wantedLevel)
			{
				messageClient(%client, '', "\c3" @ %criminal.name SPC "\c6- \c3" @ CityRPGData.getData(%criminal.bl_id).valueDemerits);

				%noCriminals = false;
			}
		}

		if(%noCriminals)
		{
			messageClient(%client, '', "\c6There are no criminals online.");
		}

		%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, (isObject(%client.player) ? %client.player : 0));

		return;
	}

	if(strReplace(%input, "2", "") !$= %input || strReplace(%input, "two", "") !$= %input)
	{
		serverCmdbuyErase(%client);
		return;
	}

	if((strReplace(%input, "3", "") !$= %input || strReplace(%input, "three", "") !$= %input) && CityRPGData.getData(%client.bl_id).valueDemerits > 0)
	{
		%client.refreshCityDemeritCosts();

		if(%client.demsAffordable <= 0)
		{
			messageClient(%client, '', "\c6You cant afford to pay off any demerits!");

			%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, (isObject(%client.player) ? %client.player : 0));

			return;
		}

		if(CityRPGData.getData(%client.bl_id).valueMoney - %demCost < 0)
		{
			messageClient(%client, '', "\c6You don't have enough money to do that.");

			return;
		}

		CityRPGData.getData(%client.bl_id).valueMoney -= %client.demCost;
		CityRPGData.getData(%client.bl_id).valueDemerits -= %client.demsAffordable;

		messageClient(%client, '', "\c6You have paid \c3$" @ %client.demCost @ "\c6. You now have\c3" SPC (CityRPGData.getData(%client.bl_id).valueDemerits ? CityRPGData.getData(%client.bl_id).valueDemerits : "no") SPC "\c6demerits.");

		%client.setInfo();

		%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, (isObject(%client.player) ? %client.player : 0));

		return;
	}
	else if((strReplace(%input, "3", "") !$= %input || strReplace(%input, "three", "") !$= %input) && CityRPGData.getData(%client.bl_id).valueDemerits == 0)
	{
		// Crime evidence
		%cash = CityRPGData.getData(%client.bl_id).valueevidence * $CityRPG::evidenceWorth;
		messageClient(%client,'',"\c6You have turned in your \c3Evidence \c6for \c3$" @ CityRPGData.getData(%client.bl_id).valueevidence * $CityRPG::evidenceWorth @ "\c6.");
		CityRPGData.getData(%client.bl_id).valueevidence = 0;
		CityRPGData.getData(%client.bl_id).valueMoney += %cash;
		%client.setInfo();

		%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, (isObject(%client.player) ? %client.player : 0));

		%client.cityLog("Turn in evidence for $" @ %cash);
	}

	else if((strReplace(%input, "4", "") !$= %input || strReplace(%input, "four", "") !$= %input) && CityRPGData.getData(%client.bl_id).valueDemerits > 0)
	{
		%cash = CityRPGData.getData(%client.bl_id).valueevidence * $CityRPG::evidenceWorth;
		messageClient(%client,'',"\c6You have turned in your \c3Evidence \c6for \c3$" @ CityRPGData.getData(%client.bl_id).valueevidence * $CityRPG::evidenceWorth @ "\c6.");
		CityRPGData.getData(%client.bl_id).valueevidence = 0;
		CityRPGData.getData(%client.bl_id).valueMoney += %cash;
		%client.setInfo();

		%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, (isObject(%client.player) ? %client.player : 0));

		%client.cityLog("Turn in evidence for $" @ %cash);
	}
	else
	{

	messageClient(%client, '', "\c3" @ %text SPC "\c6is not a valid option!");

	return;
	}
}
