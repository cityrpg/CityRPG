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
// Menu
// ============================================================
function CityMenu_Jobs(%client, %brick)
{
	%client.cityMenuMessage("\c3" @ $Pref::Server::City::name @ " Employment Office");
	%client.cityMenuMessage("\c6Your current job is\c3" SPC %client.getJobSO().name @ "\c6 with an income of \c3$" @ %client.getJobSO().pay @ "\c6.");

	%menu =	"View job list."
			TAB "Apply for job.";

	%functions = 	"CityMenu_Jobs_List"
						TAB "CityMenu_Jobs_ApplyPrompt";

	%client.cityMenuOpen(%menu, %functions, %brick, "\c6Thanks, come again.");
}

function CityMenu_Jobs_List(%client, %brick)
{
	serverCmdhelp(%client, "jobs");
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
