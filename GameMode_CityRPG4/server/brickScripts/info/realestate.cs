// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGREBrickData : brick2x4FData)
{
	category = "CityRPG";
	subCategory = "Info Bricks";

	uiName = "Real Estate Brick";

	CityRPGBrickType = $CityBrick_Info;
	CityRPGBrickAdmin = true;
	CityRPGBrickCost = 100;

	triggerDatablock = CityRPGInputTriggerData;
	triggerSize = "2 4 1";
	trigger = 0;
};

// ============================================================
// Trigger Data
// ============================================================
function CityRPGREBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus !$= "")
	{
		if(%triggerStatus == true)
		{
			messageClient(%client, '', "\c3" @ $Pref::Server::City::name @ " Real Estate Office");
			messageClient(%client, '', "\c6WIP - Real Estate is currently unavailable.");

			%client.stage = 0;
		}

		if(%triggerStatus == false)
		{
			messageClient(%client, '', "\c6Thanks, come again.");

			%client.stage = "";
		}

		return;
	}

	return;
}

//package CityRPG_RealEsate
//{
//	function fxDtsBrick::setName(%brick, %name)
//	{
//		parent::setName(%brick, %name);
//	}
//};
//deactivatePackage(CityRPG_RealEsate);
//activatePackage(CityRPG_RealEsate);
