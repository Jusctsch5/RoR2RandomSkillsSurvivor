using RoR2;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Security.Cryptography;
using System;

namespace RandomSkillsSurvivor
{
    static class SurvivorUtils
    {

        public static string SurvivorIndexToBodyString(SurvivorIndex index)
        {
            switch (index)
            {
                case SurvivorIndex.Commando:
                    return "CommandoBody";
                case SurvivorIndex.Engi:
                    return "EngiBody";
                case SurvivorIndex.Huntress:
                    return "HuntressBody";
                case SurvivorIndex.Mage:
                    return "MageBody";
                case SurvivorIndex.Merc:
                    return "MercBody";
                case SurvivorIndex.Toolbot:
                    return "ToolbotBody";
                case SurvivorIndex.Treebot:
                    return "TreebotBody";
                case SurvivorIndex.Loader:
                    return "LoaderBody";
                default:
                    return "";
            }
        }

        public static SurvivorIndex BodyStringToSurvivorIndex(string bodyString)
        {
            if (bodyString.Equals("CommandoBody")) { return SurvivorIndex.Commando; }
            if (bodyString.Equals("EngiBody")) { return SurvivorIndex.Engi; }
            if (bodyString.Equals("HuntressBody")) { return SurvivorIndex.Huntress; }
            if (bodyString.Equals("MageBody")) { return SurvivorIndex.Mage; }
            if (bodyString.Equals("MercBody")) { return SurvivorIndex.Merc; }
            if (bodyString.Equals("ToolbotBody")) { return SurvivorIndex.Toolbot; }
            if (bodyString.Equals("TreebotBody")) { return SurvivorIndex.Treebot; }
            if (bodyString.Equals("LoaderBody")) { return SurvivorIndex.Loader; }
            if (bodyString.Equals("CrocoBody")) { return SurvivorIndex.Croco; }
            return SurvivorIndex.None;
        }

        public static bool BodyStringIsSurvivor(string bodyString)
        {
            bool isSurvivor = BodyStringToSurvivorIndex(bodyString) == SurvivorIndex.None ? false : true;
            if (bodyString == "HaulerBody" || bodyString == "BanditBody" || bodyString == "HANDBody")
            {
                isSurvivor = true;
            }

            return isSurvivor;
        }

        public static CharacterBody GetBody()
        {
            if (LocalUserManager.GetFirstLocalUser() == null)
            {
                Chat.AddMessage($"### No local user to use");
                return null;
            }

            if (LocalUserManager.GetFirstLocalUser().currentNetworkUser == null)
            {
                Chat.AddMessage($"### No current network user to use");
                return null;
            }

            var localId = LocalUserManager.GetFirstLocalUser().currentNetworkUser.Network_id;

            foreach (var master in PlayerCharacterMasterController.instances)
            {
                if (master.networkUser.Network_id.Equals(localId))
                    return master.master.GetBody();
            }

            Chat.AddMessage($"### Couldn't find body for user with network_id:{localId}");
            return null;
        }

        public static SurvivorIndex GetRandomValidSurvivorIndex()
        {
            SurvivorIndex validIndex = SurvivorIndex.Bandit;
            while (validIndex == SurvivorIndex.Bandit)
            {
                validIndex = (SurvivorIndex)GetRandomInRange((int)SurvivorIndex.Count);
            }
            Chat.AddMessage($"### RandomIndex:{validIndex}");

            return validIndex;
        }

        public static uint GetRandomInRange(uint range)
        {
            uint randomNum = 0;
            using (RNGCryptoServiceProvider gRandomProvider = new RNGCryptoServiceProvider())
            {
                var byteArray = new byte[4];
                gRandomProvider.GetBytes(byteArray);

                //convert 4 bytes to an integer
                uint value = BitConverter.ToUInt32(byteArray, 0);
                randomNum = value % range;
            }
            return randomNum;
        }
    }
}