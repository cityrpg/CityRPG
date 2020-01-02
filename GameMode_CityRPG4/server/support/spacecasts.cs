//////////////////////////////////////////////////////////////////////////////////////////////////
//				  Support_RaycastingWeapons.cs  				//
//Creator: Space Guy [ID 130]									//
//Allows you to create weapons that function by instant raycasts rather than projectiles	//
//Set these fields in the datablock:								//
//raycastWeaponRange:		Range of weapon	(> 0)						//
//raycastWeaponTargets:		Typemasks							//
//raycastDirectDamage:		Direct Damage							//
//raycastDirectDamageType:	Damage Type ID							//
//raycastExplosionProjectile:	Creates this projectile on impact				//
//raycastExplosionSound:	AudioProfile of sound to play on impact				//
//												//
//Radius and Brick Damage can be done through the projectile and explosion.			//
//////////////////////////////////////////////////////////////////////////////////////////////////

if($SpaceMods::Server::RaycastingWeaponsVersion > 1)
 return;

$SpaceMods::Server::RaycastingWeaponsVersion = 1;

package RaycastingFire
{
 function WeaponImage::onFire(%this,%obj,%slot)
 {
  if(%this.raycastWeaponRange <= 0)
   return Parent::onFire(%this,%obj,%slot);
  
  %range = %this.raycastWeaponRange*getWord(%obj.getScale(),2);
  %targets = %this.raycastWeaponTargets;
  %start = %obj.getEyePoint();
  
  %fvec = %obj.getForwardVector();
  %fX = getWord(%fvec,0);
  %fY = getWord(%fvec,1);
  
  %evec = %obj.getEyeVector();
  %eX = getWord(%evec,0);
  %eY = getWord(%evec,1);
  %eZ = getWord(%evec,2);
  
  %eXY = mSqrt(%eX*%eX+%eY*%eY);
  
  %aimVec = %fX*%eXY SPC %fY*%eXY SPC %eZ; //SHOULD get the direction you're aiming in

  %end = vectorAdd(%start,vectorScale(%aimVec,%range));
  
  %ray = ContainerRayCast(%start, %end, %targets, %obj);

  %col = getWord(%ray,0);
  
  if(!isObject(%col))
   return;

  %pos = posFromRaycast(%ray);
  %normal = normalFromRaycast(%ray);
  
  %this.onHitObject(%obj,%slot,%col,%pos,%normal);
 }
 function disconnect()
 {
  Parent::disconnect();
  $SpaceMods::Server::RaycastingWeaponsVersion = -1;
  schedule(10, 0, deActivatePackage, RaycastingFire); //we probably don't want to de-activate a package while we're in it, so schedule it 
 }
};activatePackage(RaycastingFire);

function WeaponImage::onHitObject(%this,%obj,%slot,%col,%pos,%normal)
{ 
 if(%this.raycastDirectDamage > 0 && %col.getType() & ($TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType))
 {
  if(isObject(%col.spawnBrick) && %col.getGroup().client == %obj.client)
   %dmg = 1;
  if(miniGameCanDamage(%obj,%col) == 1 || %dmg)
  {
   %damageType = $DamageType::Direct;
   if(%this.raycastDirectDamageType)
      %damageType = %this.raycastDirectDamageType;
   
   %scale = getWord(%obj.getScale(), 2);
   %directDamage = mClampF(%this.raycastDirectDamage, -100, 100) * %scale;
   
   %col.damage(%obj, %pos, %directDamage, %damageType);
  }
 }

 if(isObject(%this.raycastExplosionProjectile))
 {
  %scaleFactor = getWord(%obj.getScale(), 2);
  %p = new Projectile()
  {
  	dataBlock = %this.raycastExplosionProjectile;
  	initialPosition = %pos;
  	initialVelocity = %normal;
  	sourceObject = %obj;
  	client = %obj.client;
  	sourceSlot = 0;
  	originPoint = %pos;
  };
  MissionCleanup.add(%p);
  %p.setScale(%scaleFactor SPC %scaleFactor SPC %scaleFactor);
  %p.explode();
 }
 
 if(isObject(%this.raycastExplosionSound))
 {
  serverplay3d(%this.raycastExplosionSound,%pos);
 }
}