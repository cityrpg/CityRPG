// This package contains important overrides that take priority over other add-ons.
package CityRPG_Overrides
{
	function serverCmdPlantBrick(%client)
	{
		if($LoadingBricks_Client !$= "")
		{
			if(%client.isAdmin)
			{
				%client.centerPrint("\c6You cannot build while bricks are loading in CityRPG.<br>\c6If you believe this is in error, cancel the upload with /cancelSaveFileUpload.", 4);
			}
			else
			{
				%client.centerPrint("\c6You cannot build while bricks are loading in CityRPG.<br>\c6If you believe this is in error, ask an admin to use /cancelSaveFileUpload.", 4);
			}

			return;
		}

		Parent::serverCmdPlantBrick(%client);
	}

	function fxDTSBrick::onHoleSpawnPlanted(%obj)
	{
		if(%obj.getGroup().bl_id != getNumKeyId())
		{
			if(isObject(%obj.getGroup().client)) {
				%obj.getGroup().client.centerPrint("\c6Sorry, bot holes are currently host-only in CityRPG.", 3);
			}

			%obj.killBrick();
			return;
		}

		Parent::onHoleSpawnPlanted(%obj);
	}

	// Write our own version of confirmCenterprintMenu.
	// For compatibility, the original function will be used if any other add-ons use menus.
	function serverCmdPlantBrick(%cl)
	{
		if(%cl.cityMenuOpen)
		{
			%cl.confirmCityMenu();
			return;
		}

		return parent::serverCmdPlantBrick(%cl);
	}

	function GameConnection::confirmCityMenu(%client)
	{
		if (!%client.isInCenterprintMenu)
		{
			return;
		}

		%menu = %client.centerprintMenu;
		%option = %client.currOption;

		%client.exitCenterprintMenu();
		%func = %menu.menuFunction[%option];
		if (%menu.playSelectAudio)
		{
			playCenterprintMenuSound(%client, 'MsgAdminForce');
		}

		if (%func !$= "" && !isFunction(%func))
		{
			error("ERROR: confirmCityMenu: cannot find function " @ %func @ "!");
			return;
		}
		else
		{
			call(%func, %client, %option+1, %client.cityMenuID);
		}
	}
};

// No need to activate yet--this will be done in City_Init
