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
		//if(%obj.getGroup().bl_id != getNumKeyId())
		//{
			// Attempt to detect the type of bot. No attack bots, no rideables.
			%datablock = %obj.getDatablock();
			if(!isObject(%datablock.holeBot) || %datablock.holeBot.hMelee != 0 || %datablock.holeBot.rideable)
			{
				if(isObject(%obj.getGroup().client)) {
					%obj.getGroup().client.centerPrint("You cannot spawn this type of bot.", 3);
				}

				%obj.killBrick();
				return;
			}
		//}

		Parent::onHoleSpawnPlanted(%obj);
	}
};

// No need to activate yet--this will be done in City_Init
