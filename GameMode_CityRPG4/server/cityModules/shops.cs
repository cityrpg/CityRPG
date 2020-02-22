// CityMenu_SellItem
// Called from the
function CityMenu_SellItem(%client, %brick, %item, %markup)
{
	%menu = "Yes" TAB "No";

	%functions =  "CityMenu_Placeholder" TAB "CityMenu_Close";

	// TODO: This currently uses the client's lot brick trigger, however it should work off the brick's location instead.
	messageClient(%client, '', "\c6Would you like to purchase this \c3" @ %item.uiName @ "\c6 from " @ %client.CityRPGLotBrick.getCityLotName() @ " for \c3$" @ %markup @ "\c6?");

	%client.cityMenuOpen(%menu, %functions, %brick, "\c6Purchase cancelled.");
}
