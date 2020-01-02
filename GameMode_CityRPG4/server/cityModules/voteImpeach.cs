function CityMayor_VoteImpeach(%client)
{
	if(($Pref::Server::City::Mayor::ImpeachCost == 0) || ($Pref::Server::City::Mayor::ImpeachCost $= ""))
		$Pref::Server::City::Mayor::ImpeachCost = 500;
	$City::Mayor::ImpeachRequirement = 15;
	if($City::Mayor::String $= "" || $City::Mayor::String $= "None")
		messageClient(%client,'',"\c6That person doesn't exist!");
	else {
		if(!CityMayor_getDataimpeachersDatabase(%client.BL_ID))
		{
			if(CityRPGData.getData(%client.bl_id).valueMoney < $Pref::Server::City::Mayor::ImpeachCost)
			{
				messageClient(%client,'',"You don't have the required money to remove the Mayor!");
				return;
			}

			CityRPGData.getData(%client.bl_id).valueMoney = CityRPGData.getData(%client.bl_id).valueMoney - $Pref::Server::City::Mayor::ImpeachCost;
			$City::Mayor::Impeach++;
			messageAll('', %client.name SPC "\c6has voted to remove the Mayor from office.");
			messageAll('',"\c6Current vote count:\c0" SPC $City::Mayor::Impeach @ "\c6. Needed:\c0" SPC $City::Mayor::ImpeachRequirement);
			CityMayor_impeachersDatabase(%client.BL_ID);

			if($City::Mayor::Impeach >=  $City::Mayor::ImpeachRequirement)
			{
				CityMayor_resetImpeachVotes();
				$Pref::Server::City::Mayor::Active = 0;
				$City::Mayor::Voting = 0;

				CityMayor_resetCandidates();

				$City::Mayor::ID = -1;
				$City::Mayor::String = "";
				messageAll('',"\c6>>\c0THE MAYOR HAS BEEN REMOVED FROM OFFICE!");
			}
		} else {
			messageClient(%client,'',"\c6Chill out, you have already voted to remove the Mayor!");
		}
	}
}

function serverCmdforceImpeach(%client)
{
	%client.cityLog("/forceImpeach");

	if(%client.isAdmin)
	{
		CityMayor_resetImpeachVotes();
		$Pref::Server::City::Mayor::Active = 0;
		$City::Mayor::Voting = 0;
		CityMayor_resetCandidates();
		$City::Mayor::String = "";
		messageAll('',"\c6>>\c0THE MAYOR HAS BEEN REMOVED FROM OFFICE!\c6 Forced by:" SPC %client.name);
	}
}
function CityMayor_resetImpeachVotes()
{
	for(%c = 0; %c < ClientGroup.getCount(); %c++)
	{
		%subClient = ClientGroup.getObject(%c);
		%subClient.impeachVoted = 0;
		$City::Mayor::Impeach = 0;
	}
}

function CityMayor_getDataimpeachersDatabase(%string)
{
	for(%i = 0; %i < 25; %i++)
	{
		if($impeachers[%i] $= %string) {
			return true;
		}
	}
	return false;
}

function CityMayor_impeachersDatabase(%string)
{
	for(%i = 0; %i < 25; %i++)
	{
		if($impeachers[%i] $= "") {
			$impeachers[%i] = %string;
			%i = 26;
		} else if($impeachers[%i] $= %string) {
			%i = 26;
		}
	}
}


function CityMayor_resetimpeachers()
{
	messageClient(%client,'',"All impeachers have been reset!");
	for(%i = 0; %i < 25; %i++)
	{
		$impeachers[%i] = "";
	}
}
