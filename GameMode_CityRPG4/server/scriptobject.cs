/// ============================================================
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
	$City::DefaultJobs = 1;
	$City::CivilianJobID = 1;

	// NOTE: Order is incredibly important. Jobs are referenced by ID, which is determined by order.
	// Mixing up the order of these professions will cause save data to reference the wrong job.
	%so.addJobFromFile("civilian");               // 1
	%so.addJobFromFile("miner");                  // 2
	%so.addJobFromFile("lumberjack");             // 3
	%so.addJobFromFile("grocer");                 // 4
	%so.addJobFromFile("armsdealer");             // 5
	%so.addJobFromFile("shopowner");							// 6
	%so.addJobFromFile("shopceo");           		  // 7
	%so.addJobFromFile("bountyhunter");           // 8
	%so.addJobFromFile("bountyhunterpro");        // 9
	%so.addJobFromFile("policeasst");             // 10
	%so.addJobFromFile("policeman");              // 11
	%so.addJobFromFile("policechief");            // 12
	%so.addJobFromFile("councilmember");          // 13
	%so.addJobFromFile("mayor");		  						// 14
}

function JobSO::addJobFromFile(%so, %file)
{
	// First check for a path in the game-mode, then check for a direct path
	%filePath = $City::ScriptPath @ "jobs/" @ %file @ ".cs";
	if(!isFile(%filePath))
	{
		%filePath = %file;
	}

	// If there's still nothing, throw an error.
	if(!isFile(%filePath))
	{
		error("JobSO::addJobFromFile - Unable to find the corresponding job file '" @ %file @ "'. This job will not load.");
	}
	else
	{
		%jobID = %so.getJobCount() + 1;
		exec(%filePath);
		%so.job[%jobID] = new scriptObject()
		{
			id		= %jobID;

			name		= $CityRPG::jobs::name;
			invest		= $CityRPG::jobs::initialInvestment;
			pay		= $CityRPG::jobs::pay;
			tools		= $CityRPG::jobs::tools;
			education	= $CityRPG::jobs::education;
			db		= $CityRPG::jobs::datablock;
			hostonly	= $CityRPG::jobs::hostonly;
			adminonly	= $CityRPG::jobs::adminonly;
			usepolicecars	= $CityRPG::jobs::usepolicecars;
			usecrimecars	= $CityRPG::jobs::usecrimecars;
			useparacars		= $CityRPG::jobs::useparacars;
			outfit		= $CityRPG::jobs::outfit;

			sellItems	= $CityRPG::jobs::sellItems;
			sellFood	= $CityRPG::jobs::sellFood;
			sellServices 	= $CityRPG::jobs::sellServices; // Unused.
			sellClothes 	= $CityRPG::jobs::sellClothes;

			law		= $CityRPG::jobs::law;
			canPardon	= $CityRPG::jobs::canPardon;

			thief		= $CityRPG::jobs::thief;
			hideJobName	= $CityRPG::jobs::hideJobName;

			bountyOffer	= $CityRPG::jobs::offerer;
			bountyClaim	= $CityRPG::jobs::claimer;

			laborer		= $CityRPG::jobs::labor;

			tmHexColor	= $CityRPG::jobs::tmHexColor;
			helpline	= $CityRPG::jobs::helpline;
			flavortext	= $CityRPG::jobs::flavortext;
		};

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
}

function JobSO::getJobCount(%so)
{
	for(%a = 0; isObject(%so.job[%a + 1]); %a++) { }
	return %a;
}

// ============================================================
// CitySO
// ============================================================
function CitySO::loadData(%so)
{
	if(isFile("config/server/CityRPG/CityRPG/City.cs"))
	{
		exec("config/server/CityRPG/CityRPG/City.cs");
		%so.minerals		= $CityRPG::temp::citydata::datumminerals;
		%so.lumber			= $CityRPG::temp::citydata::datumlumber;
		%so.economy			= $Economics::Condition;

	}
	else
	{
		%so.value["minerals"] = 0;
		%so.value["lumber"] = 0;
		%so.value["economy"] = 0;
	}
}

function CitySO::saveData(%so)
{
	$CityRPG::temp::citydata::datum["minerals"]		= %so.minerals;
	$CityRPG::temp::citydata::datum["lumber"]		= %so.lumber;
	export("$CityRPG::temp::citydata::*", "config/server/CityRPG/CityRPG/City.cs");
}

if(!isObject(CitySO))
{
	new scriptObject(CitySO) { };
	CitySO.loadData();
}

// ============================================================
// CalendarSO
// ============================================================
function CalendarSO::loadCalendar(%so)
{
	// Counters
	%so.numOfMonths = 12;
	%so.zbNumMonths = %so.numOfMonths - 1;

	// Names
	%so.nameOfMonth[0] = "January";
	%so.nameOfMonth[1] = "February";
	%so.nameOfMonth[2] = "March";
	%so.nameOfMonth[3] = "April";
	%so.nameOfMonth[4] = "May";
	%so.nameOfMonth[5] = "June";
	%so.nameOfMonth[6] = "July";
	%so.nameOfMonth[7] = "August";
	%so.nameOfMonth[8] = "September";
	%so.nameOfMonth[9] = "October";
	%so.nameOfMonth[10] = "November";
	%so.nameOfMonth[11] = "December";

	// Days
	%so.daysInMonth[0] = 31;
	%so.daysInMonth[1] = 28;
	%so.daysInMonth[2] = 31;
	%so.daysInMonth[3] = 30;
	%so.daysInMonth[4] = 31;
	%so.daysInMonth[5] = 30;
	%so.daysInMonth[6] = 31;
	%so.daysInMonth[7] = 31;
	%so.daysInMonth[8] = 30;
	%so.daysInMonth[9] = 31;
	%so.daysInMonth[10] = 30;
	%so.daysInMonth[11] = 31;

	// Holidays
	%so.holiday[1] = "\c2Happy New Year!";
	//%so.holiday[91] = "\c2A\c1p\c2r\c1i\c2l \c0F\c3o\c0o\c3l\c0s \c7D\c6a\c7y\c6!";
	//%so.holiday[350] = "\c0H\c3a\c2p\c1p\c5y\c6 Holidays\c7!";
}

function CalendarSO::getDateStr(%so, %client)
{
	%ticks = %so.date;

	for(%a = 0; %ticks > %so.daysInMonth[%a % %so.numOfMonths]; %a++)
	{
		%ticks -= %so.daysInMonth[%a % %so.numOfMonths];
	}

	%year = mFloor(%a / %so.numOfMonths)+1;

	// If the second number from last is a "1" (e.g. 12 or 516), the suffix will always be "th"
	if(strlen(%ticks) > 1 && getSubStr(%ticks, (strlen(%ticks) - 2), 1) $= "1")
	{
		%suffix = "th";
	}
	// If not, it can either be "st," "nd," "rd," or "th," depending on the last numeral.
	else
	{
		switch(getSubStr(%ticks, (strlen(%ticks) - 1), 1))
		{
			case 1: %suffix = "st";
			case 2: %suffix = "nd";
			case 3: %suffix = "rd";
			default: %suffix = "th";
		}
	}

	return "\c3" @ %so.nameOfMonth[%so.getMonth()] SPC %ticks @ %suffix @ "\c6, Year \c3" @ %year;
}

function CalendarSO::getMonth(%so)
{
	%ticks = %so.date;

	for(%a = 0; %ticks > %so.daysInMonth[%a % %so.numOfMonths]; %a++)
		%ticks -= %so.daysInMonth[%a % %so.numOfMonths];

	%month = %a % %so.numOfMonths;
	return %month;
}

function CalendarSO::dumpCalendar(%so)
{
	for(%a = 0; %so.daysInMonth[%a] !$= ""; %a++)
	{
		echo(%so.nameOfMonth[%a] SPC "has" SPC %so.daysInMonth[%a] SPC "days.");
	}
}

function CalendarSO::getYearLength(%so)
{
	for(%a = 0; %so.daysInMonth[%a] > 0; %a++)
	{
		%totalLength += %so.daysInMonth[%a];
	}

	return %totalLength;
}

function CalendarSO::getCurrentDay(%so)
{
	return (%so.date % %so.getYearLength());
}

function CalendarSO::loadData(%so)
{
	if(isFile("config/server/CityRPG/CityRPG/Calendar.cs"))
	{
		exec("config/server/CityRPG/CityRPG/Calendar.cs");
		%so.date = $CityRPG::temp::calendar::datumdate;
	}
	else
	{
		%so.date = 1;
	}

	%so.loadCalendar();
}

function CalendarSO::saveData(%so)
{
	$CityRPG::temp::calendar::datum["date"]	= %so.date;
	export("$CityRPG::temp::calendar::*", "config/server/CityRPG/CityRPG/Calendar.cs");
}

if(!isObject(CalendarSO))
{
	new scriptObject(CalendarSO) { };
	CalendarSO.schedule(1, "loadData");
}

// ============================================================
// ClothesSO
// ============================================================
function ClothesSO::loadClothes(%so)
{
	// Clothing Data
	%so.color["none"]		= "1 1 1 1";
	%so.node["none"]		= "0";

	// Outfits
	// Outfits use index instead of names.
	// Do not repeat indexes.
	// This is the order they appear in the GUI.
	%so.str[1]	= "none none none none whitet whitet skin bluejeans blackshoes default default";
	%so.uiName[1]	= "Default";
	%so.sellName[1]	= "Default Suit";

	%so.str[2]	= "none brownhat keep keep greenshirt greenshirt keep greyPants blackshoes default default";
	%so.uiName[2]	= "Basic";
	%so.sellName[2]	= "Basic Outfit";

	%so.str[3]	= "keep skullcap keep keep blackshirt blackshirt blackgloves blackPants blackshoes default default";
	%so.uiName[3]	= "Gimp";
	%so.sellName[3]	= "Gimp Suit";

	%so.str[4]	= "none none none none whitet redsleeve keep brightbluePants blueshoes default default";
	%so.uiName[4]	= "Blockhead";
	%so.sellName[4]	= "Blockhead Clothes";

	%so.str[5]	= "keep keep keep keep greenshirt greenshirt keep brownPants blackshoes default worm-sweater";
	%so.uiName[5]	= "Nerd";
	%so.sellName[5]	= "Nerd Suit";

	%so.str[6]	= "keep keep keep keep blackshirt blackshirt keep blackPants blackshoes default Mod-Suit";
	%so.uiName[6]	= "Business";
	%so.sellName[6]	= "Business Suit";

	%so.str[7]	= "keep keep keep keep blueshirt blueshirt keep bluePants blackshoes default Mod-Suit";
	%so.uiName[7]	= "Council";
	%so.sellName[7]	= "Council Suit";

	%so.str[8]	= "keep keep keep keep skingen skingen skingen skingen skingen default default";
	%so.uiName[8]	= "Naked";
	%so.sellName[8]	= "B-Day Suit";

	%so.str[9]	= "keep keep keep keep blackshirt blackshirt skingen blackpants blackshoes default Mod-Suit";
	%so.uiName[9]	= "Suit";
	%so.sellName[9]	= "Suit & Tie";

	%so.str[10]	= "DrKleiner DrKleiner DrKleiner DrKleiner whitet whitet brightbluegloves whitet blackshoes DrKleiner DrKleiner";
	%so.uiName[10]	= "Doctor";
	%so.sellName[10]	= "Doctor";

	// Hats
	%so.color["brownhat"]	= "0.329 0.196 0.000 1.000";
	%so.node["brownhat"]	= "4";
	%so.str["brownhat"]	= "keep this keep keep keep keep keep keep keep";

	%so.color["piratehat"]	= "0.078 0.078 0.078 1";
	%so.node["piratehat"]	= "5";
	%so.str["piratehat"]	= "keep this keep keep keep keep keep keep keep";

	%so.color["copHat"]	= "0 0.141176 0.333333 1";
	%so.node["copHat"]	= "6";
	%so.str["copHat"]	= "keep this keep keep keep keep keep keep keep";

	%so.color["skullcap"]	= "0.200 0.200 0.200 1.000";
	%so.node["skullcap"]	= "7";
	%so.str["skullcap"]	= "keep this keep keep keep keep keep keep keep";

	%so.color["copHat2"]	= "0 0.000 0.500 0.250 1.000";
	%so.node["copHat2"]	= "8";
	%so.str["copHat2"]	= "keep this keep keep keep keep keep keep keep";

	// Gloves
	%so.color["blackgloves"] = "0.200 0.200 0.200 1.000";
	%so.node["blackgloves"]	= "0";
	%so.str["blackgloves"]	= "keep keep keep keep keep keep this keep keep";

	%so.color["brightbluegloves"] = "0.500 0.400 0.800 1.000";
	%so.node["brightbluegloves"]	= "0";
	%so.str["brightbluegloves"]	= "keep keep keep keep keep keep this keep keep";

	// Shirts
	%so.color["pinkt"]	= "1 0.75 0.79 1";
	%so.node["pinkt"]	= "gender";
	%so.str["pinkt"]	= "keep keep keep keep this this keep keep keep";

	%so.color["greyShirt"]	= "0.000 0.000 0.000 1.000";
	%so.node["greyShirt"]	= "gender";
	%so.str["greyShirt"]	= "keep keep keep keep this skingen keep keep keep";

	%so.color["whitet"]	= "1 1 1 1";
	%so.node["whitet"]	= "gender";
	%so.str["whitet"]	= "keep keep keep keep this this keep keep keep";

	%so.color["copShirt"]	= "0 0.141176 0.333333 1";
	%so.node["copShirt"]	= "gender";
	%so.str["copShirt"]	= "keep keep keep keep this this keep keep keep";

	%so.color["jumpsuit"]	= "1 0.617 0 1";
	%so.node["jumpsuit"]	= "gender";
	%so.str["jumpsuit"]	= "keep keep keep keep this this keep this this";

	%so.color["blackshirt"]	= "0.200 0.200 0.200 1.000";
	%so.node["blackshirt"]	= "gender";
	%so.str["blackshirt"]	= "keep keep keep keep this this keep keep keep";

	%so.color["brownshirt"]	= "0.329 0.196 0.000 1.000";
	%so.node["brownshirt"]	= "gender";
	%so.str["brownshirt"]	= "keep keep keep keep this this keep keep keep";

	%so.color["greenshirt"]	= "0.00 0.262 0.00 1";
	%so.node["greenshirt"]	= "gender";
	%so.str["greenshirt"]	= "keep keep keep keep this this keep keep keep";

	%so.color["blueshirt"]	= "0.0 0.141 0.333 1";
	%so.node["blueshirt"]	= "gender";
	%so.str["blueshirt"]	= "keep keep keep keep this this keep keep keep";

	// Pants
	%so.color["bluejeans"]	= "0 0.141 0.333 1";
	%so.node["bluejeans"]	= "0";
	%so.str["bluejeans"]	= "keep keep keep keep keep keep keep this keep";

	%so.color["blackPants"] = "0.200 0.200 0.200 1.000";
	%so.node["blackPants"]	= "0";
	%so.str["blackPants"]	= "keep keep keep keep keep keep keep this keep";

	%so.color["brownPants"] = "0.329 0.196 0.000 1.000";
	%so.node["brownPants"]	= "0";
	%so.str["brownPants"]	= "keep keep keep keep keep keep keep this keep";

	%so.color["greyPants"] = "0.000 0.000 0.000 1.000";
	%so.node["greyPants"]	= "0";
	%so.str["greyPants"]	= "keep keep keep keep keep keep keep this keep";

	%so.color["brightbluePants"] = "0.200 0.000 0.800 1.000";
	%so.node["brightbluePants"]	= "0";
	%so.str["brightbluePants"]	= "keep keep keep keep keep keep keep this keep";

	%so.color["bluePants"] = "0.0 0.141 0.333 1";
	%so.node["bluePants"]	= "0";
	%so.str["bluePants"]	= "keep keep keep keep keep keep keep this keep";

	// Shoes
	%so.color["blackshoes"]	= "0.200 0.200 0.200 1.000";
	%so.node["blackshoes"]	= "0";
	%so.str["blackshoes"]	= "keep keep keep keep keep keep keep keep this";

	%so.color["brownshoes"]	= "0.329 0.196 0.000 1.000";
	%so.node["brownshoes"]	= "0";
	%so.str["brownshoes"]	= "keep keep keep keep keep keep keep keep this";

	%so.color["blueshoes"]	= "0.000 0.000 0.000 1.000";
	%so.node["blueshoes"]	= "0";
	%so.str["bluehoes"]	= "keep keep keep keep keep keep keep keep this";

	//Misc
	%so.color["redsleeve"] = "0.900 0.220 0.000 1.000";
	%so.node["redsleeve"] = "gender";
	%so.color["redsleeve"] = "keep keep keep keep this this keep keep keep";
}

function ClothesSO::postEvents(%so)
{
	%str = "list";

	for(%a = 1; %so.str[%a] !$= ""; %a++)
		%str = %str SPC %so.uiName[%a] SPC %a;

	if(%str !$= "")
	{
		registerOutputEvent("fxDTSBrick", "sellClothes", %str TAB "int 0 500 1");

		for(%b = 0; %b < ClientGroup.getCount(); %b++)
		{
			%subClient = ClientGroup.getObject(%b);
			serverCmdRequestEventTables(%subClient);
		}
	}
}

function ClothesSO::getColor(%so, %client, %item)
{
	if(%item $= "skin" || %item $= "skingen")
		return %client.headColor;
	else
	{
		%color = %so.color[%item];

		if(%color $= "")
		{
			warn("ClothesSO::getColor - Returned blank color for '" @ %item @ "'! Defaulting to white.");
			%color = "1 1 1 1";
		}

		return %color;
	}
}

function ClothesSO::getNode(%so, %client, %item)
{
	if(%item $= "skin")
		return 0;
	else
	{
		%node = %so.node[%item];

		return %node;
	}
}

function ClothesSO::getDecal(%so, %client, %segment, %item)
{
	if(%item $= "" || %item $= "default")
	{
		if(%segment $= "face")
			return "smiley";
		else if(%segment $= "chest")
			return "AAA-none";
	}
	else
		return %item;
}

function ClothesSO::giveItem(%so, %client, %item)
{
	if(strLen(%so.str[%item]) && isObject(%client))
	{
		%outfit = CityRPGData.getData(%client.bl_id).valueOutfit;

		for(%a = 0; %a < getWordCount(%outfit); %a++)
		{
			if(getWord(%so.str[%item], %a) $= "keep")
				%newOutfit = (%newOutfit $= "" ? getWord(%outfit, %a) : %newOutfit SPC getWord(%outfit, %a));
			else if(getWord(%so.str[%item], %a) $= "this")
				%newOutfit = (%newOutfit $= "" ? %item : %newOutfit SPC %item);
			else
				%newOutfit = (%newOutfit $= "" ? getWord(%so.str[%item], %a) : %newOutfit SPC getWord(%so.str[%item], %a));
		}

		CityRPGData.getData(%client.bl_id).valueOutfit = %newOutfit;
		%client.applyBodyParts();
		%client.applyBodyColors();
	}
}

if(!isObject(ClothesSO))
{
	new scriptObject(ClothesSO) { };
	ClothesSO.schedule(1, "loadClothes");
	//ClothesSO.schedule(1, "postEvents");
}

// ============================================================
// ResourceSO
// ============================================================

function ResourceSO::addResources(%this)
{
	//makes sure it's clear when this is re-exec'd
	for(%a = 0; isObject(ResourceSO.resource[%a]); %a++)
	{
		ResourceSO.resource[%a].delete();
		ResourceSO.resource[%a] = "";
	}

	ResourceSO.mineralCount = 6;
	ResourceSO.treeCount = 3;

	ResourceSO.mineral[1] = new scriptObject() {
		id = 1;
		name = "Dirt";
		totalHits = 8;
		BPH = 0.21;
		color = "0.392 0.196 0 1";
		respawnMultiplier = 2.5; };

	ResourceSO.mineral[2] = new scriptObject() {
		id = 2;
		name = "Tin";
		totalHits = 12;
		BPH = 0.21;
		color = "0.6 0.6 0.6 1";
		respawnMultiplier = 1; };

	ResourceSO.mineral[3] = new scriptObject() {
		id = 3;
		name = "Copper";
		totalHits = 24;
		BPH = 0.21;
		color = "1 0.45 0.2 1";
		respawnMultiplier = 1; };

	ResourceSO.mineral[4] = new scriptObject() {
		id = 4;
		name = "Iron";
		totalHits = 40;
		BPH = 0.21;
		color = "0.5 0.5 0.5 1";
		respawnMultiplier = 1; };

	ResourceSO.mineral[5] = new scriptObject() {
		id = 5;
		name = "Silver";
		totalHits = 52;
		BPH = 0.21;
		color = "0.9 0.9 0.9 1";
		respawnMultiplier = 1; };

	ResourceSO.mineral[6] = new scriptObject() {
		id = 6;
		name = "Gold";
		totalHits = 60;
		BPH = 0.22;
		color = "0.9 0.9 0 1";
		respawnMultiplier = 1; };

	ResourceSO.tree[1] = new scriptObject() {
		id = 1;
		name = "Pine";
		BPH = 0.21;
		TotalHits = 20;
		 Color = "0 .5 .25 1"; };

	ResourceSO.tree[2] = new scriptObject() {
		id = 2;
		name = "Dead";
		BPH = 0.21;
		TotalHits = 20;
		 Color = "0.5 0.2 0.2 1"; };

	ResourceSO.tree[3] = new scriptObject() {
		id = 3;
		name = "Birch";
		BPH = 0.21;
		TotalHits = 40;
		Color = "1 1 1 1"; };
}

if(isObject(ResourceSO))
	ResourceSO.delete();

if(!isObject(ResourceSO))
{
	new scriptObject(ResourceSO) { };
	ResourceSO.addResources();
}
