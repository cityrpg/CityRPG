
// Client.cityMenuOpen(names, functions, exitMsg, autoClose, canOverride)
// Utilizes a combination of Conan's center-print menus for selections, and chat inputs for text.
function GameConnection::cityMenuOpen(%client, %menu, %functions, %menuID, %exitMsg, %autoClose, %canOverride, %title)
{
	if(%client.cityMenuOpen && !%client.cityMenuCanOverride)
	{
		return;
	}

	// Blank if -1
	if(%exitMsg == -1)
	{
		%exitMsg = "";
	}

	if(getFieldCount(%menu) != 0)
	{
		%menuObj = new ScriptObject()
		{
			isCenterprintMenu = 1;
			menuName = $c_p @ %title; // Leave it blank.

			justify = "<just:center>";

			deleteOnExit = 1;

			fontA = "<font:palatino linotype:24>\c6";
			fontB = "<font:palatino linotype:24><div:1>\c6";

			menuOptionCount = getFieldCount(%menu);
		};
		MissionCleanup.add(%menuObj);

		if(%menuID $= "")
		{
			error("CityRPG 4 - Attempting to open a menu with no ID. Aborting.");
			echo("Menu data: " @ %menu);

			return;
		}

		for(%i = 0; %i < getFieldCount(%menu); %i++)
		{
			%menuObj.menuOption[%i] = getField(%menu, %i);
			%menuObj.menuFunction[%i] = getField(%functions, %i);
		}

		%client.startCenterprintMenu(%menuObj);
	}

	// Set the necessary values to the client
	// Most of these are covered by the centerprint menu system, but we're retaining them for flexibility.
	%client.cityMenuOpen = true; // Package checks for this
	%client.cityMenuFunction = %functions;
	%client.cityMenuAutoClose = %autoClose;
	%client.cityMenuID = %menuID;
	%client.cityMenuExitMsg = %exitMsg;
	%client.cityMenuCanOverride = %canOverride;

	// Event
	if(isObject(%menuID) && %menuID.getClassName() $= "fxDTSBrick")
	{
		%menuID.onMenuOpen();
	}

	// Return to indicate everything went smoothly.
	return true;
}

// Client.cityMenuInput(input)
// Called when a user enters an input for a city menu.
function GameConnection::cityMenuInput(%client, %input)
{
	// Event
	if(isObject(%client.cityMenuID) && %client.cityMenuID.getClassName() $= "fxDTSBrick")
	{
		%client.cityMenuID.onMenuInput();
	}

	// If there's multiple fields, this is a numbered menu.
	if(getFieldCount(%client.cityMenuFunction) > 1)
	{
		%function = getField(%client.cityMenuFunction, %input-1);

		if(!isFunction(%function))
		{
			messageClient(%client, '', $c_p @ %input @ "\c6 is not a valid option. Please try again.");
			return false;
		}

		%id = %client.cityMenuID;
		if(%client.cityMenuAutoClose)
			%client.cityMenuClose();

		// It's important to do this after closing in case the function we're calling opens another menu.
		call(%function, %client, %input, %id);

		return true;
	}
	else
	{
		%function = %client.cityMenuFunction;
		// Not a numbered input, call the specified function, passing the client object and message.
		if(isFunction(%function))
		{
			call(%function, %client, %input, %client.cityMenuID);
		}
	}
}

// Client.cityMenuClose(silent)
// silent: (bool) If set to true, the exit message will not show even if defined.
// Automatically called if the client leaves a trigger with an ID corresponding to
// either the menu's ID or the ID of %client.cityMenuBack via CityRPGInputTriggerData::onLeaveTrigger.
function GameConnection::cityMenuClose(%client, %silent)
{
	if(%client.cityMenuOpen)
	{
		%client.exitCenterprintMenu();

		// Use a 1ms delay so the 'closed' message shows after any other messages
		if(!%silent)
			%client.cityMenuMessage(%client.cityMenuExitMsg);

		// Event
		if(isObject(%client.cityMenuID) && %client.cityMenuID.getClassName() $= "fxDTSBrick")
		{
			%client.cityMenuID.onMenuClose();
		}

		%client.cityMenuOpen = false;
		%client.cityMenuFunction = "";
		%client.cityMenuID = "";
		%client.cityMenuExitMsg = "";
		%client.cityMenuAutoClose = "";
		%client.cityMenuBack = "";
	}
}

// Client.cityMenuClose(silent)
// msg: (str) Message to display to the client. Accepts color codes (" @ $c_p @ ", etc.)
function GameConnection::cityMenuMessage(%client, %msg)
{
	%client.cityMenuLastMsg = %msg;
	%client.cityMenuMsgTime = atof(getSimTime());
	// We're using messageClient for now--simple enough, but this is subject to change.
	messageClient(%client, '', %msg);
}

// Hook functions (CityMenu_*) - These functions are used within menus.
function CityMenu_Close(%client)
{
	%client.cityMenuClose();
}
function CityMenu_Placeholder(%client)
{
	%client.cityMenuMessage("\c6Sorry, this feature is currently not available. Please try again later.");
}