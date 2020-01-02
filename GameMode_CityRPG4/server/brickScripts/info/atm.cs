// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGATMBrickData : brick2x4FData)
{
	category = "CityRPG";
	subCategory = "Info Bricks";

	uiName = "ATM Brick";

	CityRPGBrickType = $CityBrick_Info;
	CityRPGBrickAdmin = true;

	triggerDatablock = CityRPGInputTriggerData;
	triggerSize = "2 4 1";
	trigger = 0;
};

// ============================================================
// Trigger Data
// ============================================================
function resetAccessableATM(%client)
{
	%client.AccessableATM = 0;
}

function CityRPGATMBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus !$= "")
	{
		if(%triggerStatus == true && %client.stage $= "")
		{
			messageClient(%client, '', "\c3ATM");
			if(CityRPGData.getData(%client.bl_id).valueBank > 0)
			{
				messageClient(%client, '', "\c6You have \c3$" @ CityRPGData.getData(%client.bl_id).valueBank SPC "\c6in your account.");
			}

			messageClient(%client, '', "\c6Type a number in chat:");


			messageClient(%client, '', "\c31 \c6- Withdraw money.");
			//messageClient(%client, '', "\c32 \c6- Hack ATM.");

			%client.stage = 0;
		}

		if(%triggerStatus == false && %client.stage !$= "")
		{
			messageClient(%client, '', "\c6Logging out..");

			%client.stage = "";
		}

		return;
	}

	%input = strLwr(%text);

	if(mFloor(%client.stage) == 0)
	{
		if(strReplace(%input, "1", "") !$= %input || strReplace(%input, "one", "") !$= %input)
		{
			%client.stage = 1.1;

			messageClient(%client, '', "\c6Withdraw amount:");

			return;
		}

		//if(strReplace(%input, "2", "") !$= %input || strReplace(%input, "two", "") !$= %input)
		//{
		//        if(!(ClientGroup.getCount() > 3))
		//        {
		//            messageClient(%client,'',"\c6The server must have 4 players to beable to do this action.");
		//            return;
		//        }
		//	if(%client.AccessableATM == 1)
		//        {
		//            messageClient(%client,'',"\c6You need to wait a while before hacking again. 7sec from the last.");
		//            return;
		//        } else {
		//            if(CityRPGData.getData(%client.bl_id).valueEducation >= 3)
		//	    {
		//		    %stealchance = getRandom(1,2);
		//		    %caughtchance = getRandom(1,4);
		//		    %lockoutchance = getRandom(1,3);
		//		    %beencaught = 1;
		//                if(%stealchance == 1)
		//		    {
		//			    if(%lockoutchance != 1)
		//			    {
		//						%client.cityLog("ATM hack for $" @ %stolen);
		//				    %stolen = getRandom($ATM::Min,$ATM::Max);
		//				    messageClient(%client,'',"\c6You managed to steal \c3$" @ %stolen @ "\c6 from the ATM.");
		//				    CityRPGData.getData(%client.bl_id).valueMoney += %stolen;
		//                        %client.AccessableATM = 1;
		//                        CityRPGData.getData(%client.bl_id).valueDemerits += $ATM::Demerits;
		//                        schedule(7000,0,"resetAccessableATM",%client);
		//			        if(%caughtchance != 1)
		//				    {
		//					    messageAll('',"\c3" @ %client.name @ "\c6 has been caught hacking an ATM!");
		//					    CityRPGData.getData(%client.bl_id).valueDemerits += $ATM::Demerits;
		//				    }
		//			    }
		//			    else
		//			    {
		//				    %this.hackable = 0;
		//				    messageClient(%client,'',"\c6Your attempt failed, and you have been locked out of the machine.");

		//			    }
		//                } else {
		//                    messageClient(%client,'',"\c6Failed to hack.");
		//                }
		//	    }
		//	    else
		//	    {
		//		    messageClient(%client,'',"\c6Your education level must be \c3" @ $Pref::Server::City::hack::education @ "\c6.");
		//	    }
		//	    return;
		//    }
		//  }
		messageClient(%client, '', "\c3" @ %text SPC "\c6is not a valid option!");

		return;
	}

	if(mFloor(%client.stage) == 1)
	{
		if(%client.stage == 1.1)
		{
			if(mFloor(%input) < 1)
			{
				messageClient(%client, '', "\c6Error.");

				return;
			}

			if(CityRPGData.getData(%client.bl_id).valueBank - mFloor(%input) < 0)
			{
				if(CityRPGData.getData(%client.bl_id).valueATM < 1)
				{
					messageClient(%client, '', "\c6Insufficient funds.");

					%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, (isObject(%client.player) ? %client.player : 0));

					return;
				}

				%input = CityRPGData.getData(%client.bl_id).valueATM;
			}

			messageClient(%client, '', "\c6You have withdrawn \c3$" @ mFloor(%input) @ "\c6.");

			%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, (isObject(%client.player) ? %client.player : 0));

			CityRPGData.getData(%client.bl_id).valueBank -= mFloor(%input);
			CityRPGData.getData(%client.bl_id).valueMoney += mFloor(%input);

			%client.SetInfo();
		}
		return;
	}
}
