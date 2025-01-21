using RimWorld;
using UnityEngine;
using Verse;

namespace PestilentWorlds
{
    public static class Logging
    {
        public static void LogPlantDamageData(Plant plant, float baseDamage, 
            float growthDamageFactor, float healthDamageFactor, float finalDamageAmount)
        {
            PWLog.Message($"{plant.LabelCap}, " +
                          $"at {plant.Position}, " +
                          $"base damage: {baseDamage:F5}, " +
                          $"growth factor: {growthDamageFactor:F2}, " +
                          $"health factor: {healthDamageFactor:F2}, " +
                          $"damaged for: {Mathf.CeilToInt(finalDamageAmount * 
                                                          Constants.PlantTickLong)}");
        }

        public static void LogIncidentCauseData(string defName, Plant plant)
        {
            PWLog.Warning($"Incident def: {defName}");
            PWLog.Warning($"Incident fired because of plant: {plant.LabelCap} " +
                          $"at position: {plant.Position}");
        }
    }
}