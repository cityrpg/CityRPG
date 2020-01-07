package CityRPG_MainPackage
{
	// ============================================================
	// Brick Functions
	// ============================================================
	function fxDTSBrick::onActivate(%brick, %obj, %client, %pos, %dir)
	{
		parent::onActivate(%brick, %obj, %client, %pos, %dir);
		if(isObject(%brick.getDatablock().CityRPGMatchingLot))
		{
			if(!isObject(%client.player.serviceOrigin))
			{
				%client.player.serviceType = "zone";
				%client.player.serviceOrigin = %brick;
				%client.player.serviceFee = %brick.getDatablock().CityRPGMatchingLot.initialPrice;
				messageClient(%client, '', '\c6It costs \c3%1\c6 to build in this zone. Type \c3/yes\c6 to accept and \c3/no\c6 to decline', %client.player.serviceFee);
			}
			else if(isObject(%client.player.serviceOrigin) && %client.player.serviceOrigin != %brick)
				messageClient(%client, '', "\c6You already have an active transfer. Type \c3/no\c6 to decline it.");
		}
	}

	function fxDTSBrick::onDeath(%brick)
	{
		switch(%brick.getDatablock().CityRPGBrickType)
		{
			case 1:
				%brick.handleCityRPGBrickDelete();
			case 2:
				%brick.handleCityRPGBrickDelete();
			case 3:
				if(getWord($CityRPG::temp::spawnPoints, 0) == %brick)
					$CityRPG::temp::spawnPoints = strReplace($CityRPG::temp::spawnPoints, %brick @ " ", "");
				else
					$CityRPG::temp::spawnPoints = strReplace($CityRPG::temp::spawnPoints, " " @ %brick, "");
		}

		parent::onDeath(%brick);
	}

	// New Duplicator compatibility
	function ND_Selection::plantBrick(%this, %i, %position, %angleID, %brickGroup, %client, %bl_id)
	{
		%brick = Parent::plantBrick(%this, %i, %position, %angleID, %brickGroup, %client, %bl_id);

		if(%brick != -1 && %brick.getDataBlock().CityRPGBrickType == $CityBrick_Lot)
		{
			// Force init as a new lot
			%brick.initializeCityLot();
			%brick.assignCityLotName();
		}

		City_OnPlant(%brick);
		return %brick;
	}

	function fxDTSBrick::onPlant(%brick)
	{
		Parent::onPlant(%brick);

		City_OnPlant(%brick);
	}

	// TODO: Rewrite this
	function City_OnPlant(%brick) {
		if(!isObject(%brick)) {
			return;
		}

		%client = %brick.getGroup().client;

		if(%brick == $LastLoadedBrick && %brick.getDatablock().CityRPGBrickType !$= $CityBrick_Lot) {
			switch(%brick.getDatablock().CityRPGBrickType) {
				case $CityBrick_Info:
					%brick.schedule(1, "createCityTrigger");
				case $CityBrick_Spawn:
					$CityRPG::temp::spawnPoints = ($CityRPG::temp::spawnPoints $= "") ? %brick : $CityRPG::temp::spawnPoints SPC %brick;
				case $CityBrick_ResourceLumber:
					%seed = getRandom(1, ResourceSO.treeCount);
					%brick.id = ResourceSO.tree[%seed].id;
					%brick.BPH = ResourceSO.tree[%seed].BPH;
					%brick.name = ResourceSO.tree[%seed].name;
					%brick.totalHits = ResourceSO.tree[%seed].totalHits;
					%brick.color = getClosestPaintColor(ResourceSO.tree[%seed].color);
					%brick.setColor(%brick.color);
				case $CityBrick_ResourceOre:
					%seed = getRandom(1, ResourceSO.mineralCount);
					%brick.id = ResourceSO.mineral[%seed].id;
					%brick.BPH = ResourceSO.mineral[%seed].BPH;
					%brick.name = ResourceSO.mineral[%seed].name;
					%brick.totalHits = ResourceSO.mineral[%seed].totalHits;
					%brick.color = getClosestPaintColor(ResourceSO.mineral[%seed].color);
					%brick.setColor(%brick.color);
			}

			return;
		}
		else if(%brick.getDataBlock().CityRPGBrickType $= $CityBrick_Lot)
		{
			if(!%client.isAdmin)
			{
				commandToClient(%client, 'centerPrint', "\c6Only admins can create new lot bricks.<br>\c6To purchase a lot, you must find an unclaimed lot and type \c3/lot\c6 over it.", 3);
				%brick.schedule(0, "delete");
				return -1;
			}
		}

		if(%brick.getDatablock().CityRPGBrickType && isObject(%brick.client)) {
			%brick.client.cityLog("Attempt to plant " @ %brick.getDatablock().getName());
		}

		if(isObject(%client) || %brick.getDatablock().CityRPGBrickType == $CityBrick_Lot) {
			if(mFloor(getWord(%brick.rotation, 3)) == 90) {
				%boxSize = getWord(%brick.getDatablock().brickSizeY, 1) / 2.5 SPC getWord(%brick.getDatablock().brickSizeX, 0) / 2.5 SPC getWord(%brick.getDatablock().brickSizeZ, 2) / 2.5;
			}
			else {
				%boxSize = getWord(%brick.getDatablock().brickSizeX, 1) / 2.5 SPC getWord(%brick.getDatablock().brickSizeY, 0) / 2.5 SPC getWord(%brick.getDatablock().brickSizeZ, 2) / 2.5;
			}

			initContainerBoxSearch(%brick.getWorldBoxCenter(), %boxSize, $typeMasks::triggerObjectType);

			while(isObject(%trigger = containerSearchNext())) {
				if(%trigger.getDatablock() == CityRPGLotTriggerData.getID()) {
					%lotTrigger = %trigger;
				}
			}

			if(%lotTrigger && %brick.getDatablock().CityRPGBrickType != $CityBrick_Lot)
			{
				%lotTriggerMinX = getWord(%lotTrigger.getWorldBox(), 0);
				%lotTriggerMinY = getWord(%lotTrigger.getWorldBox(), 1);
				%lotTriggerMinZ = getWord(%lotTrigger.getWorldBox(), 2);

				%lotTriggerMaxX = getWord(%lotTrigger.getWorldBox(), 3);
				%lotTriggerMaxY = getWord(%lotTrigger.getWorldBox(), 4);
				%lotTriggerMaxZ = getWord(%lotTrigger.getWorldBox(), 5);

				%brickMinX = getWord(%brick.getWorldBox(), 0) + 0.0016;
				%brickMinY = getWord(%brick.getWorldBox(), 1) + 0.0013;
				%brickMinZ = getWord(%brick.getWorldBox(), 2) + 0.00126;

				%brickMaxX = getWord(%brick.getWorldBox(), 3) - 0.0016;
				%brickMaxY = getWord(%brick.getWorldBox(), 4) - 0.0013;
				%brickMaxZ = getWord(%brick.getWorldBox(), 5) - 0.00126;

				// Ensure that the bricks do not go over the edge of the lot boundaries.
				if(%brickMinX < %lotTriggerMinX || %brickMinY < %lotTriggerMinY || %brickMinZ < %lotTriggerMinZ) {
					%brick.schedule(0, "delete");
					return -1;
				}

				if(%brickMaxX > %lotTriggerMaxX || %brickMaxY > %lotTriggerMaxY || %brickMaxZ > %lotTriggerMaxZ) {
					%brick.schedule(0, "delete");
					return -1;
				}

				if(%brick.getDatablock().CityRPGBrickAdmin && !%client.isAdmin) {
					if(%brick.getDatablock().CityRPGBrickPlayerPrivliage) {
						if(CityRPGData.getData(%client.bl_id).valueMoney >= %brick.getDatablock().CityRPGBrickCost) {
							messageClient(%client,'',"\c6You have bought the brick for \c3$" @ %brick.getDatablock().CityRPGBrickCost @ "\c6.");
							CityRPGData.getData(%client.bl_id).valueMoney -= %brick.getDatablock().CityRPGBrickCost;

							switch(%brick.getDatablock().CityRPGBrickType) {
								case $CityBrick_Info:
									%brick.schedule(0, "createCityTrigger");
								case $CityBrick_Spawn:
									$CityRPG::temp::spawnPoints = ($CityRPG::temp::spawnPoints $= "") ? %brick : $CityRPG::temp::spawnPoints SPC %brick;
								case $CityBrick_ResourceLumber:
									%seed = getRandom(1, ResourceSO.treeCount);
									%brick.id = ResourceSO.tree[%seed].id;
									%brick.BPH = ResourceSO.tree[%seed].BPH;
									%brick.name = ResourceSO.tree[%seed].name;
									%brick.totalHits = ResourceSO.tree[%seed].totalHits;
									%brick.color = getClosestPaintColor(ResourceSO.tree[%seed].color);
									%brick.setColor(%brick.color);
								case $CityBrick_ResourceOre:
									%seed = getRandom(1, ResourceSO.mineralCount);
									%brick.id = ResourceSO.mineral[%seed].id;
									%brick.BPH = ResourceSO.mineral[%seed].BPH;
									%brick.name = ResourceSO.mineral[%seed].name;
									%brick.totalHits = ResourceSO.mineral[%seed].totalHits;
									%brick.color = getClosestPaintColor(ResourceSO.mineral[%seed].color);
									%brick.setColor(%brick.color);
								default:
									if(%brick.getDatablock().getID() == brickVehicleSpawnData.getID()) {
										if(CityRPGData.getData(%client.bl_id).valueMoney >= mFloor($CityRPG::prices::vehicleSpawn)) {
											commandToClient(%client, 'centerPrint', "\c6You have paid \c3$" @ mFloor($CityRPG::prices::vehicleSpawn) @ "\c6 to plant this vehicle spawn.", 3);
											CityRPGData.getData(%client.bl_id).valueMoney -= $CityRPG::prices::vehicleSpawn;
											schedule(3000, 0, removeMoney, %brick, %client, mFloor($CityRPG::prices::vehicleSpawn));
										}
										else {
											commandToClient(%client, 'centerPrint', "\c6You need at least \c3$" @ mFloor($CityRPG::prices::vehicleSpawn) SPC "\c6in order to plant this vehicle spawn!", 3);
											%brick.schedule(0, "delete");
											return -1;
										}
									}
							}
						} else {
							%brick.schedule(0, "delete");
							commandToClient(%client, 'centerPrint', "\c6You don't have enough money to buy this brick! \c3($" @ %brick.getDatablock().CityRPGBrickCost @ ")", 3);
							return -1;
						}
					} else {
						commandToClient(%client, 'centerPrint', "You must be an admin to plant this brick.", 3);
						%brick.schedule(0, "delete");
						return -1;
					}
				} else {
					switch(%brick.getDatablock().CityRPGBrickType) {
						case $CityBrick_Info:
							%brick.schedule(0, "createCityTrigger");
						case $CityBrick_Spawn:
							$CityRPG::temp::spawnPoints = ($CityRPG::temp::spawnPoints $= "") ? %brick : $CityRPG::temp::spawnPoints SPC %brick;
						case $CityBrick_ResourceLumber:
							%seed = getRandom(1, ResourceSO.treeCount);
							%brick.id = ResourceSO.tree[%seed].id;
							%brick.BPH = ResourceSO.tree[%seed].BPH;
							%brick.name = ResourceSO.tree[%seed].name;
							%brick.totalHits = ResourceSO.tree[%seed].totalHits;
							%brick.color = getClosestPaintColor(ResourceSO.tree[%seed].color);
							%brick.setColor(%brick.color);
						case $CityBrick_ResourceOre:
							%seed = getRandom(1, ResourceSO.mineralCount);
							%brick.id = ResourceSO.mineral[%seed].id;
							%brick.BPH = ResourceSO.mineral[%seed].BPH;
							%brick.name = ResourceSO.mineral[%seed].name;
							%brick.totalHits = ResourceSO.mineral[%seed].totalHits;
							%brick.color = getClosestPaintColor(ResourceSO.mineral[%seed].color);
							%brick.setColor(%brick.color);
						default:
							if(!%brick.brickGroup.client.isAdmin && %brick.getDatablock().getID() == brickVehicleSpawnData.getID()) {
								if(CityRPGData.getData(%client.bl_id).valueMoney >= mFloor($CityRPG::prices::vehicleSpawn)) {
									commandToClient(%client, 'centerPrint', "\c6You have paid \c3$" @ mFloor($CityRPG::prices::vehicleSpawn) @ "\c6 to plant this vehicle spawn.", 3);
									CityRPGData.getData(%client.bl_id).valueMoney -= $CityRPG::prices::vehicleSpawn;
									schedule(3000, 0, removeMoney, %brick, %client, mFloor($CityRPG::prices::vehicleSpawn));
									%client.setInfo();
								}
								else {
									commandToClient(%client, 'centerPrint', "\c6You need at least \c3$" @ mFloor($CityRPG::prices::vehicleSpawn) SPC "\c6in order to plant this vehicle spawn!", 3);
									%brick.schedule(0, "delete");
									return -1;
								}
							}
					}
				}
			}
			else if(%lotTrigger && %brick.getDatablock().CityRPGBrickType == $CityBrick_Lot) {
				commandToClient(%client, 'centerPrint', "Only Chuck Norris can put a lot on top of another lot.", 3);
				%brick.schedule(0, "delete");
				return -1;
			}
			else if(!%lotTrigger && %brick.getDatablock().CityRPGBrickType == $CityBrick_Lot) {
				%brick.schedule(0, "createCityTrigger");
			}
			else if(!%lotTrigger) {
				if(!%client.isAdmin) {
					commandToClient(%client, 'centerPrint', "You cannot plant a brick outside of a lot.\n\c6Use a lot brick to start your build!", 3);
					%brick.schedule(0, "delete");
					return -1;
				}
				else {
					// Admin check
					if(%brick.getDatablock().CityRPGBrickAdmin && !%client.isAdmin) {
						if(%brick.getDatablock().CityRPGBrickPlayerPrivliage) {
							if(CityRPGData.getData(%client.bl_id).valueMoney >= %brick.getDatablock().CityRPGBrickCost) {
								messageClient(%client,'',"\c6You have bought the brick for \c3$" @ %brick.getDatablock().CityRPGBrickCost @ "\c6.");
								CityRPGData.getData(%client.bl_id).valueMoney -= %brick.getDatablock().CityRPGBrickCost;
								switch(%brick.getDatablock().CityRPGBrickType) {
									case $CityBrick_Info:
										%brick.schedule(0, "createCityTrigger");
									case $CityBrick_Spawn:
										$CityRPG::temp::spawnPoints = ($CityRPG::temp::spawnPoints $= "") ? %brick : $CityRPG::temp::spawnPoints SPC %brick;
									case $CityBrick_ResourceLumber:
										%seed = getRandom(1, ResourceSO.treeCount);
										%brick.id = ResourceSO.tree[%seed].id;
										%brick.BPH = ResourceSO.tree[%seed].BPH;
										%brick.name = ResourceSO.tree[%seed].name;
										%brick.totalHits = ResourceSO.tree[%seed].totalHits;
										%brick.color = getClosestPaintColor(ResourceSO.tree[%seed].color);
										%brick.setColor(%brick.color);
									case $CityBrick_ResourceOre:
										%seed = getRandom(1, ResourceSO.mineralCount);
										%brick.id = ResourceSO.mineral[%seed].id;
										%brick.BPH = ResourceSO.mineral[%seed].BPH;
										%brick.name = ResourceSO.mineral[%seed].name;
										%brick.totalHits = ResourceSO.mineral[%seed].totalHits;
										%brick.color = getClosestPaintColor(ResourceSO.mineral[%seed].color);
										%brick.setColor(%brick.color);
									default:
										if(%brick.getDatablock().getID() == brickVehicleSpawnData.getID()) {
											if(CityRPGData.getData(%client.bl_id).valueMoney >= mFloor($CityRPG::prices::vehicleSpawn)) {
												commandToClient(%client, 'centerPrint', "\c6You have paid \c3$" @ mFloor($CityRPG::prices::vehicleSpawn) @ "\c6 to plant this vehicle spawn.", 3);
												CityRPGData.getData(%client.bl_id).valueMoney -= $CityRPG::prices::vehicleSpawn;
												schedule(3000, 0, removeMoney, %brick, %client, mFloor($CityRPG::prices::vehicleSpawn));
											}
											else {
												commandToClient(%client, 'centerPrint', "\c6You need at least \c3$" @ mFloor($CityRPG::prices::vehicleSpawn) SPC "\c6in order to plant this vehicle spawn!", 3);
												%brick.schedule(0, "delete");
											}
										}
								}
							} else {
								%brick.schedule(0, "delete");
								return -1;
								commandToClient(%client, 'centerPrint', "\c6You don't have enough money to buy this brick! \c3($" @ %brick.getDatablock().CityRPGBrickCost @ ")", 3);
							}
						} else {
							commandToClient(%client, 'centerPrint', "You must be an admin to plant this brick.", 3);
							%brick.schedule(0, "delete");
							return -1;
						}
					}
					else {
						switch(%brick.getDatablock().CityRPGBrickType) {
							case $CityBrick_Info:
								%brick.schedule(0, "createCityTrigger");
							case $CityBrick_Spawn:
								$CityRPG::temp::spawnPoints = ($CityRPG::temp::spawnPoints $= "") ? %brick : $CityRPG::temp::spawnPoints SPC %brick;
							case $CityBrick_ResourceLumber:
								%seed = getRandom(1, ResourceSO.treeCount);
								%brick.id = ResourceSO.tree[%seed].id;
								%brick.BPH = ResourceSO.tree[%seed].BPH;
								%brick.name = ResourceSO.tree[%seed].name;
								%brick.totalHits = ResourceSO.tree[%seed].totalHits;
								%brick.color = getClosestPaintColor(ResourceSO.tree[%seed].color);
								%brick.setColor(%brick.color);
							case $CityBrick_ResourceOre:
								%seed = getRandom(1, ResourceSO.mineralCount);
								%brick.id = ResourceSO.mineral[%seed].id;
								%brick.BPH = ResourceSO.mineral[%seed].BPH;
								%brick.name = ResourceSO.mineral[%seed].name;
								%brick.totalHits = ResourceSO.mineral[%seed].totalHits;
								%brick.color = getClosestPaintColor(ResourceSO.mineral[%seed].color);
								%brick.setColor(%brick.color);
							default:
								if(%brick.getDatablock().getID() == brickVehicleSpawnData.getID()) {
									if(CityRPGData.getData(%client.bl_id).valueMoney >= mFloor($CityRPG::prices::vehicleSpawn)) {
										commandToClient(%client, 'centerPrint', "\c6You have paid \c3$" @ mFloor($CityRPG::prices::vehicleSpawn) @ "\c6 to plant this vehicle spawn.", 3);
										CityRPGData.getData(%client.bl_id).valueMoney -= $CityRPG::prices::vehicleSpawn;
										schedule(3000, 0, removeMoney, %brick, %client, mFloor($CityRPG::prices::vehicleSpawn));
									}
									else {
										commandToClient(%client, 'centerPrint', "\c6You need at least \c3$" @ mFloor($CityRPG::prices::vehicleSpawn) SPC "\c6in order to plant this vehicle spawn!", 3);
										%brick.schedule(0, "delete");
										return -1;
									}
								}
						}
					}
				}
			}
			else {
				error("City_OnPlant() - Brick fell through tests!");
			}
		}
		else {
			switch(%brick.getDatablock().CityRPGBrickType) {
				case $CityBrick_Info:
					%brick.schedule(0, "createCityTrigger");
				case $CityBrick_Spawn:
					$CityRPG::temp::spawnPoints = ($CityRPG::temp::spawnPoints $= "") ? %brick : $CityRPG::temp::spawnPoints SPC %brick;
				case $CityBrick_ResourceLumber:
					%seed = getRandom(1, ResourceSO.treeCount);
					%brick.id = ResourceSO.tree[%seed].id;
					%brick.BPH = ResourceSO.tree[%seed].BPH;
					%brick.name = ResourceSO.tree[%seed].name;
					%brick.totalHits = ResourceSO.tree[%seed].totalHits;
					%brick.color = getClosestPaintColor(ResourceSO.tree[%seed].color);
					%brick.setColor(%brick.color);
				case $CityBrick_ResourceOre:
					%seed = getRandom(1, ResourceSO.mineralCount);
					%brick.id = ResourceSO.mineral[%seed].id;
					%brick.BPH = ResourceSO.mineral[%seed].BPH;
					%brick.name = ResourceSO.mineral[%seed].name;
					%brick.totalHits = ResourceSO.mineral[%seed].totalHits;
					%brick.color = getClosestPaintColor(ResourceSO.mineral[%seed].color);
					%brick.setColor(%brick.color);
			}
		}

		if(%brick.getDatablock().CityRPGBrickType && isObject(%brick.client)) {
			%brick.client.cityLog("---- Passed CityRPG checks", 1);
		}
	}

	function fxDTSBrick::onRemove(%brick,%client)
	{
		switch(%brick.getDatablock().CityRPGBrickType)
		{
			case $CityBrick_Lot:
				%brick.handleCityRPGBrickDelete();
			case $CityBrick_Info:
				%brick.handleCityRPGBrickDelete();
			case $CityBrick_Spawn:
				if(getWord($CityRPG::temp::spawnPoints, 0) == %brick)
					$CityRPG::temp::spawnPoints = strReplace($CityRPG::temp::spawnPoints, %brick @ " ", "");
				else
					$CityRPG::temp::spawnPoints = strReplace($CityRPG::temp::spawnPoints, " " @ %brick, "");
		}

		parent::onRemove(%brick);
	}

	function fxDTSBrick::setVehicle(%brick, %vehicle)
	{
		if(%brick.getDatablock().getName() !$= "CityRPGPoliceVehicleData")
		{
			if(!isObject(%brick.getGroup().client) || !%brick.getGroup().client.isAdmin)
			{
				if(isObject(%vehicle))
				{
					for(%a = 0; $CityRPG::vehicles::banned[%a] !$= "" && !%hasBeenBanned; %a++)
					{
						if(%vehicle.getName() $= $CityRPG::vehicles::banned[%a])
						{
							if(isObject(%brick.getGroup().client))
							{
								messageClient(%brick.getGroup().client, '', "\c6Standard users may not spawn a\c3" SPC %vehicle.uiName @ "\c6.");
							}
							%vehicle = 0;
							%hasBeenBanned = true;
						}
					}
				}
			}
		}

		parent::setVehicle(%brick, %vehicle);
	}

	function fxDTSBrick::setItem(%brick, %datablock, %client)
	{
		if(!%brick.getDatablock().CityRPGPermaspawn && %brick != $LastLoadedBrick)
		{
			if(!isObject(%brick.item) || %brick.item.getDatablock() != %datablock)
			{
				%ownerBG = getBrickGroupFromObject(%brick);

				if(%ownerBG.client.isAdmin)
					parent::setItem(%brick, %datablock, %client);
			}
			else
				parent::setItem(%brick, %datablock, %client);
		}
		else
			parent::setItem(%brick, %datablock, %client);
	}

	function fxDTSBrick::setItem(%brick, %datablock, %client)
	{
		if(!%brick.getDatablock().CityRPGPermaspawn && %brick != $LastLoadedBrick)
		{
			if(!isObject(%brick.item) || %brick.item.getDatablock() != %datablock)
			{
				%ownerBG = getBrickGroupFromObject(%brick);

				if(%ownerBG.client.isAdmin)
					parent::setItem(%brick, %datablock, %client);
			}
			else
				parent::setItem(%brick, %datablock, %client);
		}
		else
			parent::setItem(%brick, %datablock, %client);
	}

	function fxDTSBrick::spawnItem(%brick, %pos, %datablock, %client)
	{
		if(isObject(%owner = getBrickGroupFromObject(%brick).client) && %owner.isAdmin)
			parent::spawnItem(%brick, %pos, %datablock, %client);
	}

	function fxDTSBrick::respawnVehicle(%brick, %client)
	{
		if(%brick.getDatablock().getName() $= "CityRPGPoliceVehicleData")
		{
			%stars = City_GetMaxStars();
			if(%stars == 6)
				%brick.setVehicle(tankVehicle);
			else if(%stars >= 3)
				%brick.setVehicle(blockoCarVehicle);
			else
				%brick.setVehicle(horseArmor);
		}

		parent::respawnVehicle(%brick, %client);
	}

	// Hack to work around wonky packaging issues
	function fxDTSBrick::onCityLoadPlant(%this, %brick)
	{

	}

	function fxDTSBrick::onLoadPlant(%this, %brick)
	{
		parent::onLoadPlant(%this, %brick);
		City_OnPlant(%this);
		%this.onCityLoadPlant(%this, %brick);
	}

	// ============================================================
	// Client Packages
	// ============================================================
	function gameConnection::cityLog(%client, %data, %nodate) {
		if(!$Pref::Server::City::loggerEnabled) {
			return;
		}

		// Re-open the file for each item that is logged.
		// This probably isn't great for performance, but it's much more secure
		// because we need to be able to retain logs when the server hard crashes.
		%client.logFile.openForAppend("config/server/CityRPG/Logs/" @ %client.bl_id @ ".log");
		%client.logFile.writeLine((!%nodate?"[" @ getDateTime() @ "] ":"") @ %data);
		%client.logFile.close();
	}

	function gameConnection::onClientEnterGame(%client)
	{
		parent::onClientEnterGame(%client);

		if(isObject(CityRPGMini))
			CityRPGMini.addMember(%client);
		else
			City_Init_Minigame();

		//applyForcedBodyParts();

		if(CityRPGData.getData(%client.bl_id).valueJobID $= "")
		{
			// Reset if there is no job data.
			resetFree(%client);

			messageClient(%client, '', "\c6Welcome to " @ $Pref::Server::City::name @ "!");
		}
		else
		{
			messageClient(%client, '', "\c6Welcome back! Today is " @ CalendarSO.getDateStr());
		}

		messageClient(%client, '', "\c6 - Your current job is\c3" SPC %client.getJobSO().name @ "\c6 with an income of \c3$" @ %client.getJobSO().pay @ "\c6.");

		if(CityRPGData.getData(%client.bl_id).valueStudent > 0)
		{
			messageClient(%client, '', "\c6 - You will complete your education in \c3" @ CityRPGData.getData(%client.bl_id).valueStudent @ "\c6 days.");
		}

		messageClient(%client, '', "\c6 - City mayor: \c3" @ $City::Mayor::String);
		%client.doCityHungerStatus();

		// Note: Not implemented yet.
		%earnings = CityRPGData.getData(%client.bl_id).valueShopEarnings;
		if(%earnings > 0)
		{
			messageClient(%client, '', "\c6 - You earned \c3$" @ %earnings @ "\c6 in sales while you were out.");
			CityRPGData.getData(%client.bl_id).valueShopEarnings = 0;
		}
	}

	function gameConnection::onClientLeaveGame(%client)
	{
		%time = mFloor((getRealTime()/60000)-%client.joinTimeMin);

		// Drop a warning flag if the session lasted longer than 6 hours to catch idlers
		if(%time >= 360) {
			%suffix = "(!!!)";
		}

		%client.cityLog("Left game ~" @ %time @ " min" @ %suffix);
		if($missionRunning && isObject(%client.player) && !getWord(CityRPGData.getData(%client.bl_id).valueJailData, 1))
		{
			for(%a = 0; %a < %client.player.getDatablock().maxTools; %a++)
			{
				%tool = %client.player.tool[%a];

				if(isObject(%tool))
				{
					%tool = %tool.getName();
					%tools = (%tools !$= "" ? %tools SPC %tool : %tool);
				}
			}

			if(%tools !$= "")
				CityRPGData.getData(%client.bl_id).valueTools = %tools;
			else
				error("No Tool String!");
		}

		parent::onClientLeaveGame(%client);
	}

	function GameConnection::autoadmincheck(%client)
	{
		// Logging
		%client.logFile = new fileObject();
		%client.joinTimeMin = getRealTime()/60000;
		%client.cityLog("Joined game");

		// multi-client check
		for(%a = 0; %a < ClientGroup.getCount(); %a++)
		{
			%subClient = ClientGroup.getObject(%a);

			if(%client.bl_id == %subClient.bl_id)
			{
				if(%client.getID() > %subClient.getID())
				{
					%subClient.delete();
				}
			}
		}

		parent::autoadmincheck(%client);

		if(CityRPGData.getData(%client.bl_id).valueJobID $= "")
		{
			schedule(1, 0, messageClient, %client, '', "\c2Type \c6/help starters\c2 to learn more about how to get started in CityRPG.");
		}
	}
	function onMissionLoaded()
	{
		Parent::onMissionLoaded();
		if($ND::Version $= "1.6.2")
		{
			if(!isFile("Add-Ons/Tool_NewDuplicator/DGEDIT.txt"))
			{
				$Pref::Server::ND::FillBricksAdminOnly = 1;
			}
		}
	}
	function GameConnection::startLoad(%client)
	{
		Parent::startLoad(%client);
		if(%client.bl_id==getNumKeyID())
		{
			if($ND::Version $= "1.6.2")
			{
				if(!isFile("Add-Ons/Tool_NewDuplicator/DGEDIT.txt"))
				{
					if(!$ToldHostAboutFillBricksGlitch)
					{
						messageClient(%client,'',"\c5CityRPG4\c6: Version 1.6.2 of the New Duplicator contains a known bug which resets the fillbricks admin control setting every restart.");
						messageClient(%client,'',"\c5CityRPG4\c6: The fillbricks command can be used by non-admins to place admin-only resources, such as minerals.");
						messageClient(%client,'',"\c5CityRPG4\c6: We have set the fillbricks admin control setting to \"Admin-Only\" for you convenience.");
						$ToldHostAboutFillBricksGlitch = 1;
					}
				}
			}
		}
	}

	function gameConnection::spawnPlayer(%client)
	{
		%client.applyForcedBodyColors();
		%client.applyForcedBodyParts();

		parent::spawnPlayer(%client);

		if(!CityRPGData.getData(%client.bl_id))
			return;

		if(%client.moneyOnSuicide > 0)
		{
			CityRPGData.getData(%client.bl_id).valueMoney = %client.moneyOnSuicide;
		}
		if(%client.lumberOnSuicide > 0)
		{
			CityRPGData.getData(%client.bl_id).valueResources = %client.lumberOnSuicide;
		}

		%client.hasBeenDead = 0;
		%client.moneyOnSuicide = 0;
		%client.lumberOnSuicide = 0;

		%client.player.setScale("1 1 1");
		%client.player.setDatablock(%client.getJobSO().db);
		%client.player.giveDefaultEquipment();

		if(CityRPGData.getData(%client.bl_id).valueHunger < 3) {
			// Set a 'damage override' so we can package Player::emote and hide the pain better than Harold.
			%client.player.cityDamageOverride = 1;
			%client.player.setHealth(%client.player.dataBlock.maxDamage*0.80);
		}

		%client.SetInfo();
	}

	function gameConnection::onDeath(%client, %killerPlayer, %killer, %damageType, %unknownA)
	{
		if(!getWord(CityRPGData.getData(%client.bl_id).valueJailData, 1))
		{
			if(%client.player.currTool)
				serverCmddropTool(%client, %client.player.currTool);
		}

		if(isObject(%client.CityRPGTrigger))
			%client.CityRPGTrigger.getDatablock().onLeaveTrigger(%client.CityRPGTrigger, %client.player);

		if(isObject(%killer) && %killer != %client)
		{
			if(CityRPGData.getData(%client.bl_id).valueBounty > 0)
			{
				if(!%killer.getJobSO().bountyClaim)
				{
					commandToClient(%killer, 'centerPrint', "\c6You have committed a crime. [\c3Claiming a Hit\c6]", 1);
					City_AddDemerits(%killer.bl_id, $CityRPG::demerits::bountyClaiming);
				}

				%killer.cityLog("Claim bounty on " @ %client.bl_id @ " for $" @ CityRPGData.getData(%client.bl_id).valueBounty);
				messageClient(%killer, '', "\c6Hit was completed successfully. The money has been wired to your bank account.");
				CityRPGData.getData(%killer.bl_id).valueBank += CityRPGData.getData(%client.bl_id).valueBounty;
				CityRPGData.getData(%client.bl_id).valueBounty = 0;
			}
			else if(City_illegalAttackTest(%killer, %client))
			{
				if(%killer.lastKill + 15 >= $sim::time)
				{
					commandToClient(%killer, 'centerPrint', "\c6You have committed a crime. [\c3Killing Spree\c6]", 1);
					City_AddDemerits(%killer.bl_id, ($CityRPG::demerits::murder * 1.5));
				}
				else
				{
					commandToClient(%killer, 'centerPrint', "\c6You have committed a crime. [\c3Murder\c6]", 1);
					City_AddDemerits(%killer.bl_id, $CityRPG::demerits::murder);
				}
				%killer.lastKill = $sim::time;
			}
		}

		CityRPGData.getData(%client.bl_id).valueTools = "";
		CityRPGData.getData(%client.bl_id).valueResources = "0 0";
		parent::onDeath(%client, %player, %killer, %damageType, %unknownA);
	}

	function gameConnection::setScore(%client, %score)
	{
		if($Score::Type $= "Money")
			%score = CityRPGData.getData(%client.bl_id).valueMoney + CityRPGData.getData(%client.bl_id).valueBank;
		else if($Score::Type $= "Edu")
			%score = CityRPGData.getData(%client.bl_id).valueEducation;
		else
			%score = CityRPGData.getData(%client.bl_id).valueMoney + CityRPGData.getData(%client.bl_id).valueBank;
		parent::setScore(%client, %score);
	}

	function gameConnection::bottomPrint(%this, %text, %time, %showBar)
	{
		if(%time > 0)
		{
			%this.cityHudTimer = $sim::time + %time;
		}

		parent::bottomPrint(%this, %text, %time, %showBar);
	}

	function bottomPrint(%client, %message, %time, %lines)
	{
		if(%client.getID() == CityRPGHostClient.getID())
		{
			CityRPGHostClient.onBottomPrint(%message);
		}

		parent::bottomPrint(%client, %message, %time, %lines);
	}

	// ============================================================
	// Player Packages
	// ============================================================
	function player::mountImage(%this, %datablock, %slot)
	{
		if(!getWord(CityRPGData.getData(%this.client.bl_id).valueJailData, 1) || $CityRPG::demerits::jail::image[%datablock.getName()])
			parent::mountImage(%this, %datablock, %slot);
		else
			%this.playthread(2, root);
	}

	function player::damage(%this, %obj, %pos, %damage, %damageType)
	{
		if(isObject(%obj.client) && isObject(%this.client) && isObject(%this))
		{
			if(%obj.getDatablock().getName() $= "deathVehicle")
				return;

			if(%this.getDamageLevel() < %this.getDatablock().maxDamage)
			{
				%atkr = %obj.client;
				%vctm = %this.client;
				%atkrSO = CityRPGData.getData(%atkr.bl_id);
				%vctmSO = CityRPGData.getData(%vctm.bl_id);

				if(!getWord(%atkr.valueJailData, 1))
				{
					if(City_illegalAttackTest(%atkr, %vctm))
					{
						commandToClient(%atkr, 'centerPrint', "\c6You have committed a crime. [\c3Assault\c6]", 1);

						if(!%atkr.getWantedLevel())
							%demerits = $Pref::Server::City::demerits::wantedLevel - %atkr.valueDemerits;
						else
							%demerits = mFloor($CityRPG::demerits::murder * (%damage / %this.getDatablock().maxDamage));

						City_AddDemerits(%atkr.bl_id, $CityRPG::demerits::hittingInnocents);
					}
				}
				else
					return;
			}
		}

		parent::damage(%this, %obj, %pos, %damage, %damageType);

		if(isObject(%obj.client))
		{
			%obj.client.setGameBottomPrint();
		}
	}

	function player::setShapeNameColor(%this, %color)
	{
		if(isObject(%client = %this.client) && isObject(%client.player) && %this.getState() !$= "dead")
		{
			if(%client.getWantedLevel())
				%color = "1 0 0 1";
			else if(CityRPGData.getData(%client.bl_id).valueReincarnated)
				%color = "1 1 0 1";
		}

		parent::setShapeNameColor(%this, %color);
	}

	function player::setShapeNameDistance(%this, %dist)
	{
		%dist = 24;

		if(isObject(%client = %this.client) && isObject(%client.player))
		{
			if(%client.getWantedLevel())
				%dist *= %client.getWantedLevel();
		}

		parent::setShapeNameDistance(%this, %dist);
	}

	function Player::emote(%player, %emote)
	{
		if(%player.cityDamageOverride)
		{
			%player.cityDamageOverride = 0;
			return;
		}

		Parent::emote(%player, %emote);
	}

	function removeMoney(%col, %client, %arg1)
	{
		if(!%col.isPlanted())
		{
			CityRPGData.getData(%client.bl_id).valueMoney += %arg1;
			messageClient(%client, '', "Your money has been returned to you because you were unable to plant the brick!");
		}
	}

	// ============================================================
	// Misc Functions
	// ============================================================
	// Namespace Overrides
	function Armor::damage(%this, %obj, %src, %unk, %dmg, %type)
	{
		if(isObject(%obj.client.minigame) && %type == $DamageType::Vehicle)
		{
			if(%obj.client.minigame.vehicleRunOverDamage)
				parent::damage(%this, %obj, %src, %unk, %dmg, %type);
		}
		else
			parent::damage(%this, %obj, %src, %unk, %dmg, %type);
	}

	function WheeledVehicle::onActivate(%this, %obj, %client, %pos, %dir)
	{
		if(!%this.locked && getTrustLevel(%obj.client.brickGroup, %this.spawnBrick.getGroup()) > 0)
			parent::onActivate(%this, %obj, %client, %pos, %dir);
	}


	function WheeledVehicleData::onCollision(%this, %obj, %col, %pos, %vel)
	{
		if(%obj.locked && %col.getType() & $TypeMasks::PlayerObjectType && isObject(%col.client))
			commandToClient(%col.client, 'centerPrint', "\c6The vehicle is locked.", 3);
		else if(isObject(%obj.spawnBrick) && %obj.spawnBrick.getDatablock().getName() $= "CityRPGCrimeVehicleData" && isObject(%col.client) && !%col.client.getJobSO().usecrimecars)
			commandToClient(%col.client, 'centerPrint', "\c6This vehicle is a criminal vehicle.", 3);
		else if(isObject(%obj.spawnBrick) && %obj.spawnBrick.getDatablock().getName() $= "CityRPGPoliceVehicleData" && isObject(%col.client) && !%col.client.getJobSO().usepolicecars)
			commandToClient(%col.client, 'centerPrint', "\c6This vehicle is property of the Police Deparment.", 3);
		else
			parent::onCollision(%this, %obj, %col, %pos, %vel);
	}
	function itemData::onPickup(%this, %item, %obj)
	{
		parent::onPickup(%this, %item, %obj);

		if(isObject(%item.spawnBrick))
		{
			if(!%item.spawnBrick.getDatablock().CityRPGPermaspawn)
				%item.spawnBrick.setItem(0, ((isObject(getBrickGroupFromObject(%item.spawnBrick).client)) ? getBrickGroupFromObject(%item.spawnBrick).client : 0), true);
		}
	}

	function HammerImage::onHitObject(%this, %obj, %slot, %col, %pos, %normal)
	{
		if(%col.getClassName() $= "Player" && isObject(%col.client) && !%col.client.getWantedLevel())
			return;

		parent::onHitObject(%this, %obj, %slot, %col, %pos, %normal);
	}

	function KeyProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal)
	{
		parent::onCollision(%this, %obj, %col, %fade, %pos, %normal);

		if(%col.getDatablock().getClassName() $= "WheeledVehicleData" && mFloor(VectorLen(%col.getvelocity())) == 0)
		{
			if(getTrustLevel(%col.brickGroup, %obj.client.brickGroup) > 0)
			{
				%col.locked = !%col.locked;
				commandToClient(%obj.client, 'centerPrint', "\c6The vehicle is now \c3" @ (%col.locked ? "locked" : "unlocked") @ "\c6.", 3);
			}
			else
				commandToClient(%obj.client, 'centerPrint', "\c6The key does not fit.", 3);
		}
	}

	function MinigameSO::pickSpawnPoint(%mini, %client)
	{
		if(getWord(CityRPGData.getData(%client.bl_id).valueJailData, 1) > 0 && City_FindSpawn("jailSpawn"))
			%spawn = City_FindSpawn("jailSpawn");
		else
		{
			if(City_FindSpawn("personalSpawn", %client.bl_id))
				%spawn = City_FindSpawn("personalSpawn", %client.bl_id);
			else
			{
				if(City_FindSpawn("jobSpawn", CityRPGData.getData(%client.bl_id).valueJobID) && CityRPGData.getData(%client.bl_id).valueJobID != 1)
					%spawn = City_FindSpawn("jobSpawn", CityRPGData.getData(%client.bl_id).valueJobID);
				else
					%spawn = City_FindSpawn("jobSpawn", 1);
			}
		}

		if(%spawn)
			return %spawn;
		else
			parent::pickSpawnPoint(%mini, %client);
	}

	// Namespaceless Overrides
	function onServerDestroyed()
	{
		echo("Exporting CityRPG data...");

		// Prevents ticks from running post-mission end.
		if(!$Server::Dedicated && CityRPGData.scheduleTick)
			cancel(CityRPGData.scheduleTick);

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

		deleteVariables("$City::*");
		deleteVariables("$CityRPG::*");
		deleteVariables("$CityBrick_*");

		CityRPGData.delete();
		CityRPGMini.delete();
		JobSO.delete();
		CitySO.delete();
		CalendarSO.delete();
		ResourceSO.delete();

		return parent::onServerDestroyed();
	}

	// Always-in-Minigame Overrides
	function miniGameCanDamage(%obj1, %obj2)
	{
		return 1;
	}

	function miniGameCanUse(%obj1, %obj2)
	{
		return 1;
	}

	function getMiniGameFromObject(%obj)
	{
		return CityRPGMini;
	}

	// ============================================================
	// Chat Functions/Packages
	// ============================================================
	function serverCmdmessageSent(%client, %text)
	{
		if(%client.cityMenuOpen)
		{
			%client.cityMenuInput(%text);
			return;
		}

		if(isObject(%client.player) && isObject(%client.CityRPGTrigger) && isObject(%client.CityRPGTrigger.parent) && %client.CityRPGTrigger.parent.getDatablock().CityRPGBrickType == $CityBrick_Info)
		{
			// Legacy menu logging support
			%client.cityLog(%client.CityRPGTrigger.parent.getDatablock().getName() SPC %text);
			%client.CityRPGTrigger.parent.getDatablock().parseData(%client.CityRPGTrigger.parent, %client, "", %text);
			return;
		}
		else if(getWord(CityRPGData.getData(%client.bl_id).valueJailData, 1) > 0)
		{
			serverCmdteamMessageSent(%client, %text);
			return;
		}

		parent::serverCmdmessageSent(%client, %text);
	}

	function serverCmdteamMessageSent(%client, %text)
	{
		%text = StripMLControlChars(%text);

		if(%text !$= "" && %text !$= " ")
		{
			if(getWord(CityRPGData.getData(%client.bl_id).valueJailData, 1))
			{
				for(%i = 0; %i < ClientGroup.getCount();%i++)
				{
					%subClient = ClientGroup.getObject(%i);
					if(getWord(CityRPGData.getData(%subClient.bl_id).valueJailData, 1))
					{
						messageClient(%subClient, '', "\c3[<color:777777>Inmate\c3]" SPC %client.name @ "<color:777777>:" SPC %text);
					}
				}
				echo("(Convict Chat)" SPC %client.name @ ":" SPC %text);
			}
			else
			{
				for(%i = 0;%i < ClientGroup.getCount();%i++)
				{
					%subClient = ClientGroup.getObject(%i);
					if(CityRPGData.getData(%subClient.bl_id).valueJobID == CityRPGData.getData(%client.bl_id).valueJobID && !getWord(CityRPGData.getData(%subClient.bl_id).valueJailData, 1))
					{
						messageClient(%subClient, '', "\c3[<color:" @ %client.getJobSO().tmHexColor @ ">" @ %client.getJobSO().name @ "\c3]" SPC %client.name @ "<color:" @ %client.getJobSO().tmHexColor @ ">:" SPC %text);
					}
				}
			}
		}
	}

	function serverCmdcreateMiniGame(%client)
	{
		messageClient(%client, '', "I'm afraid I can't let you do that," SPC %client.name @ ".");
	}

	function serverCmdleaveMiniGame(%client)
	{
		messageClient(%client, '', "I'm afraid I can't let you do that," SPC %client.name @ ".");
	}

	function serverCmddropTool(%client, %toolID)
	{
		if(getWord(CityRPGData.getData(%client.bl_id).valueJailData, 1) > 0)
			messageClient(%client, '', "\c6You can't drop tools while in jail.");
		else
			parent::serverCmddropTool(%client, %toolID);
	}

	function serverCmdsuicide(%client)
	{
		if(getWord(CityRPGData.getData(%client.bl_id).valueJailData, 1) > 0)
			commandToClient(%client, '', "\c6You cannot suicide while in jail.", 3);
		else if(%client.getWantedLevel())
		{
			for(%a = 0; %a < ClientGroup.getCount(); %a++)
			{
				%subClient = ClientGroup.getObject(%a);
				if(isObject(%subClient.player) && isObject(%client.player) && %subClient != %client)
				{
					if(VectorDist(%subClient.player.getPosition(), %client.player.getPosition()) <= 30)
					{
						if(%subClient.player.currTool > -1)
						{
							if(%subClient.player.tool[%subClient.player.currTool].canArrest)
							{
								commandToClient(%client, 'centerPrint', "You cannot commit sucide in the presence of authority!", 3);
								return;
							}
						}
					}
				}
			}

			parent::serverCmdsuicide(%client);
		}
		else
		{
			%client.moneyOnSuicide = CityRPGData.getData(%client.bl_id).valueMoney;
			%client.lumberOnSuicide = CityRPGData.getData(%client.bl_id).valueResources;
			parent::serverCmdsuicide(%client);
		}
	}

	function serverCmdUpdateBodyColors(%client, %headColor)
	{
		// The only thing we want from this command is the facial color, which determines skin color in the clothing mod.
		%client.headColor = %headColor;
		%client.applyForcedBodyColors();
	}

	function serverCmdUpdateBodyParts(%client)
	{
		// There is no useful information that the game could derive from UpdateBodyParts. Simply returning.
		return;
	}

	function serverCmdForcePlant(%client)
	{
		if(!%client.isAdmin)
		{
			messageClient(%client, '', "\c6Force Plant is admin only in CityRPG. Ask an admin for help.");
			return;
		}

		Parent::serverCmdForcePlant(%client);
	}

	function WandImage::onHitObject(%this, %player, %slot, %hitObj, %hitPos, %hitNormal)
	{
		if(getWord(%hitPos,2)<10)
			return Parent::onHitObject(%this, %player, %slot, %hitObj, %hitPos, %hitNormal);
		if(%hitObj.getType() & $TypeMasks::PlayerObjectType)
			return;
		return Parent::onHitObject(%this, %player, %slot, %hitObj, %hitPos, %hitNormal);
	}
};
deactivatePackage(CityRPG_MainPackage);
activatepackage(CityRPG_MainPackage);
