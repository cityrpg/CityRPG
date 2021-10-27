// ============================================================
// CitySO
// ============================================================
function CitySO::loadData(%so)
{
	// As an additional caution, use discoverFile.
	// This covers cases such as the admin deleting the file after the game starts.
	discoverFile($City::SavePath @ "City.cs");

	if(isFile($City::SavePath @ "City.cs"))
	{
		exec($City::SavePath @ "City.cs");
		%so.minerals		= $CityRPG::temp::citydata::datumminerals;
		%so.lumber			= $CityRPG::temp::citydata::datumlumber;
		%so.lotListings	= $CityRPG::temp::citydata::lotListings;
		%so.economy			= $Economics::Condition;

		%so.version			= $CityRPG::temp::citydata::version;
	}
	else
	{
		%so.version = $City::Version;
		%so.value["minerals"] = 0;
		%so.value["lumber"] = 0;
		%so.value["lotListings"] = "";
		%so.value["economy"] = 0;
	}
}

function CitySO::saveData(%so)
{
	// Always override the version with the current one.
	$CityRPG::temp::citydata::version							= $City::Version;

	$CityRPG::temp::citydata::datum["minerals"]		= %so.minerals;
	$CityRPG::temp::citydata::datum["lumber"]		= %so.lumber;
	$CityRPG::temp::citydata::lotListings = %so.lotListings;
	export("$CityRPG::temp::citydata::*", $City::SavePath @ "City.cs");
}

if(!isObject(CitySO))
{
	new scriptObject(CitySO) { };
	CitySO.loadData();

	// Until the saver is replaced, we sadly have to worry about save data compatibility.
	if(CitySO.version != $City::Version || CitySO.version $= "")
	{
		// Set a flag to display a warning for the host when they join.
		$City::DisplayVersionWarning = 1;

		echo("-----------------------------------------------------------" NL $City::VersionWarning NL "-----------------------------------------------------------");
	}
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
	if(isFile($City::SavePath @ "Calendar.cs"))
	{
		exec($City::SavePath @ "Calendar.cs");
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
	export("$CityRPG::temp::calendar::*", $City::SavePath @ "Calendar.cs");
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
		%outfit = City.get(%client.bl_id, "outfit");

		for(%a = 0; %a < getWordCount(%outfit); %a++)
		{
			if(getWord(%so.str[%item], %a) $= "keep")
				%newOutfit = (%newOutfit $= "" ? getWord(%outfit, %a) : %newOutfit SPC getWord(%outfit, %a));
			else if(getWord(%so.str[%item], %a) $= "this")
				%newOutfit = (%newOutfit $= "" ? %item : %newOutfit SPC %item);
			else
				%newOutfit = (%newOutfit $= "" ? getWord(%so.str[%item], %a) : %newOutfit SPC getWord(%so.str[%item], %a));
		}

		City.set(%client.bl_id, "outfit", %newOutfit);
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
