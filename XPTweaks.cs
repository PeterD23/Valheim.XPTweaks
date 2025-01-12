using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XPTweaks
{
    [BepInPlugin("XPTweaks", "XP Gain Tweaks", "1.0.0")]
    [BepInProcess("valheim.exe")]
    public class XPTweaks : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("XPTweaks");
        private static ManualLogSource Logs;

        public static HashSet<Character> NoOfTargetsHit = new HashSet<Character>();

        private void Awake()
        {
            harmony.PatchAll();
            Logs = Logger;
            Logs.LogInfo("XP Tweaks loaded successfully!");
        }

        [HarmonyPatch(typeof(FishingFloat))]
        class Fish_Experience_On_Catch
        {
            [HarmonyPatch(nameof(FishingFloat.Catch))]
            [HarmonyPostfix]
            static void Patch_Escaping_Usage(ref string __result, Fish fish, Character owner)
            {
                // Set Multiplier based on Fish Quality (Size)
                ItemDrop item = fish.gameObject.GetComponent<ItemDrop>();
                float qualityXpMultiplier = Mathf.Max(1, item.m_itemData.m_quality);

                // Set Multiplier based on Fish Struggling Stamina Cost
                float staminaCostXpMultiplier = fish.m_escapeStaminaUse / 2f;

                // Start with 1 XP (0.25 x 4) and multiply by (escape cost/2) then quality
                // Perch Lv 1: 1 * 5 * 1 = 5 XP
                // Pike Lv 3: 1 * 7.5 * 3 = 22.5 XP
                // Anglerfish Lv 2: 1 * 19 * 2  = 38 XP
                // Northern Salmon Lv 5: 1 * 30 * 5 = 150 XP

                float experience = (float)Math.Round(1 * staminaCostXpMultiplier * qualityXpMultiplier, 1);
                Logs.LogInfo($"Fish catch XP is {experience} from 1 * {fish.m_escapeStaminaUse}/10 * {qualityXpMultiplier}");
                owner.RaiseSkill(Skills.SkillType.Fishing, experience * 4);
                __result += $" ({experience} XP) ";
            }
        }

        [HarmonyPatch(typeof(Player))]
        class Raise_Skill_Based_on_Targets
        {
            private readonly static int[] _applicableSkills = { 1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 12, 14 };

            [HarmonyPatch(nameof(Player.RaiseSkill))]
            [HarmonyPrefix]
            static void Raise_Skill_Based_On_Targets(Skills.SkillType skill, ref float value)
            {
                if (_applicableSkills.Contains((int) skill))
                {
                    Logs.LogInfo($"NoOfTargetsHit is {NoOfTargetsHit.Count}");
                    var bonusXp = Mathf.Max(0, (value / 2) * (NoOfTargetsHit.Count - 1));
                    Logs.LogInfo($"Adding XP: {value} + {bonusXp}");
                    value += bonusXp;
                    NoOfTargetsHit.Clear();
                    Logs.LogInfo($"Reset NoOfTargetsHit, now {NoOfTargetsHit.Count}");
                }
            }
        }

        [HarmonyPatch(typeof(Character))]
        class Track_Number_Of_Targets_Hit_Player
        {
            [HarmonyPatch(nameof(Character.Damage))]
            [HarmonyPostfix]
            static void Get_Projectile_Owner(HitData hit, Character __instance)
            {
                if (hit.GetAttacker() == Player.m_localPlayer)
                {
                    NoOfTargetsHit.Add(__instance);
                }
            }
        }
    }
}
