// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGATMBrickData : brick2x4FData)
{
	category = "CityRPG";
	subCategory = "Info Bricks";

	uiName = "ATM Brick";

	CityRPGBrickType = $CityBrick_Info;
	CityRPGBrickAdmin = false;

	triggerDatablock = CityRPGInputTriggerData;
	triggerSize = "2 4 1";
	trigger = 0;
};

// ============================================================
// Menu
// ============================================================
function CityMenu_ATM(%client, %brick)
{
	%client.cityMenuMessage("\c6You have \c3$" @ CityRPGData.getData(%client.bl_id).valueBank SPC "\c6in your account.");

	%client.cityLog("Enter ATM");

	%menu = "Withdraw money";
	// We can call directly on the same prompt that the bank uses.
	%functions = "CityMenu_BankWithdrawPrompt";

	%client.cityMenuOpen(%menu, %functions, %brick, "\c6Thanks, come again.", 0, "\c3ATM");
}

// ============================================================
// Trigger Data
// ============================================================
function CityRPGATMBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus == true && !%client.cityMenuOpen)
	{
		if(%client.getWantedLevel())
		{
			%client.cityMenuMessage("\c6The service refuses to serve you.");
			return;
		}

		%client.cityMenuMessage("\c6Welcome to the " @ $Pref::Server::City::name @ " Bank. Your account balance is \c3$" @ CityRPGData.getData(%client.bl_id).valueBank @ "\c6. Current economy value: \c3" @ %econColor @ $City::Economics::Condition @ "\c6%");

		CityMenu_ATM(%client, %brick);
	}
}
