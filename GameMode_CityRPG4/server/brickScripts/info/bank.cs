// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGBankBrickData : brick2x4FData)
{
	category = "CityRPG";
	subCategory = "Info Bricks";

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

	%client.cityMenuOpen(%menu, %functions, %brick, "\c6Thanks, come again.");
}

// Withdraw money.
function CityMenu_BankWithdrawPrompt(%client)
{
	%client.cityLog("Bank withdraw prompt");

	messageClient(%client, '', "\c6Please enter the amount of money you wish to withdraw.");
	%client.cityMenuFunction = CityMenu_BankWithdraw;
}

function CityMenu_BankWithdraw(%client, %input)
{
	if(mFloor(%input) < 1)
	{
		messageClient(%client, '', "\c6Please enter a valid amount of money to withdraw.");

		return;
	}

	if(CityRPGData.getData(%client.bl_id).valueBank - mFloor(%input) < 0)
	{
		if(CityRPGData.getData(%client.bl_id).valueBank < 1)
		{
			messageClient(%client, '', "\c6You don't have that much money in the bank to withdraw.");

			%client.cityMenuClose();

			return;
		}

		%input = CityRPGData.getData(%client.bl_id).valueBank;
	}

	%client.cityLog("Bank withdraw $" @ mFloor(%input));
	messageClient(%client, '', "\c6You have withdrawn \c3$" @ mFloor(%input) @ "\c6.");

	%client.cityMenuClose();

	CityRPGData.getData(%client.bl_id).valueBank -= mFloor(%input);
	CityRPGData.getData(%client.bl_id).valueMoney += mFloor(%input);

	%client.SetInfo();

}

// Deposit money.
function CityMenu_BankDepositPrompt(%client)
{
	%client.cityLog("Bank deposit prompt");

	messageClient(%client, '', "\c6Please enter the amount of money you wish to deposit.");
	%client.cityMenuFunction = CityMenu_BankDeposit;
}

function CityMenu_BankDeposit(%client, %input)
{
	if(mFloor(%input) < 1)
	{
		messageClient(%client, '', "\c6Please enter a valid amount of money to deposit.");

		return;
	}

	if(CityRPGData.getData(%client.bl_id).valueMoney - mFloor(%input) < 0)
	{
		if(CityRPGData.getData(%client.bl_id).valueMoney < 1)
		{
			messageClient(%client, '', "\c6You don't have that much money to deposit.");

			%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, (isObject(%client.player) ? %client.player : 0));

			return;
		}

		%input = CityRPGData.getData(%client.bl_id).valueMoney;
	}

	%client.cityLog("Bank deposit $" @ mFloor(%input));

	messageClient(%client, '', "\c6You have deposited \c3$" @ mFloor(%input) @ "\c6!");

	CityRPGData.getData(%client.bl_id).valueBank += mFloor(%input);
	CityRPGData.getData(%client.bl_id).valueMoney -= mFloor(%input);

	%client.cityMenuClose();
	%client.SetInfo();
}

// Deposit all money.
function CityMenu_BankDepositAll(%client)
{
	%client.cityLog("Bank deposit all");
	CityMenu_BankDeposit(%client, CityRPGData.getData(%client.bl_id).valueMoney);
}

// Donate to the economy.
function CityMenu_BankDonatePrompt(%client)
{
	%client.cityLog("Bank donate prompt");
	messageClient(%client, '', "\c6Enter the amount you would like to donate:");
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
			messageClient(%client, '', "\c6The service refuses to serve you.");
			return;
		}

		messageClient(%client, '', "\c6Welcome to the " @ $Pref::Server::City::name @ " Bank. Your account balance is \c3$" @ CityRPGData.getData(%client.bl_id).valueBank @ "\c6. Current economy value: \c3" @ %econColor @ $Economics::Condition @ "\c6%");

		CityMenu_Bank(%client, %brick);
	}
}
