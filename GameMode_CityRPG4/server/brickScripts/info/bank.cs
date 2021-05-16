// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGBankBrickData : brick2x4FData)
{
	category = "CityRPG";
	subCategory = "City Info Bricks";

	uiName = "Bank Brick";

	CityRPGBrickType = $CityBrick_Info;
	CityRPGBrickAdmin = true;

	triggerDatablock = CityRPGInputTriggerData;
	triggerSize = "2 4 1";
	trigger = 0;
};

// ============================================================
// Menu
// ============================================================
// Initial menu
function CityMenu_Bank(%client, %brick)
{
	%menu =	"Withdraw money."
			TAB "Deposit money."
			TAB "Deposit all money."
			TAB "Donate to the economy.";

	%functions = 	"CityMenu_BankWithdrawPrompt"
						TAB "CityMenu_BankDepositPrompt"
						TAB "CityMenu_BankDepositAll"
						TAB "CityMenu_BankDonatePrompt";

	%client.cityLog("Enter bank");

	%client.cityMenuOpen(%menu, %functions, %brick, "\c6Thanks, come again.", 0, 0, "Bank");
}

// Withdraw money.
function CityMenu_BankWithdrawPrompt(%client)
{
	%client.cityLog("Bank withdraw prompt");

	%client.cityMenuMessage("\c6Please enter the amount of money you wish to withdraw.");
	%client.cityMenuFunction = CityMenu_BankWithdraw;
}

function CityMenu_BankWithdraw(%client, %input)
{
	if(mFloor(%input) < 1)
	{
		%client.cityMenuMessage("\c6Please enter a valid amount of money to withdraw.");

		return;
	}

	if(City.get(%client.bl_id, "bank") - mFloor(%input) < 0)
	{
		if(City.get(%client.bl_id, "bank") < 1)
		{
			%client.cityMenuMessage("\c6You don't have that much money in the bank to withdraw.");

			%client.cityMenuClose();

			return;
		}

		%input = City.get(%client.bl_id, "bank");
	}

	%client.cityLog("Bank withdraw $" @ mFloor(%input));
	%client.cityMenuMessage("\c6You have withdrawn \c3$" @ mFloor(%input) @ "\c6.");

	%client.cityMenuClose();

	City.subtract(%client.bl_id, "bank", mFloor(%input));
	City.add(%client.bl_id, "money", mFloor(%input));

	%client.SetInfo();

}

// Deposit money.
function CityMenu_BankDepositPrompt(%client)
{
	%client.cityLog("Bank deposit prompt");

	%client.cityMenuMessage("\c6Please enter the amount of money you wish to deposit.");
	%client.cityMenuFunction = CityMenu_BankDeposit;
}

function CityMenu_BankDeposit(%client, %input)
{
	if(mFloor(%input) < 1)
	{
		%client.cityMenuMessage("\c6Please enter a valid amount of money to deposit.");

		return;
	}

	if(City.get(%client.bl_id, "money") - mFloor(%input) < 0)
	{
		if(City.get(%client.bl_id, "money") < 1)
		{
			%client.cityMenuMessage("\c6You don't have that much money to deposit.");

			%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, (isObject(%client.player) ? %client.player : 0));

			return;
		}

		%input = City.get(%client.bl_id, "money");
	}

	%client.cityLog("Bank deposit $" @ mFloor(%input));

	%client.cityMenuMessage("\c6You have deposited \c3$" @ mFloor(%input) @ "\c6!");

	City.add(%client.bl_id, "bank", mFloor(%input));
	City.subtract(%client.bl_id, "money", mFloor(%input));

	%client.cityMenuClose();
	%client.SetInfo();
}

// Deposit all money.
function CityMenu_BankDepositAll(%client)
{
	%client.cityLog("Bank deposit all");
	CityMenu_BankDeposit(%client, City.get(%client.bl_id, "money"));
}

// Donate to the economy.
function CityMenu_BankDonatePrompt(%client)
{
	%client.cityLog("Bank donate prompt");
	%client.cityMenuMessage("\c6Enter the amount you would like to donate:");
	%client.cityMenuFunction = CityMenu_BankDonate;
}

function CityMenu_BankDonate(%client, %input)
{
	serverCmddonate(%client, %input);
	return;
}

// ============================================================
// Trigger Data
// ============================================================
function CityRPGBankBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus == true && !%client.cityMenuOpen)
	{
		if(%client.getWantedLevel())
		{
			%client.cityMenuMessage("\c6The service refuses to serve you.");
			return;
		}

		%client.cityMenuMessage("\c6Welcome to the " @ $Pref::Server::City::name @ " Bank.<br>Your account balance is \c3$" @ City.get(%client.bl_id, "bank") @ "\c6. Current economy value: \c3" @ %econColor @ $City::Economics::Condition @ "\c6%");

		CityMenu_Bank(%client, %brick);
	}
}
