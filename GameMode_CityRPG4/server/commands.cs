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
				messageClient(%client, '', "\c6Type \c3/help jobs\c6 for a list of available jobs");
				messageClient(%client, '', "\c6More: \c3/help events\c6, \c3/help admin");


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

				messageClient(%client, '', %sentenceStr SPC $City::VersionTitle @ " (\c3" @ $City::Version @ "\c6)" @ %suffix);
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
				messageClient(%client, '', "\c6Admin commands: /\c3updateScore\c6, /\c3setMinerals\c6 [\c3value\c6], /\c3setLumber\c6 [\c3value\c6], /\c3editEducation\c6 [\c3level\c6] [\c3player\c6]");
				messageClient(%client, '', "\c6/\c3clearMoney\c6, \c6/\c3gMoney\c6 [\c3amount\c6] [\c3player\c6], /\c3dMoney\c6 [\c3amount\c6] [\c3player\c6], /\c3cleanse\c6, /\c3editHunger\c6 [\c3level\c6], /\c3manageLot");

			case "jobs":
				messageClient(%client, '', "\c6Type \c3/job\c6 then one of the jobs below to apply for that job.");

				for(%a = 1; %a <= JobSO.getJobCount(); %a++)
				{
						if(!JobSO.job[%a].adminonly && !JobSO.job[%a].hostonly)
						{
						messageClient(%client, '', "\c3" @ JobSO.job[%a].name SPC "\c6- Inital Investment: \c3" @ JobSO.job[%a].invest SPC "- \c6Pay: \c3" @ JobSO.job[%a].pay SPC "- \c6Required Education: \c3" @ JobSO.job[%a].education);
						messageClient(%client, '', JobSO.job[%a].helpline);

						if(JobSO.job[%a].flavortext !$= "")
						{
							messageClient(%client, '', "<color:A6A6A6>" @ JobSO.job[%a].flavortext);
						}
					}
				}

				if(!$City::DefaultJobs)
				{
					messageClient(%client, '', "\c3This server is running a customized job tree.");
				}

				messageClient(%client, '', "\c3Use the Page Up and Page Down keys to scroll through the list of jobs.");

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

		if(!isObject(%client.player))
			return;

		if(isObject(%client.player.serviceOrigin))
		{
			messageClient(%client, '', "\c6You have rejected the service fee!");

			%client.player.serviceOrigin.onTransferDecline(%client);

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

		if(%arg1*0.15+$Economics::Condition > $Pref::Server::City::Economics::Cap) {
			%arg1 = mFloor(($Pref::Server::City::Economics::Cap-$Economics::Condition)/0.15);
		}

		if(%arg1 > 0)
		{
			if($Economics::Condition > $Pref::Server::City::Economics::Cap)
			{
				messageClient(%client, '', "\c6The economy is currently at the maxiumum rate. Please try again later.");
			}
			else
			{
				if((CityRPGData.getData(%client.bl_id).valueMoney - %arg1) >= 0)
				{
					%amoutPer = %arg1 * 0.15;
					CityRPGData.getData(%client.bl_id).valueMoney -= %arg1;
					messageClient(%client, '', "\c6You've donated \c3$" @ %arg1 SPC "\c6to the economy! (" @ %amoutPer @ "%)");
					messageAll('',"\c3" @ %client.name SPC "\c6has donated \c3$" @ %arg1 SPC "\c6to the economy! (" @ %amoutPer @ "%)");
					$Economics::Condition = $Economics::Condition + %amoutPer;
					%client.setGameBottomPrint();
				}
				else
				{
					messageClient(%client, '', "\c6You don't have that much money to donate to the economy.");
				}
			}
		}
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
		if((CityRPGData.getData(%client.bl_id).valueMoney - %cost) >= 0)
		{
			if(getWord(CityRPGData.getData(%client.bl_id).valueJailData, 0))
			{
				if(CityRPGData.getData(%client.bl_id).valueMoney >= %cost || %client.isAdmin)
				{
					CityRPGData.getData(%client.bl_id).valueJailData = "0" SPC getWord(CityRPGData.getData(%client.bl_id).valueJailData, 1);
					messageClient(%client, '', "\c6You have erased your criminal record.");
					%client.spawnPlayer();
					%client.setInfo();
					CityRPGData.getData(%client.bl_id).valueMoney -= %cost;
				}
				else
				{
					messageClient(%client, '', "\c6You need at least \c3$" @ %cost SPC "\c6to erase someone's record.");
				}
			}
			else
			{
				messageClient(%client, '', %target @ "\c6You do not have a criminal record.");
			}
		}
		else
		{
			messageClient(%client, '', "\c6You don't have $" @ %cost @ ".");
		}
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

		if(%money > 0)
		{
			if((CityRPGData.getData(%client.bl_id).valueMoney - %money) >= 0)
			{
				if(isObject(%client.player))
				{
					if(%name !$= "")
					{
						%target = findclientbyname(%name);
					}
					else
					{
						%target = containerRayCast(%client.player.getEyePoint(), vectorAdd(vectorScale(vectorNormalize(%client.player.getEyeVector()), 5), %client.player.getEyePoint()), $typeMasks::playerObjectType,%client.player).client;
					}
					if(isObject(%target))
					{
						%client.cityLog("Give money to " @ %target.bl_id);
						messageClient(%client, '', "\c6You give \c3$" @ %money SPC "\c6to \c3" @ %target.name @ "\c6.");
						messageClient(%target, '', "\c3" @ %client.name SPC "\c6has given you \c3$" @ %money @ "\c6.");

						CityRPGData.getData(%client.bl_id).valueMoney -= %money;
						CityRPGData.getData(%target.bl_id).valueMoney += %money;

						%client.SetInfo();
						%target.SetInfo();
					}
					else
						messageClient(%client, '', "\c6You must be looking at and be in a reasonable distance of the player in order to give them money. \nYou can also type in the person's name after the amount.");
				}
				else
					messageClient(%client, '', "\c6Spawn first before you use this command.");
			}
			else
				messageClient(%client, '', "\c6You don't have that much money to give.");
		}
		else
			messageClient(%client, '', "\c6You must enter a valid amount of money to give.");
	}

	// TODO: Rewrite this spaghetti mess
	function serverCmdjobs(%client, %job, %job2, %job3, %job4, %job5)
	{
		if(%client.cityRateLimitCheck())
		{
			return;
		}

		%client.cityLog("/jobs" SPC %job SPC %job2 SPC %job3 SPC %job4 SPC %job5);

		if(%job !$= "")
		{
			if(!isObject(%client.player))
				return;

			// Concat Job Words
			%job = %job @ (%job2 !$= "" ? " " @ %job2 @ (%job3 !$= "" ? " " @ %job3 @ (%job4 !$= "" ? " " @ %job4 @ (%job5 !$= "" ? " " @ %job5 : "") : "") : "") : "");

			for(%a = 1; %a <= JobSO.getJobCount(); %a++)
			{
				if(strlwr(%job) $= strLwr(JobSO.job[%a].name))
				{
					%foundJob = true;

					if(%a == CityRPGData.getData(%client.bl_id).valueJobID)
					{
						messageClient(%client, '', "\c6You are already" SPC City_DetectVowel(JobSO.job[%a].name) SPC "\c3" @ JobSO.job[%a].name @ "\c6!");
					}
					else
					{
						if(JobSO.job[%a].law && getWord(CityRPGData.getData(%client.bl_id).valueJailData, 0) == 1)
						{
							messageClient(%client, '', "\c6You don't have a clean criminal record. You can't become" SPC City_DetectVowel(JobSO.job[%a].name) SPC "\c3" @ JobSO.job[%a].name @ "\c6.");
						}
						else
						{
							if(mFloor(JobSO.job[%a].education) > 0)
							{
								if(CityRPGData.getData(%client.bl_id).valueEducation < JobSO.job[%a].education)
								{
									messageClient(%client, '', "\c6You are not educated enough to get this job.");
								}
								else
								{
									if(CityRPGData.getData(%client.bl_id).valueMoney < JobSO.job[%a].invest)
									{
										messageClient(%client, '', "\c6It costs \c3$" @ JobSO.job[%a].invest SPC "\c6to become" SPC City_DetectVowel(JobSO.job[%a].name) SPC JobSO.job[%a].name @ "\c6.");
									}
									else
									{
										if(JobSO.job[%a].hostonly == 1)
										{
											if(%client.BL_ID == getNumKeyID())
											{
												%gotTheJob = true;
												messageClient(%client, '', "\c6Congratulations, you are now" SPC City_DetectVowel(JobSO.job[%a].name) SPC JobSO.job[%a].name @ "\c6. Your new salary is \c3$" @ JobSO.job[%a].pay @ "\c6 per day.");
												CityRPGData.getData(%client.bl_id).valueMoney -= JobSO.job[%a].invest;
											}
											else
											{
												messageClient(%client, '', "\c6Sorry, only the Host can be" SPC City_DetectVowel(JobSO.job[%a].name) SPC JobSO.job[%a].name @ "\c6.");
											}
										}
										else if(JobSO.job[%a].adminonly == 1)
										{
											if(%client.isAdmin || %client.isSuperAdmin)
											{
												%gotTheJob = true;
												messageClient(%client, '', "\c6Congratulations, you are now" SPC City_DetectVowel(JobSO.job[%a].name) SPC JobSO.job[%a].name @ "\c6. Your new salary is \c3$" @ JobSO.job[%a].pay @ "\c6 per day.");
												CityRPGData.getData(%client.bl_id).valueMoney -= JobSO.job[%a].invest;
											}
											else
											{
												messageClient(%client, '', "\c6Sorry, only an Admin or a Super Admin can be" SPC City_DetectVowel(JobSO.job[%a].name) SPC JobSO.job[%a].name @ "\c6.");
											}
										}
										else
										{
											%gotTheJob = true;
											messageClient(%client, '', "\c6Congratulations, you are now" SPC City_DetectVowel(JobSO.job[%a].name) SPC JobSO.job[%a].name @ "\c6. Your new salary is \c3$" @ JobSO.job[%a].pay @ "\c6 per day.");
											CityRPGData.getData(%client.bl_id).valueMoney -= JobSO.job[%a].invest;
										}
									}
								}
							}
							else
							{
								if(CityRPGData.getData(%client.bl_id).valueMoney < JobSO.job[%a].invest)
								{
									messageClient(%client, '', "\c6It costs \c3$" @ JobSO.job[%a].invest SPC "\c6to become" SPC City_DetectVowel(JobSO.job[%a].name) SPC JobSO.job[%a].name @ "\c6.");
								}
								else
								{
									if(JobSO.job[%a].hostonly == 1)
									{
										if(%client.BL_ID == getNumKeyID())
										{
											%gotTheJob = true;
											messageClient(%client, '', "\c6Congratulations, you are now" SPC City_DetectVowel(JobSO.job[%a].name) SPC JobSO.job[%a].name @ "\c6. Your new salary is \c3$" @ JobSO.job[%a].pay @ "\c6 per day.");
											CityRPGData.getData(%client.bl_id).valueMoney -= JobSO.job[%a].invest;
										}
										else
										{
											messageClient(%client, '', "\c6Sorry, only the Host can be" SPC City_DetectVowel(JobSO.job[%a].name) SPC JobSO.job[%a].name @ "\c6.");
										}
									}
									else if(JobSO.job[%a].adminonly == 1)
									{
										if(%client.isAdmin || %client.isSuperAdmin)
										{
											%gotTheJob = true;
											messageClient(%client, '', "\c6Congratulations, you are now" SPC City_DetectVowel(JobSO.job[%a].name) SPC JobSO.job[%a].name @ "\c6. Your new salary is \c3$" @ JobSO.job[%a].pay @ "\c6 per day.");
											CityRPGData.getData(%client.bl_id).valueMoney -= JobSO.job[%a].invest;
										}
										else
										{
											messageClient(%client, '', "\c6Sorry, only an Admin or a Super Admin can be" SPC City_DetectVowel(JobSO.job[%a].name) SPC JobSO.job[%a].name @ "\c6.");
										}
									}
									else
									{
										%gotTheJob = true;
										messageClient(%client, '', "\c6You have made your own initiative to become" SPC City_DetectVowel(JobSO.job[%a].name) SPC "\c3" @ JobSO.job[%a].name @ "\c6.");
										CityRPGData.getData(%client.bl_id).valueMoney -= JobSO.job[%a].invest;
									}
								}
							}
						}
					}

					if(%gotTheJob)
					{
						CityRPGData.getData(%client.bl_id).valueJobID = %a;

						if(isObject(%client.player))
						{
							serverCmdunUseTool(%client);
							%client.player.giveDefaultEquipment();
							%client.applyForcedBodyColors();
							%client.applyForcedBodyParts();
						}

						%client.SetInfo();
					}
				}
			}

			if(!%foundJob)
				messageClient(%client, '', "\c6No such job as \c3" @ %job @ "\c6. Type \c3/help jobs\c6 to see a list of the jobs.");
		}
		else
			messageClient(%client, '', "\c6Type \c3/help jobs\c6 to see a list of jobs.");
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
			%client.cityLog("***Account reset***");
			messageClient(%client, '', "\c6Your account has been reset.");
			messageAll('',"\c3"@ %client.name @" \c6has reset their account.");
			CityRPGData.removeData(%client.bl_id);
			CityRPGData.addData(%client.bl_id);

			if(isObject(%client.player))
			{
				%client.spawnPlayer();
			}
		}
		else
			messageClient(%client, '', "\c6You need at least \c3$" @ $Pref::Server::City::prices::reset SPC "\c6to do that.");
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

		if(%client.getJobSO().canPardon || %client.isSuperAdmin)
		{
			if(%name !$= "")
			{
				%target = findClientByName(%name);

				if(isObject(%target))
				{
					if(getWord(CityRPGData.getData(%target.bl_id).valueJailData, 1))
					{
						%cost = $Pref::Server::City::demerits::pardonCost * getWord(CityRPGData.getData(%target.bl_id).valueJailData, 1);
						if(CityRPGData.getData(%client.bl_id).valueMoney >= %cost || %client.isAdmin)
						{
							if((%client.BL_ID == getNumKeyID() || %target != %client))
							{
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
							else
							{
								messageClient(%client, '', "\c6The extent of your legal corruption only goes so far. You cannot pardon yourself.");
							}
						}
						else
						{
							messageClient(%client, '', "\c6You need at least \c3$" @ %cost SPC "\c6to pardon someone.");
						}
					}
					else
					{
						messageClient(%client, '', "\c6That person is not a convict.");
					}
				}
				else
				{
					messageClient(%client, '', "\c6That person does not exist.");
				}
			}
			else
			{
				messageClient(%client, '' , "\c6Please enter a name.");
			}
		}
		else
		{
			messageClient(%client, '', "\c6You can't pardon people.");
		}
	}

	function serverCmderaseRecord(%client, %name)
	{
		if(%client.cityRateLimitCheck())
		{
			return;
		}

		%client.cityLog("/eraseRecord" SPC %name);

		if(%client.getJobSO().canPardon || %client.BL_ID == getNumKeyID())
		{
			if(%name !$= "")
			{
				%target = findClientByName(%name);

				if(isObject(%target))
				{
					if(getWord(CityRPGData.getData(%target.bl_id).valueJailData, 0))
					{
						%cost = $Pref::Server::City::demerits::recordShredCost;

						if(CityRPGData.getData(%client.bl_id).valueMoney >= %cost || %client.isAdmin)
						{
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
						}
						else
						{
							messageClient(%client, '', "\c6You need at least \c3$" @ %cost SPC "\c6to erase someone's record.");
						}
					}
					else
					{
						messageClient(%client, '', "\c6That person does not have a criminal record.");
					}
				}
				else
				{
					messageClient(%client, '', "\c6That person does not exist.");
				}
			}
			else
			{
				messageClient(%client, '' , "\c6Please enter a name.");
			}
		}
		else
		{
			messageClient(%client, '', "\c6You can't erase people's record!");
		}
	}

	function serverCmdReincarnate(%client, %do)
	{
		if(%client.cityRateLimitCheck())
		{
			return;
		}

		%client.cityLog("/reincarnate" SPC %do);

		if(!CityRPGData.getData(%client.bl_id).valueReincarnated)
		{
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
		else
			messageClient(%client, '', "\c6You have already reincarnated.");
	}

	function serverCmddropmoney(%client,%amt)
	{
		if(%client.cityRateLimitCheck())
		{
			return;
		}

		%client.cityLog("/dropmoney" SPC %amt);

		%amt = mFloor(%amt);
		if(%amt >= 50)
		{
			if(CityRPGData.getData(%client.bl_id).valueMoney >= %amt)
			{
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
			else
				messageClient(%client,'',"\c6You don't have that much money to drop!");
		}
		else
			messageClient(%client,'',"\c6The least you can drop is \c3$50\c6.");
	}

	function serverCmdstats(%client, %name)
	{
		if(%client.cityRateLimitCheck())
		{
			return;
		}

		%client.cityLog("/stats" SPC %name);

		%data = CityRPGData.getData(%target.bl_id);

		if(!isObject(%client.player))
			return;

		if(%client.isAdmin && %name !$= "")
			%target = findClientByName(%name);
		else
			%target = %client;

		if(isObject(%target))
		{
			%string = "Career:" SPC "\c3" @ JobSO.job[%data.valueJobID].name;
			%string = %string @ "\n" @ "Money in Wallet:" SPC "\c3" @ %data.valueMoney;
			%string = %string @ "\n" @ "Net Worth:" SPC "\c3" @ (%data.valueMoney + %data.valueBank);
			%string = %string @ "\n" @ "Arrest Record:" SPC "\c3" @ (getWord(%data.valueJailData, 0) ? "Yes" : "No");
			%string = %string @ "\n" @ "Ticks left in Jail:" SPC "\c3" @ getWord(%data.valueJailData, 1);
			%string = %string @ "\n" @ "Total Demerits:" SPC "\c3" @ %data.valueDemerits;
			%string = %string @ "\n" @ "Education:" SPC "\c3" @ %data.valueEducation;
			commandToClient(%client, 'MessageBoxOK', %target.name, %string);
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

	// ============================================================
	// Admin Commands
	// ============================================================
	function serverCmdEditEducation(%client, %int, %name)
	{
		%client.cityLog("/EditEducation" SPC %int SPC %name);

		if(!%client.isAdmin)
		{
			messageClient(%client, '', "\c6Must be a super admin to use this command.");
			return;
		}

		%int = mFloor(%int);

		if(%int < 0)
			%int = 0;
		else if(%int > 8)
			%int = 8;

		if(%name !$= "" || %name !$= null)
		{
			if(isObject(%target = findClientByName(%name)))
			{
				CityRPGData.getData(%target.bl_id).valueEducation = %int;
				%target.setGameBottomPrint();
				messageClient(%client, '', "\c6You have set\c3" SPC %target.name @ "'s \c6education to \c3" @ %int);
				messageClient(%target, '', "\c6Your education has been set to " @ %int @ " by an admin.");
			}
			else
			{
				messageClient(%client, '', "\c6Invalid user.");
			}
		}
		else
		{
			messageClient(%client, '', %name @ "<<");
			CityRPGData.getData(%client.bl_id).valueEducation = %int;
			%client.setGameBottomPrint();
			messageClient(%client, '', "\c6Your education has been set to " @ %int);
		}
	}

	function serverCmddMoney(%client, %money, %name)
	{
		%client.cityLog("/dMoney" SPC %money SPC %name);

		if(%client.isAdmin)
		{
			if(%money $= "All")
				%money = CityRPGData.getData(%client.bl_id).valueMoney;

			%money = mFloor(%money);

			if(%money > 0)
			{
				if(%name !$= "")
				{
					if(isObject(%target = findClientByName(%name)))
					{
						if(%target != %client)
						{
							messageClient(%client, '', "\c6You deducted \c3$" @ %money SPC "\c6from \c3" @ %target.name @ "\c6.");
							CityRPGData.getData(%target.bl_id).valueMoney -= %money;
							%target.SetInfo();
							return;
						}
						else
						{
							CityRPGData.getData(%client.bl_id).valueMoney -= %money;
							messageClient(%client, '', "\c6You deducted yourself \c3$" @ %money @ "\c6. Left:" SPC CityRPGData.getData(%target.bl_id).valueMoney);
							%client.SetInfo();
							return;
						}
					}
					else
						messageClient(%client, "\c6The name you entered could not be matched up to a person.");
				}
				else if(isObject(%client.player))
				{
					%target = containerRayCast(%client.player.getEyePoint(), vectorAdd(vectorScale(vectorNormalize(%client.player.getEyeVector()), 5), %client.player.getEyePoint()), $typeMasks::playerObjectType).client;

					if(isObject(%target))
					{
						CityRPGData.getData(%target.bl_id).valueMoney -= %money;
						messageClient(%client, '', "\c6You deducted yourself \c3$" @ %money @ "\c6. Left:" SPC CityRPGData.getData(%target.bl_id).valueMoney);
						%target.SetInfo();
					}
				}
				else
					messageClient(%client, '', "\c6Spawn first before you use this command or enter a valid player's name.");
			}
			else
				messageClient(%client, '', "\c6You must enter a valid amount of money to grant.");
		}
		else
			messageClient(%client, '', "\c6You must be admin to use the this command.");
	}

	function serverCmdClearMoney(%client)
	{
		%client.cityLog("/clearMoney");

		if(%client.isAdmin)
		{
			CityRPGData.getData(%client.bl_id).valueMoney = 0;
			%client.setGameBottomPrint();
			messageClient(%client, '', "\c6You have cleared your money.");
		}
		else
			messageClient(%client, '', "\c6You must be admin to use the this command.");
	}

	function serverCmddBank(%client, %Bank, %name)
	{
		%client.cityLog("/bank" SPC %Bank SPC %name);

		if(%client.isAdmin)
		{
			%Bank = mFloor(%Bank);
			if(%Bank > 0)
			{
				if(%name !$= "")
				{
					if(isObject(%target = findClientByName(%name)))
					{
						if(%target != %client)
						{
							messageClient(%client, '', "\c6You deducted \c3$" @ %Bank SPC "\c6from \c3" @ %target.name @ "\c6.");
							CityRPGData.getData(%target.bl_id).valueBank -= %Bank;
							%target.SetInfo();
							return;
						}
						else
						{
							CityRPGData.getData(%client.bl_id).valueBank -= %Bank;
							messageClient(%client, '', "\c6You deducted yourself \c3$" @ %Bank @ "\c6. Left:" SPC CityRPGData.getData(%target.bl_id).valueBank);
							%client.SetInfo();
							return;
						}
					}
					else
						messageClient(%client, "\c6The name you entered could not be matched up to a person.");
				}
				else if(isObject(%client.player))
				{
					%target = containerRayCast(%client.player.getEyePoint(), vectorAdd(vectorScale(vectorNormalize(%client.player.getEyeVector()), 5), %client.player.getEyePoint()), $typeMasks::playerObjectType).client;

					if(isObject(%target))
					{
						CityRPGData.getData(%target.bl_id).valueBank -= %Bank;
						messageClient(%client, '', "\c6You deducted yourself \c3$" @ %Bank @ "\c6. Left:" SPC CityRPGData.getData(%target.bl_id).valueBank);
						%target.SetInfo();
					}
				}
				else
					messageClient(%client, '', "\c6Spawn first before you use this command or enter a valid player's name.");
			}
			else
				messageClient(%client, '', "\c6You must enter a valid amount of Bank to grant.");
		}
		else
			messageClient(%client, '', "\c6You must be admin to use the this command.");
	}

	// Not to be confused with G Money Pranks™
	function serverCmdgmoney(%client, %money, %name)
	{
		%client.cityLog("/gmoney" SPC %money SPC %name);

		if(!isObject(%client.player))
			return;

		if(%client.isAdmin)
		{
			%money = mFloor(%money);
			if(%money > 0)
			{
				if(%name !$= "")
				{
					if(isObject(%target = findClientByName(%name)))
					{
						if(%target != %client)
						{
							messageClient(%client, '', "\c6You grant \c3$" @ %money SPC "\c6to \c3" @ %target.name @ "\c6.");
							messageClient(%target, '', "\c3An admin has granted you \c3$" @ %money @ "\c6.");
						}
						else
							messageClient(%client, '', "\c6You grant yourself \c3$" @ %money @ "\c6.");
							CityRPGData.getData(%target.bl_id).valueMoney += %money;
							%target.SetInfo();
					}
					else
						messageClient(%client, "\c6The name you entered could not be matched up to a person.");
				}
				else if(isObject(%client.player))
				{
					%target = containerRayCast(%client.player.getEyePoint(), vectorAdd(vectorScale(vectorNormalize(%client.player.getEyeVector()), 5), %client.player.getEyePoint()), $typeMasks::playerObjectType).client;
					if(isObject(%target))
					{
						messageClient(%client, '', "\c6You grant \c3$" @ %money SPC "\c6to \c3" @ %target.name @ "\c6.");
						messageClient(%target, '', "\c3An admin has granted you \c3$" @ %money @ "\c6.");
						CityRPGData.getData(%target.bl_id).valueMoney += %money;
						%target.SetInfo();
					}
				}
				else
					messageClient(%client, '', "\c6Spawn first before you use this command or enter a valid player's name.");
			}
			else
				messageClient(%client, '', "\c6You must enter a valid amount of money to grant.");
		}
		else
			messageClient(%client, '', "\c6You must be admin to use the this command.");
	}

	function serverCmdaddDemerits(%client, %dems, %name)
	{
		%client.cityLog("/addDemerits" SPC %dems SPC %name);

		if(!isObject(%client.player))
			return;

		if(%client.isSuperAdmin)
		{
			%dems = mFloor(%dems);

			if(%dems > 0)
			{
				if(%name !$= "")
				{
					if(isObject(%target = findClientByName(%name)))
					{
						commandToClient(%target, 'centerPrint', "\c6You have committed a crime. [\c3Angering a Badmin\c6]", 5);
						messageClient(%client, '', '\c6User \c3%1 \c6was given \c3%2\c6 demerits.', %target.name , %dems);
						City_AddDemerits(%target.bl_id, %dems);
					}
					else
						messageClient(%client, "\c6The name you entered could not be matched up to a person.");
					}
					else if(isObject(%client.player))
					{
						%target = containerRayCast(%client.player.getEyePoint(), vectorAdd(vectorScale(vectorNormalize(%client.player.getEyeVector()), 5), %client.player.getEyePoint()), $typeMasks::playerObjectType).client;

						if(isObject(%target))
						{
							commandToClient(%target, 'centerPrint', "\c6You have committed a crime. [\c3Angering a Badmin\c6]", 5);
							messageClient(%client, '', '\c6User \c3%1 \c6was given \c3%2\c6 demerits.', %target.name , %dems);
							City_AddDemerits(%target.bl_id, %dems);
						}
					}
					else
						messageClient(%client, '', "\c6Spawn first before you use this command or enter a valid player's name.");
				}
				else
					messageClient(%client, '', "\c6You must enter a valid number to grant.");
			}
			else
				messageClient(%client, '', "\c6You must be admin to use the this command.");
	}

	function serverCmdresetuser(%client, %name)
	{
		%client.cityLog("/resetuser" SPC %name);

		if(!isObject(%client.player))
			return;

		if(%client.isAdmin)
		{
			if(%name !$= "")
			{
				if(isObject(%target = findClientByName(%name)))
				{
					messageClient(%target, '', "\c6Your account was reset by an admin.");
					messageClient(%client, '', "\c3" @ %target.name @ "\c6's account was reset.");
					CityRPGData.removeData(%target.bl_id);
					CityRPGData.addData(%target.bl_id);
					if(isObject(%target.player))
					{
						%target.player.delete();
						%target.spawnPlayer();
					}
				}
				else
					messageClient(%client, '', "\c6That person does not exist.");
			}
			else
				messageClient(%client, '' , "\c6Please enter a name.");
		}
	}

	function serverCmdcleanse(%client,%name)
	{
		%client.cityLog("/cleanse" SPC %name);

		if(!isObject(%client.player))
			return;

		if(%client.isAdmin)
		{
			if(%name $= "")
			{
				if(CityRPGData.getData(%client.bl_id).valueDemerits > 0)
				{
					CityRPGData.getData(%client.bl_id).valueDemerits = 0;
					messageClient(%client, '', "\c6The heat is gone.");
					%client.setInfo();
				}
				else
					messageClient(%client, '', "You are not wanted!");
			}
			else if(isObject(findClientByName(%name)))
			{
				%target = findClientByName(%name);
				CityRPGData.getData(%client.bl_id).valueDemerits = 0;
				messageClient(%client, '', "\c6You cleared \c3" @ %target.name @ "\c6's demerits.");
				messageClient(%target, '', "\c6Your demerits have vanished.");
				%target.setInfo();
			}
		}
	}

	function serverCmdedithunger(%client, %int)
	{
		%client.cityLog("/edithunger" SPC %int);

		if(!isObject(%client.player))
			return;

		if(%client.isAdmin && mFloor(%int))
		{
			%int = mFloor(%int);

			if(%int > 12)
				%int = 12;
			else if(%int < 1)
				%int = 1;

			CityRPGData.getData(%client.bl_id).valueHunger = %int;
			%client.setGameBottomPrint();

			%client.doCityHungerStatus();
		}
	}

	function serverCmdupdateScore(%client)
	{
		%client.cityLog("/updateScore");

		if(!isObject(%client.player))
			return;

		if(%client.isAdmin)
		{
			for(%d = 0; %d < ClientGroup.getCount(); %d++)
			{
				%subClient = ClientGroup.getObject(%d);
				gameConnection::setScore(%subClient, %score);
			}

			messageClient(%client, '', "\c6You've updated the score.");
		}
		else
		{
			messageClient(%client, '', "\c6Must be admin to use this command.");
		}
	}

	function serverCmdRespawnAllPlayers(%client)
	{
		%client.cityLog("/respawnAllPlayers");

		if(!isObject(%client.player))
			return;

		if(%client.isAdmin)
		{
			messageAll('', '\c3%1\c5 respawned all players.', %client.name);

			for(%a = 0; %a < ClientGroup.getCount(); %a++)
				ClientGroup.getObject(%a).spawnPlayer();
		}
	}

	function serverCmdsetMinerals(%client, %int)
	{
		%client.cityLog("/setMinerals" SPC %int);

		if(!isObject(%client.player))
			return;

		if(%client.isSuperAdmin)
		{
			CitySO.minerals = %int;
			messageClient(%client, '', "\c6City's minerals set to \c3" @ %int @ "\c6.");
		}
		else
		{
			messageClient(%client, '', "\c6You need to be a Super Admin to use this function.");
		}
	}

	function serverCmdsetLumber(%client, %int)
	{
		%client.cityLog("/setLumber" SPC %int);

		if(!isObject(%client.player))
			return;

		if(%client.isSuperAdmin)
		{
			CitySO.lumber = %int;
			messageClient(%client, '', "\c6City's lumber set to \c3" @ %int @ "\c6.");
		}
		else
		{
			messageClient(%client, '', "\c6You need to be a Super Admin to use this function.");
		}
	}
};

deactivatePackage("CityRPG_Commands");
activatePackage("CityRPG_Commands");
