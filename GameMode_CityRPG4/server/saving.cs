//--------------------
// Purpose:	Saves a user's data.
//--------------------


if(!isObject(SassyGroup))
{
	new simGroup(SassyGroup) {};
}

function Sassy::onAdd(%this)
{
	if(%this.getName() $= "")
	{
		error("Sassy::onAdd(): This database has no name! Deleting database..");

		%this.schedule(0, "delete");

		return false;
	}

	if($server::lan)
	{
		error(%this.getName() @ "::onAdd(): Sassy does not support LAN servers! Deleting database..");

		%this.schedule(0, "delete");

		return false;
	}

	if(%this.dataFile $= "")
	{
		error(%this.getName() @ "::onAdd(): Sassy needs a dataFile variable! Deleting database..");

		%this.schedule(0, "delete");

		return false;
	}

	%this.valueCount = 0;
	%this.dataCount = 0;

	%this.loadedSaveFile = false;

	if(isFile(%this.dataFile))
	{
		echo(%this.getName() @ "::onAdd(): Previous save file found. Loading..");

		%this.loadData();

		%this.loadedSaveFile = true;
	}

	return true;
}

function Sassy::onRemove(%this)
{
	for(%b = 1; %b <= %this.dataCount; %b++)
	{
		%this.data[%b].delete();
	}

	return true;
}

function Sassy::saveData(%this)
{
	fileCopy(%this.dataFile, %this.dataFile @ ".bak");

	%file = new fileObject();
	%file.openForWrite(%this.dataFile);

	%file.writeLine("values");

	for(%a = 1; %a <= %this.valueCount; %a++)
	{
		%file.writeLine(" " @ %this.value[%a] SPC %this.defaultValue[%a]);
	}

	if(%this.dataCount > 0)
	{
		%file.writeLine("");
	}

	for(%b = 1; %b <= %this.dataCount; %b++)
	{
		if(!isObject(%this.data[%b]))
		{
			continue;
		}

		%file.writeLine("ID " @ %this.data[%b].ID);

		for(%c = 1; %c <= %this.valueCount; %c++)
		{
			%file.writeLine(" " @ %this.value[%c] SPC %this.data[%b].value[%this.value[%c]]);
		}

		if(%b < %this.dataCount)
		{
			%file.writeLine("");
		}
	}

	%file.close();
	%file.delete();

	return true;
}

function Sassy::loadData(%this)
{
	if(!isFile(%this.dataFile))
	{
		error("Sassy::loadData(): File '" @ %this.dataFile @ "' not found. Aborting..");

		return false;
	}

	%file = new fileObject();
	%file.openForRead(%this.dataFile);

	%valueListFound = false;
	%defaultValueListFound = false;

	while(!%file.isEOF())
	{
		%line = %file.readLine();

		if(%line $= "")
		{
			%currentState = "";

			continue;
		}

		if(getSubStr(%line, 0, 1) !$= " ")
		{
			%currentState = getWord(%line, 0);

			if(getWord(%line, 0) $= "ID")
			{
				if(!%this.getData(getWord(%line, 1)))
				{
					%this.dataCount++;

					%this.data[%this.dataCount] = new scriptObject()
					{
						class = SassyData;

						ID = getWord(%line, 1);
						parent = %this;
					};
				}
			}
		}

		if(getSubStr(%line, 0, 1) $= " ")
		{
			if(%currentState $= "values")
			{
				%this.valueCount++;

				%this.value[%this.valueCount] = getWord(%line, 1);
				%this.defaultValue[%this.valueCount] = getWords(%line, 2, getWordCount(%line) - 1);
			}

			if(%currentState $= "ID")
			{
				%this.data[%this.dataCount].value[getWord(%line, 1)] = getWords(%line, 2, getWordCount(%line) - 1);

				continue;
			}
		}
	}

	%file.close();
	%file.delete();

	return true;
}

function Sassy::addValue(%this, %value, %defaultValue)
{
	if(%value $= "" || %defaultValue $= "")
	{
		error("Sassy::addValue([value: " @ %value @ "], [defaultValue: " @ %defaultValue @ "]): Incorrect amount of arguments! Aborting..");

		return false;
	}

	if(%this.findvalue(%value))
	{
		error("Sassy::addValue([value: " @ %value @ "]): Value '" @ %value @ "' is already in the database! Aborting..");

		return false;
	}

	if(getWordCount(%value) > 1)
	{
		error("Sassy::addValue([value: " @ %value @ "], [defaultValue: " @ %defaultValue @ "]): Values can't be longer then one word! Aborting..");

		return false;
	}

	%this.valueCount++;

	%this.value[%this.valueCount] = %value;
	%this.defaultValue[%this.valueCount] = %defaultValue;

	for(%a = 1; %a <= %this.dataCount; %a++)
	{
		%this.data[%a].value[%value] = %defaultValue;
	}

	return true;
}

function Sassy::removeValue(%this, %value)
{
	if(%value $= "")
	{
		error("Sassy::removeValue([value: " @ %value @ "]): Incorrect amount of arguments! Aborting..");

		return false;
	}

	if(!%this.findvalue(%value))
	{
		error("Sassy::removeValue([value: " @ %value @ "]): Value '" @ %value @ "' is not found in the database! Aborting..");

		return false;
	}

	for(%a = 0; %a < %this.valueCount; %a++)
	{
		if(%this.value[%a] $= %value)
		{
			%foundValue = true;

			%this.value[%a] = "";
			%this.defaultValue[%a] = "";

			continue;
		}

		if(%foundValue)
		{
			%this.value[%a - 1] = %this.value[%a];
			%this.defaultValue[%a - 1] = %this.defaultValue[%a];

			%this.value[%a] = "";
			%this.defaultValue[%a] = "";
		}
	}

	%this.valueCount--;

	for(%b = 1; %b <= %this.dataCount; %b++)
	{
		%this.data[%b].value[%value] = "";
	}

	return true;
}

function Sassy::findValue(%this, %value)
{
	for(%a = 0; %a <= %this.valueCount; %a++)
	{
		if(%this.value[%a] $= %value)
		{
			return %a;
		}
	}

	return false;
}

function Sassy::addData(%this, %ID)
{
	if(%ID $= "")
	{
		error(%this.getName() @ "::addData([ID: " @ %ID @ "]): Incorrect amount of arguments! Aborting..");

		return false;
	}

	if(%this.getData(%ID))
	{
		error(%this.getName() @ "::addData([ID: " @ %ID @ "]): Data for ID '" @ %ID @ "' is already in the database! Aborting..");

		return false;
	}

	%data = new scriptObject()
	{
		class = SassyData;

		ID = %ID;
		parent = %this;
	};

	%this.dataCount++;

	%this.data[%this.dataCount] = %data;

	return true;
}

function Sassy::removeData(%this, %ID)
{
	if(%ID $= "")
	{
		error(%this.getName() @ "::removeData([ID: " @ %ID @ "]): Incorrect amount of arguments! Aborting..");

		return false;
	}

	if(!%this.getData(%ID))
	{
		error(%this.getName() @ "::removeData([ID: " @ %ID @ "]): Data for ID " @ %ID @ " is not found in the database! Aborting..");

		return false;
	}

	%foundID = false;

	for(%a = 1; %a <= %this.dataCount; %a++)
	{
		if(%this.data[%a].ID == %ID)
		{
			%foundID = true;

			%this.data[%a].delete();

			%this.data[%a] = "";

			continue;
		}

		if(%foundID)
		{
			%this.data[%a - 1] = %this.data[%a];

			%this.data[%a] = "";
		}
	}

	%this.dataCount--;

	return true;
}

function Sassy::getData(%this, %ID)
{
	for(%a = 0; %a <= %this.dataCount; %a++)
	{
		if(%this.data[%a].ID == %ID)
		{
			return %this.data[%a];
		}
	}

	return false;
}

function SassyData::onAdd(%this)
{
	if(%this.ID $= "")
	{
		error("SassyData::onAdd(): ID variable not specified! Deleting data..");

		%this.delete();

		return false;
	}

	if(%this.parent $= "")
	{
		error("SassyData::onAdd(): Parent variable not specified! Deleting data..");

		%this.delete();

		return false;
	}

	SassyGroup.add(%this);

	for(%a = 1; %a <= %this.parent.valueCount; %a++)
	{
		%this.value[%this.parent.value[%a]] = %this.parent.defaultValue[%a];
	}

	return true;
}
