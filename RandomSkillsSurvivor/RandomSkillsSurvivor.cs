using BepInEx;
using RoR2;
using UnityEngine;
using System;
using System.Collections.Generic;

using BepInEx.Configuration;
using RoR2.UI;
using System.IO;
using System.Linq;
using RoR2.Skills;
using static RandomSkillsSurvivor.SurvivorUtils;

/*
 * 
 * TODO
 * - Implement RandomSkillsSurvivor as own survivor
 * - Figure out why skills don't have cooldowns for characters other than the original owner
 * 
 * 
 * - Trigger on game start, not every new stage (Fixed kind of...)
 * 
 * Common things to look for:
        RoR2.Loadout
 * 
 * 
 * 
 */
namespace RandomSkillsSurvivor
{
    
    // Tell BepInEx to initialize R2API before this plugin so it's safe to use R2API.
    [BepInDependency("com.bepis.r2api")]

    // Describe package metadata.
    [BepInPlugin("com.larquis.RandomSkillsSurvivor", "RandomSkillsSurvivor", "1.0")]
    public class RandomSkillsSurvivor : BaseUnityPlugin
    {
        public static Dictionary<string, GenericSkill> ExistingSkillsList = new Dictionary<string, GenericSkill>();
        public static Dictionary<string, GenericSkill> ValidPrimarySkillList = new Dictionary<string, GenericSkill>();
        public static Dictionary<string, GenericSkill> ValidSecondarySkillList = new Dictionary<string, GenericSkill>();
        public static Dictionary<string, GenericSkill> ValidUtilitySkillList = new Dictionary<string, GenericSkill>();
        public static Dictionary<string, GenericSkill> ValidSpecialSkillList = new Dictionary<string, GenericSkill>();

        public static bool gChooseRandomSurvivor = false;
        public static bool gChooseRandomSkills = true;
        public static SkillClassifier gSkillClassifierSelf;

        public static bool gUseSkillReroll = false;
        public static uint gSkillRatingDesired = 1;
        public static uint gSkillGenerationAttempts = 10;

        public static bool gUseExpandedSkillList = false;
        public static RandomSkillsSurvivor gRandomSkillsSurvivor;

        static List<SurvivorSkillOverride> _skillDefPlayerList;

        public class SurvivorSkillOverride
        {
            public SurvivorSkillOverride(uint player, CharacterBody characterBody, SkillDef primarySkill, SkillDef secondarySkill, SkillDef utilitySkill, SkillDef specialSkill)
            {
                _player = player;
                _characterBody = characterBody;
                _primarySkill = primarySkill;
                _secondarySkill = secondarySkill;
                _utilitySkill = utilitySkill;
                _specialSkill = specialSkill;
            }

            public SkillDef _primarySkill { get; set; }
            public SkillDef _secondarySkill { get; set; }
            public SkillDef _utilitySkill { get; set; }
            public SkillDef _specialSkill { get; set; }
            public uint _player { get; set; }
            public CharacterBody _characterBody { get; set; }

            public void SetSkillsForSurvivor()
            {
                SetSkillForSurvivor(_characterBody, _primarySkill, SkillSlot.Primary);
                SetSkillForSurvivor(_characterBody, _secondarySkill, SkillSlot.Secondary);
                SetSkillForSurvivor(_characterBody, _utilitySkill, SkillSlot.Utility);
                SetSkillForSurvivor(_characterBody, _specialSkill, SkillSlot.Special);
            }

            public void SetSkillForSurvivor(CharacterBody iPreFab, SkillDef iSkill, SkillSlot iSlot)
            {
                SkillLocator locator = iPreFab.GetComponent<SkillLocator>();
                if (locator == null)
                {
                    Chat.AddMessage("Couldn't find the player's body");
                    return;
                }
                GenericSkill oldSkill = locator.GetSkill(iSlot);
                if (oldSkill == null)
                {
                    Chat.AddMessage($"Couldn't find the player's skill for slot: {iSlot}");
                    return;
                }
                Chat.AddMessage($"### Replacing old skill: {oldSkill.skillName} with new skill: {iSkill.skillName}");
                // Chat.AddMessage($"### Skilldef: {iSkill.skillDef.name}");

                SkillUtils.OverrideSkillForObject(iPreFab, oldSkill, iSkill);
            }
        }

        //The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {

            _skillDefPlayerList = new List<SurvivorSkillOverride>();
            gRandomSkillsSurvivor = this;
            On.RoR2.Run.OnServerCharacterBodySpawned += BodySpawnHook;
            
            On.RoR2.Stage.RespawnCharacter += (orig, instance, characterMaster) =>
            {
                RespawnCheckAndGenerateRandomSkills(characterMaster);
                orig(instance, characterMaster);
            };

            EntryCounter.counter = 0;

            SkillClassifier skillClassifier = new SkillClassifier();
            skillClassifier.EvaluateStuff();
            gSkillClassifierSelf = skillClassifier;
        }

        private void BodySpawnHook(On.RoR2.Run.orig_OnServerCharacterBodySpawned orig, Run self, CharacterBody body)
        {
            if (GetBody() == body)
            {
                /*
                foreach (SkillDef skillDefTemp in SkillCatalog.allSkillDefs)
                {
                    Chat.AddMessage($" {skillDefTemp.skillName} = {skillDefTemp.skillIndex},");
                }
                */

                if (EntryCounter.counter < 1)
                {
                    Chat.AddMessage($"### Respawn body, reassign new skills");
                    SurvivorSkillOverride survivorSkillOverride = GenerateRandomSkillsForSurvivor(1, body);
                    survivorSkillOverride.SetSkillsForSurvivor();
                    _skillDefPlayerList.Add(survivorSkillOverride);
                    EntryCounter.counter = 1;
                }
                else
                {
                    foreach (SurvivorSkillOverride survivorSkillOverride in _skillDefPlayerList) // Loop through List with foreach
                    {
                        if (survivorSkillOverride._player == 1)
                        {
                            Chat.AddMessage($"### Respawn body, assign previously generated skills for player 1");
                            survivorSkillOverride._characterBody = body;
                            survivorSkillOverride.SetSkillsForSurvivor();
                        }
                    }
                }
            }

            orig.Invoke(self, body);
        }

        static class EntryCounter
        {
            public static int counter;
        }

        public void RespawnCheckAndGenerateRandomSkills(CharacterMaster characterMaster)
        {
            Chat.AddMessage($"#### Respawn character1 ");

            // Reset all survivors to default skills

            Chat.AddMessage($"#### Player has picked: {characterMaster.bodyPrefab.name} ####");
        }

        private SurvivorSkillOverride GenerateRandomSkillsForSurvivor(uint player, CharacterBody characterBody)
        {
            // Calculate the actual survivor to use (both int, string, and prefab form)

            Chat.AddMessage($"#### Generating new skills for: {characterBody.GetDisplayName()} ####");
            
            SkillDef primary = new SkillDef();
            SkillDef secondary = new SkillDef();
            SkillDef utility = new SkillDef();
            SkillDef special = new SkillDef();
            for (uint skillGenerationAttempts= 0; skillGenerationAttempts < gSkillGenerationAttempts; skillGenerationAttempts++)
            {

                primary = GetValidSkillDefForSurvivor(characterBody, SkillSlot.Primary);
                secondary = GetValidSkillDefForSurvivor(characterBody, SkillSlot.Secondary);
                utility = GetValidSkillDefForSurvivor(characterBody, SkillSlot.Utility);
                special = GetValidSkillDefForSurvivor(characterBody, SkillSlot.Special);

                uint skillRating = gSkillClassifierSelf.RateSurvivorAndSkills(SurvivorIndex.Commando, primary, secondary, utility, special);

                skillGenerationAttempts += 1;
                Chat.AddMessage($"#### Character has rating {skillRating}");
                if (skillRating > gSkillRatingDesired || skillGenerationAttempts == gSkillGenerationAttempts - 1 || gUseSkillReroll == false)
                {
                    break;
                }
            }

            Chat.AddMessage($"#### End generation of new skills for: {characterBody.GetDisplayName()} ####");

            SurvivorSkillOverride skillOverride = new SurvivorSkillOverride(player, characterBody, primary, secondary, utility, special);
            return skillOverride;
        }
           
        public SkillDef GetValidSkillDefForSurvivor(CharacterBody survivor, SkillSlot skillType)
        {
            // Come up with list of skills for survivor's slot which are valid.

            var skillListForSlot = new List<SkillDef> { };
            Chat.AddMessage($"#### Finding valid skill for slot:{skillType}");

            foreach (SkillDef skillDefTemp in SkillCatalog.allSkillDefs)
            {

                bool isValidForSurvivor = gSkillClassifierSelf.IsSkillDefValidForSurvivor(survivor.name, skillDefTemp);
                SkillSlot skillSlot = gSkillClassifierSelf.SkillToSlot(skillDefTemp);
                bool isrightSlot = (skillType == skillSlot) ? true : false;

                // Chat.AddMessage($"#### JSS skill def:{skillDefTemp.skillName} index: {skillDefTemp.skillIndex} isValidForSurvivor:{isValidForSurvivor} isProperSlot:{isrightSlot}");
                if (isValidForSurvivor && isrightSlot) {
                    skillListForSlot.Add(skillDefTemp);
                }
            }

            // Randomly pick a skill from that list.

            SkillDef newSkill = skillListForSlot[(int)GetRandomInRange((uint)skillListForSlot.Count)];

            Chat.AddMessage($"### Random JSS skill def:{newSkill.skillName} index: {newSkill.skillIndex}");

            return newSkill;
        }

        // Random example command to set multiplier with
        [ConCommand(commandName = "set_skill", flags = ConVarFlags.None, helpText = "Sets the desired skill to desired slot")]
        private static void CCSetSkill(ConCommandArgs args)
        {
            args.CheckArgumentCount(2);

            if (!int.TryParse(args[0], out var skillIndex))
            {
                Debug.Log("Invalid argument skillIndex.");
                return;
            }

            if (!sbyte.TryParse(args[1], out var skillSlotInt))
            {
                Debug.Log("Invalid argument skillSlot.");
                return;
            }
            CharacterBody body = GetBody();

            SkillDef skillDef = SkillCatalog.GetSkillDef(skillIndex);
            if (skillDef == null)
            {
                Debug.Log($"Unable to find skillDef with index:{skillIndex}.");
                return;
            }

            if (Enum.IsDefined(typeof(SkillSlot), skillSlotInt) == false)
            {
                Debug.Log($"Unable to find skillSlot with value:{skillSlotInt}.");
                return;
            }

            SkillSlot skillSlot = (SkillSlot)skillSlotInt;

            foreach (SurvivorSkillOverride skillListForPlayer in _skillDefPlayerList)
            {
                if (skillListForPlayer._characterBody == body)
                {
                    if (skillSlot == SkillSlot.Primary) skillListForPlayer._primarySkill = skillDef;
                    else if (skillSlot == SkillSlot.Secondary) skillListForPlayer._secondarySkill = skillDef;
                    else if (skillSlot == SkillSlot.Utility) skillListForPlayer._utilitySkill = skillDef;
                    else if (skillSlot == SkillSlot.Special) skillListForPlayer._specialSkill = skillDef;
                    skillListForPlayer.SetSkillsForSurvivor();
                }
            }
        }
    }
}