// ============================================================
// Package Data
// ============================================================
package CityRPG_Cash
{
	// Drop Money
	function gameConnection::onDeath(%client, %killerPlayer, %killer, %damageType, %unknownA)
	{
		if(!getWord(CityRPGData.getData(%client.bl_id).valueJailData, 1) && CityRPGData.getData(%client.bl_id).valueMoney && !%client.moneyOnSuicide)
		{
			if($Pref::Server::City::misc::cashdrop == 1)
			{
				%cashval = mFloor(CityRPGData.getData(%client.bl_id).valueMoney);
				%cashcheck = 0;
				if(%cashval > 1000)
				{
					%cashval = 1000;
					%cashcheck = 1;
				}
				%cash = new Item()
				{
					datablock = cashItem;
					canPickup = false;
					value = %cashval;
				};

				%cash.setShapeName("Drop $" @ %cash.value @ " from death");

				%cash.setTransform(setWord(%client.player.getTransform(), 2, getWord(%client.player.getTransform(), 2) + 2));
				%cash.setVelocity(VectorScale(%client.player.getEyeVector(), 10));

				MissionCleanup.add(%cash);
				%cash.setShapeName("$" @ %cash.value);
				if(%cashcheck == 1)
					City.set(%client.bl_id, "money", CityRPGData.getData(%client.bl_id).valueMoney - 1000);
				else
					City.set(%client.bl_id, "money", 0);

				%client.SetInfo();
			}

		}
		parent::onDeath(%client, %killerPlayer, %killer, %damageType, %unknownA);
	}

	// Money Pickup
	function Armor::onCollision(%this, %obj, %col, %thing, %other)
	{
		if(%col.getDatablock().getName() $= "CashItem")
		{
			if(isObject(%obj.client))
			{
				if(isObject(%col))
				{
					if(%obj.client.minigame)
					{
						%col.minigame = %obj.client.minigame;
					}

					if(isObject(%col.dropper))
					{
						$City::Cache::DroppedCash[%col.dropper.bl_id]--;
					}

					City.add(%obj.client.bl_id, "money", %col.value);
					messageClient(%obj.client, '', "\c6You have picked up \c3$" @ %col.value SPC "\c6off the ground.");

					%obj.client.cityLog("Pick up $" @ %col.value);

					%obj.client.SetInfo();
					%col.canPickup = false;
					%col.delete();
				}
				else
				{
					%col.delete();
					MissionCleanup.remove(%col);
				}
			}
		}

		if(isObject(%col))
			parent::onCollision(%this, %obj, %col, %thing, %other);
	}

	function CashItem::onAdd(%this, %item, %b, %c, %d, %e, %f, %g)
	{
		parent::onAdd(%this, %item, %b, %c, %d, %e, %f, %g);
		%item.schedule($Pref::Server::City::moneyDieTime, delete);
	}
};
activatePackage(CityRPG_Cash);

// ============================================================
// Money Datablock
// ============================================================
datablock ItemData(cashItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "base/data/shapes/brickWeapon.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	doColorShift = true;
	colorShiftColor = "0 0.6 0 1";
	image = cashImage;
	candrop = true;
	canPickup = false;
};

datablock ShapeBaseImageData(cashImage)
{
	shapeFile = "base/data/shapes/brickWeapon.dts";
	emap = true;

	doColorShift = true;
	colorShiftColor = cashItem.colorShiftColor;
	canPickup = false;
};
