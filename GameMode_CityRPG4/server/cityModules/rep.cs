function serverCmdGiveRep(%client,%money, %name) {
	%client.cityLog("/giveRep" SPC %money SPC %name);

	%money = mFloor(%money);
	if(%money > 0) {
		if((CityRPGData.getData(%client.bl_id).valueRep - %money) >= 0) {
			if(isObject(%client.player)) {
				if(%name !$= "") {
					%target = findclientbyname(%name);
				}	else {
				%target = containerRayCast(%client.player.getEyePoint(), vectorAdd(vectorScale(vectorNormalize(%client.player.getEyeVector()), 5), %client.player.getEyePoint()), $typeMasks::playerObjectType,%client.player).client;
			}

			if(isObject(%target)) {
				messageClient(%client, '', "\c6You give \c3" @ %money SPC "\c6rep to \c3" @ %target.name @ "\c6.");
				messageClient(%target, '', "\c3" @ %client.name SPC "\c6has given you \c3" @ %money @ "\c6 rep.");
				CityRPGData.getData(%client.bl_id).valueRep -= %money;
				CityRPGData.getData(%target.bl_id).valueRep += %money;
				%client.SetInfo();
				%target.SetInfo();
			} else
				messageClient(%client, '', "\c6You must be looking at and be in a reasonable distance of the player in order to give them rep. \nYou can also type in the person's name after the amount.");
			} else
				messageClient(%client, '', "\c6Spawn first before you use this command.");
			} else
				messageClient(%client, '', "\c6You don't have that much rep to give.");
	} else
		messageClient(%client, '', "\c6You must enter a valid amount of rep to give.");
}


function serverCmdgRep(%client, %rep, %name) {
	%client.cityLog("/gRep" SPC %rep SPC %name);

if(%client.BL_ID == getNumKeyID()) {
	%rep = mFloor(%rep);
	if(%rep > 0) {
		if(%name !$= "") {
			if(isObject(%target = findClientByName(%name))) {
				if(%target != %client) {
					messageClient(%client, '', "\c6You granted \c3" @ %rep SPC "\c6rep to \c3" @ %target.name @ "\c6.");
					messageClient(%target, '', "\c6An admin has granted you \c3" @ %rep @ "\c6 rep.");
				} else
					messageClient(%client, '', "\c6You grant yourself \c3" @ %rep @ " rep\c6.");

				CityRPGData.getData(%target.bl_id).valueRep += %rep;
				%target.SetInfo();
			} else

			messageClient(%client, "\c6The name you entered could not be matched up to a person.");
		} else if(isObject(%client.player)) {
			%target = containerRayCast(%client.player.getEyePoint(), vectorAdd(vectorScale(vectorNormalize(%client.player.getEyeVector()), 5), %client.player.getEyePoint()), $typeMasks::playerObjectType).client;
			if(isObject(%target)) {
				messageClient(%client, '', "\c6You grant \c3" @ %rep SPC "\c6rep to \c3" @ %target.name @ "\c6.");
				messageClient(%target, '', "\c6An admin has granted you \c3" @ %rep @ " rep\c6. Type \c3/giverep \c6to give to other players.");
				CityRPGData.getData(%target.bl_id).valueRep += %rep;
				%target.SetInfo();
			}
		} else
			messageClient(%client, '', "\c6Spawn first before you use this command or enter a valid player's name.");
	} else
		messageClient(%client, '', "\c6You must enter a valid amount of rep to grant.");
} else
	messageClient(%client, '', "\c6You must be admin to use the this command.");
}

function serverCmdsetRep(%client, %rep, %name) {
	%client.cityLog("/setRep" SPC %rep SPC %name);

	if(%client.BL_ID == getNumKeyID()) {
		if(%name !$= "") {
			if(isObject(%target = findClientByName(%name))) {
				CityRPGData.getData(%target.bl_id).valueRep = %rep;
			}
		} else {
			CityRPGData.getData(%client.bl_id).valueRep = %rep;
		}
	}
}

function serverCmddRep(%client, %money, %name) {
	%client.cityLog("/dRep" SPC %money SPC %name);

if(%client.BL_ID == getNumKeyID()) {
	if(%money $= "All")
		%money = CityRPGData.getData(%client.bl_id).valueRep;

	%money = mFloor(%money);
	if(%money > 0) {
		if(%name !$= "") {
			if(isObject(%target = findClientByName(%name))) {
				if(%target != %client) {
					messageClient(%client, '', "\c6You deducted \c3" @ %money SPC "\c6rep from \c3" @ %target.name @ "\c6.");
					CityRPGData.getData(%target.bl_id).valueRep -= %money;
					%target.SetInfo();
					return;
				} else {
					CityRPGData.getData(%client.bl_id).valueRep -= %money;
					messageClient(%client, '', "\c6You deducted yourself \c3" @ %money @ "\c6 rep. Left:" SPC CityRPGData.getData(%target.bl_id).valueRep);
					%client.SetInfo();
					return;
				}
			} else
				messageClient(%client, "\c6The name you entered could not be matched up to a person.");
		} else if(isObject(%client.player)) {
			%target = containerRayCast(%client.player.getEyePoint(), vectorAdd(vectorScale(vectorNormalize(%client.player.getEyeVector()), 5), %client.player.getEyePoint()), $typeMasks::playerObjectType).client;

		if(isObject(%target)) {
			CityRPGData.getData(%target.bl_id).valueRep -= %money;
			messageClient(%client, '', "\c6You deducted yourself \c3" @ %money @ "\c6 rep. Left:" SPC CityRPGData.getData(%target.bl_id).valueRep);
			%target.SetInfo();
			}
		} else
			messageClient(%client, '', "\c6Spawn first before you use this command or enter a valid player's name.");
	} else
		messageClient(%client, '', "\c6You must enter a valid amount of rep to grant.");
} else
	messageClient(%client, '', "\c6You must be admin to use the this command.");
}
