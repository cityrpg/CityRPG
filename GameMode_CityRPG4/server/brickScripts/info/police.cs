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

function GameConnection::refreshCityDemeritCosts(%client)
{
	%data = CityRPGData.getData(%client.bl_id);

	%client.dems = %data.valueDemerits;
	%client.demsAffordableTotal = mFloor(%data.valueMoney / $Pref::Server::City::demerits::demeritCost);
	%client.demsAffordable = (%client.demsAffordableTotal > %client.dems ? %client.dems : %client.demsAffordableTotal);
	%client.demCost = mFloor(%client.demsAffordable * $Pref::Server::City::demerits::demeritCost);
}

// ============================================================
// Menu
// ============================================================
function CityMenu_Police(%client, %brick)
{
	%data = CityRPGData.getData(%client.bl_id);

	%menu =	"View active criminals";
	%functions = "CityMenu_Police_ViewCrims";

	%client.cityLog("Enter police");

	// Record clear option
	if(getWord(%data.valueJailData, 0))
	{
		%menu = %menu TAB "Clear your record (\c3$" @ %client.getCityRecordClearCost() @ "\c6)";
		%functions = %functions TAB "CityMenu_Police_ClearRecord";
	}

	// Demerits clear option
	if(CityRPGData.getData(%client.bl_id).valueDemerits)
	{
		%client.refreshCityDemeritCosts();

		if(%client.demsAffordable >= %client.dems)
		{
			%menu = %menu TAB "Pay off Demerits (\c3$" @ %client.demCost @ "\c6)";
		}
		else
		{
			%menu = %menu TAB "Pay Partial Demerits (\c3" @ %client.demsAffordable @ "\c6 out of \c3" @ %client.dems @ "\c6 for \c3$" @ %client.demCost @ "\c6)";
		}

		%functions = %functions TAB "CityMenu_Police_PayDems";
	}

	// Crime evidence option (currently unused in default CityRPG)
	if(CityRPGData.getData(%client.bl_id).valueEvidence)
	{
		%menu = %menu TAB "Turn in evidence";
		%functions = %functions TAB "CityMenu_Police_TurnInEvidence";

	}

	%client.cityMenuOpen(%menu, %functions, %brick, "\c6Thanks, come again.");
}

function CityMenu_Police_ViewCrims(%client)
{
	%noCriminals = true;

	for(%a = 0; %a < clientGroup.getCount(); %a++)
	{
		%criminal = clientGroup.getObject(%a);

		if(CityRPGData.getData(%criminal.bl_id).valueDemerits >= $Pref::Server::City::demerits::wantedLevel)
		{
			%client.cityMenuMessage("\c3" @ %criminal.name SPC "\c6- \c3" @ CityRPGData.getData(%criminal.bl_id).valueDemerits);

			%noCriminals = false;
		}
	}

	if(%noCriminals)
	{
		messageClient(%client, '', "\c6There are no criminals online.");
	}

	%client.cityMenuClose();

	return;
}

function CityMenu_Police_ClearRecord(%client)
{
	serverCmdbuyErase(%client);
	return;
}

function CityMenu_Police_PayDems(%client)
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
	%client.cityMenuClose();
	%client.cityLog("Pay off " @ %client.demsAffordable @ " dems -$" @ %client.demCost);

	return;
}

function CityMenu_Police_TurnInEvidence(%client)
{
	%cash = CityRPGData.getData(%client.bl_id).valueevidence * $CityRPG::evidenceWorth;
	messageClient(%client,'',"\c6You have turned in your \c3Evidence \c6for \c3$" @ CityRPGData.getData(%client.bl_id).valueevidence * $CityRPG::evidenceWorth @ "\c6.");
	CityRPGData.getData(%client.bl_id).valueevidence = 0;
	CityRPGData.getData(%client.bl_id).valueMoney += %cash;
	%client.setInfo();

	%client.cityLog("Turn in evidence for $" @ %cash);
	%client.cityMenuClose();
}

// ============================================================
// Trigger Data
// ============================================================
function CityRPGPoliceBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus !$= "")
	{
		if(%triggerStatus == true && !%client.cityMenuOpen)
		{
			%data = CityRPGData.getData(%client.bl_id);

			%client.cityMenuMessage("\c3" @ $Pref::Server::City::name @ " Police Department");


			if(%data.valueDemerits > 0)
			{
				%client.cityMenuMessage("\c6You have \c3" @ CityRPGData.getData(%client.bl_id).valueDemerits SPC "\c6demerits.");
			}
		}

		CityMenu_Police(%client, %brick);
	}
}
