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
$City::Menu::ATMBaseTxt = "Withdraw money.";

// We can call directly on the same prompt that the bank uses.
$City::Menu::ATMBaseFunc = "CityMenu_BankWithdrawPrompt";

function CityMenu_ATM(%client, %brick)
{
	%client.cityMenuMessage("\c6You have " @ $c_p @ "$" @ City.get(%client.bl_id, "bank") SPC "\c6in your account.");

	%client.cityLog("Enter ATM");

	%client.cityMenuOpen($City::Menu::ATMBaseTxt, $City::Menu::ATMBaseFunc, %brick, "\c6Thanks, come again.", 0, 0, $c_p @ "ATM");
}

// ============================================================
// Trigger Data
// ============================================================
function CityRPGATMBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus == true && !%client.cityMenuOpen)
	{
		%client.cityMenuMessage("\c6Welcome to the " @ $Pref::Server::City::name @ " Bank. Your account balance is " @ $c_p @ "$" @ City.get(%client.bl_id, "bank") @ "\c6. Current economy value: " @ $c_p @ %econColor @ $City::Economics::Condition @ "\c6%");

		CityMenu_ATM(%client, %brick);
	}
}
