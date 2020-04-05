// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGLaborBrickData : brick2x4FData)
{
	category = "CityRPG";
	subCategory = "City Info Bricks";

	uiName = "Labor Brick";

	CityRPGBrickType = $CityBrick_Info;
	CityRPGBrickAdmin = true;

	triggerDatablock = CityRPGInputTriggerData;
	triggerSize = "2 4 1";
	trigger = 0;
};

// ============================================================
// Trigger Data
// ============================================================
function CityRPGLaborBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	%client.buyResources();
}
