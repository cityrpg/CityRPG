// ============================================================
// Bricks
// ============================================================
datablock fxDTSBrickData(CityRPGOreData)
{
	brickFile = $City::DataPath @ "bricks/4x Cube.blb";
	iconName = $City::DataPath @ "ui/BrickIcons/4x Cube";

	category = "CityRPG";
	subCategory = "Resources";

	uiName = "Ore";

	CityRPGBrickType = $CityBrick_ResourceOre;
	CityRPGBrickAdmin = true;
};

datablock fxDTSBrickData(CityRPGSmallOreData)
{
	brickFile = $City::DataPath @ "bricks/Small Ore.blb";
	iconName = "base/client/ui/brickIcons/2x2";

	category = "CityRPG";
	subCategory = "Resources";

	uiName = "Small Ore";

	CityRPGBrickType = $CityBrick_ResourceOre;
	CityRPGBrickAdmin = true;
};

// ============================================================
// Brick Functions
// ============================================================
function fxDTSBrick::onCityMine(%this, %client)
{
	%client.onCityMine(%this, "Tin");
}

// ============================================================
// Client Functions
// ============================================================
function gameConnection::onCityMine(%client, %brick, %resource)
{
	%progressBar = "";

	for(%i = 0; %i <= getSimTime()/515 % 4; %i++)
	{
		%progressBar = %progressBar @ "-";
	}

	%client.centerPrint("\c6" @ %progressBar @ " Mining " @ %progressBar @ "<br>\c3" @ %resource, 2);
}
