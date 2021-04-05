// Derived from findClientByName in the base game.
function findJobByName(%partialName)
{
	%pnLen = strlen (%partialName);
	%bestJob = -1;
	%bestPos = 9999;
    for(%i = 0; %i <= JobSO.getJobCount()-1; %i++)
    {
        %jobObject = JobSO.job[getField(JobSO.jobsIndex, %i)];

		%pos = -1;
		%name = strlwr(%jobObject.name);
		%pos = strstr(%name, strlwr (%partialName));
		if(%pos != -1)
		{
			%bestJob = %jobObject;
			if(%pos == 0)
			{
				return %jobObject;
			}
			if(%pos < %bestPos)
			{
				%bestPos = %pos;
				%bestJob = %jobObject;
			}
		}
    }

	if (%bestJob != -1)
	{
		return %bestJob;
	}
	else 
	{
		return 0;
	}
}

// Client::setCityJob
// Attempts to change the player's job.
// If %force is not enabled, will validate eligibility and display an error if the player is not eligible.
function GameConnection::setCityJob(%client, %jobID, %force)
{
	%jobObject = JobSO.job[%jobID];
	%data = CityRPGData.getData(%client.bl_id);

	if(!%force)
	{
		%jobEligible = 1;

		if(%jobID $= %data.valueJobID)
		{
			messageClient(%client, '', "\c6- You are already" SPC City_DetectVowel(%jobObject.name) SPC "\c3" @ %jobObject.name @ "\c6!");
			%jobEligible = 0;
		}

		if(%jobObject.law && getWord(%data.valueJailData, 0) == 1)
		{
			messageClient(%client, '', "\c6- You do not have a clean criminal record to become" SPC City_DetectVowel(%jobObject.name) SPC "\c3" @ %jobObject.name @ "\c6.");
			%jobEligible = 0;
		}

		if(%jobObject.adminonly == 1)
		{
			messageClient(%client, '', "\c6- Only an Admin or a Super Admin can become" SPC City_DetectVowel(%jobObject.name) SPC %jobObject.name @ "\c6.");
			%jobEligible = 0;
		}

		if(%data.valueMoney < %jobObject.invest)
		{
			messageClient(%client, '', "\c6- It costs \c3$" @ %jobObject.invest SPC "\c6to become" SPC City_DetectVowel(%jobObject.name) SPC %jobObject.name @ "\c6.");
			%jobEligible = 0;
		}

		if(%data.valueEducation < %jobObject.education)
		{
			messageClient(%client, '', "\c6- You need to reach an education level of \c3" @ %jobObject.education @ "\c6 to become" SPC City_DetectVowel(%jobObject.name) SPC %jobObject.name @ "\c6.");
			%jobEligible = 0;
		}

		// Return if the player is not eligible.
		if(!%jobEligible)
		{
			return 0;
		}

		// Operations for player-initiated job changes only.
		if(%jobObject.education == 0)
		{
			messageClient(%client, '', "\c6You have made your own initiative to become" SPC City_DetectVowel(%jobObject.name) SPC "\c3" @ %jobObject.name @ "\c6.");
		}
		else
		{
			messageClient(%client, '', "\c6Congratulations, you are now" SPC City_DetectVowel(%jobObject.name) SPC %jobObject.name @ "\c6. Your new salary is \c3$" @ %jobObject.pay @ "\c6 per day.");
		}

		CityRPGData.getData(%client.bl_id).valueMoney -= %jobObject.invest;
	}
	else
	{
		// Operations for forced job changes only.
		messageClient(%client, '', "\c6Your job has changed to" SPC City_DetectVowel(%jobObject.name) SPC %jobObject.name @ "\c6. Your new salary is \c3$" @ %jobObject.pay @ "\c6 per day.");
	}

	%data.valueJobID = %jobObject.id;
	serverCmdunUseTool(%client);
	%client.player.giveDefaultEquipment();
	%client.applyForcedBodyColors();
	%client.applyForcedBodyParts();
	%client.player.setDatablock(%jobObject.db);

	if(%job == $City::MayorJobID)
	{
		$City::Mayor::String = %client.name;
		$City::Mayor::Enabled = 0;
		serverCmdClearImpeach(%client);
	}
	%client.SetInfo();
	%client.onCityJobChange();
	return 1;
}

function GameConnection::onCityJobChange(%client)
{
	
}

