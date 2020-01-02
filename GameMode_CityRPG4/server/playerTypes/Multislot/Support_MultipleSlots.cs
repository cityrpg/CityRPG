//Setting the support so we can see all Slots
package InventorySlotAdjustment
{
	function Armor::onNewDatablock(%data,%this)
	{
		Parent::onNewDatablock(%data,%this);
		if(isObject(%this.client) && %data.maxTools != %this.client.lastMaxTools)
		{
			%this.client.lastMaxTools = %data.maxTools;
			commandToClient(%this.client,'PlayGui_CreateToolHud',%data.maxTools);
			for(%i=0;%i<%data.maxTools;%i++)
			{
				if(isObject(%this.tool[%i]))
					messageClient(%this.client,'MsgItemPickup',"",%i,%this.tool[%i].getID(),1);
				else
					messageClient(%this.client,'MsgItemPickup',"",%i,0,1);
			}
		}
	}
	function GameConnection::setControlObject(%this,%obj)
	{
		Parent::setControlObject(%this,%obj);
		if(%obj == %this.player && %obj.getDatablock().maxTools != %this.lastMaxTools)
		{
			%this.lastMaxTools = %obj.getDatablock().maxTools;
			commandToClient(%this,'PlayGui_CreateToolHud',%obj.getDatablock().maxTools);
		}
	}
	function Player::changeDatablock(%this,%data,%client)
	{
		if(%data != %this.getDatablock() && %data.maxTools != %this.client.lastMaxTools)
		{
			%this.client.lastMaxTools = %data.maxTools;
			commandToClient(%this.client,'PlayGui_CreateToolHud',%data.maxTools);
		}
		Parent::changeDatablock(%this,%data,%client);
	}
};
activatePackage(InventorySlotAdjustment);
