using RoR2;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using RoR2.Skills;
using System.Linq;


// Provide ability to describe a skill with a rating
namespace RandomSkillsSurvivor
{
    public class SkillClassifier
    {

        public Dictionary<string, uint> skillRating;
        public Dictionary<SurvivorIndex, uint> survivorRating;
        public Dictionary<string, int> skillSynergy;
        List<MySkillIndex> _primarySkills;
        List<MySkillIndex> _secondarySkills;
        List<MySkillIndex> _utilitySkills;
        List<MySkillIndex> _specialSkills;

        public SkillClassifier()
        {
            skillRating = new Dictionary<string, uint> { };
            survivorRating = new Dictionary<SurvivorIndex, uint> { };
            skillSynergy =  new Dictionary<string, int> {};
            _primarySkills = new List<MySkillIndex>();
            _secondarySkills = new List<MySkillIndex>();
            _utilitySkills = new List<MySkillIndex>();
            _specialSkills = new List<MySkillIndex>();
        }            

        public void EvaluateStuff()
        {
            Chat.AddMessage($"### EvaluateStuff");

            AssignSkillLocators();
            EvaluateSkills();
            EvaluateSurvivors();
            EvaluateSkillSynergy();
        }

        public void AssignSkillLocators()
        {
            List<MySkillIndex> primarySkills = new List<MySkillIndex>()
            {
                MySkillIndex.SkillIndex_FireShotgun,
                MySkillIndex.SkillIndex_FirePistol2,
                MySkillIndex.SkillIndex_CrocoSlash,
                MySkillIndex.SkillIndex_FireSeekingArrow,
                MySkillIndex.SkillIndex_FireBuzzsaw,
                MySkillIndex.SkillIndex_FireGrenadeLauncher,
                MySkillIndex.SkillIndex_FireNailgun,
                MySkillIndex.SkillIndex_FireSpear,
                MySkillIndex.SkillIndex_FireSeekingArrow,
                MySkillIndex.SkillIndex_FireFirebolt,
                MySkillIndex.SkillIndex_FireLightningBolt,
                MySkillIndex.SkillIndex_GroundLight,
                MySkillIndex.SkillIndex_FireSyringe,
                MySkillIndex.SkillIndex_FireGrenade2,

                MySkillIndex.SkillIndex_LunarPrimaryReplacement,
                MySkillIndex.SkillIndex_FireShotgun,
            };

            List<MySkillIndex> secondarySkills = new List<MySkillIndex>()
            {
                MySkillIndex.SkillIndex_FireFMJ2,
                MySkillIndex.SkillIndex_FireShotgunBlast,
                MySkillIndex.SkillIndex_StunDrone,
                MySkillIndex.SkillIndex_Glaive,
                MySkillIndex.SkillIndex_Whirlwind,
                MySkillIndex.SkillIndex_Uppercut,
                MySkillIndex.SkillIndex_IceBomb,
                MySkillIndex.SkillIndex_NovaBomb,
                MySkillIndex.SkillIndex_PlaceSpiderMine,
                MySkillIndex.SkillIndex_AimMortar2,
                MySkillIndex.SkillIndex_AimMortarRain,
                MySkillIndex.SkillIndex_FireHook1,
                MySkillIndex.SkillIndex_FireYankHook,

                MySkillIndex.SkillIndex_CrocoPoison,

                MySkillIndex.SkillIndex_LightsOut,

            };

            List<MySkillIndex> utilitySkills = new List<MySkillIndex>()
            {
                MySkillIndex.SkillIndex_Blink2,
                MySkillIndex.SkillIndex_Blink3,
                MySkillIndex.SkillIndex_ToolbotDash,
                MySkillIndex.SkillIndex_Wall,
                MySkillIndex.SkillIndex_SonicBoom2,
                MySkillIndex.SkillIndex_SonicBoom,
                MySkillIndex.SkillIndex_ChargeFist,
                MySkillIndex.SkillIndex_ChargeZapFist,
                MySkillIndex.SkillIndex_CrocoLeap,
                MySkillIndex.SkillIndex_CrocoChainableLeap,
                MySkillIndex.SkillIndex_FlyUp,
                MySkillIndex.SkillIndex_PlaceBubbleShield,

                MySkillIndex.SkillIndex_Cloak,
                // MySkillIndex.SkillIndex_LunarUtilityReplacement, doesn't work
            };

            List<MySkillIndex> specialSkills = new List<MySkillIndex>()
            {
                MySkillIndex.SkillIndex_Barrage1,
                MySkillIndex.SkillIndex_ArrowRain,
                MySkillIndex.SkillIndex_Evis1,
                MySkillIndex.SkillIndex_Evis2,
                MySkillIndex.SkillIndex_Flamethrower2,
                MySkillIndex.SkillIndex_PlaceTurret,
                MySkillIndex.SkillIndex_PlaceWalkerTurret,
                MySkillIndex.SkillIndex_FireFlower2,
                MySkillIndex.SkillIndex_CrocoDisease,
                MySkillIndex.SkillIndex_LoaderSatellite,
            };

            _primarySkills = _primarySkills.Concat(primarySkills).ToList();
            _secondarySkills= _secondarySkills.Concat(secondarySkills).ToList();
            _utilitySkills =  _utilitySkills.Concat(utilitySkills).ToList();
            _specialSkills = _specialSkills.Concat(specialSkills).ToList();

        }

        public void ValidateSkills()
        {
            Chat.AddMessage($"primary");
            foreach (int skillIndex in _primarySkills)
            {
                Chat.AddMessage($"skill:{skillIndex}");
            }

            Chat.AddMessage($"secondary");
            foreach (int skillIndex in _secondarySkills)
            {
                Chat.AddMessage($"skill:{skillIndex}");
            }

            Chat.AddMessage($"utility");
            foreach (int skillIndex in _utilitySkills)
            {
                Chat.AddMessage($"skill:{skillIndex}");
            }

            Chat.AddMessage($"special");
            foreach (int skillIndex in _specialSkills)
            {
                Chat.AddMessage($"skill:{skillIndex}");

            }
        }
        public void EvaluateSkills()
        {
            Chat.AddMessage($"#### EvaluateSkills");

            skillRating["FirePistol"] = 20;
            skillRating["FireFMJ"] = 19;
            skillRating["Roll"] = 20;
            skillRating["Barrage"] = 21;

            skillRating["FireGrenade"] = 18;
            skillRating["PlaceMine"] = 18;
            skillRating["PlaceBubbleShield"] = 19;
            skillRating["PlaceTurret"] = 24;

            skillRating["FireSeekingArrow"] = 19;
            skillRating["Glaive"] = 22;
            skillRating["Blink"] = 20;
            skillRating["ArrowRain"] = 19;

            skillRating["FireFirebolt"] = 20;
            skillRating["NovaBomb"] = 22;
            skillRating["Wall"] = 20;
            skillRating["Flamethrower"] = 18;

            skillRating["GroundLight"] = 18;
            skillRating["Whirlwind"] = 20;
            skillRating["Dash"] = 22;
            skillRating["Evis"] = 22;

            skillRating["FireSpear"] = 18;
            skillRating["FireNailgun"] = 22;
            skillRating["StunDrone"] = 21;
            skillRating["ToolbotDash"] = 20;
            skillRating["Swap"] = 20;

            skillRating["FireSyringe"] = 20;
            skillRating["AimMortar2"] = 19;
            skillRating["SonicBoom"] = 18;
            skillRating["FireFlower2"] = 22;

            skillRating["FireShotgun"] = 21;
            skillRating["LightsOut"] = 20;
            skillRating["Cloak"] = 20;
            skillRating["Grenade"] = 20;
        }

        public void EvaluateSurvivors()
        {
            Chat.AddMessage($"#### EvaluateSurvivors");

            survivorRating[SurvivorIndex.Commando] = 20;
            survivorRating[SurvivorIndex.Engi] = 20;
            survivorRating[SurvivorIndex.Huntress] = 21;
            survivorRating[SurvivorIndex.Bandit] = 20;
            survivorRating[SurvivorIndex.Mage] = 22;
            survivorRating[SurvivorIndex.Merc] = 23;
            survivorRating[SurvivorIndex.Toolbot] = 19;
            survivorRating[SurvivorIndex.Treebot] = 20;
            survivorRating[SurvivorIndex.Loader] = 21;
        }

        public void EvaluateSkillSynergy()
        {
            Chat.AddMessage($"#### EvaluateSkillSynergy");

            skillSynergy["LightsOut" + "PlaceTurret"] = 1;
            skillSynergy["LightsOut" + "FireFlower2"] = 1;
            skillSynergy["PlaceBubbleShield" + "PlaceTurret"] = 1;
            skillSynergy["GroundLight" + "Dash"] = 2;
            skillSynergy["AimMortar2" + "FireFlower2"] = 3;
        }

        public uint FindRatingForSkill(string skillName)
        {
            uint value;
            if (skillRating.TryGetValue(skillName, out value))
            {
                return value;
            }
            return 20;
        }

        public uint FindRatingForSurvivor(SurvivorIndex iSurvivorIndex)
        {
            uint value;
            if (survivorRating.TryGetValue(iSurvivorIndex, out value))
            {
                return value;
            }
            return 20;
        }

        public int FindRatingForSynergy(string skillNames)
        {
            int value;
            if (skillSynergy.TryGetValue(skillNames, out value))
            {
                return value;
            }
            return 0;
        }
        public int FindModifierForSynergy(List<SkillDef> iSkillList)
        {
            int modifier = 0;
            foreach (var skillOne in iSkillList)
            {
                foreach (var skillTwo in iSkillList)
                {
                    if (string.Compare(skillOne.skillName, skillTwo.skillName) == 0)
                    {
                        continue;
                    }

                    string combination = skillOne.skillName + skillTwo.skillName;
                    modifier += FindRatingForSynergy(combination);
                }
            }

            return modifier;
        }

        public uint RateSurvivorAndSkills(SurvivorIndex iSurvivorIndex, SkillDef primary, SkillDef secondary, SkillDef utility, SkillDef special)
        {

            // Rate Survivor

            uint rating = FindRatingForSurvivor(iSurvivorIndex);

            // list skills to iterate over.

            var skillList = new List<SkillDef> {};
            skillList.Add(primary);
            skillList.Add(secondary);
            skillList.Add(utility);
            skillList.Add(special);

            // Evaluate the skills individually

            foreach (var skill in skillList)
            {
                rating += FindRatingForSkill(skill.skillName);
            }

            // Evaluate skill pairs, or abscense of skill pairs

            int modifier = FindModifierForSynergy(skillList);

            rating = (uint) ((int)rating + modifier);
            return rating;
        }

        public bool IsSkillDefValidForSurvivor(string survivorName, SkillDef skill)
        {
            var skillName = skill.skillName;

            // Skills that don't work

            if (skillName == "Blink" || skillName == "Flamethrower" || skillName == "Grenade")
            {
                return false;
            }

            // Skills that only work on certain survivors

            if ((survivorName != "MercenaryBody") && 
                (skillName == "GroundLight" || skillName == "Whirlwind" || skillName == "Uppercut" || skillName == "Dash" || skillName == "Evis"))
            {
                return false;
            }

            if ((survivorName != "HuntressBody") &&
                (skill.skillIndex == (int)MySkillIndex.SkillIndex_FireSeekingArrow ||
                skill.skillIndex == (int)MySkillIndex.SkillIndex_Glaive)) 
            { 
                return false;
            }

            if ((survivorName != "EngiBody") &&
                (skillName == "PlaceMine" || skillName == "FireGrenade"))
            {
                return false;
            }

            if ((survivorName != "LoaderBody") &&
                 (skill.skillIndex == (int)MySkillIndex.SkillIndex_FireHook1 || 
                  skill.skillIndex == (int)MySkillIndex.SkillIndex_FireYankHook ||
                  skill.skillIndex == (int)MySkillIndex.SkillIndex_ChargeFist ||
                  skill.skillIndex == (int)MySkillIndex.SkillIndex_ChargeZapFist ||
                  skill.skillIndex == (int)MySkillIndex.SkillIndex_SonicBoom ||
                  skill.skillIndex == (int)MySkillIndex.SkillIndex_LoaderSatellite))
            {
                return false;
            }

            if ((survivorName != "CrocoBody") &&
                (skill.skillIndex == (int)MySkillIndex.SkillIndex_CrocoSlash))
            {
                return false;
            }
            
            return true;
        }

        public SkillSlot SkillToSlot(SkillDef skill)
        {           
            if (_primarySkills.Contains((MySkillIndex)skill.skillIndex)) {
                return SkillSlot.Primary;
            }
            else if (_secondarySkills.Contains((MySkillIndex)skill.skillIndex))
            {
                return SkillSlot.Secondary;
            }
            else if (_utilitySkills.Contains((MySkillIndex)skill.skillIndex))
            {
                return SkillSlot.Utility;
            }
            else if (_specialSkills.Contains((MySkillIndex)skill.skillIndex))
            {
                return SkillSlot.Special;
            }
            return SkillSlot.None;
        }
    }
}