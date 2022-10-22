%error = ForceRequiredAddOn("GameMode_CityRPG4");
if(%error == $Error::AddOn_NotFound)
{
  error("ERROR: CityRPG_Jobs_Template - required add-on GameMode_CityRPG4 not found");
  return;
}

exec("./CityRPG_Jobs_Template.cs");
