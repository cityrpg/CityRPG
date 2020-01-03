datablock AudioProfile(knifeDrawSound)
{
	filename    = "base/data/sound/weaponSwitch.wav";
	description = AudioClosest3d;
	preload = true;
};
datablock AudioProfile(knifeHitSound)
{
	filename    = "Add-Ons/Weapon_Sword/swordHit.wav";
	description = AudioClosest3d;
	preload = true;
};


//effects
datablock ParticleData(knifeExplosionParticle)
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

datablock ParticleEmitterData(knifeExplosionEmitter)
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
	particles = "knifeExplosionParticle";

	uiName = "knife Hit";
};

datablock ExplosionData(knifeExplosion)
{
	//explosionShape = "";
	lifeTimeMS = 500;

	soundProfile = knifeHitSound;

	particleEmitter = knifeExplosionEmitter;
	particleDensity = 10;
	particleRadius = 0.2;

	faceViewer     = true;
	explosionScale = "1 1 1";

	shakeCamera = true;
	camShakeFreq = "0 0 0";
	camShakeAmp = "0 0 0";
	camShakeDuration = 0;
	camShakeRadius = 0;

	// Dynamic light
	lightStartRadius = 3;
	lightEndRadius = 0;
	lightStartColor = "00.0 0.2 0.6";
	lightEndColor = "0 0 0";
};


//projectile
AddDamageType("knife",   '<bitmap:add-ons/Weapon_Sword/CI_sword> %1',    '%2 <bitmap:add-ons/Weapon_Sword/CI_sword> %1',0.75,1);
datablock ProjectileData(knifeProjectile)
{
	directDamage        = 8;
	directDamageType  = $DamageType::Sword;
	radiusDamageType  = $DamageType::Sword;
	explosion           = knifeExplosion;
	//particleEmitter     = as;

	muzzleVelocity      = 50;
	velInheritFactor    = 1;

	armingDelay         = 0;
	lifetime            = 100;
	fadeDelay           = 70;
	bounceElasticity    = 0;
	bounceFriction      = 0;
	isBallistic         = false;
	gravityMod          = 0.0;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";

	uiName = "Sword Slice";
};


//////////
// item //
//////////
datablock ItemData(knifeItem)
{
	category = "Weapon";  // Mission editor category
	className = "Weapon"; // For inventory system

	// Basic Item Properties
	shapeFile = $City::DataPath @ "/shapes/knife2.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	//gui stuff
	uiName = "Knife";
	iconName = $City::DataPath @ "/shapes/knifeicon";
	doColorShift = true;
	colorShiftColor = "0.471 0.471 0.471 1.000";

	// Dynamic properties defined by the scripts
	image = knifeImage;

	canDrop = true;
};

////////////////
//weapon image//
////////////////
datablock ShapeBaseImageData(knifeImage)
{
	// Basic Item properties
	shapeFile = $City::DataPath @ "/shapes/knife2.dts";
	emap = true;

	// Specify mount point & offset for 3rd person, and eye offset
	// for first person rendering.
	mountPoint = 0;
	offset = "0 0 0";

	// When firing from a point offset from the eye, muzzle correction
	// will adjust the muzzle vector to point to the eye LOS point.
	// Since this weapon doesn't actually fire from the muzzle point,
	// we need to turn this off.
	correctMuzzleVector = false;

	eyeOffset = "0 0 0";

	// Add the WeaponImage namespace as a parent, WeaponImage namespace
	// provides some hooks into the inventory system.
	className = "WeaponImage";

	// Projectile && Ammo.
	item = knifeItem;
	ammo = " ";
	projectile = knifeProjectile;
	projectileType = Projectile;

	//melee particles shoot from eye node for consistancy
	melee = true;
	doRetraction = false;
	//raise your arm up or not
	armReady = true;

	//casing = " ";
	doColorShift = true;
	colorShiftColor = "0.471 0.471 0.471 1.000";

	// Images have a state system which controls how the animations
	// are run, which sounds are played, script callbacks, etc. This
	// state system is downloaded to the client so that clients can
	// predict state changes and animate accordingly.  The following
	// system supports basic ready->fire->reload transitions as
	// well as a no-ammo->dryfire idle state.

	// Initial start up state
	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 0.5;
	stateTransitionOnTimeout[0]      = "Ready";
	stateSound[0]                    = knifeDrawSound;

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

function knifeImage::onPreFire(%this, %obj, %slot)
{
	%obj.playthread(2, armattack);
}

function knifeImage::onStopFire(%this, %obj, %slot)
{
	%obj.playthread(2, root);
}

//function knifeImage::onMount(%this, %obj, %slot)
//{
//	%obj.hidenode("RHand");
//}

//function knifeImage::onUnMount(%this, %obj, %slot)
//{
//	%obj.unhidenode("RHand");
//}
