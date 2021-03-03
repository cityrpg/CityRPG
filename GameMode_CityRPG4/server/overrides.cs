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
};

// No need to activate yet--this will be done in City_Init
