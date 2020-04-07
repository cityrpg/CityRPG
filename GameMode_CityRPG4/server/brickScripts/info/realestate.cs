// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGREBrickData : brick2x4FData)
{
	category = "CityRPG";
	subCategory = "City Info Bricks";

	uiName = "Real Estate Brick";

	CityRPGBrickType = $CityBrick_Info;
	CityRPGBrickAdmin = true;
	CityRPGBrickCost = 100;

	triggerDatablock = CityRPGInputTriggerData;
	triggerSize = "2 4 1";
	trigger = 0;
};

// ============================================================
// Menu
// ============================================================
function CityMenu_RealEstate(%client, %brick)
{
	%client.cityMenuMessage("\c3" @ $Pref::Server::City::name @ "\c3 Real Estate Office");

	%lotCount = $City::RealEstate::TotalLots || 0;
	%lotCountUnclaimed = $City::RealEstate::UnclaimedLots || 0;
	%lotCountSale = $City::RealEstate::LotCountSale;

	if(%lotCountUnclaimed > 0)
		%message = "\c6" @ $Pref::Server::City::name @ "\c6 has \c3" @ %lotCount @ "\c6 lots, \c3" @ %lotCountUnclaimed @ "\c6 of which are unclaimed lots for sale.";
	else
		%message = "\c6" @ $Pref::Server::City::name @ "\c6 has \c3" @ %lotCount @ "\c6 total lots. There are no unclaimed lots available for sale.";

	if($City::RealEstate::LotCountSale > 0)
	{
		if($City::RealEstate::LotCountSale == 1)
			%message = %message SPC "\c6There is \c31\c6 pre-owned lot available for sale.";
		else
			%message = %message SPC "\c6There are \c3" @ $City::RealEstate::LotCountSale @ "\c6 pre-owned lots available for sale.";

		%menu = "List a lot for sale"
				TAB "View pre-owned lots for sale"
				TAB "Purchase a pre-owned lot";

		%functions = "CityMenu_Placeholder"
						 TAB "CityMenu_Placeholder"
						 TAB "CityMenu_Placeholder";
	}
	else
	{
		%message = %message SPC "There are no pre-owned lots available for sale at this time.";
		%menu = "List a lot for sale";
		%functions = "CityMenu_Placeholder";
	}

	%client.cityMenuMessage(%message);
	%client.cityMenuOpen(%menu, %functions, %brick, "\c6Thanks, come again.");
}

// ============================================================
// Trigger Data
// ============================================================
function CityRPGREBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus == true && !%client.cityMenuOpen)
	{
		CityMenu_RealEstate(%client, %brick);
	}
}
