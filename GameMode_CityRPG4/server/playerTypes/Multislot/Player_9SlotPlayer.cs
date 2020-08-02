//9 slot player.
datablock PlayerData(Player9SlotPlayer : PlayerStandardArmor)
{
	minJetEnergy = 0;
	jetEnergyDrain = 0;
	canJet = 0;

	uiName = "9 Slot Player";
	showEnergyBar = false;
	maxTools = 9;
	maxWeapons = 9;
};

datablock PlayerData(Player9SlotJetPlayer : Player9SlotPlayer)
{
	canJet = 1;
	uiName = "";
};
