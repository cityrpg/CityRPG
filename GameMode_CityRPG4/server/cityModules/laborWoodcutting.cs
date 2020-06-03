// ============================================================
// Bricks
// ============================================================

datablock fxDTSBrickData(CityRPGTreeData : brickPineTreeData)
{
	category = "CityRPG";
	subCategory = "Resources";

	uiName = "Lumber Tree";

	CityRPGBrickType = $CityBrick_ResourceLumber;
	CityRPGBrickAdmin = true;
};

// ============================================================
// Brick Functions
// ============================================================
function fxDTSBrick::onChop(%this, %client)
{
	// WIP
}
