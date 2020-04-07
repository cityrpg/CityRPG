// ============================================================
// Brick Data
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

// ============================================================
// Events
// ============================================================
function fxDTSBrick::adjustColorOnOreContent(%this, %case)
{
	if(isObject(%this))
	{
		if(%this.getDatablock().uiName $= "Ore" || %this.getDatablock().uiName $= "Small Ore")
		{
			if(%case == 0)
			{
				%this.schedule(getRandom(45000, 90000), "adjustColorOnOreContent", 1);
				%this.color = getClosestPaintColor("0 0 0 1");
				%this.setColor(%this.color);
			}

			if(%case == 1)
			{
				%seed = getRandom(1, getRandom(1, ResourceSO.mineralCount));
				echo(%seed);
				%this.id = ResourceSO.mineral[%seed].id;
				%this.name = ResourceSO.mineral[%seed].name;
				%this.totalHits = ResourceSO.mineral[%seed].totalHits;
				%this.BPH = ResourceSO.mineral[%seed].BPH;
				%this.color = getClosestPaintColor(ResourceSO.mineral[%seed].color);
				%this.setColor(%this.color);
			}
		}
	}
}

function fxDTSBrick::onMine(%this, %client)
{
	if(%this.totalHits == 1)
	{
		%this.totalHits--;
		%this.adjustColorOnOreContent(0);
		CityRPGData.getData(%client.bl_id).valueResources = getWord(CityRPGData.getData(%client.bl_id).valueResources, 0) SPC (getWord(CityRPGData.getData(%client.bl_id).valueResources, 1) + (%this.BPH * ResourceSO.mineral[%this.id].totalHits)) SPC getWord(CityRPGData.getData(%client.bl_id).valueResources, 2);
		%client.SetInfo();

		messageClient(%client, '', "\c6Mined \c3" @ %this.name @ "\c6.");
		return;
	}

	if(%this.totalHits > 0)
	{
		%this.totalHits--;

		%col1 = "\c6";
		%col2 = "\c3";
		%client.centerPrint("<just:left>" @ $CityRPG::MainFont @ %col1 @ %this.name @ %col2 @ ":" SPC %this.totalHits, 3);

		if(getRandom(1, 100) > 100 - (CityRPGData.getData(%client.bl_id).valueEducation / 2))
			%gemstone = true;

		if(%gemstone)
		{
			%value = getRandom(5, 50);
			messageClient(%client, '', "\c6Extracted a gem from the rock worth \c3$" @ %value @ "\c6.");
			CityRPGData.getData(%client.bl_id).valueMoney += %value;
		}
	}

	if(%this.totalHits == 0)
		%client.centerPrint("<just:left>" @ $CityRPG::MainFont @ "\c6Resource empty", 3);
}
