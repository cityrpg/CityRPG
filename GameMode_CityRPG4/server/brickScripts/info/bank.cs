// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGBankBrickData : brick2x4FData)
{
	category = "CityRPG";
	subCategory = "Info Bricks";

	uiName = "Bank Brick";

	CityRPGBrickType = $CityBrick_Info;
	CityRPGBrickAdmin = true;

	triggerDatablock = CityRPGInputTriggerData;
	triggerSize = "2 4 1";
	trigger = 0;
};

// ============================================================
// Trigger Data
// ============================================================
function CityRPGBankBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%client.getWantedLevel())
	{
		messageClient(%client, '', "\c6The service refuses to serve you.");
		return;
	}

	if(%triggerStatus !$= "")
	{
		if(%triggerStatus == true && %client.stage $= "")
		{
			if(CityRPGData.getData(%client.bl_id).valueBank > 0)
			{
				messageClient(%client, '', "\c6Welcome to the " @ $Pref::Server::City::name @ " Bank. Your account balance is \c3$" @ CityRPGData.getData(%client.bl_id).valueBank @ "\c6. Current economy value: \c3" @ %econColor @ $Economics::Condition @ "\c6%");
			}

			messageClient(%client, '', "\c6Type a number in chat:");

			messageClient(%client, '', "\c31 \c6- Withdraw money.");
			messageClient(%client, '', "\c32 \c6- Deposit money.");
			messageClient(%client, '', "\c33 \c6- Deposit all money.");
			messageClient(%client, '', "\c34 \c6- Donate to the economy.");

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
			%client.stage = 1.1;

			messageClient(%client, '', "\c6Please enter the amount of money you wish to withdraw.");

			return;
		}

		if(strReplace(%input, "2", "") !$= %input || strReplace(%input, "two", "") !$= %input)
		{
			%client.stage = 1.2;

			messageClient(%client, '', "\c6Please enter the amount of money you wish to deposit.");

			return;
		}

		if(strReplace(%input, "3", "") !$= %input || strReplace(%input, "three", "") !$= %input)
		{
			%client.stage = 1.2;

			serverCmdMessageSent(%client, CityRPGData.getData(%client.bl_id).valueMoney);

			return;
		}

		if(strReplace(%input, "4", "") !$= %input || strReplace(%input, "four", "") !$= %input)
		{
			%client.stage = 1.3;

			messageClient(%client, '', "\c6Enter the amount you would like to donate:");
			return;
		}

		messageClient(%client, '', "\c3" @ %text SPC "\c6is not a valid option!");

		return;
	}

	if(mFloor(%client.stage) == 1)
	{
		if(%client.stage == 1.1)
		{
			if(mFloor(%input) < 1)
			{
				messageClient(%client, '', "\c6Please enter a valid amount of money to withdraw.");

				return;
			}

			if(CityRPGData.getData(%client.bl_id).valueBank - mFloor(%input) < 0)
			{
				if(CityRPGData.getData(%client.bl_id).valueBank < 1)
				{
					messageClient(%client, '', "\c6You don't have that much money in the bank to withdraw.");

					%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, (isObject(%client.player) ? %client.player : 0));

					return;
				}

				%input = CityRPGData.getData(%client.bl_id).valueBank;
			}

			messageClient(%client, '', "\c6You have withdrawn \c3$" @ mFloor(%input) @ "\c6.");

			%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, (isObject(%client.player) ? %client.player : 0));

			CityRPGData.getData(%client.bl_id).valueBank -= mFloor(%input);
			CityRPGData.getData(%client.bl_id).valueMoney += mFloor(%input);

			%client.SetInfo();
		}

		if(%client.stage == 1.2)
		{
			if(mFloor(%input) < 1)
			{
				messageClient(%client, '', "\c6Please enter a valid amount of money to deposit.");

				return;
			}

			if(CityRPGData.getData(%client.bl_id).valueMoney - mFloor(%input) < 0)
			{
				if(CityRPGData.getData(%client.bl_id).valueMoney < 1)
				{
					messageClient(%client, '', "\c6You don't have that much money to deposit.");

					%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, (isObject(%client.player) ? %client.player : 0));

					return;
				}

				%input = CityRPGData.getData(%client.bl_id).valueMoney;
			}

			messageClient(%client, '', "\c6You have deposited \c3$" @ mFloor(%input) @ "\c6!");

			%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, (isObject(%client.player) ? %client.player : 0));

			CityRPGData.getData(%client.bl_id).valueBank += mFloor(%input);
			CityRPGData.getData(%client.bl_id).valueMoney -= mFloor(%input);

			%client.SetInfo();
		}

		if(%client.stage == 1.3)
		{
			serverCmddonate(%client, %text);
			return;
		}

		return;
	}

	if(mFloor(%client.stage) == 2)
	{
		if(%client.stage == 2.0)
		{
			messageClient(%client, '', "\c6Are you looking to \c3store\c6 or \c3take\c6 an item?");
			return;
		}
		else if(%client.stage == 2.1)
		{

		}
	}
}
