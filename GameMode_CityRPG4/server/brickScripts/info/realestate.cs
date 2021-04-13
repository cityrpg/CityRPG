// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGRealEstateBrickData : brick2x4FData)
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
// These are menus for the real estate brick.
// Most of this simply refers to the menus in cityModules/lotRegistryMenu.cs

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
	%client.cityMenuOpen(%menu, %functions, %brick, "\c6Thanks, come again.", 0, 1);
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

		// Record the available options so we know which one to pick.
		// This will always correspond to an item in the menu -- The menu options will ensure the client cannot pick an invalid value.
		%client.cityLotIndex[%i+1] = %lotBrick.getCityLotID();
	}

	%client.cityLotIndexCount = %i+1;

	if(getFieldCount(%menu) == 0)
	{
		%client.cityMenuMessage("\c6You don't own any lots yet! Look for lots marked as \"For sale\" to purchase one.");
		return;
	}
	else
	{
		%client.cityMenuClose(1);
	}

	%client.cityMenuOpen(%menu, %functions, %brick, "\c6Thanks, come again.", 0, 1);
	%client.cityMenuMessage("\c6Choose one of your lots to manage. Use the PG UP and PG DOWN keys to scroll.");
}

// View & Purchase
function CityMenu_RealEstate_ViewLotListings(%client, %input, %brick)
{
	%client.cityLotIndexClear();

	// This is a slightly more complex task than the "Manage a lot" menu.
	// We need to filter through the lot listings to find the ones that actually exist in the world.
	for(%i = 0; %i <= getWordCount(CitySO.lotListings)-1; %i++)
	{
		%lotID = getWord(CitySO.lotListings, %i);
		%lotBrick = findLotBrickByID(%lotID);

		// Hide lots that are in the registry, but don't have an existing brick.
		if(%lotBrick == 0)
			continue;

		// Record the available options -- See: CityMenu_RealEstate_ViewLotsOwned
		// Important: This depends on us knowing that this lot actually exists, as checked above.
		%client.cityLotIndexCount++;
		%client.cityLotIndex[%client.cityLotIndexCount] = %lotID;

		%lotStr = %lotBrick.getCityLotName() @ " - Cost: \c2$" @ %lotBrick.getCityLotPreownedPrice() @ "\c6 - Owner: \c3" @ %lotBrick.getGroup().name @ "\c6";

		if(%client.cityLotIndexCount == 1)
		{
			// First option

			%menu = %lotStr;
			%functions = CityMenu_RealEstate_ViewLotDetail;
		}
		else
		{
			%menu = %menu TAB %lotStr;
			%functions = %functions TAB CityMenu_RealEstate_ViewLotDetail;
		}
	}

	if(%client.cityLotIndexCount == 0)
	{
		%client.cityMenuMessage("\c6There are no lots available for sale at this time. Check back later!");
		return;
	}

	%client.cityMenuOpen(%menu, %functions, %brick, "\c6Thanks, come again.", 0, 1);
	%client.cityMenuMessage("\c6Type the number to view more info. Use the PG UP and PG DOWN keys to scroll.");
}

function CityMenu_RealEstate_ViewLotDetail(%client, %input, %brick)
{
	%lotID = %client.cityLotIndex[%input];
	%lotBrick = findLotBrickByID(%lotID);

	if(%lotBrick.getCityLotOwnerID() == %client.bl_id) {
		%client.cityMenuMessage("\c3This is your lot.");
	}

	CityMenu_Lot(%client, %input);
}


// ============================================================
// Trigger Data
// ============================================================
function CityRPGRealEstateBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus == true && !%client.cityMenuOpen)
	{
		CityMenu_RealEstate(%client, 0, %brick);
	}
}
