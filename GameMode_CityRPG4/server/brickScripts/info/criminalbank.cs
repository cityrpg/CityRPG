// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGCriminalBankBrickData : brick2x4FData)
{
	category = "CityRPG";
	subCategory = "City Info Bricks";

	uiName = "Criminal Bank Brick";

	CityRPGBrickType = $CityBrick_Info;
	CityRPGBrickAdmin = true;

	triggerDatablock = CityRPGInputTriggerData;
	triggerSize = "2 4 1";
	trigger = 0;
};

// ============================================================
// Trigger Data
// ============================================================
function CityRPGCriminalBankBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus == true && !%client.cityMenuOpen)
	{
		%client.cityMenuMessage("\c6Welcome to the " @ $Pref::Server::City::name @ " Underground Bank. Your account balance is \c3$" @ CityRPGData.getData(%client.bl_id).valueBank @ "\c6. Current economy value: \c3" @ %econColor @ $City::Economics::Condition @ "\c6%");

		CityMenu_Bank(%client, %brick);
	}
}
