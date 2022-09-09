// WARNING: This jobs list is compatible with CityRPG 4 0.3.x ONLY.
// As CityRPG 4 is still in alpha, future versions are not guaranteed to be compatible.

// Config changes
$City::EducationCap	= 6;
$City::EducationReincarnateLevel = 8;
$City::AdminJobID = "Admin";
$City::MayorJobID = "GovMayor";
$City::CivilianJobID = "StarterCivilian";

$City::JobsPath = "Add-Ons/CityRPG_Jobs_Template/jobs";

package CityRPG_JobMod
{
	// No parent call because we're going to completely overwrite the job tree.
	function JobSO::loadJobFiles(%so)
	{
		$City::DefaultJobs = 0;

		echo("Loading custom jobs...");

		%so.createJob($City::JobsPath @ "/StarterCivilian.cs");
		%so.createJob($City::JobsPath @ "/LaborMiner.cs");
		%so.createJob($City::JobsPath @ "/LaborLumberjack.cs");
		%so.createJob($City::JobsPath @ "/BusGrocer.cs");
		%so.createJob($City::JobsPath @ "/BusArmsDealer.cs");
		%so.createJob($City::JobsPath @ "/BusOwner.cs");
		%so.createJob($City::JobsPath @ "/BusCEO.cs");
		%so.createJob($City::JobsPath @ "/BountyHunter.cs");
		%so.createJob($City::JobsPath @ "/BountyVigilante.cs");
		%so.createJob($City::JobsPath @ "/PdAsst.cs");
		%so.createJob($City::JobsPath @ "/PdOfficer.cs");
		%so.createJob($City::JobsPath @ "/PdChief.cs");
		%so.createJob($City::JobsPath @ "/Admin.cs");
		%so.createJob($City::JobsPath @ "/GovMayor.cs");
	}
};


activatePackage("CityRPG_JobMod");
