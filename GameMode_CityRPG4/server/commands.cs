package CityRPG_Commands
{
	// ============================================================
	// Common
	// ============================================================
	function GameConnection::cityRateLimitCheck(%client)
	{
		%simTime = getSimTime()+0; // Hack to compare the time.
		if(%client.cityCommandTime+$City::CommandRateLimitMS > %simTime)
		{
			%client.cityCommandTime = %simTime;
			return 1;
		}
		else
		{
			%client.cityCommandTime = %simTime;
			return 0;
		}

		%client.cityCommandTime = %simTime;
	}

	// ============================================================
	// Player Commands
	// ============================================================
	function serverCmdhelp(%client, %strA, %strB, %strC)
	{
		%client.cityLog("/help" SPC %section SPC %term);

		switch$(%strA)
		{
			case "":
				messageClient(%client, '', "\c6Type \c3/help starters\c6 for information to get started in CityRPG");
				messageClient(%client, '', "\c6Type \c3/help commands\c6 to list the commands in the game");
				messageClient(%client, '', "\c6More: \c3/help events\c6, \c3/help admin\c6, \c3/stats");


				if($GameModeArg $= "Add-Ons/GameMode_CityRPG4/gamemode.txt")
				{
					%sentenceStr = "\c6This server is running vanilla CityRPG 4";
				}
				else
				{
					%sentenceStr = "\c6This server is running a \c3custom configuration\c6 of CityRPG 4";
				}

				if($City::isGitBuild)
				{
					%suffix = " (Git build)";
				}

				messageClient(%client, '', %sentenceStr @ " (\c3" @ $City::Version @ "\c6)" @ %suffix);
			case "starters":
				messageClient(%client, '', "\c6Welcome! To get started, you'll want to explore and familiarize yourself with the map.");
				messageClient(%client, '', "\c6Some of the places most important to you will include the jobs office, the education office, and the bank.");
				messageClient(%client, '', "\c6Once you've taken some time to explore, go to the jobs office to apply for your first job.");
				messageClient(%client, '', "\c6From there, you can invest in education and assets to advance yourself in the city and work your way up the ladder.");
				messageClient(%client, '', "\c6Good luck!");
			case "commands":
				messageClient(%client, '', "\c3/stats\c6 - View your stats");
				messageClient(%client, '', "\c3/pardon\c6 [player] - Issue a pardon to a player in jail. Can only be used by officials.");
				messageClient(%client, '', "\c3/eraseRecord\c6 [player] - Erases the record of a player, for a price. Can only be used by officials.");
				messageClient(%client, '', "\c3/reset\c6 - Reset your in-game account. WARNING: This will clear your save data if typed!");
				messageClient(%client, '', "\c3/dropmoney\c6 [amount] - Make it rain!");
				messageClient(%client, '', "\c3/giveMoney\c6 [amount] [player] - Give money to another player");
				messageClient(%client, '', "\c3/lot\c6 - View information about the lot you are standing on");

			case "events":
				messageClient(%client, '', "\c6 - brick -> \c3sellFood\c6 [Food] [Markup] - Feeds a player using the automated sales system.");
				messageClient(%client, '', "\c6 - brick -> \c3sellItem\c6 [Item] [Markup] - Sells an item using the automated system.");
				messageClient(%client, '', "\c6 - brick -> \c3requestFunds\c6 [Service] [Price] - Requests $\c3[Price]\c6 for \c3[Service]\c6. Charge money to call events.");
				messageClient(%client, '', "\c6 - brick -> \c3doJobTest\c6 [Job] [NoConvicts] - Tests if user's job is [Job]. NoConvicts will fail inmates. Calls onJobTestFail and onJobTestPass");
			case "admin":
				messageClient(%client, '', "\c6Type \c3/admin\c6 to access the main admin panel.");
				messageClient(%client, '', "\c6Admin commands: /\c3updateScore\c6, /\c3setMinerals\c6 [\c3value\c6], /\c3setLumber\c6 [\c3value\c6], /\c3editEducation\c6 [\c3level\c6] [\c3player\c6]");
				messageClient(%client, '', "\c6/\c3clearMoney\c6, \c6/\c3gMoney\c6 [\c3amount\c6] [\c3player\c6], /\c3dMoney\c6 [\c3amount\c6] [\c3player\c6], /\c3cleanse\c6, /\c3editHunger\c6 [\c3level\c6], /\c3manageLot");
				messageClient(%client, '', "\c6/\c3resetAllJobs\c6");

			case "jobs":
				messageClient(%client, '', "\c6Visit the jobs office to view the available jobs.");

			default:
				messageClient(%client, '', "\c6Unknown help section. Please try again.");
		}
	}

	function serverCmdYes(%client)
	{
		if(%client.cityRateLimitCheck())
		{
			return;
		}

		%client.cityLog("/yes");

		if(!isObject(%client.player))
			return;

		if(isObject(%client.player) && isObject(%client.player.serviceOrigin))
		{
			if(mFloor(VectorDist(%client.player.serviceOrigin.getPosition(), %client.player.getPosition())) < 16)
			{
				if(CityRPGData.getData(%client.bl_id).valueMoney >= %client.player.serviceFee)
				{
					%ownerBL_ID = %client.player.serviceOrigin.getGroup().bl_id;
					switch$(%client.player.serviceType)
					{
						case "service":
							%client.cityLog("Evnt buy service for " @ %client.player.serviceFee @ " from " @ %sellerID);
							CityRPGData.getData(%client.bl_id).valueMoney -= %client.player.serviceFee;

							CityRPGData.getData(%client.player.serviceOrigin.getGroup().bl_id).valueBank += %client.player.serviceFee;

							messageClient(%client, '', "\c6You have accepted the service fee of \c3$" @ %client.player.serviceFee @ "\c6!");
							%client.setInfo();

						if(%client.player.serviceOrigin.getGroup().client)
							messageClient(%client.player.serviceOrigin.getGroup().client, '', "\c3" @ %client.name @ "\c6 has wired you \c3$" @ %client.player.serviceFee @ "\c6 for a service.");

						%client.player.serviceOrigin.onTransferSuccess(%client);

						case "food":
							%client.sellFood(%ownerBL_ID, %client.player.serviceSize, %client.player.serviceItem, %client.player.serviceFee, %client.player.serviceMarkup);

						case "item":
							%client.sellItem(%ownerBL_ID, %client.player.serviceItem, %client.player.serviceFee, %client.player.serviceMarkup);

						case "zone":
							%client.sellZone(%ownerBL_ID, %client.player.serviceOrigin, %client.player.serviceFee);

						case "clothes":
							%client.sellClothes(%ownerBL_ID, %client.player.serviceOrigin, %client.player.serviceItem, %client.player.serviceFee);
					}
				}
				else
				{
					messageClient(%client, '', "\c6You cannot afford this service.");
				}
			}
			else
			{
				messageClient(%client, '', "\c6You are too far away from the service to purchase it!");
			}
		}
		else
		{
			messageClient(%client, '', "\c6You have no active tranfers that you may accept!");
		}

		%client.player.serviceType = "";
		%client.player.serviceFee = "";
		%client.player.serviceMarkup = "";
		%client.player.serviceItem = "";
		%client.player.serviceSize = "";
		%client.player.serviceOrigin = "";
	}

	function serverCmdNo(%client)
	{
		%client.cityLog("/no");
		%serviceOrigin = %client.player.serviceOrigin;

		if(!isObject(%client.player))
			return;

		if(isObject(%serviceOrigin) || (!isObject(%serviceOrigin) && %serviceOrigin !$= ""))
		{
			messageClient(%client, '', "\c6You have rejected the service fee!");

			if(isObject(%serviceOrigin))
			{
				%serviceOrigin.onTransferDecline(%client);
			}

			%client.player.serviceType = "";
			%client.player.serviceFee = "";
			%client.player.serviceMarkup = "";
			%client.player.serviceItem = "";
			%client.player.serviceSize = "";
			%client.player.serviceOrigin = "";
		}
		else
			messageClient(%client, '', "\c6You have no active tranfers that you may decline!");
	}

	function serverCmddonate(%client, %arg1)
	{
		%client.cityLog("/donate" SPC %arg1);

		if(!isObject(%client.player))
			return;

		%arg1 = mFloor(%arg1);

		if(%arg1*0.15+$City::Economics::Condition > $Pref::Server::City::Economics::Cap) {
			%arg1 = mFloor(($Pref::Server::City::Economics::Cap-$City::Economics::Condition)/0.15);
		}

		if(%arg1 <= 0)
		{
			return;
		}

		if($City::Economics::Condition > $Pref::Server::City::Economics::Cap)
		{
			messageClient(%client, '', "\c6The economy is currently at the maxiumum rate. Please try again later.");
			return;
		}

		if((CityRPGData.getData(%client.bl_id).valueMoney - %arg1) < 0)
		{
			messageClient(%client, '', "\c6You don't have that much money to donate to the economy.");
			return;
		}

		%amoutPer = %arg1 * 0.15;
		CityRPGData.getData(%client.bl_id).valueMoney -= %arg1;
		messageClient(%client, '', "\c6You've donated \c3$" @ %arg1 SPC "\c6to the economy! (" @ %amoutPer @ "%)");
		messageAll('',"\c3" @ %client.name SPC "\c6has donated \c3$" @ %arg1 SPC "\c6to the economy! (" @ %amoutPer @ "%)");
		$City::Economics::Condition = $City::Economics::Condition + %amoutPer;
		%client.setGameBottomPrint();
	}

	function serverCmdbuyErase(%client)
	{
		if(%client.cityRateLimitCheck())
		{
			return;
		}

		%client.cityLog("/buyErase");

		if(!isObject(%client.player))
			return;

		%cost = %client.getCityRecordClearCost();
		if((CityRPGData.getData(%client.bl_id).valueMoney - %cost) < 0)
		{
			messageClient(%client, '', "\c6You don't have $" @ %cost @ ".");
			return;
		}

		if(!getWord(CityRPGData.getData(%client.bl_id).valueJailData, 0))
		{
			messageClient(%client, '', %target @ "\c6You do not have a criminal record.");
			return;
		}

		if(CityRPGData.getData(%client.bl_id).valueMoney < %cost && !%client.isAdmin)
		{
			messageClient(%client, '', "\c6You need at least \c3$" @ %cost SPC "\c6to erase someone's record.");
			return;
		}

		CityRPGData.getData(%client.bl_id).valueJailData = "0" SPC getWord(CityRPGData.getData(%client.bl_id).valueJailData, 1);
		messageClient(%client, '', "\c6You have erased your criminal record.");
		%client.spawnPlayer();
		%client.setInfo();
		CityRPGData.getData(%client.bl_id).valueMoney -= %cost;
	}

	function serverCmdgiveMoney(%client, %money, %name)
	{
		if(%client.cityRateLimitCheck())
		{
			return;
		}

		%client.cityLog("/giveMoney" SPC %money SPC %name);

		if(!isObject(%client.player))
			return;

		%money = mFloor(%money);

		if(%money <= 0)
		{
			messageClient(%client, '', "\c6You must enter a valid amount of money to give.");
			return;
		}

		if((CityRPGData.getData(%client.bl_id).valueMoney - %money) < 0)
		{
			messageClient(%client, '', "\c6You don't have that much money to give.");
			return;
		}

		if(!isObject(%client.player))
		{
			messageClient(%client, '', "\c6Spawn first before you use this command.");
			return;
		}

		if(%name !$= "")
		{
			%target = findclientbyname(%name);
		}
		else
		{
			%target = containerRayCast(%client.player.getEyePoint(), vectorAdd(vectorScale(vectorNormalize(%client.player.getEyeVector()), 5), %client.player.getEyePoint()), $typeMasks::playerObjectType,%client.player).client;
		}

		if(!isObject(%target))
		{
			messageClient(%client, '', "\c6You must be looking at and be in a reasonable distance of the player in order to give them money. \nYou can also type in the person's name after the amount.");
			return;
		}

		%client.cityLog("Give money to " @ %target.bl_id);
		messageClient(%client, '', "\c6You give \c3$" @ %money SPC "\c6to \c3" @ %target.name @ "\c6.");
		messageClient(%target, '', "\c3" @ %client.name SPC "\c6has given you \c3$" @ %money @ "\c6.");

		CityRPGData.getData(%client.bl_id).valueMoney -= %money;
		CityRPGData.getData(%target.bl_id).valueMoney += %money;

		%client.SetInfo();
		%target.SetInfo();
	}

	function serverCmdjobs(%client, %str1, %str2, %str3, %str4)
	{
		if(%client.cityRateLimitCheck() || !isObject(%client.player))
		{
			return;
		}

		%client.cityLog("/jobs" SPC %job SPC %job2 SPC %job3 SPC %job4 SPC %job5);

		// Combine the job input.
		// Trim spaces for args that are not used.
		%jobInput = rtrim(%str1 SPC %str2 SPC %str3 SPC %str4);
		%jobObject = findJobByName(%jobInput);

		if(!isObject(%jobObject))
		{
			messageClient(%client, '', "\c6No such job. Please try again.");
			return;
		}

		%client.setCityJob(%jobObject.id);
	}

	function serverCmdreset(%client)
	{
		if(%client.cityRateLimitCheck())
		{
			return;
		}

		%client.cityLog("/reset");

		if(!isObject(%client.player))
			return;

		if(CityRPGData.getData(%client.bl_id).valueMoney - $Pref::Server::City::prices::reset >= 0)
		{
			%client.cityMenuMessage("\c6Would you like to reset your CityRPG profile?");
			%client.cityMenuMessage("\c0WARNING: You are about to reset all of your progress on this server. Are you sure?");

			%menu = "Reset my account." TAB "Cancel.";
			%functions = CityMenu_Reset_Confirm TAB CityMenu_Close;
			%client.cityMenuOpen(%menu, %functions, %client, "\c6Your account will not be reset.");
		}
		else
			messageClient(%client, '', "\c6You need at least \c3$" @ $Pref::Server::City::prices::reset SPC "\c6to do that.");
	}

	function CityMenu_Reset_Confirm(%client)
	{
		%client.cityLog("***Account reset***");
		messageClient(%client, '', "\c6Your account has been reset.");
		messageAll('',"\c3"@ %client.name @" \c6has reset their account.");
		CityRPGData.removeData(%client.bl_id);
		CityRPGData.addData(%client.bl_id);

		CityRPGData.getData(%client.bl_id).valueBank = 100;

		if(isObject(%client.player))
		{
			%client.spawnPlayer();
		}
	}

	function serverCmdeducation(%client, %do) {
		messageClient(%client, '', "\c6Find the education office to enroll for an education.");
	}

	function serverCmdpardon(%client, %name)
	{
		if(%client.cityRateLimitCheck())
		{
			return;
		}

		%client.cityLog("/pardon" SPC %name);

		if(!isObject(%client.player))
			return;

		if(!%client.getJobSO().canPardon && !%client.isSuperAdmin)
		{
			messageClient(%client, '', "\c6You can't pardon people.");
			return;
		}

		if(%name $= "")
		{
			messageClient(%client, '' , "\c6Please enter a name.");
			return;
		}

		%target = findClientByName(%name);
		if(!isObject(%target))
		{
			messageClient(%client, '', "\c6That person does not exist.");
			return;
		}

		if(!getWord(CityRPGData.getData(%target.bl_id).valueJailData, 1))
		{
			messageClient(%client, '', "\c6That person is not a convict.");
			return;
		}

		%cost = $Pref::Server::City::demerits::pardonCost * getWord(CityRPGData.getData(%target.bl_id).valueJailData, 1);
		if(CityRPGData.getData(%client.bl_id).valueMoney < %cost && !%client.isAdmin)
		{
			messageClient(%client, '', "\c6You need at least \c3$" @ %cost SPC "\c6to pardon someone.");
			return;
		}

		if(%client.BL_ID != getNumKeyID() && %target == %client)
		{
			messageClient(%client, '', "\c6The extent of your legal corruption only goes so far. You cannot pardon yourself.");
			return;
		}

		CityRPGData.getData(%client.bl_id).valueMoney -= (%client.isAdmin ? 0 : %cost);
		CityRPGData.getData(%target.bl_id).valueJailData = getWord(CityRPGData.getData(%target.bl_id).valueJailData, 0) SPC 0;

		if(%target != %client)
		{
			messageClient(%client, '', "\c6You have let\c3" SPC %target.name SPC "\c6out of prison.");
			messageClient(%target, '', "\c3" @ %client.name SPC "\c6has issued you a pardon.");
		}
		else
		{
			messageClient(%client, '', "\c6You have pardoned yourself.");
		}

		%target.buyResources();
		%target.spawnPlayer();
		%client.SetInfo();
	}

	function serverCmderaseRecord(%client, %name)
	{
		if(%client.cityRateLimitCheck())
		{
			return;
		}

		%client.cityLog("/eraseRecord" SPC %name);

		if(!%client.getJobSO().canPardon && %client.BL_ID != getNumKeyID())
		{
			messageClient(%client, '', "\c6You can't erase people's record!");
			return;
		}

		if(%name $= "")
		{
			messageClient(%client, '' , "\c6Please enter a name.");
			return;
		}

		%target = findClientByName(%name);
		if(!isObject(%target))
		{
			messageClient(%client, '', "\c6That person does not exist.");
			return;
		}

		if(!getWord(CityRPGData.getData(%target.bl_id).valueJailData, 0))
		{
			messageClient(%client, '', "\c6That person does not have a criminal record.");
			return;
		}

		%cost = $Pref::Server::City::demerits::recordShredCost;
		if(CityRPGData.getData(%client.bl_id).valueMoney < %cost && !%client.isAdmin)
		{
			messageClient(%client, '', "\c6You need at least \c3$" @ %cost SPC "\c6to erase someone's record.");
			return;
		}

		CityRPGData.getData(%target.bl_id).valueJailData = "0" SPC getWord(CityRPGData.getData(%target.bl_id).valueJailData, 1);
		if(%target != %client)
		{
			messageClient(%client, '', "\c6You have ran\c3" SPC %target.name @ "\c6's criminal record through a paper shredder.");
			messageClient(%target, '', "\c3It seems your criminal record has simply vanished...");

			if(!%client.BL_ID == getNumKeyID())
				CityRPGData.getData(%client.bl_id).valueMoney -= %cost;
		}
		else
			messageClient(%client, '', "\c6You have erased your criminal record.");

		%target.spawnPlayer();
		%client.setInfo();
		
		return true;
	}

	function serverCmdReincarnate(%client, %do)
	{
		if(%client.cityRateLimitCheck())
		{
			return;
		}

		%client.cityLog("/reincarnate" SPC %do);

		if(CityRPGData.getData(%client.bl_id).valueReincarnated)
		{
			messageClient(%client, '', "\c6You have already reincarnated.");
			return;
		}

		if(%do $= "accept")
		{
			if((CityRPGData.getData(%client.bl_id).valueMoney + CityRPGData.getData(%client.bl_id).valueBank) >= 100000)
			{
				CityRPGData.removeData(%client.bl_id);
				CityRPGData.addData(%client.bl_id);
				CityRPGData.getData(%client.bl_id).valueReincarnated = 1;
				CityRPGData.getData(%client.bl_id).valueEducation = $City::EducationReincarnateLevel;

				if(isObject(%client.player))
				{
					%client.spawnPlayer();
				}

				messageAllExcept(%client, '', '\c3%1\c6 has been reincarnated!', %client.name);
				messageClient(%client, '', "\c6You have been reincarnated.");
			}
		}
		else
		{
			messageClient(%client, '', "\c6Reincarnation is a method for those who are on top to once again replay the game.");
			messageClient(%client, '', "\c6It costs $100,000 to Reincarnate yourself. Your account will almost completely reset.");
			messageClient(%client, '', "\c6The perks of doing this are...");
			messageClient(%client, '', "\c6 - You will start with a level " @ $City::EducationReincarnateLevel @ " education (+" @ $City::EducationReincarnateLevel-$City::EducationCap @ " maximum)");
			messageClient(%client, '', "\c6 - Your name will be yellow by default and white if you are wanted.");
			messageClient(%client, '', "\c6Type \c3/reincarnate accept\c6 to start anew!");
		}
	}

	function serverCmddropmoney(%client,%amt)
	{
		if(%client.cityRateLimitCheck())
		{
			return;
		}

		%client.cityLog("/dropmoney" SPC %amt);

		%amt = mFloor(%amt);
		if(%amt < 50)
		{
			messageClient(%client,'',"\c6The least you can drop is \c3$50\c6.");
			return;
		}

		if(CityRPGData.getData(%client.bl_id).valueMoney < %amt)
		{
			messageClient(%client,'',"\c6You don't have that much money to drop!");
			return;
		}

		%cash = new Item()
		{
			datablock = cashItem;
			canPickup = false;
			value = %amt;
		};

		%cash.setTransform(setWord(%client.player.getTransform(), 2, getWord(%client.player.getTransform(), 2) + 4));
		%cash.setVelocity(VectorScale(%client.player.getEyeVector(), 10));
		MissionCleanup.add(%cash);
		%cash.setShapeName("$" @ %cash.value);
		CityRPGData.getData(%client.bl_id).valueMoney = CityRPGData.getData(%client.bl_id).valueMoney - %amt;
		%client.setInfo();

		messageClient(%client,'',"\c6You drop \c3$" @ %amt @ ".");
		%client.cityLog("Drop '$" @ %amt @ "'");
	}

	function serverCmdstats(%client, %name)
	{
		if(%client.cityRateLimitCheck())
		{
			return;
		}

		%client.cityLog("/stats" SPC %name);

		if(!isObject(%client.player))
			return;

		if(%client.isAdmin && %name !$= "")
			%target = findClientByName(%name);
		else
			%target = %client;

		%data = CityRPGData.getData(%target.bl_id);

		if(isObject(%target))
		{
			%job = %target.getJobSo();

			// Career
			%string = "Career:" SPC "\c3" @ %target.getJobSO().track;

			// Title
			if(%job.title !$= "")
			{
				%string = %string @ "\n" @ "Title:" SPC %job.title SPC %target.name;
			}

			// Job
			%string = %string @ "\n" @ "Job:" SPC %job.name;

			// Net worth
			%string = %string @ "\n" @ "Net worth:" SPC "\c3$" @ (%data.valueMoney + %data.valueBank);

			// Crim record
			%string = %string @ "\n" @ "Criminal record:" SPC "\c3" @ (getWord(%data.valueJailData, 0) ? "Yes" : "No");

			// Education
			%level = %data.valueEducation;
			if($CityRPG::EducationStr[%level] !$= "")
			{
				%eduString = $CityRPG::EducationStr[%level];
			}
			else
			{
				%eduString = "Level " @ %level;
			}
			%string = %string @ "\n" @ "Education:" SPC "\c3" @ %eduString;
			
			// Lots visited
			%lotsVisited = getWordCount(%data.valueLotsVisited);
			%string = %string @ "\nLots visited: " @ (%data.valueLotsVisited == -1? 0 : %lotsVisited);


			commandToClient(%client, 'MessageBoxOK', "Stats for " @ %target.name, %string);
		}
		else
			messageClient(%client, '', "\c6Either you did not enter or the person specified does not exist.");
	}

	function serverCmdjob(%client, %job, %job2, %job3, %job4, %job5)
	{
		%client.cityLog("/job [...]");

		serverCmdjobs(%client, %job, %job2, %job3, %job4, %job5);
	}

	function serverCmdLot(%client)
	{
		if(%client.cityMenuOpen)
		{
			if(isObject(%client.cityMenuID.dataBlock) && %client.cityMenuID.dataBlock.CityRPGBrickType == $CityBrick_Lot)
			{
				// If the open menu is a lot menu, close it.
				%client.cityMenuClose();
			}

			return;
		}

		CityMenu_Lot(%client);
	}
};

deactivatePackage("CityRPG_Commands");
activatePackage("CityRPG_Commands");
