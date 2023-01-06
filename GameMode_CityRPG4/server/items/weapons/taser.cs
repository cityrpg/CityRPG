// ============================================================
// Datablocks
// ============================================================
if(!isObject(taserItem))
{
	// Used tagged string function so we can follow the data path var
	AddDamageType("Taser", addTaggedString("<bitmap:" @ $City::DataPath @ "ui/ci/taser> %1"), addTaggedString("%2 <bitmap:" @ $City::DataPath @ "ui/ci/taser> %1"), 0.5, 1);

	datablock AudioProfile(taserExplosionSound)
	{
		filename	 = $City::DataPath @ "sounds/radioWaveExplosion.wav";
		description = AudioClosest3d;
		preload = true;
	};

	datablock ParticleData(taserDischargeParticle)
	{
		dragCoefficient			= 8;
		gravityCoefficient		= 0;
		inheritedVelFactor		= 0.2;
		constantAcceleration	= 0.0;
		lifetimeMS			  	= 500;
		lifetimeVarianceMS		= 100;
		textureName			 	= "Add-ons/Projectile_Radio_wave/bolt";
		spinSpeed				= 3380.0;
		spinRandomMin			= -50.0;
		spinRandomMax			= 50.0;
		colors[0]	 	= "1 1 0.0 0.6";
		colors[1]	 	= "1 1 1 0";
		sizes[0]		= 0.51;
		sizes[1]		= 0.26;

		useInvAlpha = true;
	};
	datablock ParticleEmitterData(taserDischargeEmitter)
	{
		ejectionPeriodMS	= 1;
		periodVarianceMS	= 0;
		ejectionVelocity	= 5;
		velocityVariance	= 0.6;
		ejectionOffset		= 0.8;
		thetaMin			= 0;
		thetaMax			= 0;
		phiReferenceVel		= 0;
		phiVariance			= 30;
		overrideAdvance		= false;
		particles			= "taserDischargeParticle";
	};

	datablock ParticleData(taserInduceParticle)
	{
		dragCoefficient		= 1;
		gravityCoefficient	= 0;
		inheritedVelFactor	= 0.2;
		constantAcceleration = 0.0;
		lifetimeMS			  = 700;
		lifetimeVarianceMS	= 400;
		textureName			 = "Add-ons/Projectile_Radio_wave/bolt";
		spinSpeed		= 10.0;
		spinRandomMin		= -50.0;
		spinRandomMax		= 50.0;
		colors[0]	  = "0.3 0.6 0.8 0.4";
		colors[1]	  = "1 1 0 0.1";
		colors[2]	  = "1 1 1 0.0";
		sizes[0]		= 0.15;
		sizes[1]		= 0.35;
		sizes[1]		= 0.45;

		useInvAlpha = true;
	};

	datablock ParticleEmitterData(taserInduceEmitter)
	{
		ejectionPeriodMS = 6;
		periodVarianceMS = 0;
		ejectionVelocity = -2.5;
		velocityVariance = 1.0;
		ejectionOffset	= 1.0;
		thetaMin			= 80;
		thetaMax			= 100;
		phiReferenceVel  = 360;
		phiVariance		= 360;
		overrideAdvance = false;
		particles = "taserInduceParticle";
	};

	datablock ProjectileData(taserProjectile)
	{

		directDamage		  = 0;
		directDamageType	 = $DamageType::Taser;
		radiusDamageType	 = $DamageType::Taser;

		brickExplosionRadius = 0;
		brickExplosionImpact = true;
		brickExplosionForce  = 10;
		brickExplosionMaxVolume = 1;
		brickExplosionMaxVolumeFloating = 2;

		impactImpulse		= 0;
		verticalImpulse		= 0;
		explosion			= radioWaveExplosion;
		particleEmitter		= taserDischargeEmitter;

		muzzleVelocity		= 120;
		velInheritFactor	= 1;

		armingDelay			= 00;
		lifetime			= 100;
		fadeDelay			= 800;
		bounceElasticity	= 0.5;
		bounceFriction		= 0.20;
		isBallistic			= false;
		gravityMod = 0.0;

		hasLight	 = true;
		lightRadius = 1.0;
		lightColor  = "1.0 1.0 0.5";

		uiName = "Taser Discharge";
	};

	datablock ItemData(taserItem)
	{
		category = "Weapon";
		className = "Weapon";

		shapeFile = $City::DataPath @ "shapes/taser.1.dts";
		rotate = false;
		mass = 1;
		density = 0.2;
		elasticity = 0.2;
		friction = 0.6;
		emap = true;


		uiName = "Taser";
		iconName = $City::DataPath @ "ui/ItemIcons/taser";
		doColorShift = false;
		colorShiftColor = "0.25 0.25 0.25 1.000";

		image = taserImage;
		canDrop = true;

		// CityRPG Properties
		canArrest = true;
	};

	datablock ShapeBaseImageData(taserImage)
	{
		shapeFile = $City::DataPath @ "shapes/taser.1.dts";
		emap = true;

		mountPoint = 0;
		offset = "0 0 0";
		eyeOffset = 0;
		rotation = eulerToMatrix( "0 0 0" );

		correctMuzzleVector = true;

		className = "WeaponImage";

		item = BowItem;
		ammo = " ";
		projectile			= taserProjectile;
		projectileType		= Projectile;

		//casing				= taserShellDebris;
		shellExitDir		= "1.0 -1.3 1.0";
		shellExitOffset		= "0 0 0";
		shellExitVariance	= 0.0;
		shellVelocity		= 15.0;

		melee = false;
		armReady = true;

		doColorShift		= true;
		colorShiftColor 	= taserItem.colorShiftColor;

		stateName[0]					= "Activate";
		stateTimeoutValue[0]			= 0.10;
		stateTransitionOnTimeout[0]		= "Ready";
		stateSound[0]					= weaponSwitchSound;

		stateName[1]					= "Ready";
		stateTransitionOnTriggerDown[1] = "Fire";
		stateAllowImageChange[1]		= true;
		stateSequence[1]				= "Ready";

		stateName[2]					= "Fire";
		stateTransitionOnTimeout[2]	 	= "Recharge";
		stateTimeoutValue[2]			= 0.10;
		stateFire[2]					= true;
		stateAllowImageChange[2]		= false;
		stateSequence[2]				= "Fire";
		stateScript[2]					= "onFire";
		stateWaitForTimeout[2]			= true;
		stateEmitter[2]					= taserDischargeEmitter;
		stateEmitterTime[2]				= 0.05;
		stateEmitterNode[2]				= "muzzleNode";
		stateSound[2]					= TaserExplosionSound;
		stateEjectShell[2]				= true;

		stateName[3]					= "Recharge";
		stateEmitterTime[3]				= 0.50;
		stateEmitter[3]					= taserInduceEmitter;
		stateEmitterNode[3]				= "muzzleNode";
		stateTimeoutValue[3]			= 0.50;
		stateTransitionOnTimeout[3]	 	= "Reload";

		stateName[4]					= "Reload";
		stateSequence[4]				= "Reload";
		stateTransitionOnTriggerUp[4]	= "Ready";
		stateSequence[4]				= "Ready";

	};
}

// ============================================================
// Functions
// ============================================================

function taserProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal)
{
	if((%col.getType() & $typeMasks::playerObjectType) && isObject(%col.client))
	{
		%col.setVelocity(VectorScale(getRandom(0, 0.250) SPC getRandom(0, 0.250) SPC "1", 10));
		tumble(%col);

		if(City_illegalAttackTest(%obj.client, %col.client))
		{
			commandToClient(%obj.client, 'centerPrint', "\c6You have committed a crime. [" @ $c_p @ "Tasing Innocents\c6]", 3);
			City_AddDemerits(%obj.client.bl_id, $CityRPG::demerits::tasingBros);
		}
	}
}

package CityRPG_TaserPackage
{
	function Armor::damage(%this, %obj, %src, %unk, %dmg, %type)
	{
		// Taser Abuse Preventitive Measures
		if(!(isObject(%src) && %src.getDatablock().getName() $= "deathVehicle"))
		{
			parent::damage(%this, %obj, %src, %unk, %dmg, %type);
		}
	}
};
activatePackage(CityRPG_TaserPackage);
