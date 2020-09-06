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
		%message = "\c6" @ $Pref::Server::City::name @ "\c6 has \c3" @ %lotCount @ "\c6 total lots. There are no unclaimed lots for sale.";

	if($City::RealEstate::LotCountSale > 0)
	{
		if($City::RealEstate::LotCountSale == 1)
			%message = %message SPC "\c6There is \c31\c6 pre-owned lot available for sale.";
		else
			%message = %message SPC "\c6There are \c3" @ $City::RealEstate::LotCountSale @ "\c6 pre-owned lots for sale.";

		%menu = "List a lot for sale"
				TAB "View pre-owned lots for sale";

		%functions = "CityMenu_RealEstate_ListForSalePrompt"
						 TAB "CityMenu_RealEstate_ViewLots";
	}
	else
	{
		%message = %message SPC "There are no pre-owned lots for sale at this time.";
		%menu = "List a lot for sale";
		%functions = "CityMenu_RealEstate_ListForSalePrompt";
	}

	%client.cityMenuMessage(%message);
	%client.cityMenuOpen(%menu, %functions, %brick, "\c6Thanks, come again.");
}

// List for sale
function CityMenu_RealEstate_ListForSalePrompt(%client, %input, %brick)
{
	%client.cityMenuClose(1);

	for(%i = 0; %i <= getWordCount($City::Cache::LotsOwnedBy[%client.bl_id])-1; %i++)
	{
		%lotBrick = getWord($City::Cache::LotsOwnedBy[%client.bl_id], %i);

		%option = %lotBrick.getCityLotName();

		if(%i == 0)
		{
			%menu = %option;
			%functions = CityMenu_RealEstate_ListForSalePricePrompt;
		}
		else
		{
			%menu = %menu TAB %option;
			%functions = %functions TAB CityMenu_RealEstate_ListForSalePricePrompt;
		}
	}

	if(getFieldCount(%menu) == 0)
	{
		%client.cityMenuMessage("\c0You do not own any lots that you can list for sale!");

		%client.cityMenuClose(1);
		CityMenu_RealEstate(%client, %brick);
		return;
	}

	%client.cityMenuOpen(%menu, %functions, %brick, "\c6Thanks, come again.");
	%client.cityMenuMessage("\c6Choose one of your lots to list for sale. Use the PG UP and PG DOWN keys to scroll.");
}

function CityMenu_RealEstate_ListForSalePricePrompt(%client, %input)
{
	%i = atof(%input)-1;
	%lotBrick = getWord($City::Cache::LotsOwnedBy[%client.bl_id], %i);
	%client.cityLotSelected = %lotBrick;

	%client.cityMenuMessage("\c6You have chosen the lot: \c3" @ %lotBrick.getCityLotName());
	%client.cityMenuMessage("\c6How much money would you like to sell this lot for? Enter a number.");

	%client.cityMenuFunction = CityMenu_RealEstate_ListForSaleConfirmPrompt;
}

function CityMenu_RealEstate_ListForSaleConfirmPrompt(%client, %input)
{
	%price = atof(%input);
	%lotBrick = %client.cityLotSelected;

	if(%price < 0)
		%price = 0;

	%client.cityMenuMessage("\c6You are listing the lot \c3" @ %lotBrick.getCityLotName() @ "\c6 on sale for \c3$" @ strFormatNumber(%price));
	%client.cityMenuMessage("\c0Warning!\c6 Once a player purchases this lot, they will become the permanent owner of your lot. Are you sure?");

	%client.cityLotPrice = %price;

	if(%price == 0)
		%client.cityMenuMessage("\c0You are about to list this lot for free. Are you sure?");

	%client.cityMenuMessage("\c6Type \c31\c6 to confirm, or \c32\c6 to cancel.");

	%client.cityMenuFunction = CityMenu_RealEstate_ListForSale;
}

function CityMenu_RealEstate_ListForSale(%client, %input)
{
	%lotBrick = %client.cityLotSelected;
	%lotID = %lotBrick.getCityLotID();

	if(%input !$= "1")
	{
		%client.cityMenuMessage("\c0Lot listing cancelled.");
		%client.cityMenuClose();
		return;
	}

	// Security check
	if(%lotBrick.getCityLotOwnerID() != %client.bl_id)
	{
		%client.cityLog("Lot " @ %lotBrick.getCityLotID() @ " sale listing fell through", 0, 1);

		// Security check falls through
		%client.cityMenuMessage("\c0Sorry, you are no-longer able to list that lot for sale at this time.");
		%client.cityMenuClose();
		return;
	}

	$City::RealEstate::LotCountSale++;

	// Append the lot to the fields under CitySO.lotListings.
	CitySO.lotListings = CitySO.lotListings $= ""? CitySO.lotListings = %lotID : CitySO.lotListings = CitySO.lotListings SPC %lotID;
	%lotBrick.setCityLotPreownedPrice(%client.cityLotPrice);

	%client.cityMenuMessage("\c6You have listed your lot for sale.");
	%client.cityMenuClose();
}

// View & Purchase
function CityMenu_RealEstate_ViewLots(%client, %input, %brick)
{
	for(%i = 0; %i <= getWordCount(CitySO.lotListings)-1; %i++)
	{
		%lotID = getWord(CitySO.lotListings, %i);
		%lotBrick = findLotBrickByID(%lotID);

		// Hide lots that are in the registry, but don't have an existing brick.
		if(%lotBrick == -1)
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
		CityMenu_RealEstate(%client, %brick);
	}
}
