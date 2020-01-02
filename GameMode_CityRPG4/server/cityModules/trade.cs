function serverCmdTrade(%client, %arg1, %arg2, %arg3)
{
	%client.cityLog("/trade" SPC %arg1 SPC %arg2 SPC %arg3);

	%arg3 = %arg3 * %arg2;
	if(%arg1 $= "Lumber")
	{
		if(%client.tradeID > 0)
		{
			messageClient(%client, '', "\c6You are currently in a trade. Type \c3/trade clear");
			return;
		}
		if(!(%arg2 $= "") && !(%arg3 $= ""))
		{
			%target = containerRayCast(%client.player.getEyePoint(), vectorAdd(vectorScale(vectorNormalize(%client.player.getEyeVector()), 5), %client.player.getEyePoint()), $typeMasks::playerObjectType,%client.player).client;
			if(isObject(%target))
			{
				if(getWord(CityRPGData.getData(%client.bl_id).valueResources, 0) >= %arg2)
				{
					%getTradeID = getRandom(1,100);
					messageClient(%client, '', "\c3.Trade.");
					messageClient(%target, '', "\c3.Trade.");
					messageClient(%client, '', "\c6You have initiated a trade with\c3" SPC %target.name SPC "\c6. Selling him/her\c3" SPC %arg2 SPC "Lumber \c6for \c3$" @ %arg3 @"\c6.");
					messageClient(%target, '', "\c3" @ %client.name SPC "\c6has initiated a trade with you. Selling you\c3" SPC %arg2 SPC "Lumber \c6for \c3$" @ %arg3 @ "\c6.");
					messageClient(%target, '', "\c6Would you like to accept this trade? \c3/yestradelumber \c6or \c3/notradelumber\c6.");
					%client.tradeID = %getTradeID;
					%target.tradeID = %getTradeID;
					%client.asking = %arg3;
					%client.has = %arg2;
				} else {
				messageClient(%client, '', "\c3.Trade.");
				messageClient(%client, '', "\c6You don't have\c3" SPC %arg2 SPC "Lumber\c6.");
			}
		} else {
				messageClient(%client, '', "\c3.Trade.");
				messageClient(%client, '', "\c6The player must be infront of you to trade him.");
			}
		} else {
			messageClient(%client, '', "\c3.Trade.");
			messageClient(%client, '', "\c6-Fill out the command correctly:");
			messageClient(%client, '', "\c6-/trade lumber [\c3amount\c6] [\c3cost each\c6]");
			messageClient(%client, '', "\c6-/trade stocks [\c3amount\c6] [\c3cost each\c6]");
			messageClient(%client, '', "\c6-Ex: /trade lumber 10 50");
		}
	} else if(%arg1 $= "Clear") {
			clearTradeByTradeID(%client);
	} else if(%arg1 $= "Stocks") {
		tradeStock(%client, %arg1, %arg2, %arg3);
	} else {
		messageClient(%client, '', "\c3.Trade.");
		messageClient(%client, '', "\c6-Options are:");
		messageClient(%client, '', "\c6-/trade lumber [\c3amount\c6] [\c3cost each\c6]");
		messageClient(%client, '', "\c6-/trade stocks [\c3amount\c6] [\c3cost each\c6]");

		messageClient(%client, '', "\c6-/trade clear");
	}
}

function serverCmdyestradelumber(%client)
{
	%client.cityLog("/yestradelumber");

	%target = containerRayCast(%client.player.getEyePoint(), vectorAdd(vectorScale(vectorNormalize(%client.player.getEyeVector()), 5), %client.player.getEyePoint()), $typeMasks::playerObjectType,%client.player).client;
	if(isObject(%target))
	{
		if(%target.tradeID > 0)
		{
			if(%target.tradeID == %client.tradeID)
			{
				if(CityRPGData.getData(%client.bl_id).valueMoney >= %target.asking)
				{
					messageClient(%client, '', "\c6You have bought\c3" SPC %target.has SPC "Lumber \c6for \c3$" @ %target.asking @ "\c6.");
					messageClient(%target, '', "\c6You have sold\c3" SPC %target.has SPC "Lumber \c6for \c3$" @ %target.asking @ "\c6.");
					CityRPGData.getData(%target.bl_id).valueResources = (getWord(CityRPGData.getData(%target.bl_id).valueResources, 0) - %target.has) SPC getWord(CityRPGData.getData(%target.bl_id).valueResources, 1);
					CityRPGData.getData(%client.bl_id).valueResources = (getWord(CityRPGData.getData(%client.bl_id).valueResources, 0) + %target.has) SPC getWord(CityRPGData.getData(%client.bl_id).valueResources, 1);
					CityRPGData.getData(%target.bl_id).valueMoney += %target.asking;
					CityRPGData.getData(%client.bl_id).valueMoney -= %target.asking;
				} else {
					messageClient(%client, '', "You don't have enought money to do this transaction.");
					messageClient(%target, '', %client.name SPC "don't have enought money to do this transaction.");
				}
				clearTrade(%client, %target);
			}
		} else {
			messageClient(%client, '', "This player isn't trading");
		}
	} else {
		messageClient(%client, '', "\c3.Trade.");
		messageClient(%client, '', "\c6The player must be infront of you to trade him.");
	}
}

function serverCmdnotradelumber(%client)
{
	%client.cityLog("/notradelumber");
	clearTrade(%client, %target);
}

function clearTrade(%client, %target)
{
	%target.asking = 0;
	%target.has = 0;
	%client.asking = 0;
	%client.has = 0;
	%client.tradeID = 0;
	%target.tradeID = 0;
	messageClient(%client, '', "\c6Your trade with\c3" SPC %target.name SPC "\c6has ended.");
	messageClient(%target, '', "\c6Your trade with\c3" SPC %client.name SPC "\c6has ended.");
	%client.setInfo();
	%target.setInfo();
}

function clearTradeByTradeID(%client)
{
	%before = %client.tradeID;
	%client.asking = 0;
	%client.has = 0;
	%client.tradeID = 0;

	if(%before == 0)
			return;

	for(%c = 0; %c < ClientGroup.getCount(); %c++)
	{
		%subClient = ClientGroup.getObject(%c);
		if(%subClient.tradeID == %before)
		{
			if(!(%subClient.name $= %client.name))
			{
				%subClient.asking = 0;
				%subClient.has = 0;
				%subClient.tradeID = 0;
				messageClient(%client, '', "\c6Your trade with\c3" SPC %subClient.name SPC "\c6has ended.");
				messageClient(%subClient, '', "\c6Your trade with\c3" SPC %client.name SPC "\c6has ended.");
			}
		}
	}
	%client.setInfo();
	%target.setInfo();
}
