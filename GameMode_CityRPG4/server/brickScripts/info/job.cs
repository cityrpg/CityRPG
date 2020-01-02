// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGJobBrickData : brick2x4FData)
{
	category = "CityRPG";
	subCategory = "Info Bricks";

	uiName = "Job Brick";

	CityRPGBrickType = $CityBrick_Info;
	CityRPGBrickAdmin = true;
	CityRPGBrickPlayerPrivliage = true;
	CityRPGBrickCost = 3000;

	triggerDatablock = CityRPGInputTriggerData;
	triggerSize = "2 4 1";
	trigger = 0;
};

// ============================================================
// Trigger Data
// ============================================================
function CityRPGJobBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus !$= "")
	{
		if(%triggerStatus == true && %client.stage $= "")
		{
			messageClient(%client, '', "\c3" @ $Pref::Server::City::name @ " Employment Office");
			messageClient(%client, '', "\c6Your current job is\c3" SPC %client.getJobSO().name @ "\c6 with an income of \c3$" @ %client.getJobSO().pay @ "\c6.");
			messageClient(%client, '', "\c6Type a number in chat:");

			messageClient(%client, '', "\c31 \c6- View Job List.");
			messageClient(%client, '', "\c32 \c6- Apply for job.");

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
			serverCmdhelp(%client, "jobs");
			return;
		}

		if(strReplace(%input, "2", "") !$= %input || strReplace(%input, "two", "") !$= %input)
		{
			%client.stage = 1.1;

			messageClient(%client, '', "\c6Job name:");
			return;
		}

		messageClient(%client, '', "\c3" @ %text SPC "\c6is not a valid option!");

		return;
	}

	if(mFloor(%client.stage) == 1)
	{
		if(%client.stage == 1.1)
		{
			serverCmdjobs(%client, %text, %job2, %job3, %job4, %job5);
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
			messageClient(%client, '', "\c6Please enter in the ID matching the Item you want to store.");

			for(%a = 0; %a < %client.player.getDatablock().maxTools; %a++)
			{
				%tool = %client.player.tool[%a];

				if(isObject(%tool))
				{
					messageClient(%client, '', "\c3" @ %a @ "\c6 - \c3" @ %tool.uiName);
				}

				return;
			}
		}
	}
}
