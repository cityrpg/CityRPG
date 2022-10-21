// ============================================================
// Datablocks
// ============================================================
datablock AudioProfile(CityRPGBatonDrawSound)
{
	filename    = $City::DataPath @ "sounds/BatonDraw.wav";
	description = AudioClosest3d;
	preload = true;
};

datablock AudioProfile(CityRPGBatonHitSound)
{
	filename    = $City::DataPath @ "sounds/BatonHit.wav";
	description = AudioClosest3d;
	preload = true;
};
datablock ParticleData(CityRPGBatonExplosionParticle)
{

	dragCoefficient      = 2;
	gravityCoefficient   = 1.0;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	spinRandomMin = -90;
	spinRandomMax = 90;
	lifetimeMS           = 500;
	lifetimeVarianceMS   = 300;
	textureName          = "base/data/particles/chunk";
	colors[0]     = "0.7 0.7 0.9 0.9";
	colors[1]     = "0.9 0.9 0.9 0.0";
	sizes[0]      = 0.5;
	sizes[1]      = 0.25;
};

datablock ParticleEmitterData(CityRPGBatonExplosionEmitter)
{
	ejectionPeriodMS = 7;
	periodVarianceMS = 0;
	ejectionVelocity = 8;
	velocityVariance = 1.0;
	ejectionOffset   = 0.0;
	thetaMin         = 0;
	thetaMax         = 60;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "CityRPGBatonExplosionParticle";
	uiName = "CityRPGBaton Hit";
};

datablock ExplosionData(CityRPGBatonExplosion)
{
	lifeTimeMS = 500;
	soundProfile = CityRPGBatonHitSound;
	particleEmitter = CityRPGBatonExplosionEmitter;
	particleDensity = 10;
	particleRadius = 0.2;
	faceViewer     = true;
	explosionScale = "1 1 1";
	shakeCamera = true;
	camShakeFreq = "20.0 22.0 20.0";
	camShakeAmp = "1.0 1.0 1.0";
	camShakeDuration = 0.5;
	camShakeRadius = 10.0;
	lightStartRadius = 3;
	lightEndRadius = 0;
	lightStartColor = "00.0 0.2 0.6";
	lightEndColor = "0 0 0";
};

datablock ProjectileData(CityRPGBatonProjectile)
{
	directDamage        = 35;
	directDamageType  = $DamageType::CityRPGBaton;
	radiusDamageType  = $DamageType::CityRPGBaton;
	explosion           = CityRPGBatonExplosion;
	muzzleVelocity      = 50;
	velInheritFactor    = 1;
	armingDelay         = 0;
	lifetime            = 100;
	fadeDelay           = 70;
	bounceElasticity    = 0;
	bounceFriction      = 0;
	isBallistic         = false;
	gravityMod = 0.0;
	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";
	uiName = "CityRPGBaton Slice";
};

datablock ItemData(CityRPGBatonItem)
{
	shapeFile = $City::DataPath @ "shapes/baton.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;
	uiName = "Baton";
	iconName = $City::DataPath @ "ui/ItemIcons/baton";
	doColorShift = false;
	colorShiftColor = "0.471 0.471 0.471 1.000";
	image = CityRPGBatonImage;
	canDrop = false;
	noSpawn = true;
	canArrest = true;
	showBricks = 1;
	category = "Weapon";
	className = "Weapon";
};

datablock ShapeBaseImageData(CityRPGBatonImage)
{
	raycastWeaponRange = 6;
	raycastWeaponTargets = $TypeMasks::VehicleObjectType | $TypeMasks::PlayerObjectType | $TypeMasks::FxBrickObjectType;
	raycastDirectDamage = 25;
	raycastDirectDamageType = $DamageType::CityRPGBaton;
	raycastExplosionProjectile = CityRPGBatonProjectile;
	raycastExplosionSound = CityRPGBatonHitSound;
	shapeFile = $City::DataPath @ "shapes/baton.dts";
	emap = true;
	mountPoint = 0;
	correctMuzzleVector = false;
	offset = "0 0.4 0";
	eyeOffset = "0 0 0";
	className = "WeaponImage";
	item = CityRPGBatonItem;
	ammo = " ";
	projectile = CityRPGBatonProjectile;
	projectileType = Projectile;
	melee = true;
	doRetraction = false;
	armReady = true;
	doColorShift = false;
	colorShiftColor = "0.471 0.471 0.471 1.000";
	item = CityRPGBatonItem;
	projectile = CityRPGBatonProjectile;
	colorShiftColor = "0.5 0.5 0.5 1";
	showBricks = 0;
	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 0.5;
	stateTransitionOnTimeout[0]      = "Ready";
	stateSound[0]                    = CityRPGBatonDrawSound;
	stateName[1]                     = "Ready";
	stateTransitionOnTriggerDown[1]  = "PreFire";
	stateAllowImageChange[1]         = true;
	stateName[2]			= "PreFire";
	stateScript[2]                  = "onPreFire";
	stateAllowImageChange[2]        = false;
	stateTimeoutValue[2]            = 0.1;
	stateTransitionOnTimeout[2]     = "Fire";
	stateName[3]                    = "Fire";
	stateTransitionOnTimeout[3]     = "CheckFire";
	stateTimeoutValue[3]            = 0.2;
	stateFire[3]                    = true;
	stateAllowImageChange[3]        = false;
	stateSequence[3]                = "Fire";
	stateScript[3]                  = "onFire";
	stateWaitForTimeout[3]		= true;
	stateName[4]			= "CheckFire";
	stateTransitionOnTriggerUp[4]	= "StopFire";
	stateTransitionOnTriggerDown[4]	= "Fire";
	stateName[5]                    = "StopFire";
	stateTransitionOnTimeout[5]     = "Ready";
	stateTimeoutValue[5]            = 0.2;
	stateAllowImageChange[5]        = false;
	stateWaitForTimeout[5]		= true;
	stateSequence[5]                = "StopFire";
	stateScript[5]                  = "onStopFire";
};

// ============================================================
// Functions
// ============================================================

// Visual Functionality
function CityRPGBatonImage::onPreFire(%this, %obj, %slot)
{
	%obj.playThread(2, "armAttack");
}

function CityRPGBatonImage::onStopFire(%this, %obj, %slot)
{
	%obj.playThread(2, "root");
}

function CityRPGBatonImage::onCityPlayerHit(%this, %obj, %slot, %col, %pos, %normal)
{
	// Generic baton check event.
}

function CityRPGBatonImage::onHitObject(%this, %obj, %slot, %col, %pos, %normal)
{
	if(%col.getClassName() $= "Player")
	{
		%client = %obj.client;
		if((%col.getType() & $typeMasks::playerObjectType) && isObject(%col.client))
		{
			if(%col.client.getWantedLevel())
			{
				if(%col.getDatablock().maxDamage - (%col.getDamageLevel() + 25) < %this.raycastDirectDamage)
				{
					%col.setDamageLevel(%this.raycastDirectDamage + 1);
					%col.client.arrest(%client);
				}
			}
			else if(CityRPGBatonImage::onCityPlayerHit(%this, %obj, %slot, %col, %pos, %normal))
			{
				// Generic baton event for external add-ons. If this returns true, further checks will not occur.
			}
			else if(City.get(%col.client.bl_id, "bounty") > 0)
				commandToClient(%client, 'CenterPrint', $c_p @ %col.client.name SPC "\c6is not wanted alive.", 3);
			else
				%doNoEvil = true;
		}
	}

	if(%doNoEvil)
	{
		%this.raycastDirectDamage = 10;
	}

	parent::onHitObject(%this, %obj, %slot, %col, %pos, %normal);

	if(%doNoEvil)
	{
		%this.raycastDirectDamage = 25;
	}
}

function CityRPGBatonItem::onPickup(%this, %item, %obj)
{
	%item.delete();

	for(%a = 0; %a < %obj.getDatablock().maxTools; %a++)
	{
		if(!isObject(%obj.tool[%a]) || %obj.tool[%a].getName() !$= "CityRPGLBItem")
			if(%freeSpot $= "" && %obj.tool[%a] $= "")
			{
				%freeSpot = %a;
			}
			else
				%alreadyOwns = true;
	}

	if(%freeSpot !$= "" && !%alreadyOwns)
	{
		%obj.tool[%freeSpot] = CityRPGLBItem.getID();
		messageClient(%obj.client, 'MsgItemPickup', "", %freeSpot, %obj.tool[%freeSpot]);
	}
}
