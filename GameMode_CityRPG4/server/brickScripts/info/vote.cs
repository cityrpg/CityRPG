// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGVoteBrickData : brick2x4FData)
{
	category = "CityRPG";
	subCategory = "Info Bricks";

	uiName = "Vote Brick";

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
function CityRPGVoteBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus !$= "")
	{
		if(%triggerStatus == true && %client.stage $= "")
		{
			messageClient(%client, '', "\c3" @ $Pref::Server::City::name @ " Voting Booth");
			if($City::Mayor::Voting == 1)
			{
				messageClient(%client, '', "\c6Type a number in chat:");
				messageClient(%client, '', "\c31 \c6- Apply for Mayor. (Costs: $" @ $Pref::Server::City::Mayor::Cost @ ")");
				messageClient(%client, '', "\c32 \c6- Vote");
				messageClient(%client, '', "\c33 \c6- View candidates");
				messageClient(%client, '', "\c34 \c6- View scores");
			} else {
				if($City::Mayor::ID != -1) {
					messageClient(%client, '', "\c6City mayor: " @ $City::Mayor::String);
					messageClient(%client, '', "\c6Type a number in chat:");
					messageClient(%client, '', "\c31 \c6- Vote to remove the Mayor from office! \c3($" @ $Pref::Server::City::Mayor::ImpeachCost @ ")");
				}

				messageClient(%client, '', "\c6There currently isn't an election. Check back later.");
			}

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
			if($City::Mayor::Voting == 0 && $City::Mayor::ID != -1)
				CityMayor_VoteImpeach(%client);
			else
				serverCmdRegisterCandidates(%client, %text);
			return;
		}

		if(strReplace(%input, "2", "") !$= %input || strReplace(%input, "two", "") !$= %input)
		{
			%client.stage = 1.1;

			messageClient(%client, '', "\c6Type the candidate's name you'd like to vote for:");
			return;
		}

		if(strReplace(%input, "3", "") !$= %input || strReplace(%input, "three", "") !$= %input)
		{
			CityMayor_getCandidates(%client);
			return;
		}

		if(strReplace(%input, "4", "") !$= %input || strReplace(%input, "four", "") !$= %input)
		{
			serverCmdtopC(%client);
			return;
		}

		messageClient(%client, '', "\c3" @ %text SPC "\c6is not a valid option!");

		return;
	}

	if(%client.stage == 1.1)
	{
		serverCmdvoteElection(%client, %text);
					return;
	}
}
