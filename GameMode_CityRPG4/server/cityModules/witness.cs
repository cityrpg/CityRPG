function CanSee(%client, %target)
{
%a = %client.player;
%b = %target.player;
%angle = %a.client.getControlCameraFOV();
%direct = vectorNormalize(vectorSub(%b.getPosition(), %a.getPosition()));
%product = vectorDot(%a.getEyeVector(), %direct);

if (%product >= 1 - (%angle / 360) * 2)
	{return true;} // ... %b is within %a's viewcone
else
	{return false;} // ... %b is outside of %a's viewcone
}

package Witness
{
	function City_illegalAttackTest(%atkr, %vctm)
	{
	if(isObject(%atkr) && isObject(%vctm) && %atkr.getClassName() $= "GameConnection" && %vctm.getClassName() $= "GameConnection")
	{
		if(%atkr != %vctm)
		{
			if(City.get(%vctm.bl_id, "bounty") && %atkr.getJobSO().bountyClaim)
				return false;
			else if(!%vctm.getWantedLevel())
				{
				for (%c = 0; %c < ClientGroup.getCount(); %c++)
					{
					%client = ClientGroup.getObject(%c);
					if(canSee(%client, %atkr))
						{
							messageClient(%client,'',"\c6You witnessed a crime! You can report it to the police station for a cash reward!");
							%client.Witness = %attkr;
						}
					}
				messageClient(%client,'',"\c6You murdured an innocent! You will become wanted if any witnesses report your crime");
				return false; //Making illegal attack test return false all the time, this causes dems to not be added if the witness reports
				}
		}
	}

	return false;
	}
function gameConnection::onDeath(%client, %killerPlayer, %killer, %damageType, %unknownA)
{
	if(%client.witness !$= "")
	{
		%client.witness = "";
		messageClient(%client,'',"\c6Because of your death, your evedience as a witness has been destroyed");
		return;
	}
	if(%client.witness $= %killer)
	{
		messageClient(%killer,'',"\c6You killed a witness to a previous murder");
		return;
	}
	parent::onDeath(%client, %killerPlayer, %killer, %damageType, %unknownA);
}
};
activatePackage(Witness);
