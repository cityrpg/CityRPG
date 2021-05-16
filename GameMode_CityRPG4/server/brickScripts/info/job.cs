// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGJobBrickData : brick2x4FData)
{
	category = "CityRPG";
	subCategory = "City Info Bricks";

	uiName = "Job Brick";

	CityRPGBrickType = $CityBrick_Info;
	CityRPGBrickAdmin = true;
	CityRPGBrickCost = 3000;

	triggerDatablock = CityRPGInputTriggerData;
	triggerSize = "2 4 1";
	trigger = 0;
};

// ============================================================
// Menu
// ============================================================
function CityMenu_Jobs(%client, %brick)
{
	%client.cityMenuClose(1);
	%client.cityMenuMessage("\c6Your current job is\c3" SPC %client.getJobSO().name @ "\c6 with an income of \c3$" @ %client.getJobSO().pay @ "\c6.");

	%menu =	"View job tracks."
			TAB "Apply for a job.";

	%functions = 	"CityMenu_Jobs_List"
						TAB "CityMenu_Jobs_ApplyPrompt";

	%client.cityMenuOpen(%menu, %functions, %brick, "\c6Thanks, come again.", 0, 1, $Pref::Server::City::name @ " Employment Office");
}

function CityMenu_Jobs_ApplyPrompt(%client, %brick)
{
	%client.cityMenuMessage("\c6Job name:");
	%client.cityMenuFunction = "CityMenu_Jobs_ApplyInput";
}

function CityMenu_Jobs_ApplyInput(%client, %input)
{
	serverCmdjobs(%client, %input);
}

function CityMenu_Jobs_List(%client, %input, %brick)
{
	%client.cityMenuMessage("\c3Select a job track to view.");

	%menu = getField($City::JobTracks, 0);
	%functions = "CityMenu_Jobs_ViewTrack";

	for(%i = 1; %i <= getFieldCount($City::JobTracks)-1; %i++)
	{
		%menu = %menu TAB getField($City::JobTracks, %i);
		%functions = %functions TAB "CityMenu_Jobs_ViewTrack";
	}

	if(!$City::DefaultJobs)
	{
	  messageClient(%client, '', "\c3This server is running a customized job tree.");
	}

	%client.cityMenuOpen(%menu, %functions, %brick, "\c6Thanks, come again.");
}

function CityMenu_Jobs_ViewTrack(%client, %input, %brick)
{
	%client.cityMenuClose(1);

	// Subtracting 1 from the input should sanitize it from any potential injection issues
	%track = getField($City::JobTracks, %input-1);

	for(%i = 0; %i <= getFieldCount($City::Jobs[%track])-1; %i++)
	{
		%jobID = getField($City::Jobs[%track], %i);
		%job = JobSO.job[%jobID];

		%client.cityMenuMessage("\c3" @ %job.name SPC "\c6- Inital Investment: \c3" @ %job.invest SPC "- \c6Pay: \c3" @ %job.pay SPC "- \c6Required Education: \c3" @ %job.education);
		%client.cityMenuMessage(%job.helpline);

		if(%job.flavortext !$= "")
		{
			%client.cityMenuMessage("<color:A6A6A6>" @ %job.flavortext);
		}
	}

	messageClient(%client, '', "\c3Use the Page Up and Page Down keys to scroll through the list of jobs.");

	%menu =	"Go back."
			TAB "Apply for a job.";

	%functions = 	"CityMenu_Jobs_List"
						TAB "CityMenu_Jobs_ApplyPrompt";

	%client.cityMenuOpen(%menu, %functions, %brick, "\c6Thanks, come again.");
}

// ============================================================
// Trigger Data
// ============================================================
function CityRPGJobBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus == true && !%client.cityMenuOpen)
	{
		CityMenu_Jobs(%client, %brick);
	}
}
