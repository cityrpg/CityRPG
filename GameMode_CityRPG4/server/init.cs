// City_Init()
// Initializes the game-mode.
function City_Init()
{
	if(!isObject(City))
	{
		// New object reference
		new scriptObject(City) {};
	}

	if(!isObject(JobSO))
	{
		new scriptObject(JobSO) { };
		JobSO.populateJobs();
	}

	if(!isObject(CityRPGData))
	{
		// Deprecated object reference
		new scriptObject(CityRPGData)
		{
			class = Sassy;
			dataFile = $City::SavePath @ "Profiles.dat";
		};

		if(!isObject($DamageType::Starvation))
			AddDamageType("Starvation", '%1 starved', '%1 starved', 0.5, 0);

		// Since the active values change so often, we'll re-attempt to add them each time.
		CityRPGData.addValue("bank", 0);
		CityRPGData.addValue("bounty", "0");
		CityRPGData.addValue("demerits", "0");
		CityRPGData.addValue("education", "0");
		CityRPGData.addValue("gender", "Male");
		CityRPGData.addValue("hunger", "7");
		CityRPGData.addValue("jailData", "0 0");
		CityRPGData.addValue("jobID", "StarterCivilian");
		CityRPGData.addValue("jobRevert", "0");
		CityRPGData.addValue("lotData", "0");
		CityRPGData.addValue("money", "0");
		CityRPGData.addValue("name", "noName");
		CityRPGData.addValue("outfit", "none none none none whitet whitet skin bluejeans blackshoes");
		CityRPGData.addValue("reincarnated", "0");
		CityRPGData.addValue("resources", "0 0");
		CityRPGData.addValue("student", "0");
		CityRPGData.addValue("Rep", "0");
		CityRPGData.addValue("ElectionID", "0");
		CityRPGData.addValue("lotsVisited", "-1");
		CityRPGData.addValue("spawnPoint", "");
		
		if(CityRPGData.loadedSaveFile)
		{
			for(%a = 1; %a <= CityRPGData.dataCount; %a++)
			{
				if(JobSO.job[CityRPGData.data[%a].valueJobID] $= "")
				{
					CityRPGData.data[%a].valueJobID = $City::CivilianJobID;
				}
			}
		}

		City_Init_AssembleEvents();

		CalendarSO.date = 0;
		CityRPGData.lastTickOn = $Sim::Time;
		CityRPGData.scheduleTick = schedule($Pref::Server::City::tick::speed * 60000, false, "City_Tick");
	}
	else
	{
		for(%a = 1; %a <= CityRPGData.dataCount; %a++)
		{
			if(CityRPGData.data[%a].valueJobID > JobSO.getJobCount() || CityRPGData.data[%a].valueJobID < 0)
			{
				CityRPGData.data[%a].valueJobID = $City::CivilianJobID;
			}
		}
	}

	// Generic client to handle checks for external utilities as the host.
	if(!isObject(CityRPGHostClient))
	{
		new ScriptObject(CityRPGHostClient)
		{
			isSuperAdmin = 1;
		};
	}

	// Generic client to run events such as spawnProjectile. See: minigameCanDamage
	if(!isObject(CityRPGEventClient))
	{
		new ScriptObject(CityRPGEventClient)
		{
		};
	}

	if(!isObject(CityRPGMini))
	{
		City_Init_Minigame();
	}

	CityMayor_refresh();

	activatePackage("CityRPG_Overrides");

	echo("CityRPG initialization complete.");
}

function CityRPGHostClient::onBottomPrint(%this, %message)
{
	return;
}

function City::get(%this, %profileID, %key)
{
	return CityRPGData.getData(%profileID).value[%key];
}

function City::set(%this, %profileID, %key, %value)
{
	CityRPGData.getData(%profileID).value[%key] = %value;
}

function City::add(%this, %profileID, %key, %value)
{
	CityRPGData.getData(%profileID).value[%key] = CityRPGData.getData(%profileID).value[%key] + %value;
}

function City::subtract(%this, %profileID, %key, %value)
{
	CityRPGData.getData(%profileID).value[%key] = CityRPGData.getData(%profileID).value[%key] - %value;
}

function City::keyExists(%this, %profileID)
{
	return CityRPGData.getData(%profileID) != 0;
}

// City_Init_Minigame()
// Creates the minigame for the game-mode.
function City_Init_Minigame()
{
	loadMayor();

	if(isObject(CityRPGMini))
	{
		for(%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%subClient = ClientGroup.getObject(%i);
			CityRPGMini.removeMember(%subClient);
		}
		CityRPGMini.delete();
	}
	else
	{
		for(%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%subClient = ClientGroup.getObject(%i);
			%subClient.minigame = NULL;
		}
	}

	new scriptObject(CityRPGMini)
	{
		class = miniGameSO;

		brickDamage = true;
		brickRespawnTime = 10000;
		colorIdx = -1;

		enableBuilding = true;
		enablePainting = true;
		enableWand = true;
		fallingDamage = true;
		inviteOnly = false;

		points_plantBrick = 0;
		points_breakBrick = 0;
		points_die = 0;
		points_killPlayer = 0;
		points_killSelf = 0;
		playerDatablock = Player9SlotPlayer;
		respawnTime = 5;
		selfDamage = true;

		playersUseOwnBricks = false;
		useAllPlayersBricks = true;
		useSpawnBricks = false;
		VehicleDamage = true;
		vehicleRespawnTime = 10000;
		weaponDamage = true;

		numMembers = 0;
		vehicleRunOverDamage = false;
	};
	for(%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%subClient = ClientGroup.getObject(%i);
		CityRPGMini.addMember(%subClient);
	}
}

// City_Init_Spawns()
// Records the spawn points for player spawning. Called on init and every tick.
// This will be optimized in the future.
function City_Init_Spawns()
{
	if($CityRPG::BuildingSpawns)
	{
		warn("City: Already building spawns, skipping init...");
		return;
	}

	if(mainBrickGroup.getCount()<1)
		return;

	$CityRPG::BuildingSpawns = 1;
	$CityRPG::temp::spawnPointsTemp = "";

	$CityRPG::BuildSpawnsSched = schedule(206,mainBrickGroup,"City_Init_Spawns_Tick",0,0);
}

function City_Init_Spawns_Tick(%bgi, %bi)
{
	cancel($CityRPG::BuildSpawnsSched);
	%mbgc = mainBrickGroup.getCount();
	for(%bgi;%bgi<%mbgc;%bgi++)
	{
		%bg = mainBrickGroup.getObject(%bgi);
		%bgc = %bg.getCount();
		for(%bi;%bi<%bgc;%bi++)
		{
			%b = %bg.getObject(%bi);
			if(%b.getDatablock().CityRPGBrickType == $CityBrick_Spawn)
			{
				$CityRPG::temp::spawnPointsTemp = (!$CityRPG::temp::spawnPointsTemp ? %b : $CityRPG::temp::spawnPointsTemp SPC %b);
			}
			%sc++;
			if(%sc>=1000)
			{
				$CityRPG::BuildSpawnsSched = schedule(206,mainBrickGroup,"City_Init_Spawns_Tick",%bgi,%bi);
				return;
			}
		}
		%bi=0;
	}
	echo("City: Built CityRPG Spawns");
	$CityRPG::BuildingSpawns = 0;
	$CityRPG::temp::spawnPoints = $CityRPG::temp::spawnPointsTemp;
}

// City_Init_AssembleEvents()
// Initializes events for the game.
function City_Init_AssembleEvents()
{
	// Basic Input
	registerInputEvent("fxDTSBrick", "onLotEntered", "Self fxDTSBrick" TAB "Player player" TAB "Client gameConnection");
	registerInputEvent("fxDTSBrick", "onLotLeft", "Self fxDTSBrick" TAB "Player player" TAB "Client gameConnection");
	registerInputEvent("fxDTSBrick", "onLotFirstEntered", "Self fxDTSBrick" TAB "Player player" TAB "Client gameConnection");
	registerInputEvent("fxDTSBrick", "onTransferSuccess", "Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection");
	registerInputEvent("fxDTSBrick", "onTransferDecline", "Self fxDTSBrick" TAB "Client GameConnection");
	registerInputEvent("fxDTSBrick", "onJobTestPass", "Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection");
	registerInputEvent("fxDTSBrick", "onMenuOpen", "Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection");
	registerInputEvent("fxDTSBrick", "onMenuClose", "Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection");
	registerInputEvent("fxDTSBrick", "onMenuInput", "Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection");

	// Basic Output
	registerOutputEvent("fxDTSBrick", "requestFunds", "string 80 200" TAB "int 1 9000 1");

	for(%a = 1; $CityRPG::portion[%a] !$= ""; %a++)
	{
		%sellFood_Portions = %sellFood_Portions SPC $CityRPG::portion[%a] SPC %a;
	}

	registerOutputEvent("fxDTSBrick", "sellFood", "list" @ %sellFood_Portions TAB "string 45 100" TAB "int 1 50 1");
	for(%b = 1; isObject(JobSO.job[%b]); %b++)
	{
		if(strlen(JobSO.job[%b].name) > 10)
			%jobName = getSubStr(JobSO.job[%b].name, 0, 9) @ ".";
		else
			%jobName = JobSO.job[%b].name;

		%doJobTest_List = %doJobTest_List SPC strreplace(%jobName, " ", "") SPC %b;
	}

	registerOutputEvent("fxDTSBrick", "doJobTest", "list NONE 0" @ %doJobTest_List TAB "list NONE 0" @ %doJobTest_List TAB "bool");
	for(%c = 0; %c <= $CityRPG::guns-1; %c++)
	{
		%sellItem_List = %sellItem_List SPC strreplace($CityRPG::prices::weapon::name[%c].uiName, " ", "") SPC %c;
	}
	registerOutputEvent("fxDTSBrick", "sellItem", "list" @ %sellItem_List TAB "int 0 500 1");
	for(%d = 0; %d < ClientGroup.getCount(); %d++)
	{
		%subClient = ClientGroup.getObject(%d);
		serverCmdRequestEventTables(%subClient);
	}
}
