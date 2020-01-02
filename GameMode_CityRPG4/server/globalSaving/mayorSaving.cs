//function serverCmdloadMayor(%client)
//{
//	exec("config/server/CityRPG/Global/Mayor.cs");
//	$City::Mayor::Loaded = 1;
//	messageClient(%client, '', "Loaded.");
//}
//
//function serverCmdloadMayorex(%client)
//{
//	exec("config/server/CityRPG/Global/Mayor.cs");
//	$City::Mayor::Loaded = 0;
//	messageClient(%client, '', "Loaded.");
//}
//
//function serverCmdsaveMayor(%client)
//{
//	export("$City::Mayor::*","config/server/CityRPG/Global/Mayor.cs");
//	messageClient(%client, '', "Saved.");
//}
//
//function serverCmdgetMayor(%client, %id, %dataType)
//{
//	messageClient(%client, '', $City::Mayor::ID[%id, %dataType]);
//}
//
//function serverCmdinputMayor(%client, %id, %dataType, %input)
//{
//	$City::Mayor::ID[%id, %dataType] = %input;
//}

//////////////////////////////////////////////////

function loadMayor()
{
	exec("config/server/CityRPG/Global/Mayor.cs");
	$City::Mayor::Loaded = 1;
}

function saveMayor()
{
	export("$City::Mayor::*","config/server/CityRPG/Global/Mayor.cs");
}

//////////////////////////////////////////////////

function getMayor(%id, %dataType)
{
	echo("******** Returning" @ $City::Mayor::ID[%id, %dataType] SPC "(" @ %id SPC %dataType @ ")");
	return $City::Mayor::ID[%id, %dataType];
}

function inputMayor(%id, %dataType, %input)
{
	$City::Mayor::ID[%id, %dataType] = %input;
}
