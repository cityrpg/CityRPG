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
function CityMenu_RealEstate(%client, %input, %brick)
{
	// Note that %brick doubles as the menu's identifier.

	// If our brick ID is a lot, we've just come back from the lot menu.
	// We need to use the value assigned in CityMenu_Lot to identify the menu.
	if(%brick.getDataBlock().CityRPGBrickType == $CityBrick_Lot)
	{
		%brick = %client.cityMenuBack;

		// Close the last menu.
		%client.cityMenuClose(1);
		%client.cityMenuBack = "";
	}

	%client.cityMenuMessage("\c3" @ $Pref::Server::City::name @ "\c3 Real Estate Office");

	%lotCount = $City::RealEstate::TotalLots || 0;
	%lotCountUnclaimed = $City::RealEstate::UnclaimedLots || 0;
	%lotCountSale = $City::RealEstate::LotCountSale;

	if(%lotCountUnclaimed > 0)
		%message = "\c6" @ $Pref::Server::City::name @ "\c6 has \c3" @ %lotCount @ "\c6 lots, \c3" @ %lotCountUnclaimed @ "\c6 of which are unclaimed lots for sale.";
	else
		%message = "\c6" @ $Pref::Server::City::name @ "\c6 has \c3" @ %lotCount @ "\c6 total lots. There are no unclaimed lots for sale.";

	%menu = "Manage a lot" TAB "View pre-owned lots for sale";

	%functions = "CityMenu_RealEstate_ViewLotsOwned" TAB "CityMenu_RealEstate_ViewLotListings";

	if($City::RealEstate::LotCountSale > 0)
	{
		if($City::RealEstate::LotCountSale == 1)
			%message = %message SPC "\c6There is \c31\c6 pre-owned lot available for sale.";
		else
			%message = %message SPC "\c6There are \c3" @ $City::RealEstate::LotCountSale @ "\c6 pre-owned lots for sale.";
	}
	else
	{
		%message = %message SPC "There are no pre-owned lots for sale at this time.";
	}

	%client.cityMenuMessage(%message);
	%client.cityMenuOpen(%menu, %functions, %brick, "\c6Thanks, come again.");
}

// List for sale
function CityMenu_RealEstate_ViewLotsOwned(%client, %input, %brick)
{
	for(%i = 0; %i <= getWordCount($City::Cache::LotsOwnedBy[%client.bl_id])-1; %i++)
	{
		%lotBrick = getWord($City::Cache::LotsOwnedBy[%client.bl_id], %i);

		%option = %lotBrick.getCityLotName();

		if(%i == 0)
		{
			%menu = %option;
			%functions = CityMenu_Lot;
		}
		else
		{
			%menu = %menu TAB %option;
			%functions = %functions TAB CityMenu_Lot;
		}
	}

	if(getFieldCount(%menu) == 0)
	{
		%client.cityMenuMessage("\c0You do not own any lots to manage.");
		return;
	}
	else
	{
		%client.cityMenuClose(1);
	}

	%client.cityMenuOpen(%menu, %functions, %brick, "\c6Thanks, come again.");
	%client.cityMenuMessage("\c6Choose one of your lots to manage. Use the PG UP and PG DOWN keys to scroll.");
}

// View & Purchase
function CityMenu_RealEstate_ViewLotListings(%client, %input, %brick)
{
	for(%i = 0; %i <= getWordCount(CitySO.lotListings)-1; %i++)
	{
		%lotID = getWord(CitySO.lotListings, %i);
		%lotBrick = findLotBrickByID(%lotID);

		talk(%lotID SPC %lotBrick);

		// Hide lots that are in the registry, but don't have an existing brick.
		if(%lotBrick == 0)
			continue;

		if(%i == 0)
		{
			%menu = %lotBrick.getCityLotName();
			%functions = CityMenu_RealEstate_ViewLotDetail;
		}
		else
		{
			%menu = %menu TAB %lotBrick.getCityLotName();
			%functions = %functions TAB CityMenu_RealEstate_ViewLotDetail;
		}

		// Record the available options so we know which one to pick.
		%client.cityLotIndex[%i+1] = %lotID;
	}

	%client.cityMenuClose(1);
	%client.cityMenuOpen(%menu, %functions, %brick, "\c6Thanks, come again.");
	%client.cityMenuMessage("\c6Type the number for a listing to view more info. Use the PG UP and PG DOWN keys to scroll.");
}

function CityMenu_RealEstate_ViewLotDetail(%client, %input, %brick)
{
	%lotID = %client.cityLotIndex[%input];

	CityMenu_Placeholder(%client);
}


// ============================================================
// Trigger Data
// ============================================================
function CityRPGREBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus == true && !%client.cityMenuOpen)
	{
		CityMenu_RealEstate(%client, 0, %brick);
	}
}
