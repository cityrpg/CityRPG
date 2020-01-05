datablock fxDTSBrickData(brickRotorSafeData)
{
	brickFile = "Add-Ons/GameMode_CityRPG4/data/bricks/RotorSafeClosed.blb";//
	uiName = "Safe - Rotor";

	isWaterBrick = 1;
	orientationFix = 3;

	category = "CityRPG";
	subCategory = "Storage";
	iconName = "Add-Ons/GameMode_CityRPG4/data/bricks/RotorSafe";

	maxStoredTools = 5;
	CityRPGBrickType = 6;
	initialPrice = 75;
};

function brickRotorSafeData::onPlant(%this,%brick)
{
	Parent::onPlant(%this,%brick);
	if(isObject(%client = %brick.client))
	{
		if(!%client.hasSeenStorageHelp)
		{
			%client.chatmessage("\c3StorageHelp\c6: This brick can store items.");
			%client.chatmessage("\c3StorageHelp\c6: Select an item from your inventory by pulling it out, then closing your inventory.");
			%client.chatmessage("\c3StorageHelp\c6: Left-Click the storage brick with empty hands to put the last selected item into storage.");
			%client.chatmessage("\c3StorageHelp\c6: Right-Click with empty hands to pull an item out of storage.");
			%client.chatmessage("\c3StorageHelp\c6: Crouch with empty hands to view the brick's inventory.");
			%client.hasSeenStorageHelp = 1;
		}
		%client.centerPrint("\c6"@%this.uiName@"\n\c6Max Slots: \c3"@%this.maxStoredTools,4);
	}
}