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
