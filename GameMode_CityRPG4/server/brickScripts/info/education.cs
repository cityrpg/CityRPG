// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGEducationBrickData : brick2x4FData)
{
	category = "CityRPG";
	subCategory = "Info Bricks";

	uiName = "Education Brick";

	CityRPGBrickType = $CityBrick_Info;
	CityRPGBrickAdmin = true;
	CityRPGBrickPlayerPrivliage = true;
	CityRPGBrickCost = 2500;

	triggerDatablock = CityRPGInputTriggerData;
	triggerSize = "2 4 1";
	trigger = 0;
};

// ============================================================
// Trigger Data
// ============================================================
function CityRPGEducationBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus !$= "")
	{
		if(%triggerStatus == true && %client.stage $= "")
		{
			%level = CityRPGData.getData(%client.bl_id).valueEducation;
			if($CityRPG::EducationStr[%level] !$= "")
			{
				%string = $CityRPG::EducationStr[%level];
			}
			else
			{
				%string = "Level " @ %level @ " Education";
			}

			messageClient(%client, '', "\c6Welcome to the " @ $Pref::Server::City::name @ " College of Education. You currently hold a \c3" @ %string @ "\c6.");

			if(CityRPGData.getData(%client.bl_id).valueStudent > 0) {
				messageClient(%client, '', "\c6You are currently enrolled. Your education will be complete in \c3" @ CityRPGData.getData(%client.bl_id).valueStudent @ "\c6 days.");
			}
			else if(%level == $City::EducationCap) {
				messageClient(%client, '', "\c6Sorry, the department of education is unable to advance you any further.");
				messageClient(%client, '', "\c6Try typing /reincarnate for a new challenge.");
			}
			else if(%level == $City::EducationReincarnateLevel) {
				messageClient(%client, '', "\c6You are already far beyond what the department of education can offer.");
			}
			else {
				messageClient(%client, '', "\c6Type a number in chat:");
				messageClient(%client, '', "\c31 \c6- Enroll for \c3$" @ %client.getCityEnrollCost());

			}
			%client.stage = 0;
		}

		if(%triggerStatus == false && %client.stage !$= "")
		{
			messageClient(%client, '', "\c6Thanks, come again.");

			%client.stage = "";
		}

		return;
	}

	%input = strLwr(%text);

	if(mFloor(%client.stage) == 0)
	{
		if(strReplace(%input, "1", "") !$= %input || strReplace(%input, "one", "") !$= %input)
		{
			%client.cityEnroll();
			return;
		}

		messageClient(%client, '', "\c3" @ %text SPC "\c6is not a valid option!");
		return;
	}
}
