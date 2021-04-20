// ============================================================
// Admin Menu
// ============================================================
function serverCmdAdmin(%client)
{
  CityMenu_Admin(%client);
}

function CityMenu_Admin(%client)
{
  %client.cityMenuMessage("\c3CityRPG Game Master");

	%menu = "Toggle admin mode" TAB "Close menu.";
	%functions = "serverCmdAdminMode" TAB "CityMenu_Close";

	%client.cityMenuOpen(%menu, %functions, %client.CityRPGLotBrick);
}

// ============================================================
// Admin Mode
// ============================================================
function serverCmdAdminMode(%client)
{
  %client.cityMenuClose();

  %jobRevert = City.get(%client.bl_id, "jobRevert");
  %jobID = City.get(%client.bl_id, "jobId");

  if(%jobID $= $City::AdminJobID)
  {
    %client.setCityJob(%jobRevert !$= 0 ? %jobRevert : $City::CivilianJobID, 1);
    messageClient(%client, '', "\c6Admin mode has been disabled.");
  }
  else
  {
    City.set(%client.bl_id, "jobRevert", %jobID);
    %client.setCityJob($City::AdminJobID, 1, 1);

    messageClient(%client, '', "\c6You are now in \c4Admin Mode\c6. Time for crime!");
    %client.adminModeMessage();
  }
}

function GameConnection::AdminModeMessage(%client)
{
  messageClient(%client, '', "\c2+\c6 Building restrictions are disabled.");
	messageClient(%client, '', "\c2+\c6 You are immune to all damage, and your hunger is frozen.");
	messageClient(%client, '', "\c2+\c6 You have jets.");

  if(!$Pref::Server::City::AdminsAlwaysMonitorChat)
  {
    messageClient(%client, '', "\c2+\c6 You can see radio chat messages for all jobs.");
  }

	messageClient(%client, '', "\c3*\c6 Your job is fixed as \c3Council Member\c6. Changing jobs will disable admin mode.");
}

// ============================================================
// Jets
// ============================================================
datablock PlayerData(Player9SlotJetPlayer : Player9SlotPlayer)
{
	canJet = 1;
	uiName = "";
};


// ============================================================
// Other Admin Commands
// ============================================================
function serverCmdEditEducation(%client, %int, %name)
{
  %client.cityLog("/EditEducation" SPC %int SPC %name);

  if(!%client.isAdmin)
  {
    messageClient(%client, '', "\c6Must be a super admin to use this command.");
    return;
  }

  %int = mFloor(%int);

  if(%int < 0)
    %int = 0;
  else if(%int > 8)
    %int = 8;

  if(%name !$= "" || %name !$= null)
  {
    if(isObject(%target = findClientByName(%name)))
    {
      CityRPGData.getData(%target.bl_id).valueEducation = %int;
      %target.setGameBottomPrint();
      messageClient(%client, '', "\c6You have set\c3" SPC %target.name @ "'s \c6education to \c3" @ %int);
      messageClient(%target, '', "\c6Your education has been set to " @ %int @ " by an admin.");
    }
    else
    {
      messageClient(%client, '', "\c6Invalid user.");
    }
  }
  else
  {
    messageClient(%client, '', %name @ "<<");
    CityRPGData.getData(%client.bl_id).valueEducation = %int;
    %client.setGameBottomPrint();
    messageClient(%client, '', "\c6Your education has been set to " @ %int);
  }
}

function serverCmddMoney(%client, %money, %name)
{
  %client.cityLog("/dMoney" SPC %money SPC %name);

  if(!%client.isAdmin)
  {
    messageClient(%client, '', "\c6You must be admin to use the this command.");
    return;
  }

  if(%money $= "All")
    %money = CityRPGData.getData(%client.bl_id).valueMoney;

  %money = mFloor(%money);

  if(%money <= 0)
  {
     messageClient(%client, '', "\c6You must enter a valid amount of money to grant.");
     return;
  }

  if(%name !$= "")
  {
    if(isObject(%target = findClientByName(%name)))
    {
      if(%target != %client)
      {
        messageClient(%client, '', "\c6You deducted \c3$" @ %money SPC "\c6from \c3" @ %target.name @ "\c6.");
        CityRPGData.getData(%target.bl_id).valueMoney -= %money;
        %target.SetInfo();
        return;
      }
      else
      {
        CityRPGData.getData(%client.bl_id).valueMoney -= %money;
        messageClient(%client, '', "\c6You deducted yourself \c3$" @ %money @ "\c6. Left:" SPC CityRPGData.getData(%target.bl_id).valueMoney);
        %client.SetInfo();
        return;
      }
    }
    else
      messageClient(%client, "\c6The name you entered could not be matched up to a person.");
  }
  else if(isObject(%client.player))
  {
    %target = containerRayCast(%client.player.getEyePoint(), vectorAdd(vectorScale(vectorNormalize(%client.player.getEyeVector()), 5), %client.player.getEyePoint()), $typeMasks::playerObjectType).client;

    if(isObject(%target))
    {
      CityRPGData.getData(%target.bl_id).valueMoney -= %money;
      messageClient(%client, '', "\c6You deducted yourself \c3$" @ %money @ "\c6. Left:" SPC CityRPGData.getData(%target.bl_id).valueMoney);
      %target.SetInfo();
    }
  }
  else
    messageClient(%client, '', "\c6Spawn first before you use this command or enter a valid player's name.");
}

function serverCmdClearMoney(%client)
{
  %client.cityLog("/clearMoney");

  if(%client.isAdmin)
  {
    CityRPGData.getData(%client.bl_id).valueMoney = 0;
    %client.setGameBottomPrint();
    messageClient(%client, '', "\c6You have cleared your money.");
  }
  else
    messageClient(%client, '', "\c6You must be admin to use the this command.");
}

function serverCmddBank(%client, %Bank, %name)
{
  %client.cityLog("/bank" SPC %Bank SPC %name);

  if(%client.isAdmin)
  {
    %Bank = mFloor(%Bank);
    if(%Bank > 0)
    {
      if(%name !$= "")
      {
        if(isObject(%target = findClientByName(%name)))
        {
          if(%target != %client)
          {
            messageClient(%client, '', "\c6You deducted \c3$" @ %Bank SPC "\c6from \c3" @ %target.name @ "\c6.");
            CityRPGData.getData(%target.bl_id).valueBank -= %Bank;
            %target.SetInfo();
            return;
          }
          else
          {
            CityRPGData.getData(%client.bl_id).valueBank -= %Bank;
            messageClient(%client, '', "\c6You deducted yourself \c3$" @ %Bank @ "\c6. Left:" SPC CityRPGData.getData(%target.bl_id).valueBank);
            %client.SetInfo();
            return;
          }
        }
        else
          messageClient(%client, "\c6The name you entered could not be matched up to a person.");
      }
      else if(isObject(%client.player))
      {
        %target = containerRayCast(%client.player.getEyePoint(), vectorAdd(vectorScale(vectorNormalize(%client.player.getEyeVector()), 5), %client.player.getEyePoint()), $typeMasks::playerObjectType).client;

        if(isObject(%target))
        {
          CityRPGData.getData(%target.bl_id).valueBank -= %Bank;
          messageClient(%client, '', "\c6You deducted yourself \c3$" @ %Bank @ "\c6. Left:" SPC CityRPGData.getData(%target.bl_id).valueBank);
          %target.SetInfo();
        }
      }
      else
        messageClient(%client, '', "\c6Spawn first before you use this command or enter a valid player's name.");
    }
    else
      messageClient(%client, '', "\c6You must enter a valid amount of Bank to grant.");
  }
  else
    messageClient(%client, '', "\c6You must be admin to use the this command.");
}

// Not to be confused with G Money Pranks™
function serverCmdgmoney(%client, %money, %name)
{
  %client.cityLog("/gmoney" SPC %money SPC %name);

  if(!isObject(%client.player))
    return;

  if(%client.isAdmin)
  {
    %money = mFloor(%money);
    if(%money > 0)
    {
      if(%name !$= "")
      {
        if(isObject(%target = findClientByName(%name)))
        {
          if(%target != %client)
          {
            messageClient(%client, '', "\c6You grant \c3$" @ %money SPC "\c6to \c3" @ %target.name @ "\c6.");
            messageClient(%target, '', "\c3An admin has granted you \c3$" @ %money @ "\c6.");
          }
          else
            messageClient(%client, '', "\c6You grant yourself \c3$" @ %money @ "\c6.");
            CityRPGData.getData(%target.bl_id).valueMoney += %money;
            %target.SetInfo();
        }
        else
          messageClient(%client, "\c6The name you entered could not be matched up to a person.");
      }
      else if(isObject(%client.player))
      {
        %target = containerRayCast(%client.player.getEyePoint(), vectorAdd(vectorScale(vectorNormalize(%client.player.getEyeVector()), 5), %client.player.getEyePoint()), $typeMasks::playerObjectType).client;
        if(isObject(%target))
        {
          messageClient(%client, '', "\c6You grant \c3$" @ %money SPC "\c6to \c3" @ %target.name @ "\c6.");
          messageClient(%target, '', "\c3An admin has granted you \c3$" @ %money @ "\c6.");
          CityRPGData.getData(%target.bl_id).valueMoney += %money;
          %target.SetInfo();
        }
      }
      else
        messageClient(%client, '', "\c6Spawn first before you use this command or enter a valid player's name.");
    }
    else
      messageClient(%client, '', "\c6You must enter a valid amount of money to grant.");
  }
  else
    messageClient(%client, '', "\c6You must be admin to use the this command.");
}

function serverCmdaddDemerits(%client, %dems, %name)
{
  %client.cityLog("/addDemerits" SPC %dems SPC %name);

  if(!isObject(%client.player))
    return;

  if(%client.isSuperAdmin)
  {
    %dems = mFloor(%dems);

    if(%dems > 0)
    {
      if(%name !$= "")
      {
        if(isObject(%target = findClientByName(%name)))
        {
          commandToClient(%target, 'centerPrint', "\c6You have committed a crime. [\c3Angering a Badmin\c6]", 5);
          messageClient(%client, '', '\c6User \c3%1 \c6was given \c3%2\c6 demerits.', %target.name , %dems);
          City_AddDemerits(%target.bl_id, %dems);
        }
        else
          messageClient(%client, "\c6The name you entered could not be matched up to a person.");
        }
        else if(isObject(%client.player))
        {
          %target = containerRayCast(%client.player.getEyePoint(), vectorAdd(vectorScale(vectorNormalize(%client.player.getEyeVector()), 5), %client.player.getEyePoint()), $typeMasks::playerObjectType).client;

          if(isObject(%target))
          {
            commandToClient(%target, 'centerPrint', "\c6You have committed a crime. [\c3Angering a Badmin\c6]", 5);
            messageClient(%client, '', '\c6User \c3%1 \c6was given \c3%2\c6 demerits.', %target.name , %dems);
            City_AddDemerits(%target.bl_id, %dems);
          }
        }
        else
          messageClient(%client, '', "\c6Spawn first before you use this command or enter a valid player's name.");
      }
      else
        messageClient(%client, '', "\c6You must enter a valid number to grant.");
    }
    else
      messageClient(%client, '', "\c6You must be admin to use the this command.");
}

function serverCmdresetuser(%client, %name)
{
  %client.cityLog("/resetuser" SPC %name);

  if(!isObject(%client.player))
    return;

  if(%client.isAdmin)
  {
    if(%name !$= "")
    {
      if(isObject(%target = findClientByName(%name)))
      {
        messageClient(%target, '', "\c6Your account was reset by an admin.");
        messageClient(%client, '', "\c3" @ %target.name @ "\c6's account was reset.");
        CityRPGData.removeData(%target.bl_id);
        CityRPGData.addData(%target.bl_id);
        if(isObject(%target.player))
        {
          %target.player.delete();
          %target.spawnPlayer();
        }
      }
      else
        messageClient(%client, '', "\c6That person does not exist.");
    }
    else
      messageClient(%client, '' , "\c6Please enter a name.");
  }
}

function serverCmdcleanse(%client,%name)
{
  %client.cityLog("/cleanse" SPC %name);

  if(!isObject(%client.player))
    return;

  if(%client.isAdmin)
  {
    if(%name $= "")
    {
      if(CityRPGData.getData(%client.bl_id).valueDemerits > 0)
      {
        CityRPGData.getData(%client.bl_id).valueDemerits = 0;
        messageClient(%client, '', "\c6The heat is gone.");
        %client.setInfo();
      }
      else
        messageClient(%client, '', "You are not wanted!");
    }
    else if(isObject(findClientByName(%name)))
    {
      %target = findClientByName(%name);
      messageClient(%client, '', "\c6You cleared \c3" @ %target.name @ "\c6's demerits.");
      messageClient(%target, '', "\c6Your demerits have vanished.");
      CityRPGData.getData(%target.bl_id).valueDemerits = 0;
      %target.setInfo();
    }
  }
}

function serverCmdedithunger(%client, %int)
{
  %client.cityLog("/edithunger" SPC %int);

  if(!isObject(%client.player))
    return;

  if(%client.isAdmin && mFloor(%int))
  {
    %int = mFloor(%int);

    if(%int > 12)
      %int = 12;
    else if(%int < 1)
      %int = 1;

    CityRPGData.getData(%client.bl_id).valueHunger = %int;
    %client.setGameBottomPrint();

    %client.doCityHungerStatus();
  }
}

function serverCmdupdateScore(%client)
{
  %client.cityLog("/updateScore");

  if(!isObject(%client.player))
    return;

  if(%client.isAdmin)
  {
    for(%d = 0; %d < ClientGroup.getCount(); %d++)
    {
      %subClient = ClientGroup.getObject(%d);
      gameConnection::setScore(%subClient, %score);
    }

    messageClient(%client, '', "\c6You've updated the score.");
  }
  else
  {
    messageClient(%client, '', "\c6Must be admin to use this command.");
  }
}

function serverCmdRespawnAllPlayers(%client)
{
  %client.cityLog("/respawnAllPlayers");

  if(!isObject(%client.player))
    return;

  if(%client.isAdmin)
  {
    messageAll('', '\c3%1\c5 respawned all players.', %client.name);

    for(%a = 0; %a < ClientGroup.getCount(); %a++)
      ClientGroup.getObject(%a).spawnPlayer();
  }
}

function serverCmdsetMinerals(%client, %int)
{
  %client.cityLog("/setMinerals" SPC %int);

  if(!isObject(%client.player))
    return;

  if(%client.isSuperAdmin)
  {
    CitySO.minerals = %int;
    messageClient(%client, '', "\c6City's minerals set to \c3" @ %int @ "\c6.");
  }
  else
  {
    messageClient(%client, '', "\c6You need to be a Super Admin to use this function.");
  }
}

function serverCmdsetLumber(%client, %int)
{
  %client.cityLog("/setLumber" SPC %int);

  if(!isObject(%client.player))
    return;

  if(%client.isSuperAdmin)
  {
    CitySO.lumber = %int;
    messageClient(%client, '', "\c6City's lumber set to \c3" @ %int @ "\c6.");
  }
  else
  {
    messageClient(%client, '', "\c6You need to be a Super Admin to use this function.");
  }
}

function serverCmdResetAllJobs(%client)
{
  if(!%client.isSuperAdmin)
  {
    messageClient(%client, '', "\c6You need to be a Super Admin to use this function.");
    return;
  }

  CityMenu_ResetAllJobsPrompt(%client);
}
