// extraResources by port
// URI: https://github.com/qoh/bl-lib/blob/master/extraResources.cs
// ---------

// addExtraResource(string fileName)
// Add a new file for clients to download. Not all extensions are allowed by the engine.
// You should call this before the mission is created (inside add-on execution is fine).
// If you do need to add files after that, you'll need to call the following to update:
//
//     EnvGuiServer::PopulateEnvResourceList();
//     snapshotGameAssets();
//
// Example:
//
//     addExtraResource("Add-Ons/A_B/assets/textures/c.png");
function addExtraResource(%fileName)
{
	// Don't add the same file multiple times
	if (!ServerGroup.addedExtraResource[%fileName])
	{
		// Maintain a list of "extra" files so we can work nicely with the existing
		// resources, and call PopulateEnvResourceList without getting overwritten.
		if (ServerGroup.extraResourceCount $= "")
			ServerGroup.extraResourceCount = 0;

		ServerGroup.extraResource[ServerGroup.extraResourceCount] = %fileName;
		ServerGroup.extraResourceCount++;

		ServerGroup.addedExtraResource[%fileName] = true;
	}
}

package ExtraResources
{
	function EnvGuiServer::PopulateEnvResourceList()
	{
		Parent::PopulateEnvResourceList();

		for (%i = 0; %i < ServerGroup.extraResourceCount; %i++)
		{
			$EnvGuiServer::Resource[$EnvGuiServer::ResourceCount] = ServerGroup.extraResource[%i];
			$EnvGuiServer::ResourceCount++;
		}
	}
};

activatePackage(ExtraResources);
