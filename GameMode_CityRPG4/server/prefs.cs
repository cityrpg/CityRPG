// ============================================================
// Preferences
// ============================================================
$City::UsePrefObjects = isFunction(registerPreferenceAddon);

function City_RegisterPref(%category, %name, %variable, %type, %params, %defaultValue, %loadCallback, %updateCallback, %requireRestart, %hostOnly)
{
	if($City::UsePrefObjects)
	{
		new ScriptObject(Preference) {
			className      = "CityPref";

			addon          = "GameMode_CityRPG4";
			//category       = "General";
			category       = %category;
			//title          = "Can use";
			title          = %name;

			//type           = "dropdown";
			type           = %type;
			params         = "Host 3 Super_Admin 2 Admin 1"; //list based parameters
			params         = %params; //list based parameters

			variable       = %variable;

			defaultValue   = %defaultValue;

			updateCallback = %updateCallback; //to call after ::onUpdate (optional)
			loadCallback   = %loadCallback; //to call after ::onLoad (optional)

			hostOnly       = %hostOnly;
			requireRestart = %requireRestart;
		};
	}
	else
	{
		eval("%varRef = " @ %variable @ ";");

		// If the value does not exist, automatically set it to its default value.
		if(%varRef $= "")
			eval(%variable @ " = \"" @ %defaultValue @ "\";");
	}
}

function City_InitPrefs()
{
	if($City::UsePrefObjects)
	{
		registerPreferenceAddon("GameMode_CityRPG4", "CityRPG 4", "building");
	}

	// City Prefs
	City_RegisterPref("Game", "City name", "$Pref::Server::City::name", "string", "64", "Blocko Town");
	City_RegisterPref("Game", "Drop Cash on Death", "$Pref::Server::City::misc::cashdrop", "bool", "", true);
	City_RegisterPref("Game", "Disable Default Weapons", "$Pref::Server::City::disabledefaultweps", "bool", "", false, "", "", true);
	City_RegisterPref("Game", "Max Lots", "$Pref::Server::City::realestate::maxLots", "int", "0 999", 5);
	City_RegisterPref("Game", "Account Reset Cost (/reset)", "$Pref::Server::City::prices::reset", "int", "0 5000", 100);
	City_RegisterPref("Game", "Tick Length (minutes)", "$Pref::Server::City::tick::speed", "int", "0 10", 5);
	City_RegisterPref("Game", "Lot Rules", "$Pref::Server::City::LotRules", "string", "256", "No spam. No excessive FX, emitters, or lights.");
	City_RegisterPref("Game", "Min Bounty", "$Pref::Server::City::demerits::minBounty", "int", "0 1000", 100);
	City_RegisterPref("Game", "Max Bounty", "$Pref::Server::City::demerits::maxBounty", "int", "0 1000000", 7500);
	City_RegisterPref("Game", "Disable tumble on starve", "$Pref::Server::City::DisableHungerTumble", "bool", "", false);

	City_RegisterPref("Styling", "Text Color - Primary", "$c_p", "string", "14 ", "\c3"); // $c_p or Color - Primary
	City_RegisterPref("Styling", "Clock on HUD (Experimental)", "$Pref::Server::City::HUDShowClock", "bool", "", false);

	City_RegisterPref("Server Management", "Logging Enabled", "$Pref::Server::City::loggerEnabled", "bool", "", true);
	City_RegisterPref("Server Management", "Always show hidden chat to admins", "$Pref::Server::City::AdminsAlwaysMonitorChat", "bool", "", false);

	City_RegisterPref("Economy", "Economy Relay", "$Pref::Server::City::Economics::Relay", "int", "0 50", 2);
	City_RegisterPref("Economy", "Max Economy Percentage", "$Pref::Server::City::Economics::Greatest", "int", "-500 500", 100);
	City_RegisterPref("Economy", "Economy Cap", "$Pref::Server::City::Economics::Cap", "int", "-5000 5000", 150);

	City_RegisterPref("Economy", "Lot Cost - 16x16", "$Pref::Server::City::lotCost" @ "CityRPGSmallLotBrickData", "int", "0 999999", 500);
	City_RegisterPref("Economy", "Lot Cost - 16x32", "$Pref::Server::City::lotCost" @ "CityRPGHalfSmallLotBrickData", "int", "0 999999", 750);
	City_RegisterPref("Economy", "Lot Cost - 32x32", "$Pref::Server::City::lotCost" @ "CityRPGMediumLotBrickData", "int", "0 999999", 1500);
	City_RegisterPref("Economy", "Lot Cost - 32x64", "$Pref::Server::City::lotCost" @ "CityRPGHalfLargeLotBrickData", "int", "0 999999", 2000);
	City_RegisterPref("Economy", "Lot Cost - 64x64", "$Pref::Server::City::lotCost" @ "CityRPGLargeLotBrickData", "int", "0 999999", 4500);

	City_RegisterPref("Mayor", "Election Active", "$Pref::Server::City::Mayor::Active", "bool", "", true);
	City_RegisterPref("Mayor", "Mayor Run Cost", "$Pref::Server::City::Mayor::Cost", "int", "0 50000", 500);
	City_RegisterPref("Mayor", "Election Time (minutes)", "$Pref::Server::City::Mayor::Time", "int", "0 30", 10);
	City_RegisterPref("Mayor", "Mayor Removal Cost", "$Pref::Server::City::Mayor::ImpeachCost", "int", "0 50000", 100);
}

City_InitPrefs();

// ============================================================
// Constants
// ============================================================

$City::CommandRateLimitMS = 128;

$Pref::Server::City::DisableIntroMessage = true;

$ATM::Min = 25;
$ATM::Max = 300;
$ATM::Demerits = 500;

$Pref::Server::City::demerits::recordShredCost = 5000;
$Pref::Server::City::demerits::demeritCost = 1.4;

$Pref::Server::City::demerits::demoteLevel = 400;
$Pref::Server::City::demerits::wantedLevel = 75;

$Pref::Server::City::giveDefaultTools = true;

$Pref::Server::City::moneyDieTime = 9999999999;

$Pref::Server::City::demerits::pardonCostMultiplier = 5;
$Pref::Server::City::demerits::reducePerTick = 25;

$Pref::Server::City::defaultTools = "hammerItem wrenchItem printGun";

// Education
$City::EducationCap = 6;
$City::EducationReincarnateLevel = 8;

// Mayor
$City::MayorJobID = "GovMayor";

// ATM Hacking
$Pref::Server::City::hack::education 								= 3;
$Pref::Server::City::hack::demerits									= 1000;
$Pref::Server::City::hack::stealmin									= 100;
$Pref::Server::City::hack::stealmax									= 1000;
$Pref::Server::City::hack::revivetime								= 5; //minutes

// Game Prices
$CityRPG::prices::vehicleSpawn = 1000;
$CityRPG::prices::jailingBonus = 100;

$CityRPG::prices::resourcePrice = 1.5;

//When adding to this index, be sure to add a forceRequiredAddon("Item_Here");
//in server.cs, or else the item mod will be broken.

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
$CityRPG::vehicles::banned[8] = "CannonTurret";

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
