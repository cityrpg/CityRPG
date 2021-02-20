// ============================================================
// CityRPG 4 Lot Registry Menu Functions
// ============================================================
function GameConnection::cityLotIndexClear()
{
	for(%i = 0; %i <= %client.cityLotIndexCount; %i++)
	{
		%client.cityLotIndex[%i] = 0;
	}
}

function CityMenu_Lot(%client, %input)
{
	if(%client.cityMenuBack !$= "")
	{
		// "Go back" support for sub-menus
		%brick = %client.cityMenuBack;
		%client.cityMenuBack = "";

		%client.cityMenuClose(1);
	}
	else if(%input !$= "")
	{
		// If not going back and there's input, we're picking a lot from one of the real estate menus. Match it accordingly.
		%lotID = %client.cityLotIndex[%input];
		%brick = findLotBrickByID(%lotID);

		// Indicate that we're a sub-menu so we can display "Back" instead of "Close" later.
		// cityMenuBack identifies the real estate office by its brick.
		%isSubMenu = 1;
		%client.cityMenuBack = %client.cityMenuID;

		%client.cityLotIndexClear();
		%client.cityMenuClose(1);
	}
	else
	{
		// No input, we're running via /lot.
		if(%client.CityRPGLotBrick $= "")
		{
			%client.cityMenuMessage("\c6You are currently not on a lot.");
			return;
		}

		%brick = %client.CityRPGLotBrick;
	}

	// ## Initial display ## //
	%price = %brick.dataBlock.initialPrice;

	if(%brick.getCityLotID() == -1)
	{
		error("Attempting to access a blank lot on brick '" @ %brick @ "'! Re-initializing it...");

		%brick.initNewCityLot();
	}

	if(!%notitle)
	{
		%client.cityMenuMessage("\c3" @ %brick.getCityLotName() @ "\c6 - " @ %brick.getDataBlock().uiName);
	}

	// ## Options for all lots ## //
	%menu = "View lot rules.";
			//TAB "View warning log."

	%functions =	"CityMenu_LotRules";
						//TAB "CityMenu_Placeholder"

	// ## Options for unclaimed lots ## //
	if(%brick.getCityLotOwnerID() == -1)
	{
		%client.cityMenuMessage("\c6This lot is for sale! It can be purchased for \c3$" @ %price @ "\c6.");

		// Place these options first.
		%menu = "Purchase this lot. " TAB %menu;
		%functions = "CityMenu_LotPurchasePrompt" TAB %functions;
	}

	// ## Options for lot owners ## //
	if(%brick.getCityLotOwnerID() == %client.bl_id)
	{
		%menu = %menu
				TAB "Rename lot.";

		%functions = %functions
				TAB "CityMenu_LotSetNamePrompt";

		if(%brick.getCityLotPreownedPrice() == -1)
		{
			%menu = %menu TAB "List this lot for sale.";
			%functions = %functions TAB "CityMenu_Lot_ListForSalePrompt";
		}
		else
		{
			%menu = %menu TAB "Take this lot off sale.";
			%functions = %functions TAB "CityMenu_Lot_RemoveFromSale";
		}
	}

	// ## Options for admins ## //
	if(%client.isAdmin)
	{
		%menu = %menu TAB "\c4Open admin menu.";
		%functions = %functions TAB "CityMenu_LotAdmin";
	}

	// ## Finalization ## //
	if(%isSubMenu)
	{
		%menu = %menu TAB "Go back.";
		%functions = %functions TAB "CityMenu_RealEstate";
	}
	else
	{
		%menu = %menu TAB "Close menu.";
		%functions = %functions TAB "CityMenu_Close";
	}

	// Use the lot brick as the menu ID
	%client.cityMenuOpen(%menu, %functions, %brick, "\c3Lot management menu closed.");
}

// ## Functions for all lots ## //
function CityMenu_LotRules(%client)
{
	%client.cityMenuMessage("\c3Code enforcement requires following restrictions on this lot:");

	%lotRules = $Pref::Server::City::LotRules;
	%client.cityMenuMessage("\c6" @ %lotRules);
}

// ## Functions for unclaimed lots ## //
function CityMenu_LotPurchasePrompt(%client)
{
	%lot = %client.cityMenuID;

	%client.cityLog("Lot " @ %lot.getCityLotID() @ " purchase prompt");

	if(CityRPGData.getData(%client.bl_id).valueMoney >= %lot.dataBlock.initialPrice)
	{
		%client.cityMenuMessage("\c6You are purchasing this lot for \c3$" @ %lot.dataBlock.initialPrice @ "\c6. Make sure you have read the lot rules. Lot sales are final!");
		%client.cityMenuMessage("\c6Type \c31\c6 to confirm, or leave the lot to cancel.");

		%client.cityMenuFunction = CityLots_PurchaseLot;
		%client.cityMenuID = %lot;
	}
	else
	{
		%client.cityMenuMessage("\c6You need \c3$" @ %lot.dataBlock.initialPrice @ "\c6 on hand to purchase this lot.");
		%client.cityMenuClose();
	}
}

function CityLots_PurchaseLot(%client, %input, %lot)
{
	if(%lot $= "")
	{
		%lot = %client.cityMenuID;
	}

	if(%input !$= "1")
	{
		%client.cityMenuMessage("\c0Lot purchase cancelled.");
		%client.cityMenuClose();
	}
	else if(%lot.getCityLotOwnerID() != -1 || CityRPGData.getData(%client.bl_id).valueMoney < %lot.dataBlock.initialPrice)
	{
		%client.cityLog("Lot " @ %lot.getCityLotID() @ " purchase fell through", 0, 1);

		// Security check falls through
		%client.cityMenuMessage("\c0Sorry, you are no-longer able to purchase this lot at this time.");
		%client.cityMenuClose();
	}
	else if(CityRPGData.getData(%client.bl_id).valueMoney >= %lot.dataBlock.initialPrice)
	{
		%client.cityLog("Lot " @ %lot.getCityLotID() @ " purchase success");

		CityRPGData.getData(%client.bl_id).valueMoney -= %lot.dataBlock.initialPrice;
		%client.cityMenuMessage("\c6You have purchased this lot for \c3$" @ %lot.dataBlock.initialPrice @ "\c6!");

		%client.setInfo();

		CityLots_TransferLot(%client.cityMenuID, %client.bl_id); // The menu ID is the lot brick ID
		%client.cityMenuID.setCityLotTransferDate(getDateTime());

		// Open the menu for the new lot
		%client.cityMenuClose(1);
		CityMenu_Lot(%client);
	}
}

// ## Functions for lot owners ## //
function CityMenu_LotSetNamePrompt(%client)
{
	%client.cityLog("Lot " @ %client.cityMenuID.getCityLotID() @ " rename prompt");

	%client.cityMenuMessage("\c6Enter a new name for your lot.");
	%client.cityMenuFunction = CityMenu_LotSetName;
}

function CityMenu_LotSetName(%client, %input)
{
	%brick = %client.cityMenuID;

	%client.cityLog("Lot " @ %brick.getCityLotID() @ " rename '" @ %input @ "'");

	if(%brick.getCityLotOwnerID() != %client.bl_id)
	{
		return;
	}

	if(strlen(%input) > 40)
	{
		%client.cityMenuMessage("\c6Sorry, that name exceeds the length limit. Please try again.");
		return;
	}

	%name = StripMLControlChars(%input);

	%brick.setCityLotName(%name);
	%client.cityMenuMessage("\c6Lot name changed to \c3" @ %brick.getCityLotName() @ "\c6.");

	%client.cityMenuClose();
}

function CityMenu_Lot_RemoveFromSale(%client)
{
	%lotBrick = %client.cityMenuID;
	%lotBrick.setCityLotPreownedPrice(-1);

	CitySO.lotListings = removeWord(CitySO.lotListings, getWord(CitySO.lotListings, %lotBrick.getCityLotID()));

	%client.cityMenuMessage("\c6You have taken this lot off sale.");
	%client.cityMenuClose();
}

// ### Listing for sale ### //
function CityMenu_Lot_ListForSalePrompt(%client, %input)
{
	%lotBrick = %client.cityMenuID;

	%client.cityMenuMessage("\c6Listing this lot for sale will allow someone to buy it for the price of your choosing.");
	%client.cityMenuMessage("\c6How much money would you like to sell this lot for? Enter a number, or leave to cancel.");

	%client.cityMenuFunction = CityMenu_Lot_ListForSaleConfirmPrompt;
}

function CityMenu_Lot_ListForSaleConfirmPrompt(%client, %input)
{
	%price = atof(%input);
	%lotBrick = %client.cityMenuID;

	if(%price < 0)
		%price = 0;

	%client.cityMenuMessage("\c6You are listing the lot \c3" @ %lotBrick.getCityLotName() @ "\c6 on sale for \c3$" @ strFormatNumber(%price));
	%client.cityMenuMessage("\c0Warning!\c6 Once someone purchases this lot, they will become the permanent owner of your lot. Are you sure?");

	%client.cityLotPrice = %price;

	if(%price == 0)
		%client.cityMenuMessage("\c0You are about to list this lot for free. Are you sure?");

	%client.cityMenuMessage("\c6Type \c31\c6 to confirm, or \c32\c6 to cancel.");

	%client.cityMenuFunction = CityMenu_Lot_ListForSale;
}

function CityMenu_Lot_ListForSale(%client, %input)
{
	%lotBrick = %client.cityMenuID;
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

	// Append the lot to the fields under CitySO.lotListings.
	CitySO.lotListings = CitySO.lotListings = CitySO.lotListings @ %lotID @ " ";
	%lotBrick.setCityLotPreownedPrice(%client.cityLotPrice);

	%client.cityMenuMessage("\c6You have listed your lot for sale.");
	%client.cityMenuClose();
}

// ## Functions for admins ## //
function CityMenu_LotAdmin(%client)
{
	%brick = %client.CityMenuID;
	%client.cityMenuClose(true);
	%ownerID = %brick.getCityLotOwnerID();

	%client.cityMenuBack = %brick;

	%client.cityMenuMessage("\c3Lot Admin\c6 for: \c3" @ %brick.getCityLotName() @ "\c6 - Lot ID: \c3" @ %brick.getCityLotID() @ "\c6 - Brick ID: \c3" @ %brick.getID() @ "\c6 - Lot purchase date: \c3" @ %brick.getCityLotTransferDate());

	if(%ownerID != -1)
	{
		%owner = CityRPGData.getData(%ownerID);
		%client.cityMenuMessage("\c6Owner: \c3" @ %owner.valueName @ "\c6 (ID \c3" @ %brick.getCityLotOwnerID() @ "\c6)");
	}
	else
	{
		%client.cityMenuMessage("\c6Lot is owned by the city.");
	}

	%menu = "Force rename."
			TAB "Transfer lot to the city."
			TAB "Transfer lot to a player."
			TAB "Go back.";

	%functions =	"CityMenu_LotAdmin_SetNamePrompt"
						TAB "CityMenu_LotAdmin_TransferCity"
						TAB "CityMenu_LotAdmin_TransferPlayerPrompt"
						TAB "CityMenu_Lot";

	%client.cityMenuOpen(%menu, %functions, %brick, "\c3Lot menu closed.");
}

function CityMenu_LotAdmin_SetNamePrompt(%client)
{
	%client.cityLog("Lot MOD " @ %client.cityMenuID.getCityLotID() @ " rename prompt");
	%client.cityMenuMessage("\c6Enter a new name for the lot \c3" @ %client.cityMenuID.getCityLotName() @ "\c6. ML tags are allowed.");
	%client.cityMenuFunction = CityMenu_LotAdmin_SetName;
}

function CityMenu_LotAdmin_SetName(%client, %input)
{
	%client.cityLog("Lot MOD " @ %client.cityMenuID.getCityLotID() @ " rename '" @ %input @ "'");

	if(strlen(%input) > 40)
	{
		%client.cityMenuMessage("\c6Sorry, that name exceeds the length limit. Please try again.");
		return;
	}

	%client.cityMenuID.setCityLotName(%input);
	%client.cityMenuMessage("\c6Lot name changed to \c3" @ %client.cityMenuID.getCityLotName() @ "\c6.");

	%client.cityMenuClose();
}

function CityMenu_LotAdmin_TransferCity(%client)
{
	%hostID = getNumKeyID();

	%brick = %client.cityMenuID;

	%client.cityLog("Lot MOD " @ %brick.getCityLotID() @ " transfer city");

	CityLots_TransferLot(%brick, %hostID);
	%brick.setCityLotTransferDate(getDateTime());

	%brick.setCityLotName("Unclaimed Lot");
	%brick.setCityLotOwnerID(-1);

	%client.cityMenuMessage("\c6Lot transferred to the city successfully.");
	%client.cityMenuClose();
}

function CityMenu_LotAdmin_TransferPlayerPrompt(%client)
{
	%client.cityLog("Lot MOD " @ %client.cityMenuID.getCityLotID() @ " transfer pl prompt");

	%client.cityMenuMessage("\c6Enter a Blockland ID of the player to transfer the lot to.");
	%client.cityMenuFunction = CityMenu_LotAdmin_TransferPlayer;
}

function CityMenu_LotAdmin_TransferPlayer(%client, %input)
{
	%client.cityLog("Lot MOD " @ %client.cityMenuID.getCityLotID() @ " transfer pl '" @ %input @ "'");

	%target = findClientByBL_ID(%input);

	// Hacky workaround to detect if a non-number is passed to avoid pain.
	if(%input == 0 && %input !$= "0")
	{
		%client.cityMenuMessage("\c3" @ %input @ "\c6 is not a valid Blockland ID. Please try again.");
		return;
	}

	CityLots_TransferLot(%client.cityMenuID, %input);
	%client.cityMenuID.setCityLotTransferDate(getDateTime());

	%client.cityMenuClose();
}
