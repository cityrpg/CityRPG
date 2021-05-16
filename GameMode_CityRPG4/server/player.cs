$City::StartingCash = 250;

// Spawn preference list
$City::SpawnPreferences = "Personal Spawn";
$City::SpawnPreferenceIDs = "Personal";

if($City::CheckpointIsActive)
{
	$City::SpawnPreferences = $City::SpawnPreferences TAB "Checkpoint";
	$City::SpawnPreferenceIDs = $City::SpawnPreferenceIDs TAB "Checkpoint";
}

$City::SpawnPreferences = $City::SpawnPreferences TAB "Job Spawn";
$City::SpawnPreferenceIDs = $City::SpawnPreferenceIDs TAB "Job";

function gameConnection::arrest(%client, %cop)
{
	%client.cityLog("Arrested by '" @ %cop.bl_id @ "'");
	%cop.cityLog("Arrest player '" @ %client.bl_id @ "'");

	%ticks = mCeil(%client.getWantedLevel()/2);

	%copSO = CityRPGData.getData(%cop.bl_id);
	%robSO = CityRPGData.getData(%client.bl_id);

	if(!getWord(%robSO.valueJailData, 1))
	{
		if(%client.player.currTool)
			serverCmddropTool(%client, %client.player.currTool);
	}

	%ticks += getWord(%robSO.valueJailData, 1);
	%reward = mFloor($CityRPG::prices::jailingBonus * %client.getWantedLevel());

	if(%reward > 600)
		%reward = 600;

	%copSO.valueMoney += %reward;

	if(%robSO.valuetotalhunger < 0)
	{
		commandToClient(%client, 'messageBoxOK', "You've been Jailed by" SPC %cop.name @ "!", 'You have been jailed for %1 tick%2.\n\nYou may either wait out your jail time in game and possibly earn money by laboring, or you may leave the server and return when your time is up.\nThe choice is yours.', %ticks, %ticks == 1 ? "" : "s");
		commandToClient(%cop, 'centerPrint', "\c6You have jailed \c3" @ %client.name SPC "\c6for \c3" @ %ticks SPC"\c6tick" @ ((%ticks == 1) ? "" : "s") @ ". You were rewarded \c3$" @ %reward @ "\c6.", 5);
	}
	else
		commandToClient(%client, 'messageBoxOK', "Jailed by" SPC %cop.name @ "!", 'You have been jailed for %1 tick%2.\nYou may either wait out your jail time in game and possibly earn money by laboring, or you may leave the server and return when your time is up.\nThe choice is yours.', %ticks, %ticks == 1 ? "" : "s");

	commandToClient(%cop, 'centerPrint', "\c6You have jailed \c3" @ %client.name SPC "\c6for \c3" @ %ticks SPC"\c6tick" @ ((%ticks == 1) ? "" : "s") @ ". You were rewarded \c3$" @ %reward @ "\c6.", 5);
	%robSO.valueJailData = 1 SPC %ticks;
	%robSO.valueDemerits = 0;
	%client.SetInfo();
	%cop.SetInfo();
	if(%client.getJobSO().law)
	{
		messageClient(%client, '', "\c6You have been demoted to" SPC City_DetectVowel(JobSO.job[1].name) SPC "\c3" @ JobSO.job[1].name SPC "\c6due to your jailing.");
		%robSO.valueJobID = $City::CivilianJobID;
	}

	if(%robSO.valueBounty > 0)
	{
		%cop.cityLog("Arrest player " @ %client.bl_id @ " with bounty: " @ %robSO.valueBounty);
		messageClient(%cop, '', "\c6Wanted man was apprehended successfully. His bounty money has been wired to your bank account.");
		%copSO.valueBank += %robSO.valueBounty;
		%robSO.valueBounty = 0;
	}

	if(%robSO.valueHunger < 3)
		%robSO.valueHunger = 3;

	if(isObject(%client.player.tempBrick))
		%client.player.tempBrick.delete();

	%client.spawnPlayer();

	if(%ticks == City_GetMaxStars())
	{
		%maxWanted = City_GetMostWanted();

		if(%maxWanted)
			messageAll('', '\c6The \c3%1-star\c6 criminal \c3%2\c6 was arrested by \c3%5\c6, but \c3%3-star\c6 criminal \c3%4\c6 is still at large!', %ticks, %client.name, %maxWanted.getWantedLevel(), %maxWanted.name, %cop.name);
		else
			messageAll('', '\c6With the apprehension of \c3%1-star\c6 criminal \c3%2\c6 by \c3%3\c6, the City returns to a peaceful state.', %ticks, %client.name, %cop.name);
	}
	else
		messageAll('', '\c3%1\c6 was jailed by \c3%2\c6 for \c3%3\c6 ticks.', %client.name, %cop.name, %ticks);
}

function gameConnection::buyResources(%client)
{
	%totalResources = getWord(CityRPGData.getData(%client.bl_id).valueResources, 0) + getWord(CityRPGData.getData(%client.bl_id).valueResources, 1);
	if(mFloor(%totalResources * $CityRPG::prices::resourcePrice) > 0)
	{
		%payout = mFloor(%totalResources * $CityRPG::prices::resourcePrice);

		if(!getWord(CityRPGData.getData(%client.bl_id).valueJailData, 1))
		{
			%client.cityLog("Resource sell for " @ %payout);

			City.add(%client.bl_id, "money", %payout);
			messageClient(%client, '', "\c6The state has bought all of your resources for \c3$" @ %payout @ "\c6.");
		}
		else
		{
			%client.cityLog("Resource sell (jail) for " @ %payout);
			City.add(%client.bl_id, "bank", %payout);
			messageClient(%client, '', '\c6The state has set aside \c3$%1\c6 for when you get out of Prison.', %payout);
		}

		CitySO.lumber += getWord(CityRPGData.getData(%client.bl_id).valueResources, 0);
		CitySO.minerals += getWord(CityRPGData.getData(%client.bl_id).valueResources, 1);

		City.set(%client.bl_id, "resources", "0 0");

		%client.SetInfo();
	}
}

function gameConnection::setInfo(%client)
{
	City.set(%client.bl_id, "money", mFloor(CityRPGData.getData(%client.bl_id).valueMoney));
	City.set(%client.bl_id, "name", %client.name);

	if(isObject(%client.player))
	{
		%client.player.setShapeNameDistance(24);
		%client.player.setShapeNameColor(24);

		%client.setGameBottomPrint();
	}
}

function gameConnection::setGameBottomPrint(%client)
{
	if(%client.cityHUDTimer > $sim::time) {
		return;
	}

	%mainFont = "<font:palatino linotype:24>";

	%client.CityRPGPrint = %mainFont;

	if(!isObject(%client.player))
		%health = 0;
	else
		%health = mFloor(100 - %client.player.getDamageLevel());

	%client.CityRPGPrint = %client.CityRPGPrint @ "<bitmap:" @ $City::DataPath @ "ui/health.png>\c6 Health:" SPC %health @ "%";

	%client.CityRPGPrint = %client.CityRPGPrint @ "   <bitmap:" @ $City::DataPath @ "ui/cash.png>\c6 Cash:" SPC %client.getCashString();

	// TODO: Move wanted level to center print so this doesn't cut off the bottom HUD
	//%client.CityRPGPrint = %client.CityRPGPrint @ "   <bitmap:" @ $City::DataPath @ "ui/hunger.png>\c6 Hunger: Well-fed";

	// Placeholder
	//%client.CityRPGPrint = %client.CityRPGPrint @ "<just:right>\c6Day";

	//IMPORTANT: Wanted level must be last because it shows up on a new line
	if(CityRPGData.getData(%client.bl_id).valueDemerits >= $Pref::Server::City::demerits::wantedLevel)
	{
		%stars = %client.getWantedLevel();
		%client.CityRPGPrint = %client.CityRPGPrint SPC "<br><just:center><font:Impact:64><color:ffff00>";

		for(%a = 0; %a < %stars; %a++)
			%client.CityRPGPrint = %client.CityRPGPrint @ "*";

		%client.CityRPGPrint = %client.CityRPGPrint @ "<color:888888>";
		for(%a = %a; %a < 6; %a++)
			%client.CityRPGPrint = %client.CityRPGPrint @ "*";

		%client.CityRPGPrint = %client.CityRPGPrint;
	}

	commandToClient(%client, 'bottomPrint', %client.CityRPGPrint, 0, true);

	return %client.CityRPGPrint;
}

function gameConnection::applyForcedBodyColors(%client)
{
	if(isObject(CityRPGData.getData(%client.bl_id)))
	{
		if(getWord(CityRPGData.getData(%client.bl_id).valueJailData, 1) > 0)
			%outfit = "none none none none jumpsuit jumpsuit skin jumpsuit jumpsuit";
		else if(%client.getJobSO().outfit !$= "")
			%outfit = %client.getJobSO().outfit;
		else
			%outfit = CityRPGData.getData(%client.bl_id).valueOutfit;
	}

	if(%outfit !$= "")
	{
		%client.accentColor		= ClothesSO.getColor(%client, getWord(%outfit, 0));
		%client.hatColor		= ClothesSO.getColor(%client, getWord(%outfit, 1));

		%client.packColor		= ClothesSO.getColor(%client, getWord(%outfit, 2));
		%client.secondPackColor		= ClothesSO.getColor(%client, getWord(%outfit, 3));

		%client.chestColor		= ClothesSO.getColor(%client, getWord(%outfit, 4));

		%client.rarmColor		= ClothesSO.getColor(%client, getWord(%outfit, 5));
		%client.larmColor		= ClothesSO.getColor(%client, getWord(%outfit, 5));
		%client.rhandColor		= ClothesSO.getColor(%client, getWord(%outfit, 6));
		%client.lhandColor		= ClothesSO.getColor(%client, getWord(%outfit, 6));

		%client.hipColor		= ClothesSO.getColor(%client, getWord(%outfit, 7));

		%client.rlegColor		= ClothesSO.getColor(%client, getWord(%outfit, 8));
		%client.llegColor		= ClothesSO.getColor(%client, getWord(%outfit, 8));

		%client.applyBodyColors();
	}
}

function gameConnection::applyForcedBodyParts(%client)
{
	if(isObject(CityRPGData.getData(%client.bl_id)))
	{
		if(getWord(CityRPGData.getData(%client.bl_id).valueJailData, 1) > 0)
			%outfit = "none none none none jumpsuit jumpsuit skin jumpsuit jumpsuit";
		else if(%client.getJobSO().outfit !$= "")
			%outfit = %client.getJobSO().outfit;
		else
			%outfit = CityRPGData.getData(%client.bl_id).valueOutfit;
	}

	if(%outfit !$= "")
	{
		%client.accent = ClothesSO.getNode(%client, getWord(%outfit, 0));
		%client.hat	= ClothesSO.getNode(%client, getWord(%outfit, 1));

		%client.pack	= ClothesSO.getNode(%client, getWord(%outfit, 2));
		%client.secondPack	= ClothesSO.getNode(%client, getWord(%outfit, 3));

		%client.chest	= ClothesSO.getNode(%client, getWord(%outfit, 4));

		%client.rarm	= ClothesSO.getNode(%client, getWord(%outfit, 5));
		%client.larm	= ClothesSO.getNode(%client, getWord(%outfit, 5));
		%client.rhand	= ClothesSO.getNode(%client, getWord(%outfit, 6));
		%client.lhand	= ClothesSO.getNode(%client, getWord(%outfit, 6));

		%client.hip	= ClothesSO.getNode(%client, getWord(%outfit, 7));

		%client.rleg = ClothesSO.getNode(%client, getWord(%outfit, 8));
		%client.lleg = ClothesSO.getNode(%client, getWord(%outfit, 8));

		%client.faceName = ClothesSO.getDecal(%client, "face", getWord(%outfit, 9));
		%client.decalName = ClothesSO.getDecal(%client, "chest", getWord(%outfit, 10));

		%client.applyBodyParts();
	}
}

function gameConnection::cityLotDisplay(%client, %lotBrick)
{
	%lotStr = "<just:right><font:palatino linotype:18>\c6" @ %lotBrick.getCityLotName();

	%duration = 2;
	if(%lotBrick.getCityLotOwnerID() == -1)
	{
		%lotStr = %lotStr @ "<br>\c2For sale!\c6 Type /lot for info";
	}
	else if(%lotBrick.getCityLotPreownedPrice() != -1)
	{
		%lotStr = %lotStr @ "<br>\c2For sale by owner!\c6 Type /lot for info";
		%duration = 3;
	}

	%client.centerPrint(%lotStr, %duration);
}

// Get Functions
function gameConnection::getCashString(%client)
{
	if(CityRPGData.getData(%client.bl_id).valueMoney >= 0)
		%money = "\c6$" @ strFormatNumber(CityRPGData.getData(%client.bl_id).valueMoney);
	else
		%money = "\c0($" @ strreplace(strFormatNumber(CityRPGData.getData(%client.bl_id).valueMoney), "-", "")  @ ")";

	return %money;
}

function gameConnection::getJobSO(%client)
{
	return JobSO.job[%client.getJobID()];
}

function gameConnection::getJobID(%client)
{
	return CityRPGData.getData(%client.bl_id).valueJobID;
}

function gameConnection::getEvidence(%client)
{
	return CityRPGData.getData(%client.bl_id).valueevidence;
}

function gameConnection::getSalary(%client)
{
	return %client.getJobSO().pay;
}

function gameConnection::getWantedLevel(%client)
{
	%data = CityRPGData.getData(%client.bl_id);

	if(%data.valueDemerits >= $Pref::Server::City::demerits::wantedLevel)
	{
		%div = %data.valueDemerits / $Pref::Server::City::demerits::wantedLevel;

	if(%div <= 3)
		return 1;
	else if(%div <= 8)
		return 2;
	else if(%div <= 14)
		return 3;
	else if(%div <= 21)
		return 4;
	else if(%div <= 29)
		return 5;
	else
		return 6;
	}
	else
		return 0;
}

function gameConnection::ifWanted(%client)
{
	if(CityRPGData.getData(%client.bl_id).valueDemerits >= $Pref::Server::City::demerits::wantedLevel)
		return true;
	else
		return false;
}

function gameConnection::doReincarnate(%client)
{
	CityRPGData.removeData(%client.bl_id);
	CityRPGData.addData(%client.bl_id);
	City.set(%client.bl_id, "reincarnated", 1);
	City.set(%client.bl_id, "education", $City::EducationReincarnateLevel);

	if(isObject(%client.player))
	{
		%client.spawnPlayer();
	}

	messageAllExcept(%client, '', '\c3%1\c6 has been reincarnated!', %client.name);
	messageClient(%client, '', "\c6You have been reincarnated.");
}

// Sell Functions

// Client.sellFood(sellerID, servingID, foodName, price, profit)
// (EVENT)
// Sells food from the player 'sellerID' to 'client'.
function gameConnection::sellFood(%client, %sellerID, %servingID, %foodName, %price, %profit)
{
	if(CityRPGData.getData(%client.bl_id).valueMoney >= %price)
	{
		if(CityRPGData.getData(%client.bl_id).valueHunger < 10)
		{
			%portionName = strreplace($CityRPG::portion[%servingID], "_", " ");

			if(JobSO.job[CityRPGData.getData(%sellerID).valueJobID].sellFood || %sellerID.isAdmin)
			{
				%client.cityLog("Evnt buy food " @ %servingID @ " for " @ %price @ " from " @ %sellerID);

				switch(CityRPGData.getData(%client.bl_id).valueHunger)
				{
					case 1: %eatName = "vaccuum down";
					case 2: %eatName = "devour";
					case 3: %eatName = "devour";
					case 4: %eatName = "hungrily consume";
					case 5: %eatName = "consume";
					case 6: %eatName = "consume";
					case 7: %eatName = "take a bite of";
					case 8: %eatName = "nibble on";
					case 9: %eatName = "nibble on";
					default: %eatName = "somehow managed to break";
				}

				messageClient(%client, '', '\c6You %1 %2 \c3%3\c6 serving of \c3%4\c6.', %eatName, City_DetectVowel(%portionName), %portionName, %foodName);
				City.add(%client.bl_id, "hunger", %servingID);

				%client.player.setHealth(%client.player.getdataBlock().maxDamage);

				if(CityRPGData.getData(%client.bl_id).valueHunger > 10)
				{
					City.set(%client.bl_id, "hunger", 10);
				}

				City.subtract(%client.bl_id, "money", %price);
				City.add(%sellerID, "bank", %profit);

				if(%profit)
				{
					if(isObject(%seller = findClientByBL_ID(%sellerID)))
					{
						messageClient(%seller, '', '\c6You just gained \c3$%1\c6 for providing \c3%3\c6 to \c3%2\c6.', %profit, %client.name, %foodName);
					}
				}

				%client.player.setScale("1 1 1");
				%client.setInfo();
				%client.player.serviceOrigin.onTransferSuccess(%client);
			}
			else
				messageClient(%client, '', "\c6This vendor is not liscensed to sell food.");
		}
		else
			messageClient(%client, '', "\c6You are too full to even think about buying any more food.");
	}
	else
		messageClient(%client, '', "\c6You don't have enough money to buy this food.");
}

// Client.sellItem(sellerID, itemID, price, profit)
// (EVENT)
// Sells item 'itemID' from 'sellerID' to 'client'.
function gameConnection::sellItem(%client, %sellerID, %itemID, %price, %profit)
{
	if(isObject(%client.player) && CityRPGData.getData(%client.bl_id).valueMoney >= %price)
	{
		if(JobSO.job[CityRPGData.getData(%client.player.serviceOrigin.getGroup().bl_id).valueJobID].sellItems)
		{
			for(%a = 0; %a < %client.player.getDatablock().maxTools; %a++)
			{
				if(!isObject(%obj.tool[%a]) || %obj.tool[%a].getName() !$= $CityRPG::prices::weapon::name[%itemID])
				{
					if(%freeSpot $= "" && %client.player.tool[%a] $= "")
					{
						%freeSpot = %a;
					}
				}
				else
				{
					%alreadyOwns = true;
				}
			}

			if(%freeSpot !$= "" && !%alreadyOwns)
			{
				%client.cityLog("Evnt buy item " @ %itemID @ " for " @ %price @ " from " @ %sellerID);

				City.subtract(%client.bl_id, "money", %price);
				City.add(%sellerID, "bank", %profit);
				CitySO.minerals -= $CityRPG::prices::weapon::mineral[%itemID];

				%client.player.tool[%freeSpot] = $CityRPG::prices::weapon::name[%itemID].getID();
				messageClient(%client, 'MsgItemPickup', "", %freeSpot, %client.player.tool[%freeSpot]);

				messageClient(%client, '', "\c6You have accepted the item's fee of \c3$" @ %price @ "\c6!");
				%client.setInfo();

				if(%client.player.serviceOrigin.getGroup().client)
					messageClient(%client.player.serviceOrigin.getGroup().client, '', '\c6You gained \c3$%1\c6 selling \c3%2\c6 an item.', %profit, %client.name);

				%client.player.serviceOrigin.onTransferSuccess(%client);
			}
			else if(%alreadyOwns)
				messageClient(%client, '', "\c6You don't need another\c3" SPC $CityRPG::prices::weapon::name[%itemID].uiName @ "\c6.");
			else if(%freeSpot $= "")
				messageClient(%client, '', "\c6You don't have enough space to carry this item!");
		}
		else
			messageClient(%client, '', "\c6This vendor is not liscensed to sell items.");
	}
}

// Client.sellClothes(sellerID, brick, item, price)
// (EVENT)
// Sells clothing item 'item' from 'sellerID' to 'client'.
function gameConnection::sellClothes(%client, %sellerID, %brick, %item, %price)
{
	if(isObject(%client.player) && CityRPGData.getData(%client.bl_id).valueMoney >= %price)
	{
		if(JobSO.job[CityRPGData.getData(%client.player.serviceOrigin.getGroup().bl_id).valueJobID].sellClothes  || %sellerID.isAdmin)
		{
			messageClient(%client, '', "\c6Enjoy the new look!");
			%client.cityLog("Evnt buy clothing " @ %item @ " for " @ %price @ " from " @ %sellerID);
			City.subtract(%client.bl_id, "money", %price);
			City.add(%sellerID, "bank", %price);
			ClothesSO.giveItem(%client, %item);

			if(%price)
			{
				if(isObject(%seller = FindClientByBL_ID(%sellerID)))
				{
					messageClient(%seller, '', '\c6You just gained \c3$%1\c6 for selling clothes to \c3%2\c6.', %price, %client.name);
				}
			}

			%client.applyForcedBodyColors();
			%client.applyForcedBodyParts();

			%client.setInfo();
		}
		else
			messageClient(%client, '', "\c6This vendor is not liscensed to sell clothes.");
	}
}

function player::giveDefaultEquipment(%this)
{
	if(!getWord(CityRPGData.getData(%this.client.bl_id).valueJailData, 1))
	{
		%tools = ($Pref::Server::City::giveDefaultTools ? $Pref::Server::City::defaultTools @ " " : "") @ %this.client.getJobSO().tools;

		for(%a = 0; %a < %this.getDatablock().maxTools; %a++)
		{
			if(!isObject(getWord(%tools, %a)))
			{
				%this.tool[%a] = "";
				messageClient(%this.client, 'MsgItemPickup', "", %a, 0);
			}
			else
			{
				%this.tool[%a] = nameToID(getWord(%tools, %a));
				messageClient(%this.client, 'MsgItemPickup', "", %a, nameToID(getWord(%tools, %a)));
			}

		}
	}
	else
	{
		for(%a = 0; %a < %this.getDatablock().maxTools; %a++)
		{
			if(isObject($CityRPG::demerits::jail::item[%a]))
			{
				%tool = $CityRPG::demerits::jail::item[%a];
			}
			else
			{
				%tool = "";
			}

			%this.tool[%a] = nameToID(%tool);
			messageClient(%this.client, 'MsgItemPickup', "", %a, nameToID(%tool));
		}
	}
}

function resetFree(%client)
{
	%client.cityLog("***Account auto-reset***");
	if(CityRPGData.getData(%client.bl_id))
		CityRPGData.removeData(%client.bl_id);
	CityRPGData.addData(%client.bl_id);

	City.set(%client.bl_id, "bank", $City::StartingCash);

	%client.setCityJob($City::CivilianJobID, 1, 1);

	if(isObject(%client.player))
	{
		%client.spawnPlayer();
	}
}

// Education startingCash
$CityRPG::EducationStr["-1"] = "MyFreeCollegeDegree.com Official Certificate";
$CityRPG::EducationStr[0] = "High School Diploma";
$CityRPG::EducationStr[8] = "Divine Degree";

function GameConnection::getCityEnrollCost(%client)
{
	return (CityRPGData.getData(%client.bl_id).valueEducation + 1) * 250;
}

function GameConnection::cityEnroll(%client)
{
	if(!isObject(%client.player) || CityRPGData.getData(%client.bl_id).valueEducation >= $City::EducationCap)
		return;

	%price = %client.getCityEnrollCost();

	// Ensure the player is not already enrolled
	if(!CityRPGData.getData(%client.bl_id).valueStudent)
	{
		if(CityRPGData.getData(%client.bl_id).valueMoney >= %price)
		{
			%valueStudent = CityRPGData.getData(%client.bl_id).valueEducation + 1;
			// Number of days to complete
			City.set(%client.bl_id, "student", %valueStudent);
			// Cost
			City.subtract(%client.bl_id, "money", %price);

			messageClient(%client, '', "\c6You are now enrolled. You will complete your education in \c3" @ %valueStudent @ "\c6 days.");
			%client.setInfo();

			%client.cityLog("Enroll for edu worth " @ %price);
		}
		else
		{
			messageClient(%client, '', "\c6It costs \c3$" @ %price SPC "\c6to get enrolled. You do not have enough money.");
		}
	}
}

function GameConnection::getCityRecordClearCost(%client)
{
	return 250 * (CityRPGData.getData(%client.bl_id).valueEducation+1);
}

function GameConnection::isCityAdmin(%client)
{
	return CityRPGData.getData(%client.bl_id).valueJobID $= $City::AdminJobID;
}

function CityMenu_Player(%client)
{
	if(%client.cityLotBrick !$= "")
	{
		%menu = "Lot menu.";
		%functions = "CityMenu_Player_ManageLot";
	}

	%menu = %menu TAB "Preferred spawn point." TAB "Player stats.";
	%functions = %functions TAB "CityMenu_Player_SetSpawn" TAB "CityMenu_Player_Stats";

	// Trim extra tabs, if any.
	%menu = ltrim(%menu);
	%functions = ltrim(%functions);

	if(City.get(%client.bl_id, "jobID") $= $City::MayorJobID)
	{
		%menu = %menu TAB "Mayor actions.";
		%functions = %functions TAB "CityMenu_Mayor";
	}

	%menu = %menu TAB "Close menu.";
	%functions = %functions TAB "CityMenu_Close";
	
	%client.cityMenuOpen(%menu, %functions, %client.getID(), -1, 0, 1, "Actions Menu");
}

function CityMenu_Player_Stats(%client)
{
	serverCmdStats(%client);
	%client.cityMenuClose();
}

function CityMenu_Player_ManageLot(%client)
{
	%client.cityMenuClose(1);
	serverCmdLot(%client);
}

function CityMenu_Player_SetSpawn(%client)
{
	%menu = $City::SpawnPreferences;
	%function = "CityMenu_Player_SetSpawnConfirm";

	%client.cityMenuOpen(%menu, %function, %client, -1, 0, 1);
}

function CityMenu_Player_SetSpawnConfirm(%client, %input)
{
	%inputNum = atof(%input);
	%selection = getField($City::SpawnPreferences, %inputNum-1);
	%selectionID = getField($City::SpawnPreferenceIDs, %inputNum-1);

	if(%inputNum == 0 || %selection $= "")
	{
		%client.cityMenuMessage("\c6Invalid selection. Please try again.");
	}
	else
	{
		%client.cityMenuMessage("\c6Spawn preference set to \c3" @ %selection @ "\c6.");
		City.set(%client.bl_id, "spawnPoint", selectionID);
		%client.cityLog("Set spawn to " @ selectionID);
	}
}
