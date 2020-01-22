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
};

// No need to activate yet--this will be done in City_Init
