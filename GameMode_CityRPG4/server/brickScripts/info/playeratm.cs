// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGplayerATMBrickData : brick2x4FData)
{
	category = "CityRPG";
	subCategory = "Info Bricks";

	uiName = "PLAYER ATM Brick";

	CityRPGBrickType = $CityBrick_Info;
	CityRPGBrickAdmin = true;
	CityRPGBrickPlayerPrivliage = true;
	CityRPGBrickCost = 100;

	triggerDatablock = CityRPGInputTriggerData;
	triggerSize = "2 4 1";
	trigger = 0;
};

// ============================================================
// Trigger Data
// ============================================================
function resetAccessableplayerATM(%client)
{
	%client.AccessableplayerATM = 0;
}

function CityRPGplayerATMBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus !$= "")
	{
		if(%triggerStatus == true && %client.stage $= "")
		{
			messageClient(%client, '', "\c3ATM");
			if(CityRPGData.getData(%client.bl_id).valueBank > 0)
			{
				messageClient(%client, '', "\c6You have \c3$" @ CityRPGData.getData(%client.bl_id).valueBank SPC "\c6in your account.");
			}

			messageClient(%client, '', "\c6Type a number in chat:");


			messageClient(%client, '', "\c31 \c6- Withdraw money.");

			%client.stage = 0;
		}

		if(%triggerStatus == false && %client.stage !$= "")
		{
			messageClient(%client, '', "\c6Logging out..");

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

			messageClient(%client, '', "\c6Withdraw amount:");

			return;
		}

		if(strReplace(%input, "2", "") !$= %input || strReplace(%input, "two", "") !$= %input)
		{

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
				messageClient(%client, '', "\c6Error.");

				return;
			}

			if(CityRPGData.getData(%client.bl_id).valueBank - mFloor(%input) < 0)
			{
				if(CityRPGData.getData(%client.bl_id).valueplayerATM < 1)
				{
					messageClient(%client, '', "\c6Insufficient funds.");

					%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, (isObject(%client.player) ? %client.player : 0));

					return;
				}

				%input = CityRPGData.getData(%client.bl_id).valueplayerATM;
			}

			messageClient(%client, '', "\c6You have withdrawn \c3$" @ mFloor(%input) @ "\c6.");

			%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, (isObject(%client.player) ? %client.player : 0));

			CityRPGData.getData(%client.bl_id).valueBank -= mFloor(%input);
			CityRPGData.getData(%client.bl_id).valueMoney += mFloor(%input);

			%client.SetInfo();
		}
		return;
	}
}
