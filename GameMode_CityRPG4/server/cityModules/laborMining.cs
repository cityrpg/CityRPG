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
function fxDTSBrick::onMine(%this, %client)
{
	// WIP
}
