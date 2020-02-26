if(!isObject(CityRPGPickaxeItem))
{
	AddDamageType("Pickaxe",   "<bitmap:" @ $City::DataPath @ 'ui/ci/pickaxe> %1',    "%2 <bitmap:" @ $City::DataPath @ "ui/ci/pickaxe> %1", 0.5, 1);

	// Pickaxe Datablocks
	datablock ProjectileData(CityRPGPickaxeProjectile)
	{
		directDamage		= 15;
		directDamageType	= $DamageType::Pickaxe;
		radiusDamageType	= $DamageType::Pickaxe;

		muzzleVelocity		= 50;
		velInheritFactor	= 1;

		armingDelay			= 0;
		lifetime			= 100;
		fadeDelay			= 70;
		bounceElasticity	= 0;
		bounceFriction		= 0;
		isBallistic			= false;
		gravityMod 			= 0.0;

		hasLight			= false;
		lightRadius			= 3.0;
		lightColor			= "0 0 0.5";
	};

	datablock ItemData(CityRPGPickaxeItem)
	{
		category		= "Weapon";
		className		= "Weapon";

		shapeFile		= $City::DataPath @ "shapes/pickaxe.2.dts";
		mass			= 1;
		density 		= 0.2;
		elasticity		= 0.2;
		friction		= 0.6;
		emap			= true;

		uiName			= "Pickaxe";
		iconName		= $City::DataPath @ "ui/ItemIcons/pickaxe";
		doColorShift	= false;

		image			= CityRPGPickaxeImage;
		canDrop			= true;


		// CityRPG Properties
		noSpawn			= true;
	};

	datablock ShapeBaseImageData(CityRPGPickaxeImage)
	{
		// SpaceCasts
		raycastWeaponRange = 6;
		raycastWeaponTargets = $TypeMasks::All;
		raycastDirectDamage = 0;
		raycastDirectDamageType = $DamageType::Pickaxe;
		raycastExplosionProjectile = hammerProjectile;
		raycastExplosionSound = hammerHitSound;

		shapeFile		= $City::DataPath @ "shapes/pickaxe.2.dts";
		emap			= true;
		mountPoint		= 0;
		eyeOffset		= "0.7 1.2 -0.9";
		offset			= "0 0 0";
		correctMuzzleVector = false;
		className		= "WeaponImage";

		item			= CityRPGPickaxeItem;
		ammo			= " ";
		projectile		= CityRPGPickaxeProjectile;
		projectileType	= Projectile;

		melee			= true;
		doRetraction	= false;
		armReady		= true;

		doColorShift	= true;
		colorShiftColor = ".54 .27 .07 1";

		stateName[0]					= "Activate";
		stateTimeoutValue[0]			= 0.5;
		stateTransitionOnTimeout[0]		= "Ready";

		stateName[1]					= "Ready";
		stateTransitionOnTriggerDown[1]	= "PreFire";
		stateAllowImageChange[1]		= true;

		stateName[2]					= "PreFire";
		stateScript[2]					= "onPreFire";
		stateAllowImageChange[2]		= false;
		stateTimeoutValue[2]			= 0.1;
		stateTransitionOnTimeout[2]		= "Fire";

		stateName[3]					= "Fire";
		stateTransitionOnTimeout[3]		= "CheckFire";
		stateTimeoutValue[3]			= 0.5;
		stateFire[3]					= true;
		stateAllowImageChange[3]		= false;
		stateSequence[3]				= "Fire";
		stateScript[3]					= "onFire";
		stateWaitForTimeout[3]			= true;

		stateName[4]					= "CheckFire";
		stateTransitionOnTriggerUp[4]	= "StopFire";
		stateTransitionOnTriggerDown[4]	= "Fire";

		stateName[5]					= "StopFire";
		stateTransitionOnTimeout[5]		= "Ready";
		stateTimeoutValue[5]			= 0.2;
		stateAllowImageChange[5]		= false;
		stateWaitForTimeout[5]			= true;
		stateSequence[5]				= "StopFire";
		stateScript[5]					= "onStopFire";
	};
}

// Visual Functionality
function CityRPGPickaxeImage::onPreFire(%this, %obj, %slot)
{
	%obj.playthread(2, armAttack);
}

function CityRPGPickaxeImage::onStopFire(%this, %obj, %slot)
{
	%obj.playthread(2, root);
}

function CityRPGPickaxeImage::onHitObject(%this, %obj, %slot, %col, %pos, %normal)
{
	if(%col.getClassName() $= "fxDTSBrick" && %col.getDatablock().CityRPGBrickType == $CityBrick_ResourceOre)
		%col.onMine(%obj.client);

	parent::onHitObject(%this, %obj, %slot, %col, %pos, %normal);
}
