datablock fxDTSBrickData(CityRPGTreeData : brickPineTreeData)
{
	category = "CityRPG";
	subCategory = "Resources";

	uiName = "Lumber Tree";

	CityRPGBrickType = $CityBrick_ResourceLumber;
	CityRPGBrickAdmin = true;
};

function getClosestPaintColor(%rgba)
{
	%prevDist = 100000;
	%colorMatch = 0;
	for(%a = 0; %a < 64; %a++)
	{
		%color = getColorIDTable(%a);
		if(vectorDist(%rgba, getWords(%color, 0, 2)) < %prevDist && getWord(%rgba, 3) - getWord(%color, 3) < 0.3 && getWord(%rgba, 3) - getWord(%color, 3) > -0.3)
		{
			%prevDist = vectorDist(%rgba, %color);
			%colorMatch = %a;
		}
	}
	return %colorMatch;
}

function fxDTSBrick::onChop(%this, %client)
{
	if(%this.totalHits > 0 && !%this.isFakeDead())
	{
		%this.totalHits--;

		if(%this.totalHits)
		{
			%client.centerPrint("<just:left>\c6" @ %this.name @ ":\c3" SPC %this.totalHits, 3);
		}
		else
		{
			CityRPGData.getData(%client.bl_id).valueResources = (getWord(CityRPGData.getData(%client.bl_id).valueResources, 0) + (%this.BPH * ResourceSO.tree[%this.id].totalHits)) SPC getWord(CityRPGData.getData(%client.bl_id).valueResources, 1) SPC getWord(CityRPGData.getData(%client.bl_id).valueResources, 2);
			commandToClient(%client, 'centerPrint' , "\c6Lumber obtained, however you killed the tree in the process.", 3);
			%this.fakeKillBrick(getRandom(-10, 10) SPC getRandom(-10, 10) SPC getRandom(0, 10), getRandom(120, 180));
			%seed = getRandom(1, ResourceSO.treeCount);
			%this.id = ResourceSO.tree[%seed].id;
			%this.BPH = ResourceSO.tree[%seed].BPH;
			%this.name = ResourceSO.tree[%seed].name;
			%this.totalHits = ResourceSO.tree[%seed].totalHits;
			%this.color = getClosestPaintColor(ResourceSO.tree[%seed].color);
			%this.setColor(%this.color);
		}

		%client.SetInfo();
	}
}
