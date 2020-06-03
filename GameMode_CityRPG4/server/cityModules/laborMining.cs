// ============================================================
// Bricks
// ============================================================
datablock fxDTSBrickData(CityRPGOreData)
{
	brickFile = $City::DataPath @ "bricks/4x Cube.blb";
	iconName = $City::DataPath @ "ui/BrickIcons/4x Cube";

	category = "CityRPG";
	subCategory = "Resources";

	uiName = "Ore";

	CityRPGBrickType = $CityBrick_ResourceOre;
	CityRPGBrickAdmin = true;
};

datablock fxDTSBrickData(CityRPGSmallOreData)
{
	brickFile = $City::DataPath @ "bricks/Small Ore.blb";
	iconName = "base/client/ui/brickIcons/2x2";

	category = "CityRPG";
	subCategory = "Resources";

	uiName = "Small Ore";

	CityRPGBrickType = $CityBrick_ResourceOre;
	CityRPGBrickAdmin = true;
};

// ============================================================
// Mining Resource Drop Datablocks
// ============================================================
datablock ShapeBaseImageData(cityOreImage)
{
	shapeFile = "base/data/shapes/snowBall.dts";
	emap = true;

	doColorShift = true;
	colorShiftColor = "0.2 0.2 0.2 1";
	canPickup = false;
};

datablock ItemData(cityOreItem)
{
	category = "Weapon";
	className = "Weapon";
  uiName = "Ore";

	shapeFile = "base/data/shapes/snowBall.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	doColorShift = true;
	colorShiftColor = "0.2 0.2 0.2 1";
	image = cityOreImage;
	candrop = true;
	canPickup = false;
};

// ============================================================
// Brick Functions
// ============================================================
function fxDTSBrick::onCityMine(%this, %client)
{
	%client.onCityMine(%this, "Tin");

	// TODO: Use a set amount of damage instead of a random check
	if(getRandom(1,8) == 1)
	{
		%this.cityOreBreak(%client);
	}
}

function fxDTSBrick::citySpawnOre(%this)
{
	%item = new Item()
	{
		dataBlock = cityOreItem;
		static = 0;
	};

	%item.setVelocity(getRandom(0,2) SPC getRandom(0,2) SPC getRandom(0,2));
	%item.setTransform(%this.getPosition() SPC "0 0 0 0");
	%item.schedulePop(); // TODO: Extend the de-spawn time
	%item.spawnBrick = %this;
}

function fxDTSBrick::cityOreBreak(%this, %client)
{
	for(%i = 0; %i <= getRandom(2,4); %i++)
	{
		%this.citySpawnOre();
	}

	%this.disappear(getRandom(30,60));
	%this.playSound(brickBreakSound);
}

// ============================================================
// Client Functions
// ============================================================
function gameConnection::onCityMine(%client, %brick, %resource)
{
	%progressBar = "";

	for(%i = 0; %i <= getSimTime()/515 % 4; %i++)
	{
		%progressBar = %progressBar @ "-";
	}

	%client.centerPrint("\c6" @ %progressBar @ " Mining " @ %progressBar @ "<br>\c3" @ %resource, 2);
}
