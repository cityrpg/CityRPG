// Datablocks
if(!isObject(CityRPGLumberjackItem))
{
	datablock projectileData(CityRPGLumberjackProjectile : swordProjectile)
	{
		directDamage		= 15;
		directDamageType	= $DamageType::Sword;
		radiusDamageType	= $DamageType::Sword;

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

	datablock itemData(CityRPGLumberjackItem : CityRPGPickaxeImage)
	{
		uiName = "Lumberjack Axe";
		image = CityRPGLumberjackImage;
		shapeFile = $City::DataPath @ "/shapes/lumberjack.1.dts";

		iconName		= $City::DataPath @ "/ui/ItemIcons/axe";

		// CityRPG Properties
		noSpawn			= true;
	};

	datablock shapeBaseImageData(CityRPGLumberjackImage : swordImage)
	{
		// SpaceCasts
		raycastWeaponRange = 6;
		raycastWeaponTargets = $TypeMasks::All;
		raycastDirectDamage = 0;
		raycastDirectDamageType = $DamageType::Sword;
		raycastExplosionProjectile = swordProjectile;
		raycastExplosionSound = swordHitSound;

		eyeOffset		= "0.7 1.2 -0.7";

		item = CityRPgLumberjackItem;
		shapeFile = $City::DataPath @ "/shapes/lumberjack.1.dts";
		projectile = CityRPGLumberjackProjectile;
		armReady = true;
		melee = true;
	};
}

// Visual Functionality
function CityRPGLumberjackImage::onPreFire(%this, %obj, %slot)
{
	%obj.playThread(2, "armAttack");
}

function CityRPGLumberjackImage::onStopFire(%this, %obj, %slot)
{
	%obj.playThread(2, "root");
}

function CityRPGLumberjackImage::onHitObject(%this, %obj, %slot, %col, %pos, %normal)
{
	if(%col.getClassName() $= "fxDTSBrick" && %col.getDatablock().CityRPGBrickType == $CityBrick_ResourceLumber)
		%col.onChop(%obj.client);

	parent::onHitObject(%this, %obj, %slot, %col, %pos, %normal);
}
