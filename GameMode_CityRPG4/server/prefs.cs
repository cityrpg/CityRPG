// ============================================================
// Preferences
// ============================================================

// WARNING: If changing defaults, make sure to change them consistently in both places.

// Init prefs without registering them
function City_InitPrefs() {
	$Pref::Server::City::name = "Blocko City";

	// Game
	$Pref::Server::City::misc::cashdrop									= 1;
	$Pref::Server::City::realestate::maxLots						= 5;
	$Pref::Server::City::prices::reset									= 100;
	$Pref::Server::City::tick::speed										= 5;
	$Pref::Server::City::LotRules												= "No spam. No excessive FX, emitters, or lights.";

	// Crime
	$Pref::Server::City::demerits::minBounty						= 100;
	$Pref::Server::City::demerits::maxBounty						= 7500;

	// Management
	$Pref::Server::City::loggerEnabled 									= true;

	// Economy
	$Pref::Server::City::Economics::Relay								= 2;
	$Pref::Server::City::Economics::Greatest						= 100;
	$Pref::Server::City::Economics::Least								= -35;
	$Pref::Server::City::Economics::Cap									= 150;

	// Mayor
	$Pref::Server::City::Mayor::Active									= true;
	$Pref::Server::City::Mayor::Cost										= 500;
	$Pref::Server::City::Mayor::ImpeachCost							= 100;
	$Pref::Server::City::Mayor::Time										= 10;
}

if(!isFile("Add-Ons/System_ReturnToBlockland/server.cs"))
{
	City_InitPrefs();
} else {
	if(!$RTB::RTBR_ServerControl_Hook)
	{
		exec("Add-Ons/System_ReturnToBlockland/RTBR_ServerControl_Hook.cs");
	}

	// Register prefs if there is a compatible prefs add-on
	// For the sake of reverse compatibility, the old RTB pref function is being used for now.

	// City Prefs
	RTB_registerPref("City name", "CityRPG 4|Game", "$Pref::Server::City::name", "string 64", "GameMode_CityRPG4", "Blocko City", 0, 0);

	RTB_registerPref("Drop Cash on Death", "CityRPG 4|Game", "$Pref::Server::City::misc::cashdrop", "bool", "GameMode_CityRPG4", 1, 0, 0);
	RTB_registerPref("Max Lots", "CityRPG 4|Game", "$Pref::Server::City::realestate::maxLots", "int 0 999", "GameMode_CityRPG4", 5, 0, 0);
	RTB_registerPref("Account Reset Cost (/reset)", "CityRPG 4|Game", "$Pref::Server::City::prices::reset", "int 0 5000", "GameMode_CityRPG4", 100, 0, 0);
	RTB_registerPref("Tick Length (minutes)", "CityRPG 4|Game", "$Pref::Server::City::tick::speed", "int 0 10", "GameMode_CityRPG4", 5, 0, 0);
	RTB_registerPref("Lot Rules", "CityRPG 4|Game", "$Pref::Server::City::LotRules", "string 256", "GameMode_CityRPG4", "No spam. No excessive FX, emitters, or lights.", 0, 0);

	RTB_registerPref("Min Bounty", "CityRPG 4|Crime", "$Pref::Server::City::demerits::minBounty", "int 0 1000", "GameMode_CityRPG4", 100, 0, 0);
	RTB_registerPref("Max Bounty", "CityRPG 4|Crime", "$Pref::Server::City::demerits::maxBounty", "int 0 1000000", "GameMode_CityRPG4", 7500, 0, 0);

	RTB_registerPref("Logging Enabled", "CityRPG 4|Server Management", "$Pref::Server::City::loggerEnabled", "bool", "GameMode_CityRPG4", true, 0, 0);

	RTB_registerPref("Economy Relay", "CityRPG 4|Economy", "$Pref::Server::City::Economics::Relay", "int 0 50", "GameMode_CityRPG4", 2, 0, 0);
	RTB_registerPref("Max Economy Percentage", "CityRPG 4|Economy", "$Pref::Server::City::Economics::Greatest", "int -500 500", "GameMode_CityRPG4", 100, 0, 0);
	RTB_registerPref("Min Economy Percentage", "CityRPG 4|Economy", "$Pref::Server::City::Economics::Least", "int -500 500", "GameMode_CityRPG4", -35, 0, 0);
	RTB_registerPref("Economy Cap", "CityRPG 4|Economy", "$Pref::Server::City::Economics::Cap", "int -5000 5000", "GameMode_CityRPG4", 150, 0, 0);

	RTB_registerPref("Election Active", "CityRPG 4|Mayor", "$Pref::Server::City::Mayor::Active", "bool", "GameMode_CityRPG4", true, 0, 0);
	RTB_registerPref("Mayor Run Cost", "CityRPG 4|Mayor", "$Pref::Server::City::Mayor::Cost", "int 0 50000", "GameMode_CityRPG4", 500, 0, 0);
	RTB_registerPref("Election Time (minutes)", "CityRPG 4|Mayor", "$Pref::Server::City::Mayor::Time", "int 0 30", "GameMode_CityRPG4", 10, 0, 0);
	RTB_registerPref("Mayor Removal Cost", "CityRPG 4|Mayor", "$Pref::Server::City::Mayor::ImpeachCost", "int 0 50000", "GameMode_CityRPG4", 100, 0, 0);

	// A bit of a hack. Registering as a glass section after registering all of the RTB prefs.
	registerPreferenceAddon("GameMode_CityRPG4", "CityRPG 4", "building");
}

// ============================================================
// Constants
// ============================================================

$ATM::Min = 25;
$ATM::Max = 300;
$ATM::Demerits = 500;

$Pref::Server::City::demerits::recordShredCost			= 5000;
$Pref::Server::City::demerits::demeritCost 					= 1.4;

$Pref::Server::City::demerits::demoteLevel		  		= 400;
$Pref::Server::City::demerits::wantedLevel					= 75;

$Pref::Server::City::giveDefaultTools								= true;

$Pref::Server::City::moneyDieTime										= 9999999999;

$Pref::Server::City::demerits::pardonCost						= 1000;
$Pref::Server::City::demerits::reducePerTick				= 25;

$Pref::Server::City::defaultTools										= "hammerItem wrenchItem printGun";

// ATM Hacking
$Pref::Server::City::hack::education 								= 3;
$Pref::Server::City::hack::demerits									= 1000;
$Pref::Server::City::hack::stealmin									= 100;
$Pref::Server::City::hack::stealmax									= 1000;
$Pref::Server::City::hack::revivetime								= 5; //minutes

// Game Prices
$CityRPG::prices::vehicleSpawn = 2500;
$CityRPG::prices::jailingBonus = 100;

$CityRPG::prices::resourcePrice = 1.5;

// Weapon Prices
$CityRPG::prices::weapon::name[$CityRPG::guns] = "gunItem";
$CityRPG::prices::weapon::price[$CityRPG::guns] = 80;
$CityRPG::prices::weapon::mineral[$CityRPG::guns++] = 1;

$CityRPG::prices::weapon::name[$CityRPG::guns] = "taserItem";
$CityRPG::prices::weapon::price[$CityRPG::guns] = 40;
$CityRPG::prices::weapon::mineral[$CityRPG::guns++] = 1;

$CityRPG::prices::weapon::name[$CityRPG::guns] = "akimboGunItem";
$CityRPG::prices::weapon::price[$CityRPG::guns] = 150;
$CityRPG::prices::weapon::mineral[$CityRPG::guns++] = 1;

$CityRPG::prices::weapon::name[$CityRPG::guns] = "shotgunItem";
$CityRPG::prices::weapon::price[$CityRPG::guns] = 260;
$CityRPG::prices::weapon::mineral[$CityRPG::guns++] = 1;

$CityRPG::prices::weapon::name[$CityRPG::guns] = "sniperRifleItem";
$CityRPG::prices::weapon::price[$CityRPG::guns] = 450;
$CityRPG::prices::weapon::mineral[$CityRPG::guns++] = 1;

//$CityRPG::prices::weapon::name[$CityRPG::guns] = "PillItem";
//$CityRPG::prices::weapon::price[$CityRPG::guns] = 30;
//$CityRPG::prices::weapon::mineral[$CityRPG::guns++] = 1;

$CityRPG::prices::weapon::name[$CityRPG::guns] = "CityRPGLBItem";
$CityRPG::prices::weapon::price[$CityRPG::guns] = 100;
$CityRPG::prices::weapon::mineral[$CityRPG::guns++] = 1;

$CityRPG::prices::weapon::name[$CityRPG::guns] = "CityRPGPickaxeItem";
$CityRPG::prices::weapon::price[$CityRPG::guns] = 25;
$CityRPG::prices::weapon::mineral[$CityRPG::guns++] = 1;

$CityRPG::prices::weapon::name[$CityRPG::guns] = "CityRPGLumberjackItem";
$CityRPG::prices::weapon::price[$CityRPG::guns] = 25;
$CityRPG::prices::weapon::mineral[$CityRPG::guns++] = 1;

//When adding to this index, be sure to add a forceRequiredAddon("Item_Here");
//in server.cs, or else the item mod will be broken.

// Ticks
$CityRPG::tick::interest = 1.00;
$CityRPG::tick::creditInterest = 1.000;
$CityRPG::tick::interestTick = 999;
$CityRPG::tick::promotionLevel = 24;

// Demerit Preferences
$CityRPG::demerits::hittingInnocents = 50;
$CityRPG::demerits::attemptedMurder = 15;
$CityRPG::demerits::murder = 75;
$CityRPG::demerits::breakingAndEntering = 10;
$CityRPG::demerits::attemptedBnE = 5;
$CityRPG::demerits::bountyPlacing = 250;
$CityRPG::demerits::bountyClaiming = 500;
$CityRPG::demerits::pickpocketing = 25;
$CityRPG::demerits::bankRobbery = 3000;
$CityRPG::demerits::tasingBros = 50;
$CityRPG::demerits::grandTheftAuto = 75;

// Vehicles
$CityRPG::vehicles::allowSpawn = true;

$CityRPG::vehicles::banned[0] = "FlyingWheeledJeepVehicle";
$CityRPG::vehicles::banned[1] = "MiniJetVehicle";
$CityRPG::vehicles::banned[2] = "StuntPlaneVehicle";
$CityRPG::vehicles::banned[3] = "BiplaneVehicle";
$CityRPG::vehicles::banned[4] = "MagicCarpetVehicle";
$CityRPG::vehicles::banned[5] = "TankVehicle";
$CityRPG::vehicles::banned[6] = "horseArmor";
$CityRPG::vehicles::banned[7] = "BlackhawkVehicle";
$CityRPG::vehicles::banned[7] = "TankTurretVehicle";

// Allowed Inmate Items
$CityRPG::demerits::jail::image["CityRPGLumberjackImage"] = true;
$CityRPG::demerits::jail::image["CityRPGPickaxeImage"] = true;

// Inmate Spawn Items
$CityRPG::demerits::jail::item[0] = "CityRPGPickaxeItem";
$CityRPG::demerits::jail::item[1] = "CityRPGLumberjackItem";

// Food
$CityRPG::portion[1] = "Small";
$CityRPG::portion[2] = "Medium";
$CityRPG::portion[3] = "Large";
$CityRPG::portion[4] = "Extra-Large";
$CityRPG::portion[5] = "Super-Sized";
$CityRPG::portion[6] = "Americanized";

// Hunger Colors and States
$CityRPG::food::stateCount = 0;

$CityRPG::food::color[$CityRPG::food::stateCount++] = "FF0000";
$CityRPG::food::state[$CityRPG::food::stateCount] = "10<color:ffffff>%";

$CityRPG::food::color[$CityRPG::food::stateCount++] = "FD2000";
$CityRPG::food::state[$CityRPG::food::stateCount] = "20<color:ffffff>%";

$CityRPG::food::color[$CityRPG::food::stateCount++] = "FF5900";
$CityRPG::food::state[$CityRPG::food::stateCount] = "30<color:ffffff>%";

$CityRPG::food::color[$CityRPG::food::stateCount++] = "FD7E00";
$CityRPG::food::state[$CityRPG::food::stateCount] = "40<color:ffffff>%";

$CityRPG::food::color[$CityRPG::food::stateCount++] = "FD7E00";
$CityRPG::food::state[$CityRPG::food::stateCount] = "50<color:ffffff>%";

$CityRPG::food::color[$CityRPG::food::stateCount++] = "F7FD00";
$CityRPG::food::state[$CityRPG::food::stateCount] = "60<color:ffffff>%";

$CityRPG::food::color[$CityRPG::food::stateCount++] = "C6FF00";
$CityRPG::food::state[$CityRPG::food::stateCount] = "70<color:ffffff>%";

$CityRPG::food::color[$CityRPG::food::stateCount++] = "7EFD00";
$CityRPG::food::state[$CityRPG::food::stateCount] = "80<color:ffffff>%";

$CityRPG::food::color[$CityRPG::food::stateCount++] = "73FF00";
$CityRPG::food::state[$CityRPG::food::stateCount] = "90<color:ffffff>%";

$CityRPG::food::color[$CityRPG::food::stateCount++] = "00FF00";
$CityRPG::food::state[$CityRPG::food::stateCount] = "100<color:ffffff>%";
