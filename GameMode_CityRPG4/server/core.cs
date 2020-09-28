// ============================================================
// General Game Functions
// ============================================================

// Client.cityMenuOpen(names, functions, exitMsg, autoClose)
// Modular function hook for displaying menus in-game.
// Currently utilizes chat just like classic CityRPG, however this is subject to change.
function GameConnection::cityMenuOpen(%client, %menu, %functions, %menuID, %exitMsg, %autoClose)
{
	if(%client.cityMenuOpen)
	{
		return;
	}

	if(%menuID $= "")
	{
		error("CityRPG 4 - Attempting to open a menu with no ID. Aborting.");
		echo("Menu data: " @ %menu);

		return;
	}

	// Account for empty menus
	if(%menu !$= "")
		messageClient(%client, '', "\c6Type a number in chat:");

	for(%i = 0; %i < getFieldCount(%menu); %i++)
	{
		messageClient(%client, '', "\c3" @ %i+1 @ " \c6- " @ getField(%menu, %i));
	}

	// Set the necessary values to the client
	%client.cityMenuOpen = true; // Package checks for this
	%client.cityMenuFunction = %functions;
	%client.cityMenuAutoClose = %autoClose;
	%client.cityMenuID = %menuID;
	%client.cityMenuExitMsg = %exitMsg;

	// Event
	if(isObject(%menuID) && %menuID.getClassName() $= "fxDTSBrick")
	{
		%menuID.onMenuOpen();
	}

	// Return to indicate everything went smoothly.
	return true;
}

// Client.cityMenuInput(input)
// Called when a user enters an input for a city menu.
function GameConnection::cityMenuInput(%client, %input)
{
	// Event
	if(isObject(%client.cityMenuID) && %client.cityMenuID.getClassName() $= "fxDTSBrick")
	{
		%client.cityMenuID.onMenuInput();
	}

	// If there's multiple fields, this is a numbered menu.
	if(getFieldCount(%client.cityMenuFunction) > 1)
	{
		%function = getField(%client.cityMenuFunction, %input-1);

		if(isFunction(%function))
		{
			%id = %client.cityMenuID;
			if(%client.cityMenuAutoClose)
				%client.cityMenuClose();

			// It's important to do this after closing in case the function we're calling opens another menu.
			call(%function, %client, %input, %id);

			return true;
		}
		else
		{
			messageClient(%client, '', "\c3" @ %input @ "\c6 is not a valid option. Please try again.");
			return false;
		}
	}
	else
	{
		%function = %client.cityMenuFunction;
		// Not a numbered input, call the specified function, passing the client object and message.
		if(isFunction(%function))
		{
			call(%function, %client, %input, %client.cityMenuID);
		}
	}
}

// Client.cityMenuClose(silent)
// silent: (bool) If set to true, the exit message will not show even if defined.
function GameConnection::cityMenuClose(%client, %silent)
{
	if(%client.cityMenuOpen)
	{
		if(!%silent)
			%client.cityMenuMessage(%client.cityMenuExitMsg);

		// Event
		if(isObject(%client.cityMenuID) && %client.cityMenuID.getClassName() $= "fxDTSBrick")
		{
			%client.cityMenuID.onMenuClose();
		}

		%client.cityMenuOpen = false;
		%client.cityMenuFunction = "";
		%client.cityMenuID = "";
		%client.cityMenuExitMsg = "";
		%client.cityMenuAutoClose = "";
	}
}

// Client.cityMenuClose(silent)
// msg: (str) Message to display to the client. Accepts color codes (\c3, etc.)
function GameConnection::cityMenuMessage(%client, %msg)
{
	// We're using messageClient for now--simple enough, but this is subject to change.
	messageClient(%client, '', %msg);
}

// Hook functions (CityMenu_*) - These functions are used within menus.
function CityMenu_Close(%client)
{
	%client.cityMenuClose();
}
function CityMenu_Placeholder(%client)
{
	%client.cityMenuMessage("\c6Sorry, this feature is currently not available. Please try again later.");
}

// City_AddDemerits(blid, demerits)
// Gives demerits to a player. Handles wanted levels and demotions.
function City_AddDemerits(%blid, %demerits)
{
	%demerits = mFloor(%demerits);
	%currentDemerits = CityRPGData.getData(%blid).valueDemerits;
	%maxStars = City_GetMaxStars();

	CityRPGData.getData(%blid).valueDemerits += %demerits;

	if(CityRPGData.getData(%blid).valueDemerits >= $Pref::Server::City::demerits::demoteLevel && JobSO.job[CityRPGData.getData(%blid).valueJobID].law == true)
	{
		CityRPGData.getData(%blid).valueJobID = 1;
		CityRPGData.getData(%blid).valueJailData = 1 SPC 0;

		%client = findClientByBL_ID(%blid);

		if(isObject(%client))
		{
			messageClient(%client, '', "\c6You have been demoted to" SPC City_DetectVowel(JobSO.job[1].name) SPC "\c3" @ JobSO.job[1].name @ "\c6.");

			%client.setInfo();

			if(isObject(%client.player))
			{
				serverCmdunUseTool(%client);

				%client.player.giveDefaultEquipment();
			}
		}
	}

	if(%client = findClientByBL_ID(%blid))
	{
		%client.setInfo();

		if(%client.getWantedLevel())
		{
			%ticks = %client.getWantedLevel();

			if(%ticks > %maxStars)
			{
				if(%maxStars == 3 || %maxStars == 6)
					messageAll('', '\c6Criminal \c3%1\c6 has obtained a level \c3%2\c6 wanted level. Police vehicles have upgraded.', %client.name, %ticks);
			}
		}
	}
}

// City_DetectVowel(word)
// Detects a vowel in a string and returns "a" or "an" accordingly.
function City_DetectVowel(%word)
{
	%letter = strLwr(getSubStr(%word, 0, 1));

	if(%letter $= "a" || %letter $= "e" || %letter $= "i" || %letter $= "o" || %letter $= "u")
		return "an";
	else
		return "a";
}

function City_FindSpawn(%search, %id)
{
	%search = strlwr(%search);
	%fullSearch = %search @ (%id ? " " @ %id : "");

	for(%a = 0; %a < getWordCount($CityRPG::temp::spawnPoints); %a++)
	{
		%brick = getWord($CityRPG::temp::spawnPoints, %a);

		if(isObject(%brick))
		{
			%spawnData = strLwr(%brick.getDatablock().spawnData);

			if(%search $= %spawnData && %spawnData $= "personalspawn")
			{
				%ownerID = getBrickGroupFromObject(%brick).bl_id;

				if(%fullSearch $= (%spawnData SPC %ownerID))
					%possibleSpawns = (%possibleSpawns $= "") ? %brick : %possibleSpawns SPC %brick;
			}
			else if(%fullSearch $= %spawnData)
				%possibleSpawns = (%possibleSpawns $= "") ? %brick : %possibleSpawns SPC %brick;
		}
		else
		{
			$CityRPG::temp::spawnPoints = removeWord($CityRPG::temp::spawnPoints, %a);
			%a--;
		}
	}

	if(%possibleSpawns !$= "")
	{
		%spawnBrick = getWord(%possibleSpawns, getRandom(0, getWordCount(%possibleSpawns) - 1));
		%cords = vectorSub(%spawnBrick.getWorldBoxCenter(), "0 0" SPC (%spawnBrick.getDatablock().brickSizeZ - 3) * 0.1) SPC getWords(%spawnBrick.getTransform(), 3, 6);
		return %cords;
	}
	else
		return false;
}

function City_GetMaxStars()
{
	for(%a = 0; %a < ClientGroup.getCount(); %a++)
	{
		%subClient = ClientGroup.getObject(%a);
		%theirStars = %subClient.getWantedLevel();
		if(%theirStars > %maxStars)
			%maxStars = %theirStars;
	}

	return (%maxStars $= "" ? 0 : %maxStars);
}

function City_GetMostWanted()
{
	%maxStars = City_GetMaxStars();
	for(%a = 0; %a < Clientgroup.getCount(); %a++)
	{
		%subClient = ClientGroup.getObject(%a);
		if(%subClient.getWantedLevel() == %maxStars)
			%mostWanted = %subClient;
	}

	return (isObject(%mostWanted) ? %mostWanted : 0);
}

function City_illegalAttackTest(%atkr, %vctm)
{
	if(isObject(%atkr) && isObject(%vctm) && %atkr.getClassName() $= "GameConnection" && %vctm.getClassName() $= "GameConnection")
	{
		if(%atkr != %vctm)
		{
			if(CityRPGData.getData(%vctm.bl_id).valueBounty && %atkr.getJobSO().bountyClaim)
				return false;
			else if(!%vctm.getWantedLevel())
				return true;
		}
	}

	return false;
}

function City_Tick_Econ()
{
	$City::Economics::replayCount = $City::Economics::replayCount + 1;
	$City::Economics::randomUporDown = getRandom(1,5);
	$City::Economics::positiveNegative = getRandom(1,2);

	if($Pref::Server::City::Economics::Relay < 1)
		$Pref::Server::City::Economics::Relay = ClientGroup.getCount();

	if($City::Economics::replayCount > $Pref::Server::City::Economics::Relay)
	{
		if($City::Economics::Condition > $Pref::Server::City::Economics::Greatest)
		{
			$City::Economics::Condition = $City::Economics::Condition - $City::Economics::randomUporDown;
			$City::Economics::replayCount = 0;
		}
		else if($City::Economics::Condition < $Pref::Server::City::Economics::Least)
		{
			$City::Economics::Condition = $City::Economics::Condition + $City::Economics::randomUporDown;
			$City::Economics::replayCount = 0;
		}
		else if($City::Economics::positiveNegative == 1)
		{
			$City::Economics::Condition = $City::Economics::Condition + $City::Economics::randomUporDown;
			$City::Economics::replayCount = 0;
		}
		else if($City::Economics::positiveNegative == 2)
		{
			$City::Economics::Condition = $City::Economics::Condition - $City::Economics::randomUporDown;
			$City::Economics::replayCount = 0;
		}
	}

	if($City::Economics::Condition > $Pref::Server::City::Economics::Cap)
	{
		$City::Economics::Condition = $Pref::Server::City::Economics::Cap;
	}

	if($City::Economics::Condition $= "")
	{
		error("ERROR: GameMode_CityRPG4 - Economics condition is blank! Resetting to 0.");
		$City::Economics::Condition = 0;
	}
}

function City_Tick(%brick)
{
	CalendarSO.date++;
	CityRPGData.lastTickOn = $Sim::Time;

	if($City::Mayor::String $= "" || $City::Mayor::String $= null)
		$Pref::Server::City::Mayor::Active = 0;

	if(CityRPGData.scheduleTick)
		cancel(CityRPGData.scheduleTick);

	%dateStr = CalendarSO.getDateStr();

	messageAll('', "<bitmap:" @ $City::DataPath @ "ui/time.png> \c6Today, on " @ %dateStr @ "\c6...");

	if(%so.holiday[CalendarSO.getCurrentDay()] !$= "")
		messageAll('', "\c6 -" SPC %so.holiday[%so.getCurrentDay()]);

	if($City::Economics::Condition > 0) {
		%econColor = "<color:00ee00>";
	}
	else {
		%econColor = "<color:ee0000>";
	}

	messageAll('', "\c6 - The current economy value is " @ %econColor @ $City::Economics::Condition @ "\c6%.");

	City_Init_Spawns();
	City_Tick_Econ();
	City_TickLoop(0);
	CityRPGData.scheduleTick = schedule((60000 * $Pref::Server::City::tick::speed), false, "City_Tick");

	if(CityRPGData.datacount > 0)
	{
		CityRPGData.saveData();
	}
	else
	{
		CityRPGData.dump();
		error("CityRPG data is blank or missing! Will not attempt to export. Data object has been dumped.");
	}

	CalendarSO.saveData();
	CitySO.saveData();

	CityMayor_refresh();

	echo("City: Calendar tick complete");
}

// Hunger string
function GameConnection::doCityHungerStatus(%client)
{
	%data = CityRPGData.getData(%client.bl_id);

	switch(%data.valueHunger) {
		case 1: %msg = "\c0You're extremely malnourished! You need to eat something immediately.";
		case 2: %msg = "\c0You're starving!";
		case 3: %msg = "You could really use something to eat.";
		case 4: %msg = "You're getting hungry.";
		case 5: %msg = "You could use a bite to eat.";
		case 6: %msg = "You could use a bite to eat.";
		case 7: %msg = "You're well fed.";
		case 8: %msg = "You're full on food.";
		case 9: %msg = "You're stuffed!";
		case 10: %msg = "You're completely stuffed!";
	}

	messageClient(%client, '', "\c6 - " @ %msg);

	if(%data.valueHunger < 5)
	{
		%client.centerPrint("\c6" @ %msg, 3);
	}
}

function GameConnection::doCityHungerEffects(%client)
{
	if(%client.isCityAdmin())
	{
		return;
	}

	%rand = getRandom(1,6);
	%data = CityRPGData.getData(%client.bl_id);

	if(isObject(%client.player) && %rand != 1 && %data.valueHunger < 3) {
		messageClient(%client, '', "\c6 - Hunger cramps sieze hold of your body...");
		%player = %client.player;

		if(!$Pref::Server::City::DisableHungerTumble)
		{
			%player.addVelocity("0 0 " @ getRandom(1,3));
			tumble(%player);
		}

		%damage = %player.dataBlock.maxDamage*0.50;
		%client.player.schedule(2000, damage, %player, %player.getPosition(), %damage, $DamageType::Starvation);
	}
}

function City_TickLoop(%loop)
{
	// Each tick loop applies to one client.
	%time = (($Pref::Server::City::tick::speed * 60000) / CityRPGData.dataCount);

	%client = findClientByBL_ID(CityRPGData.data[%loop].ID);

	if(isObject(%client))
	{
		%so = CityRPGData.getData(%client.bl_id);

		if(getWord(%so.valueJailData, 1))
		{
			if(%ticks = getWord(%so.valueJailData, 1) > 1)
			{
				%daysLeft = (getWord(%so.valueJailData, 1) - 1);
					if(%daysLeft > 1)
					%daySuffix = "s";

				messageClient(%client, '', '\c6 - You have \c3%1\c6 day%2 left in Prison.', %daysLeft, %daySuffix);
			}
		if(%so.valueHunger > 3)
			%so.valueHunger--;
		else
			%so.valueHunger = 3;
		}
		else
		{
			if(%client.hasSpawnedOnce)
			{
				if((CalendarSO.date % 2) == 0)
				{
					// No hunger effects for admin jobs
					if(!%client.isCityAdmin())
					{
						%so.valueHunger--;

						if(%so.valueHunger == 0)
							%so.valueHunger = 1;
					}

					if(isObject(%client.player))
						%client.player.setScale("1 1 1");
				}

				%client.doCityHungerStatus();

				if(%so.valueHunger < 3) {
					%client.schedule(getRandom(15000,120000), doCityHungerEffects);
				}
			}

		if(%so.valueDemerits > 0 && isObject(%client.player))
		{
			if(%so.valueDemerits >= $Pref::Server::City::demerits::reducePerTick)
				 %so.valueDemerits -= $Pref::Server::City::demerits::reducePerTick;
			else
				%so.valueDemerits = 0;

			if(calendarSO.getCurrentDay() == 187)
				messageClient(%client, '', '\c6 - You have had your demerits reduced to \c3%1\c6 due to <a:https://www.youtube.com/watch?v=iq8gfaFqFpI>Statue of Limitations</a>\c6.', %so.valueDemerits);
			else
				messageClient(%client, '', '\c6 - You have had your demerits reduced to \c3%1\c6 due to <a:en.wikipedia.org/wiki/Statute_of_limitations>Statute of Limitations</a>\c6.', %so.valueDemerits);

			%client.setInfo();
		}

		if(!%so.valueStudent)
		{
			if(%client.getSalary() > 0)
			{
				if(CityRPGData.getData(%client.bl_id).valueJobID == $City::MayorJobID)
				{
					if(%client.bl_id !$= $City::Mayor::ID)
					{
						jobset(%client, $City::CivilianJobID);
						%client.colorName = "";
						// return;
					}
				}

				%sume = $City::Economics::Condition / 100;
				%osum = %client.getSalary();
				%sum = (%osum * %sume) + %osum;
				%sum = mFloor(%sum);

				if($City::Economics::Condition > 25)
					%include = 0.005;
				else
					%include = -0.025;

				if(CityRPGData.data[%loop].valueHunger < 3 && CityRPGData.data[%loop].valueBank > 30)
				{
					messageClient(%client,'',"\c6 - You were unable to collect your paycheck because you are starving.");
				}
				else if(%sum > 0)
				{
					%client.cityLog("Tick pay: " @ %sum);
					%so.valueBank += %sum;
					messageClient(%client, '', "\c6 - Your paycheck of \c3$" @ %sum @ "\c6 has been deposited into your bank account.");
				}
			}
		}
		else
		{
			%so.valueStudent--;
			if(!%so.valueStudent)
			{
				%so.valueEducation++;
				messageClient(%client, '', "\c6 - \c2You graduated\c6, receiving a level \c3" @ %so.valueEducation @ "\c6 education!");
				%client.cityLog("Tick edu +1");
			}
			else
				messageClient(%client, '', "\c6 - You will complete your education in \c3" @ %so.valueStudent @ "\c6 days.");
			}
		}

		CityRPGData.getData(%client.bl_id).valueMoney = mFloor(CityRPGData.getData(%client.bl_id).valueMoney);
		CityRPGData.getData(%client.bl_id).valueName = %client.name;

		if(isObject(%client.player))
		{
			%client.player.setShapeNameDistance(24);
			%client.setGameBottomPrint();
		}

		if(CalendarSO.date && CalendarSO.date % $CityRPG::tick::interestTick == 0)
		{
			CityRPGData.data[%loop].valueBank = mFloor(CityRPGData.data[%loop].valueBank * $CityRPG::tick::interest);

			if(isObject(%client))
				messageClient(%client, '', "\c6 - The bank is giving interest.");
		}

		if(getWord(CityRPGData.data[%loop].valueJailData, 1))
		{
			CityRPGData.data[%loop].valueJailData = 1 SPC (getWord(CityRPGData.data[%loop].valueJailData, 1) - 1);

			if(isObject(%client))
			{
				if(!getWord(CityRPGData.getData(CityRPGData.data[%loop].ID).valueJailData, 1))
				{
					%client.cityLog("Tick jail ended");
					messageClient(%client, '', "\c6 - You got out of prison.");
					%client.spawnPlayer();
				}
			}
		}
	}

	if(%loop < CityRPGData.dataCount)
		schedule(%time, false, "City_TickLoop", (%loop + 1));
}

function messageAllOfJob(%job, %type, %message)
{
	for(%a = 0; %a < ClientGroup.getCount(); %a++)
	{
		%subClient = ClientGroup.getObject(%a);
		if(%subClient.getJobSO().id == %job)
		{
			messageClient(%subClient, %type, %message);
			%sent++;
		}
	}

	return (%sent !$= "" ? %sent : 0);
}

function CityMenu_ResetAllJobsPrompt(%client)
{
	%client.cityMenuMessage("\c6Are you sure you want to reset all jobs? This action will affect every player on the server, online and offline.");
	%client.cityMenuMessage("\c6All players will be changed back to the Civilian job. The server may temporarily freeze during this operation.");

	%client.cityLog("Reset all jobs prompt");

	%menu =	"Yes"
			TAB "No";

	%functions = 	"City_ResetAllJobs"
						TAB "CityMenu_Close";

	%client.cityMenuOpen(%menu, %functions, %brick, "\c6Job reset cancelled.");

}

function City_ResetAllJobs(%client)
{
	// Extra SA check for security
	if(!%client.isSuperAdmin)
	{
		return;
	}

	%client.cityLog("Reset all jobs");

	messageAll('',"\c3" @ %client.name @ "\c0 reset all jobs.");

	for(%i = 0; %i <= CityRPGData.dataCount; %i++)
	{
		%targetClient = findClientByBL_ID(CityRPGData.data[%i].id);

		if(%targetClient != 0)
		{
			jobset(%targetClient, $City::CivilianJobID);
		}
		else
		{
			CityRPGData.data[%i].valueJobID = 1;
		}
	}

	%client.cityMenuClose(1);
}

function GameConnection::messageCityLagNotice(%client)
{
	messageClient(%client, '', "\c3Notice: This server has " @ getBrickCount() @ " bricks.");
	messageClient(%client, '', "\c3If you experience lag, consider turning down your shaders, as well as your draw distance under Advanced options.");
}

function messageCityRadio(%jobTrack, %msgType, %msgString)
{
	echo("(" @ %jobTrack @ " Chat)" SPC %msgString);

	for(%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%client = ClientGroup.getObject(%i);

		if(%client.isCityAdmin() || // Admin job always sees radio..
		($Pref::Server::City::AdminsAlwaysMonitorChat && %client.isAdmin) || // Or if the pref is enabled, allow admin to snoop...
		(%client.getJobSO().track $= %jobTrack && // Otherwise, check for a matching job track...
		!getWord(CityRPGData.getData(%client.bl_id).valueJailData, 1))) // And exclude convicts from seeing messages in their track.
		{
			messageClient(%client, '', "\c3[<color:" @ $City::JobTrackColor[%jobTrack] @ ">" @ %jobTrack @ " Radio\c3]" SPC %msgString);
		}
	}
}
