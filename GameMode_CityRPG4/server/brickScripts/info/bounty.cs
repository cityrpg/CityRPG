// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGBountyBrickData : brick2x4FData)
{
	category = "CityRPG";
	subCategory = "Info Bricks";

	uiName = "Bounty Brick";

	CityRPGBrickType = $CityBrick_Info;
	CityRPGBrickAdmin = true;
	CityRPGBrickPlayerPrivliage = true;
	CityRPGBrickCost = 1000;

	triggerDatablock = CityRPGInputTriggerData;
	triggerSize = "2 4 1";
	trigger = 0;
};

// ============================================================
// Trigger Data
// ============================================================
function CityRPGBountyBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus !$= "")
	{
		if(%triggerStatus == true && %client.stage $= "")
		{
			messageClient(%client, '', "\c3Hit Office");
			messageClient(%client, '', "\c6Type a number in chat:");

			messageClient(%client, '', "\c0Note:\c6 Placing a bounty (as a non-official) is criminal activity.");
			messageClient(%client, '', "\c31 \c6- View bounties.");
			messageClient(%client, '', "\c32 \c6- Place a bounty.");

			%client.stage = 0;
		}

		if(%triggerStatus == false && %client.stage !$= "")
		{
			messageClient(%client, '', "\c6Thanks, come again.");

			%client.stage = "";
		}

		return;
	}

	%input = strLwr(%text);

	if(mFloor(%client.stage) == 0)
	{
		if(strReplace(%input, "1", "") !$= %input || strReplace(%input, "one", "") !$= %input)
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
				messageClient(%client, '', "\c6There are no wanted people online.");
			}

			%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, (isObject(%client.player) ? %client.player : 0));

			return;
		}

		if(strReplace(%input, "2", "") !$= %input || strReplace(%input, "two", "") !$= %input)
		{
			%client.stage = 1.1;

			messageClient(%client, '', "\c6Who do you want to put a hit on? (ID or Name)");

			return;
		}

		messageClient(%client, '', "\c3" @ %text SPC "\c6is not a valid option!");

		%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, (isObject(%client.player) ? %client.player : 0));

		return;
	}

	if(mFloor(%client.stage) == 1)
	{
		if(%client.stage == 1.1)
		{
			if(!findClientByName(%input) && !findClientByBL_ID(mFloor(%input)))
			{
				messageClient(%client, '', "\c6Please enter a valid name or ID of the person you want killed.");
			}
			else if(findClientByName(%input) || findClientByBL_ID(mFloor(%input)))
			{
				%hunted = (findClientByName(%input) ? findClientByName(%input) : findClientByBL_ID(mFloor(%input)));

				if(%hunted != %client)
				{
					messageClient(%client, '', "\c6Alright, so you want a hit on \c3" @ %hunted.name @ "\c6.");
					messageClient(%client, '', "\c6How much are you wanting to place?");

					%client.stage = 1.2;
					%client.stage["hunted"] = %hunted;
				}
				else
				{
					messageClient(%client, '', "\c6What? Do you have a death wish?\n\c6You cant place a bounty on yourself.");

					%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, (isObject(%client.player) ? %client.player : 0));
				}
			}
			return;
		}

		if(%client.stage == 1.2)
		{
			if(mFloor(%input) < 1)
			{
				messageClient(%client, '', "\c6Please enter a valid amount of money to place on the victim.");

				return;
			}

			if(CityRPGData.getData(%client.bl_id).valueMoney - mFloor(%input) < 0)
			{
				if(CityRPGData.getData(%client.bl_id).valueMoney < 1)
				{
					messageClient(%client, '', "\c6You don't have that much money to place.");

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

					%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, (isObject(%client.player) ? %client.player : 0));

					CityRPGData.getData(%client.stage["hunted"].bl_id).valueBounty += %bounty;
					CityRPGData.getData(%client.bl_id).valueMoney -= mFloor(%input);

					%client.SetInfo();
				}
				else
				{
					messageClient(%client,'',"\c6That's too big of a bounty.");
				}
			}
			else
			{
				messageClient(%client, '', "\c6Sorry Pal, we don't accept chump-change.\n\c6You need at least \c3$" @ $Pref::Server::City::demerits::minBounty @ "\c6 to place a bounty.");
			}
		}

		return;
	}
}
