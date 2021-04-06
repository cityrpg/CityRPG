// ============================================================
// JobsSO
// ============================================================

function JobSO::populateJobs(%so)
{
	for(%a = 1; isObject(%so.job[%a]); %a++)
	{
		%so.job[%a].delete();
		%so.job[%a] = "";
	}

	%so.loadJobFiles();
}

function JobSO::loadJobFiles(%so)
{
	$City::DefaultJobs = "StarterCivilian";
	$City::CivilianJobID = "StarterCivilian";
	$City::AdminJobID = "Admin";

	exec($City::ScriptPath @ "/jobTrees.cs");

	%so.createJob("StarterCivilian");
	%so.createJob("LaborMiner");
	%so.createJob("LaborLumberjack");
	%so.createJob("BusGrocer");
	%so.createJob("BusArmsDealer");
	%so.createJob("BusOwner");
	%so.createJob("BusCEO");
	%so.createJob("BountyHunter");
	%so.createJob("BountyVigilante");
	%so.createJob("PdAsst");
	%so.createJob("PdOfficer");
	%so.createJob("PdChief");
	%so.createJob("Admin");
	%so.createJob("GovMayor");
}

function JobSO::createJob(%so, %file)
{
	// The identifier of each job must be unique as it is used to reference the job.
	%jobID = %file;

	// First check for a path in the game-mode, then check for a direct path
	%filePath = $City::ScriptPath @ "jobs/" @ %file @ ".cs";
	if(!isFile(%filePath))
	{
		// A full path has likely been passed to %file, process accordingly.
		%filePath = %file;
		%jobID = fileBase(%file);
	}

	// If there's still nothing, throw an error.
	if(!isFile(%filePath))
	{
		error("JobSO::addJobFromFile - Unable to find the corresponding job file '" @ %file @ "'. This job will not load.");
		return;
	}

	// Jobs must be indexed so we can loop search through them.
	%so.jobsIndex = %so.jobsIndex $= ""? %jobID : %so.jobsIndex TAB %jobID;

	exec(%filePath);
	%so.job[%jobID] = new scriptObject()
	{
		id				= %jobID;

		name			= $CityRPG::jobs::name;
		track			= $CityRPG::jobs::track;
		title			= $CityRPG::jobs::title;


		invest			= $CityRPG::jobs::initialInvestment;
		pay				= $CityRPG::jobs::pay;
		tools			= $CityRPG::jobs::tools;
		education		= $CityRPG::jobs::education;
		db				= $CityRPG::jobs::datablock;
		hostonly		= $CityRPG::jobs::hostonly;
		adminonly		= $CityRPG::jobs::adminonly;
		usepolicecars	= $CityRPG::jobs::usepolicecars;
		usecrimecars	= $CityRPG::jobs::usecrimecars;
		useparacars		= $CityRPG::jobs::useparacars;
		outfit			= $CityRPG::jobs::outfit;

		sellItems		= $CityRPG::jobs::sellItems;
		sellFood		= $CityRPG::jobs::sellFood;
		sellServices 	= $CityRPG::jobs::sellServices; // Unused.
		sellClothes 	= $CityRPG::jobs::sellClothes;

		law				= $CityRPG::jobs::law;
		canPardon		= $CityRPG::jobs::canPardon;

		thief			= $CityRPG::jobs::thief;
		hideJobName		= $CityRPG::jobs::hideJobName;

		bountyOffer		= $CityRPG::jobs::offerer;
		bountyClaim		= $CityRPG::jobs::claimer;

		laborer			= $CityRPG::jobs::labor;

		tmHexColor		= $CityRPG::jobs::tmHexColor;
		helpline		= $CityRPG::jobs::helpline;
		flavortext		= $CityRPG::jobs::flavortext;
	};

	%track = $CityRPG::jobs::track;

	if(%track $= "")
	{
		%track = "Miscellaneous";
		%so.job[%jobID].track = Miscellaneous;
	}

	// Default to a neutral grey if there is no color
	if($City::JobTrackColor[%track] $= "")
	{
		$City::JobTrackColor[%track] = "505050";
	}


	// Job track registration for menus
	// "Invisible" jobs such as admin and mayor are not included
	if(!$CityRPG::jobs::adminonly)
	{
		// Initialize if needed
		$City::Jobs[%track] = $City::Jobs[%track]!$=""?($City::Jobs[%track] TAB %jobID):%jobID;

		if(!$City::JobTrackExists[%track])
		{
			// Record the job track to a list so that we can loop through it later.
			$City::JobTracks = $City::JobTracks!$=""?($City::JobTracks TAB %track):%track;

			$City::JobTrackExists[%track] = true;
		}
	}

	if(!isObject("CityRPGJob" @ %jobID @ "SpawnBrickData"))
	{
		datablock fxDtsBrickData(CityRPGSpawnBrickData : brickSpawnPointData)
		{
			category = "CityRPG";
			subCategory = "Spawns";

			uiName = %so.job[%jobID].name SPC "Spawn";

			specialBrickType = "";

			CityRPGBrickType = $CityBrick_Spawn;
			CityRPGBrickAdmin = true;

			spawnData = "jobSpawn" SPC %jobID;
		};

		CityRPGSpawnBrickData.setName("CityRPGJob" @ %jobID @ "SpawnBrickData");
	}

	deleteVariables("$CityRPG::jobs::*");
}

function JobSO::getJobCount(%so)
{
	return getFieldCount(%so.jobsIndex);
}

// ============================================================
// Jobs Functions
// ============================================================

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

// ============================================================
// Client Functions
// ============================================================

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

		if(%jobObject.adminonly == 1 && !%client.isAdmin)
		{
			messageClient(%client, '', "\c6- You cannot directly sign up to become" SPC City_DetectVowel(%jobObject.name) SPC %jobObject.name @ "\c6.");
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
		if(%jobObject.adminonly)
		{
			messageClient(%client, '', "\c6You have used your admin powers to become" SPC City_DetectVowel(%jobObject.name) SPC %jobObject.name @ "\c6. Your new salary is \c3$" @ %jobObject.pay @ "\c6 per day.");
		}
		else if(%jobObject.education == 0)
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

