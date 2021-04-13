// --------------------------
// Name: Saver
// Description: Database System
// Version: 1.5a
// Author: McTwist
// Copyright: Free for all
// License: Do not change anything
// --------------------------
// Notes
// No saving system is the same, but all saving systems is the other one alike.
// --------------------------
// Update 1.1
// Made each ID an own file
// --------------------------
// Update 1.2
// Removed the object spamming
// Added cache to store the data temporarily
// Renamed SaverDB to SaverRow
// Moved most algorithms to SaverRow
// Added existKey list
// --------------------------
// Update 1.3
// Added debug variable
// Added SassyToSaver function
// Fixed existKey bug
// Removed countData variable
// Reduced memory leaks
// Fixed saveKey from cache bug
// Changed SaverRow::cache to SaverRow::saveToCache and added SaverRow::loadFromCache
// Improved performance
// Fixed cache bug with SaverRow
// --------------------------
// Update 1.3a
// Removed delete spam
// --------------------------
// Update 1.4
// Removed client related code
// Added getCount and getObject functions
// Added saving extension variable
// Added version variable
// Added corrupted data handler
// Added stream variable to be used for file handling
// Added isOnline function
// Added get and set functions
// Improved code
// --------------------------
// Update 1.5
// Fixed bug when saving file
// Changed everything associated ID to key
// Added special boolean values
// Added clear function to cache data

// --------------------------
// Update 1.5a
// Fixed console errors bug

///////////////
// Variables //
///////////////
// Current version
$Saver::version = 1.5;

// Debug
// 0 - None
// 1 - Normal
// 2 - Warnings
// 3 - Errors
$Saver::debug = 0;

///////////
// Saver //
///////////
// Start database
function Saver::onAdd(%this)
{
	// Database has no name
	if (%this.getName() $= "")
	{
		if ($Saver::debug >= 3) error("Saver_error: Database needs a name.");
		%this.schedule(0, "delete");
		return false;
	}
	
	// Database has no saverfile
	if (%this.file $= "" || %this.defFile $= "" || %this.folder $= "")
	{
		if ($Saver::debug >= 3) error("Saver_error: Did not find a file variable.");
		%this.schedule(0, "delete");
		return false;
	}
	
	// Saving extension
	%this.saveExt = (%this.saveExt $= "") ? "sav" : trim(strreplace(%this.saveExt, ".", ""));
	
	%this.countValue = 0;
	%this.countKeys = 0;
	%this.countOnline = 0;
	%this.countBool = 0;
	
	%this.boolChar = 28;
	
	%this.loaded = false;
	
	// Create special file object
	%this.stream = new fileObject();
	
	// Load defaultfile if exist
	%this.loaded = %this.loadDefs();
	if (%this.loaded)
		if (%this.loadKeyList())
			%this.load();
	
	return true;
}

// Shutdown database
function Saver::onRemove(%this)
{
	%this.saveDefs();
	%this.save();
	%this.saveKeyList();
	
	for (%i = 1; %i <= %this.countOnline; %i++)
		if (isObject(%this.data[%this.listOnline[%i]]))
			%this.data[%this.listOnline[%i]].delete();
	
	%this.stream.delete();
	return true;
}

// Saves data
function Saver::save(%this)
{
	if ($Saver::debug >= 1) echo("Saving database...");
	
	for (%i = 1; %i <= %this.countOnline; %i++)
		%this.saveKey(%this.listOnline[%i]);
	
	if ($Saver::debug >= 1) echo("Saved!");
}

// Save specific key
function Saver::saveKey(%this, %key)
{
	if (!isObject(%this.stream))
		return false;
	if (%this.existKey[%key] != 1)
		return;
	
	// User is online
	if (isObject(%this.data[%key]))
	{
		%this.data[%key].saveToFile();
	}
	else
	{
		%this.stream.openForWrite(%this.folder @ %key @ "." @ %this.saveExt);
		
		// Saves values
		for (%i = 1; %i <= %this.countValue; %i++)
			%this.stream.writeLine(%this.value[%i] SPC %this.cache[%this.value[%i], %key]);
		
		// Save bools
		if (%this.countBool > 0)
		{
			for (%i = 0; %i < %this.countBool; %i++)
				%words = (%words $= "") ? %this.cache["bool" @ %i, %key] : %words SPC %this.cache["bool" @ %i, %key];
			%this.stream.writeLine("bool " @ %words);
		}
		
		%this.stream.close();
	}
}

// Saves default values
function Saver::saveDefs(%this)
{
	if (!isObject(%this.stream))
		return false;
	if ($Saver::debug >= 1) echo("Saving default file...");
	%this.stream.openForWrite(%this.defFile);
	
	for (%i = 1; %i <= %this.countValue; %i++)
		%this.stream.writeLine(%this.value[%i] SPC %this.defValue[%i]);
	
	// Boolean values
	if (%this.countBool > 0)
	{
		%n = mCeil(%this.countBool / %this.boolChar);
		for (%i = 1; %i <= %n; %i++)
			%def = %def SPC %this.defBool[%i];
		// Write bools
		%this.stream.writeLine("bool" @ %def);
		for (%i = 1; %i <= %this.countBool; %i++)
			%names = (%names $= "") ? %this.bool[%i] : %names SPC %this.bool[%i];
		// Write names
		%this.stream.writeLine(%names);
	}
	
	%this.stream.close();
	
	return true;
}

// Save key list
function Saver::saveKeyList(%this, %key)
{
	if (!isObject(%this.stream))
		return false;
	if ($Saver::debug >= 1) echo("Saving list...");
	
	// Remake it
	if (%key $= "")
	{
		%this.stream.openForWrite(%this.file);
	
		// Loop throught list
		for (%i = 1; %i <= %this.countKeys; %i++)
			%this.stream.writeLine(%this.listKey[%i]);
		
		%this.stream.close();
	}
	// Add to existed one
	else
	{
		// Already exist
		if (%this.existKey[%key] == 1)
			return;
		%this.stream.openForAppend(%this.file);
		
		%this.stream.writeLine(%key);
		
		%this.stream.close();
	}
}

// Load previously saved data
function Saver::load(%this)
{
	if (!isObject(%this.stream))
		return false;
	if (!(%this.countKeys > 0))
	{
		if ($Saver::debug >= 3) error("Saver_err: No files to read.");
		return false;
	}
	
	if ($Saver::debug >= 1) echo("Loading database...");
	
	for (%i = 1; %i <= %this.countKeys; %i++)
	{
		%key = %this.listKey[%i];
		if (!isFile(%this.folder @ %key @ "." @ %this.saveExt))
			continue;
		%this.stream.openForRead(%this.folder @ %key @ "." @ %this.saveExt);
		
		while (!%this.stream.isEOF())
		{
			%line = %this.stream.readLine();
			
			if (%line $= "")
				continue;
			
			%first = firstWord(%line);
			%rest = restWords(%line);
			if (%first $= "bool")
			{
				// Load bools
				%n = getWordCount(%rest);
				for (%j = 1; %j <= %n; %j++)
					%this.cache["bool" @ %j, %key] = atoi(getWord(%rest, %j-1));
				continue;
			}
			// Cache
			%this.cache[%first, %key] = %rest;
		}
		
		%this.stream.close();
	}
	
	if ($Saver::debug >= 1) echo(%this.countKeys @ " keys found.");
}

// Load default values
function Saver::loadDefs(%this)
{
	if (!isObject(%this.stream))
		return false;
	if (!isFile(%this.defFile))
	{
		if ($Saver::debug >= 3) error("Saver_err: Default file does not exist.");
		return false;
	}
	
	if ($Saver::debug >= 1) echo("Loading default file...");
	
	%this.countValue = 0;
	%this.countBool = 0;
	
	%this.stream.openForRead(%this.defFile);
	
	while (!%this.stream.isEOF())
	{
		%line = %this.stream.readLine();
		
		if (%line $= "")
			continue;
		
		%value = firstWord(%line);
		%rest = restWords(%line);
		// Bool
		if (%value $= "bool")
		{
			%n = getWordCount(%rest);
			// Add default value
			for (%i = 1; %i <= %n; %i++)
				%this.defBool[%i] = atoi(getWord(%rest, %i-1));
			// Get names
			%line = %this.stream.readLine();
			if (%line $= "" || %rest $= "")
				continue;
			%count = getWordCount(%line);
			// Add boolean values
			for (%i = 1; %i <= %count; %i++)
			{
				%name = getWord(%line, %i-1);
				if (%name $= "")
					continue;
				%this.countBool++;
				%this.bool[%i] = %name;
				%this.boolName[%name] = %i;
			}
			continue;
		}
		// Add value
		%this.value[%this.countValue++] = %value;
		%this.defValue[%this.countValue] = %rest;
		%this.valueName[%this.value[%this.countValue]] = %this.countValue;
		
		// Add corrupted data
		if (%value $= "0")
		{
			%this.corruptedVar = %value;
			%this.corruptedKey = %this.countValue;
		}
	}
	
	%this.stream.close();
	
	return true;
}

// Get key list
function Saver::loadKeyList(%this)
{
	if (!isObject(%this.stream))
		return false;
	if (!isFile(%this.file))
	{
		if ($Saver::debug >= 3) error("Saver_err: List does not exist.");
		return false;
	}
	
	if ($Saver::debug >= 1) echo("Loading key list...");
	
	%this.stream.openForRead(%this.file);
	
	%this.countKeys = 0;
	
	while (!%this.stream.isEOF())
	{
		%line = %this.stream.readLine();
		
		// Avoid spaces
		if (trim(%line) $= "")
			continue;
		
		%this.listKey[%this.countKeys++] = getWord(%line, 0);
		%this.existKey[getWord(%line, 0)] = 1;
	}
	
	%this.stream.close();
	
	return true;
}

// Adding values to the database
function Saver::addValue(%this, %value, %defaultValue)
{
	if (%value $= "")
	{
		if ($Saver::debug >= 3) error("Saver_err: Values are invalid.");
		return false;
	}
	if (strpos(%value, " ") >= 0)
	{
		if ($Saver::debug >= 3) error("Saver_err: Value has more than one word.");
		return false;
	}
	if (%valueKey = %this.findValue(%value))
	{
		if (%this.defValue[%valueKey] !$= %defaultValue)
		{
			if ($Saver::debug >= 2) warn("Saver_not: " @ %value @ " updated default value to " @ %defaultValue @ ".");
			%this.defValue[%valueKey] = %defaultValue;
		}
		else
		{
			if ($Saver::debug >= 2) warn("Saver_not: " @ %value @ " already exist. Skipping...");
		}
		return false;
	}
	
	%this.value[%this.countValue++] = %value;
	%this.defValue[%this.countValue] = %defaultValue;
	%this.valueName[%value] = %this.countValue;
	
	// Add to each key online
	for (%i = 1; %i <= %this.countOnline; %i++)
		%this.data[%this.listOnline[%i]].value[%value] = %defaultValue;
	// Add to each key
	for (%i = 1; %i <= %this.countKeys; %i++)
		%this.cache[%value, %this.listKey[%i]] = %defaultValue;
	
	return true;
}

// Search for value
function Saver::findValue(%this, %value)
{
	return %this.valueName[%value] || 0;
}

// Add bool value
function Saver::addBool(%this, %value, %default)
{
	if (%value $= "")
	{
		if ($Saver::debug >= 3) error("Saver_err: Values are invalid.");
		return false;
	}
	if (strpos(%value, " ") >= 0)
	{
		if ($Saver::debug >= 3) error("Saver_err: Value has more than one word.");
		return false;
	}
	%default = (%default) ? true : false;
	if (%valueKey = %this.findBool(%value))
	{
		if (%this.defBool[%valueKey] != %default)
		{
			if ($Saver::debug >= 2) warn("Saver_not: " @ %value @ " updated default bool to " @ %default @ ".");
			%this.defBool[%valueKey] = %default;
		}
		else
		{
			if ($Saver::debug >= 2) warn("Saver_not: " @ %value @ " already exist. Skipping...");
		}
		return false;
	}
	
	%a = %this.countBool % %this.boolChar;
	%i = %this.countBool++;
	%b = mCeil(%i / %this.boolChar);
	%n = %default << %a;
	%this.bool[%i] = %value;
	%this.defBool[%b] |= %n;
	%this.boolName[%value] = %i;
	
	// Add to each key online
	for (%i = 1; %i <= %this.countOnline; %i++)
		%this.data[%this.listOnline[%i]].setBool(%value, %default);
	// Add to each key
	for (%i = 1; %i <= %this.countKeys; %i++)
		%this.cache["bool" @ %b, %this.listKey[%i]] |= %n;
	
	return true;
}

// Find a bool
function Saver::findBool(%this, %value)
{
	return %this.boolName[%value] || 0;
}

// Adds new key
function Saver::addKey(%this, %key)
{
	if (%key $= "")
	{
		if ($Saver::debug >= 3) error("Saver_err: Need a key.");
		return false;
	}
	
	if (%this.existKey[%key] == 1)
	{
		if ($Saver::debug >= 2) warn("Saver_not: Key " @ %key @ " already exist. Skipping...");
		return true;
	}
	
	if ($Saver::debug >= 1) echo("Key " @ %key @ " added.");
	
	// Saves ID in a list
	%this.listKey[%this.countKeys++] = %key;
	
	// Add default values
	for (%i = 1; %i <= %this.countValue; %i++)
		%this.cache[%this.value[%i], %key] = %this.defValue[%i];
		
	// Add to each key
	%n = mCeil(%this.countBool / %this.boolChar);
	for (%i = 1; %i <= %n; %i++)
		%this.cache["bool" @ %i, %key] = %this.defBool[%i];
	
	// This happens only once per ID
	%this.saveKeyList(%key);
	%this.existKey[%key] = 1;
	
	// Create and save a file
	%this.saveKey(%key);
	
	return true;
}

// Clear player information
function Saver::clearKey(%this, %key)
{
	if (%key $= "")
	{
		if ($Saver::debug >= 3) error("Saver_err: Need a key.");
		return false;
	}
	
	if (!isObject(%this.data[%key]))
	{
		if (!%this.existKey[%key])
		{
			if ($Saver::debug >= 2) warn("Saver_not: Key " @ %key @ " does not exist.");
			return false;
		}
		// Add to default values
		for (%i = 1; %i <= %this.countValue; %i++)
			%this.cache[%this.value[%i], %key] = %this.defValue[%i];
		%n = mCeil(%this.countBool / %this.boolChar);
		for (%i = 1; %i <= %n; %i++)
			%this.cache["bool" @ %i, %key] = %this.defBool[%i];
		return true;
	}
	
	// Add to default values
	%this.data[%key].clear();
	return true;
}

// Get if the user is online
function Saver::isOnline(%this, %key)
{
	return %this.existListOnline[%key];
}

// Adds online and return an object of the user
function Saver::makeOnline(%this, %key)
{
	if (%key $= "")
		return 0;
	
	// Find if ID is already online
	if (%this.isOnline(%key))
		return %this.data[%key];
	
	if ($Saver::debug >= 1) echo("Key " @ %key @ " logged in.");
	// Add ID to online list
	%this.listOnline[%this.countOnline++] = %key;
	%this.existListOnline[%key] = 1;
	
	// Create object
	%this.data[%key] = new scriptObject()
	{
		class = SaverRow;
		key = %key;
		parent = %this;
	};
	
	// Add key values
	%this.data[%key].loadFromCache();
	
	// Clear if corrupted data
	if (%this.data[%key].checkCorruptedData())
	{
		if ($Saver::debug >= 2) echo("Key " @ %key @ " data was corrupted.");
		%this.data[%key].clear();
	}
	
	return %this.data[%key];
}

// Remove offline players
function Saver::makeOffline(%this, %key)
{
	if (%this.countOnline == 0)
		return;
	
	if (%key !$= "")
	{
		if (isObject(%this.data[%key]))
			%this.data[%key].delete();
		if ($Saver::debug >= 1) echo("Key " @ %key @ " logged out.");
		%this.existListOnline[%key] = "";
	}
	// Get players online
	%amountOnline = %this.countOnline;
	%n = 0;
	
	// Loop throught online list
	for (%i = 1; %i <= %amountOnline; %i++)
	{
		if (%this.listOnline[%i] !$= %key)
			%this.listOnline[%n++] = %this.listOnline[%i];
		else
			%this.countOnline--;
	}
}

// Get data
function Saver::get(%this, %key, %value)
{
	if (%key $= "" || %value $= "")
		return "";
	if (%this.existKey[%key] != 1)
		return "";
	// Get data or cache
	return (%this.existListOnline[%key]) ? %this.data[%key].value[%value] : %this.cache[%value, %key];
}

// Set data
function Saver::set(%this, %key, %value, %data)
{
	if (%key $= "" || %value $= "")
		return "";
	if (%this.existKey[%key] != 1)
		return "";
	// Set data
	if (%this.existListOnline[%key])
		%this.data[%key].value[%value] = %data;
	// Set cache
	else
		%this.cache[%value, %key] = %data;
}

// Get bool
function Saver::getBool(%this, %key, %value)
{
	if (%key $= "" || %value $= "")
		return false;
	if (%this.existKey[%key] != 1)
		return "";
	if (!(%i = %this.findBool(%value)))
		return false;
	
	%a = (%i - 1) % %this.boolChar;
	%b = mCeil(%i / %this.boolChar);
	return (%this.existListOnline[%key]) ? %this.data[%key].getBool(%value) : ((%this.cache["bool" @ %b, %key] & (1 << %a)) >> %a);
}

// Set bool
function Saver::setBool(%this, %key, %value, %data)
{
	if (%key $= "" || %value $= "")
		return "";
	if (%this.existKey[%key] != 1)
		return "";
	if (!(%i = %this.findBool(%value)))
		return false;
	
	%data = (%data) ? true : false;
	%a = (%i - 1) % %this.boolChar;
	%b = mCeil(%i / %this.boolChar);
	// Set data
	if (%this.existListOnline[%key])
		%this.data[%key].setBool(%value, %data);
	// Set cache
	else
	{
		%n = 1 << %a;
		// True
		if (%data)
			%this.cache["bool" @ %b, %key] |= %n;
		// False
		else if (%this.cache["bool" @ %b, %key] & %n)
			%this.cache["bool" @ %b, %key] -= %n;
	}
}

// Get amount of objects
function Saver::getCount(%this)
{
	return %this.countOnline;
}

// Get object
function Saver::getObject(%this, %i)
{
	if (%i < 0 || %i >= %this.countOnline)
	{
		if ($Saver::debug >= 3) error("Saver_err: Invalid object.");
		return 0;
	}
	
	return %this.data[%this.listOnline[%i+1]];
}

//////////////
// SaverRow //
//////////////
// Add Row
function SaverRow::onAdd(%this)
{
	if(%this.key $= "")
	{
		if ($Saver::debug >= 3) error("SaverRow_err: Database needs a key.");
		return false;
	}
	
	if(!isObject(%this.parent))
	{
		if ($Saver::debug >= 3) error("SaverRow_err: Database needs a parent.");
		return false;
	}
	
	// Save the way to the file
	%this.file = %this.parent.folder @ %this.key @ "." @ %this.parent.saveExt;
	
	return true;
}

// Remove row
function SaverRow::onRemove(%this)
{
	%this.saveToFile();
	%this.saveToCache();
}

// Loads information from file
function SaverRow::loadFromFile(%this)
{
	if (!isObject(%this.parent.stream))
		return false;
	// No need to load
	if (!isFile(%this.file))
		return;
	%stream = %this.parent.stream;
	if (!isObject(%stream))
		return;
	
	%stream.openForRead(%this.file);
	
	while (!%stream.isEOF())
	{
		%line = %stream.readLine();
		
		if (%line $= "")
			continue;
		
		%first = firstWord(%line);
		%rest = restWords(%line);
		if (%first $= "bool")
		{
			// Load bools
			%n = getWordCount(%rest);
			for (%i = 1; %i <= %n; %i++)
				%this.value["bool" @ %i] = getWord(%rest, %i-1);
			continue;
		}
		
		// Save variables
		%this.value[%first] = %rest;
	}
	
	%stream.close();
}

// Save information on file
function SaverRow::saveToFile(%this)
{
	%stream = %this.parent.stream;
	if (!isObject(%stream))
		return;
	%stream.openForWrite(%this.file);
	
	// Saves values
	for (%n = 1; %n <= %this.parent.countValue; %n++)
		%stream.writeLine(%this.parent.value[%n] SPC %this.value[%this.parent.value[%n]]);
	
	// Save bools
	%n = mCeil(%this.parent.countBool / %this.parent.boolChar);
	for (%i = 1; %i <= %n; %i++)
		%words = %words SPC %this.value["bool" @ %i];
	%stream.writeLine("bool" @ %words);
	
	%stream.close();
}

// Clear ID
function SaverRow::clear(%this)
{
	// Add to default values
	for (%i = 1; %i <= %this.parent.countValue; %i++)
		%this.value[%this.parent.value[%i]] = %this.parent.defValue[%i];
	%n = mCeil(%this.parent.countBool / %this.parent.boolChar);
	for (%i = 1; %i <= %n; %i++)
		%this.value["bool" @ %i] = %this.parent.defBool[%i];
}

// Send information back to cache
function SaverRow::saveToCache(%this)
{
	for (%i = 1; %i <= %this.parent.countValue; %i++)
		%this.parent.cache[%this.parent.value[%i], %this.key] = %this.value[%this.parent.value[%i]];
	%n = mCeil(%this.parent.countBool / %this.parent.boolChar);
	for (%i = 1; %i <= %n; %i++)
		%this.parent.cache["bool" @ %i, %this.key] = %this.value["bool" @ %i];
}

// Get information from cache
function SaverRow::loadFromCache(%this)
{
	for (%i = 1; %i <= %this.parent.countValue; %i++)
		%this.value[%this.parent.value[%i]] = %this.parent.cache[%this.parent.value[%i], %this.key];
	%n = mCeil(%this.parent.countBool / %this.parent.boolChar);
	for (%i = 1; %i <= %n; %i++)
		%this.value["bool" @ %i] = %this.parent.cache["bool" @ %i, %this.key];
}

// Check corrupted data
function SaverRow::checkCorruptedData(%this)
{
	%key = %this.parent.corruptedKey;
	// Nothing to check against
	if (!(%key > 0))
		return false;
	
	// Check for corrupted data
	return %this.value[%this.parent.value[%key]] $= "";
}

// Get bool
function SaverRow::getBool(%this, %value)
{
	if (%value $= "")
		return false;
	if (!(%i = %this.parent.findBool(%value)))
		return false;
	
	%a = (%i - 1) % %this.parent.boolChar;
	%b = mCeil(%i / %this.parent.boolChar);
	return ((%this.value["bool" @ %b] & (1 << %a)) >> %a);
}

// Set bool
function SaverRow::setBool(%this, %value, %data)
{
	if (%value $= "")
		return false;
	if (!(%i = %this.parent.findBool(%value)))
		return false;
	
	%data = (%data) ? true : false;
	%a = (%i - 1) % %this.parent.boolChar;
	%b = mCeil(%i / %this.parent.boolChar);
	%n = 1 << %a;
	
	// True
	if (%data)
		%this.value["bool" @ %b] |= %n;
	// False
	else if (%this.value["bool" @ %b] & %n)
		%this.value["bool" @ %b] -= %n;
}

/////////////
// Command //
/////////////
// Version check
function ServerCmdSaverVersion(%client)
{
	messageClient(%client, '', "\c6Saver is \c3v" @ $Saver::version);
}

///////////////////////////////////////
// Transfer data from Sassy to Saver //
///////////////////////////////////////
// Normally, you can type like this: SassyToSave("config/server/PeopleMods/CityRPData.dat");
function SassyToSaver(%from)
{
	if (!isFile(%from))
	{
		echo("SassyToSaver: No such file found.");
		return;
	}
	
	if (!isObject(RPDB))
	{
		echo("SassyToSaver: Saver database is offline.");
		return;
	}
	
	if (RPDB.countOnline > 0)
	{
		echo("SassyToSaver: Clear out old player data.");
		
		// Loop throught online list
		for (%i = 1; %i <= RPDB.countOnline; %i++)
		{
			%key = RPDB.listOnline[%i];
			// Online
			if (isObject(RPDB.data[%key]))
			{
				RPDB.data[%key].delete();
				RPDB.data[%key] = 0;
			}
			// Cache
			else
			{
				RPDB.saveKey(%key);
			}
		}
		RPDB.countOnline = 0;
	}
	
	echo("SassyToSaver: Starting transferring.");
	
	%stream = new fileObject();
	%stream.openForRead(%from);
	
	%lastID = -1;
	%last = "";
	
	while (!%stream.isEOF())
	{
		%line = %stream.readLine();
		
		if (trim(%line) $= "")
			continue;
		
		switch$(getWord(%line, 0))
		{
			case "values":
				%last = "values";
			case "ID":
				%lastID = getWord(%line, 1);
				%last = "ID";
				RPDB.addKey(%lastID);
			default:
				if (%last $= "ID")
				{
					%type = getWord(%line, 1);
					// Convert variables
					switch$(%type)
					{
						case "dept":
							RPDB.cache["loan", %lastID] = getWord(%line, 2);
						case "jailData":
							%clean = getWord(%line, 2);
							%jail = getWord(%line, 3);
							RPDB.cache["jail", %lastID] = (%clean) ? -1 : %jail;
						case "resources":
							RPDB.cache["wood", %lastID] = getWord(%line, 2);
							RPDB.cache["ore", %lastID] = getWord(%line, 3);
						case "tools":
							%tools = getWords(%line, 2, getWordCount(%line) - 1);
							%newTools = "";
							for (%i = 0; %i < getWordCount(%tools); %i++)
							{
								%tool = getWord(%tools, %i);
								if (isObject(%tool))
									%newTools = (%newTools $= "") ? %tool.uiName : %newTools TAB %tool.uiName;
							}
							RPDB.cache["tools", %lastID] = %newTools;
						default:
							RPDB.cache[%type, %lastID] = getWords(%line, 2, getWordCount(%line) - 1);
					}
				}
		}
	}
	
	%stream.close();
	%stream.delete();
	
	echo("SassyToSaver: Save new data.");
	
	RPDB.saveKeyList();
	for (%i = 1; %i <= RPDB.countKeys; %i++)
		RPDB.saveKey(RPDB.listKey[%i]);
	
	echo("SassyToSaver: Give back online player data.");
	
	for (%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%client = ClientGroup.getObject(%i);
		%data = RPDB.makeOnline(%client.getSaveKey());
		if (isObject(%data))
			%client.RPData = %data;
	}
	
	echo("SassyToSaver: Finished.");
}
