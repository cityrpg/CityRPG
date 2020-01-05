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
	%reward = mFloor($CityRPG::prices::jailingBonus * %ticks);

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
		%robSO.valueJobID = 1;
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
		%product = mFloor(%totalResources * $CityRPG::prices::resourcePrice);

		if(%client.getJobSO().laborer && !getWord(CityRPGData.getData(%client.bl_id).valueJobID, 1))
		{
			%product *= getRandom(1.5 + (CityRPGData.getData(%client.bl_id).valueEducation / 12), 2.0 + (CityRPGData.getData(%client.bl_id).valueEducation / 12));
			%product = mFloor(%product);
		}

		if(!getWord(CityRPGData.getData(%client.bl_id).valueJailData, 1))
		{
			%client.cityLog("Resource sell for " @ %product);
			CityRPGData.getData(%client.bl_id).valueMoney += %product;
			messageClient(%client, '', "\c6The state has bought all of your resources for \c3$" @ %product @ "\c6.");
		}
		else
		{
			CityRPGData.getData(%client.bl_id).valueBank += %product;
			messageClient(%client, '', '\c6The state has set aside \c3$%1\c6 for when you get out of Prison.', %product);
		}

		CitySO.lumber += getWord(CityRPGData.getData(%client.bl_id).valueResources, 0);
		CitySO.minerals += getWord(CityRPGData.getData(%client.bl_id).valueResources, 1);

		CityRPGData.getData(%client.bl_id).valueResources = "0 0";

		%client.SetInfo();
	}
}

function gameConnection::setInfo(%client)
{
	CityRPGData.getData(%client.bl_id).valueMoney = mFloor(CityRPGData.getData(%client.bl_id).valueMoney);
	CityRPGData.getData(%client.bl_id).valueName = %client.name;

	if(isObject(%client.player))
	{
		%client.player.setShapeNameDistance(24);

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
	%health = 100 - %client.player.getDamageLevel();

	%client.CityRPGPrint = %client.CityRPGPrint @ "<bitmap:" @ $City::DataPath @ "ui/health.png>\c6" SPC mFloor(100 - %client.player.getDamageLevel()) @ "%";

	%client.CityRPGPrint = %client.CityRPGPrint @ " <bitmap:" @ $City::DataPath @ "ui/cash.png>" SPC %client.getCashString();

	// Placeholder
	//%client.CityRPGPrint = %client.CityRPGPrint @ "<just:right>\c6Day";

	// Lot Info
	//if(isObject(%client.CityRPGLotBrick))
	//	%client.CityRPGPrint = %client.CityRPGPrint SPC " <bitmap:Add-Ons/GameMode_CityRPG4/data/ui/location.png> \c6" @ %client.CityRPGLotBrick.getGroup().name @ "'s Lot";

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

	$Economics::replayCount = $Economics::replayCount + 1;
	$Economics::randomUporDown = getRandom(1,5);
	$Economics::positiveNegative = getRandom(1,2);

	if($Pref::Server::City::Economics::Relay < 1)
		$Pref::Server::City::Economics::Relay = ClientGroup.getCount();

	if($Economics::replayCount > $Pref::Server::City::Economics::Relay)
	{
		if($Economics::Condition > $Pref::Server::City::Economics::Greatest)
		{
			$Economics::Condition = $Economics::Condition - $Economics::randomUporDown;
			$Economics::replayCount = 0;
		}
		else if($Economics::Condition < $Pref::Server::City::Economics::Least)
		{
			$Economics::Condition = $Economics::Condition + $Economics::randomUporDown;
			$Economics::replayCount = 0;
		}
		else if($Economics::positiveNegative == 1)
		{
			$Economics::Condition = $Economics::Condition + $Economics::randomUporDown;
			$Economics::replayCount = 0;
		}
		else if($Economics::positiveNegative == 2)
		{
			$Economics::Condition = $Economics::Condition - $Economics::randomUporDown;
			$Economics::replayCount = 0;
		}
	}

	if($Economics::Condition > $Pref::Server::City::Economics::Cap)
	{
		$Economics::Condition = $Pref::Server::City::Economics::Cap;
	}

	if($Economics::Condition $= "")
	{
		error("ERROR: GameMode_CityRPG4 - Economics condition is blank! Resetting to 0.");
		$Economics::Condition = 0;
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

// Get Functions
function gameConnection::getCashString(%client)
{
	if(CityRPGData.getData(%client.bl_id).valueMoney >= 0)
		%money = "\c6$" @ CityRPGData.getData(%client.bl_id).valueMoney;
	else
		%money = "\c0($" @ strreplace(CityRPGData.getData(%client.bl_id).valueMoney, "-", "")  @ ")";

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
	if(CityRPGData.getData(%client.bl_id).valueDemerits >= $Pref::Server::City::demerits::wantedLevel)
	{
		%div = CityRPGData.getData(%client.bl_id).valueDemerits / $Pref::Server::City::demerits::wantedLevel;

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
				CityRPGData.getData(%client.bl_id).valueHunger += %servingID;

				if(CityRPGData.getData(%client.bl_id).valueHunger > 10)
				{
					CityRPGData.getData(%client.bl_id).valueHunger = 10;
				}

				CityRPGData.getData(%client.bl_id).valueMoney -= %price;

				if(%profit)
				{
					if(isObject(%seller = findClientByBL_ID(%sellerID)))
					{
						messageClient(%seller, '', '\c6You just gained \c3$%1\c6 for providing \c3%3\c6 to \c3%2\c6.', %profit, %client.name, %foodName);
						CityRPGData.getData(%sellerID).valueBank += %profit;
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

				CityRPGData.getData(%client.bl_id).valueMoney -= %price;
				CityRPGData.getData(%sellerID).valueBank += %profit;
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
			CityRPGData.getData(%client.bl_id).valueMoney -= %price;
			ClothesSO.giveItem(%client, %item);

			if(%price)
			{
				if(isObject(%seller = FindClientByBL_ID(%sellerID)))
				{
					messageClient(%seller, '', '\c6You just gained \c3$%1\c6 for selling clothes to \c3%2\c6.', %price, %client.name);
					CityRPGData.getData(%sellerID).valueBank += %price;
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

// Output Events
function gameConnection::MessageBoxOK(%client, %header, %text)
{
	commandToClient(%client, 'MessageBoxOK', %header, %text);
}

function player::giveDefaultEquipment(%this)
{
	if(!getWord(CityRPGData.getData(%this.client.bl_id).valueJailData, 1))
	{
		if(CityRPGData.getData(%this.client.bl_id).valueTools $= "")
		{
			%tools = ($Pref::Server::City::giveDefaultTools ? $Pref::Server::City::defaultTools @ " " : "") @ %this.client.getJobSO().tools;
			CityRPGData.getData(%this.client.bl_id).valueTools = "";
		}
		else
			%tools = CityRPGData.getData(%this.client.bl_id).valueTools;

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


function jobset(%client, %job, %name)
{
	if(%name $= "")
	{
		CityRPGData.getData(%client.bl_id).valueJobID = %job;
		serverCmdunUseTool(%client);
		%client.player.giveDefaultEquipment();
		%client.applyForcedBodyColors();
		%client.applyForcedBodyParts();

		if(%job == 14)
		{
			$City::Mayor::String = %client.name;
			$City::Mayor::Enabled = 0;
			serverCmdClearImpeach(%client);
		}
		%client.SetInfo();
	}
	else
	{
		%target = findClientByName(%name);
		CityRPGData.getData(%target.bl_id).valueJobID = %job;
		serverCmdunUseTool(%target);
		%target.player.giveDefaultEquipment();
		%target.applyForcedBodyColors();
		%target.applyForcedBodyParts();

		if(%job == 14)
			$City::Mayor::String = %target.name;

		%target.SetInfo();
	}
}

function resetFree(%client)
{
	%client.cityLog("***Account auto-reset***");
	CityRPGData.removeData(%client.bl_id);
	CityRPGData.addData(%client.bl_id);

	CityRPGData.getData(%client.bl_id).valueBank = 250;

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
	%client.cityLog("/education" SPC %do);

	if(!isObject(%client.player) || CityRPGData.getData(%client.bl_id).valueEducation >= 6)
		return;

	%price = %client.getCityEnrollCost();

	// Ensure the player is not already enrolled
	if(!CityRPGData.getData(%client.bl_id).valueStudent)
	{
		if(CityRPGData.getData(%client.bl_id).valueMoney >= %price)
		{
			%valueStudent = CityRPGData.getData(%client.bl_id).valueEducation + 1;
			// Number of days to complete
			CityRPGData.getData(%client.bl_id).valueStudent = %valueStudent;
			// Cost
			CityRPGData.getData(%client.bl_id).valueMoney -= %price;

			messageClient(%client, '', "\c6You are now enrolled. You will complete your education in \c3" @ %valueStudent @ "\c6 days.");
			%client.setInfo();
		}
		else
		{
			messageClient(%client, '', "\c6It costs \c3$" @ %price SPC "\c6to get enrolled. You do not have enough money.");
		}
	}
}
