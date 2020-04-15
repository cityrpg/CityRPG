// Errors
$Error::CityShop::NotLicensed = -1;
$Error::CityShop::NoResources = -2;
$Error::CityShop::NoSlot = -3;
$Error::CityShop::Owned = -4;
$Error::CityShop::InvalidItem = -5;

// CityMenu_SellItem
// Called from the
function CityMenu_SellItem(%client, %brick, %item, %markup)
{
	%menu = "Yes" TAB "No";

	%functions =  "CityMenu_Placeholder" TAB "CityMenu_Close";

	if(%client.CityRPGLotBrick)
	{
		%ownerStr = "\c3" @ %client.CityRPGLotBrick.getCityLotName() @ "\c6";
	}
	else
	{
		%ownerStr = "the city";
	}

	// TODO: This currently uses the client's lot brick trigger, however it should work off the brick's location instead.
	messageClient(%client, '', "\c6Would you like to purchase this \c3" @ %item.uiName @ "\c6 from " @ %ownerStr @ " for \c3$" @ %markup @ "\c6?");

	%client.cityMenuOpen(%menu, %functions, %brick, "\c6Purchase cancelled.", 1);
}



// Returns blank if there is no free space
// TODO: Add an option to allow duplicates
function Player::getCityItemSlot(%player, %itemID)
{
	for(%i = 0; %i < %player.getDatablock().maxTools; %i++)
	{
		if(!isObject(%player.tool[%i]) || %player.tool[%i].getName() !$= %itemID.getName())
		{
			if(%freeSpot $= "" && %player.tool[%i] $= "")
			{
				%freeSpot = %i;
			}
		}
		else
		{
			return $Error::CityShop::Owned;
		}
	}

	// If no free spot was set, there's no slot available.
	if(%freeSpot $= "")
	{
		return $Error::CityShop::NoSlot;
	}

	return %freeSpot;
}

// fxDTSBrick::citySaleCheck
// The base check for eligibility to sell an item to a player.
// targetClient is optional
function fxDTSBrick::citySaleCheck(%brick, %targetClient, %item)
{
	// TODO: This currently only checks for "sell item" and not arms sales.
	if(!JobSO.job[CityRPGData.getData(%brick.getGroup().bl_id).valueJobID].sellItems)
	{
		return $Error::CityShop::NotLicensed;
	}
	else if(%item != 0 && CitySO.minerals < $CityRPG::prices::weapon::mineral[%item])
	{
		return $Error::CityShop::NoResources;
	}
	else if(isObject(%targetClient) && %slotCheck = %targetClient.player.getCityItemSlot(%item) == $Error::CityShop::NoSlot)
	{
		// This check only runs if a client arg was specified
		return $Error::CityShop::NoSlot;
	}
	else if(%slotCheck == $Error::CityShop::Owned)
	{
		// Same as above, skip if the value is never set
		return $Error::CityShop::Owned;
	}
	//else if(true)
	//{
	//	return $Error::CityShop::InvalidItem;
	//}
	else
	{
		return $Error::None;
	}
}
