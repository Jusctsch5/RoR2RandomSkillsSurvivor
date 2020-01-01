using RoR2;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using RoR2.Skills;

namespace RandomSkillsSurvivor
{
    static class SkillUtils
    {
        public static void OverrideSkillForObject(object source, GenericSkill oldSkill, SkillDef newSkillDef)
        {
            if (oldSkill != null && newSkillDef != null)
            { 
                oldSkill.SetSkillOverride(source, newSkillDef, GenericSkill.SkillOverridePriority.Replacement);
            } else {
                Chat.AddMessage($"Not replacing cuz something was null");
            }
        }

        public static void RemoveSkillForObject(object source, GenericSkill slot, SkillDef toRemove) =>
            slot.UnsetSkillOverride(source, toRemove, GenericSkill.SkillOverridePriority.Replacement);
    }
}