// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGBountyBrickData : brick2x4FData)
{
	category = "CityRPG";
	subCategory = "City Info Bricks";

	uiName = "Bounty Brick";

	CityRPGBrickType = $CityBrick_Info;
	CityRPGBrickAdmin = true;
	CityRPGBrickCost = 1000;

	triggerDatablock = CityRPGInputTriggerData;
	triggerSize = "2 4 1";
	trigger = 0;
};

// ============================================================
// Menu
// ============================================================
function CityMenu_Bounty(%client, %brick)
{
	%client.cityMenuMessage("\c3Hit Office");
	%client.cityMenuMessage("\c0Note:\c6 Placing a bounty (as a non-official) is criminal activity.");

	%menu =	"View bounties."
			TAB "Place a bounty.";

	%functions = 	"CityMenu_Bounty_List"
						TAB "CityMenu_Bounty_PlacePromptA";

	%client.cityMenuOpen(%menu, %functions, %brick, "\c6Thanks, come again.");
}

function CityMenu_Bounty_List(%client, %brick)
{
	%noCriminals = true;

	for(%a = 0; %a < clientGroup.getCount(); %a++)
	{
		%criminal = clientGroup.getObject(%a);

		if(CityRPGData.getData(%criminal.bl_id).valueBounty > 0)
		{
			messageClient(%client, '', "\c3" @ %criminal.name SPC "\c6- \c3$" @ CityRPGData.getData(%criminal.bl_id).valueBounty);

			%noCriminals = false;
		}
	}

	if(%noCriminals)
	{
		%client.cityMenuMessage("\c6There are no wanted people online.");
	}

	%client.cityMenuClose();
}

function CityMenu_Bounty_PlacePromptA(%client, %input)
{
	%client.cityMenuMessage("\c6Who do you want to put a hit on? (ID or Name)");

	// Trigger the next menu.
	%client.cityMenuFunction = "CityMenu_Bounty_PlacePromptB";
}

function CityMenu_Bounty_PlacePromptB(%client, %input)
{
	if(!findClientByName(%input) && !findClientByBL_ID(mFloor(%input)))
	{
		%client.cityMenuMessage("\c6Please enter a valid name or ID of the person you want killed.");
	}
	else if(findClientByName(%input) || findClientByBL_ID(mFloor(%input)))
	{
		%hunted = (findClientByName(%input) ? findClientByName(%input) : findClientByBL_ID(mFloor(%input)));

		if(%hunted != %client)
		{
			%client.cityMenuMessage("\c6Alright, so you want a hit on \c3" @ %hunted.name @ "\c6.");
			%client.cityMenuMessage("\c6How much are you wanting to place?");

			%client.stage["hunted"] = %hunted;

			// Trigger the next menu.
			%client.cityMenuFunction = "CityMenu_Bounty_PlacePromptC";
		}
		else
		{
			%client.cityMenuMessage("\c6What? Do you have a death wish?\n\c6You cant place a bounty on yourself.");
			%client.cityMenuClose();
		}
	}
	return;
}

function CityMenu_Bounty_PlacePromptC(%client, %input)
{
	if(mFloor(%input) < 1)
	{
		%client.cityMenuMessage("\c6Please enter a valid amount of money to place on the victim.");

		return;
	}

	if(CityRPGData.getData(%client.bl_id).valueMoney - mFloor(%input) < 0)
	{
		if(CityRPGData.getData(%client.bl_id).valueMoney < 1)
		{
			%client.cityMenuMessage("\c6You don't have that much money to place.");

			%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, (isObject(%client.player) ? %client.player : 0));

			return;
		}

		%input = CityRPGData.getData(%client.bl_id).valueMoney;
	}

	if(mFloor(%input) >= $Pref::Server::City::demerits::minBounty)
	{
		if(mFloor(%input) <= $Pref::Server::City::demerits::maxBounty)
		{
			%bounty = mFloor(%input);

			%client.cityLog("Place bounty $" @ %bounty @ " on " @ %client.stage["hunted"].bl_id);

			messageAll('', "\c3" @ %client.name @ "\c6 has placed \c3$" @ %bounty @ "\c6 on \c3" @ %client.stage["hunted"].name @"\c6's head!");
			if(!%client.getJobSO().bountyOffer)
			{
				commandToClient(%client, 'centerPrint', "\c6You have committed a crime. [\c3Placing an Illegal Hit\c6]", 1);
				City_AddDemerits(%client.bl_id, $CityRPG::demerits::bountyPlacing);
			}

			%client.cityMenuClose();

			CityRPGData.getData(%client.stage["hunted"].bl_id).valueBounty += %bounty;
			CityRPGData.getData(%client.bl_id).valueMoney -= mFloor(%input);

			%client.SetInfo();
		}
		else
		{
			%client.cityMenuMessage("\c6That's too big of a bounty.");
		}
	}
	else
	{
		%client.cityMenuMessage("\c6Sorry Pal, we don't accept chump-change.\n\c6You need at least \c3$" @ $Pref::Server::City::demerits::minBounty @ "\c6 to place a bounty.");
	}
}

// ============================================================
// Trigger Data
// ============================================================
function CityRPGBountyBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus == true && !%client.cityMenuOpen)
	{
		CityMenu_Bounty(%client, %brick);
	}
}
