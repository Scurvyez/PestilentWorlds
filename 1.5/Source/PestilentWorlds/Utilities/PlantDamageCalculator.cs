using RimWorld;
using UnityEngine;
using Verse;

namespace PestilentWorlds
{
    public static class PlantDamageCalculator
    {
        /// <summary>
        /// Should calculate the base damage amount applied to a plant.
        /// </summary>
        /// <param name="damageAmount">The final amount of damage to apply, as a base value.</param>
        /// <returns>0 (if chance fails), damageAmount (if chance succeeds).</returns>
        public static float TryGetInsectDamageForPlantCategory(float damageAmount)
        {
            return !(damageAmount > 0f) ? 0f : Mathf.Max(0f, damageAmount);
        }
        
        /// <summary>
        /// Should calculate the damage amount applied to a plant, as a factor of the plants' growth percentage.
        /// </summary>
        /// <param name="targetPlant">The plant to apply damage too.</param>
        /// <returns>1 (if chance fails), factor between 1-5 (if chance succeeds).</returns>
        public static float TryGetInsectDamageFactorForPlantGrowth(Plant targetPlant)
        {
            if (targetPlant == null)
                return 1f;
            
            float curGrowth = Mathf.Clamp01(targetPlant.Growth);
            float factor = Mathf.Lerp(5f, 1f, curGrowth);
            return factor;
        }

        /// <summary>
        /// Should calculate a damage factor based on the target plant's current hitpoints.
        /// The factor should increase logarithmically as the plant takes more damage.
        /// </summary>
        /// <param name="targetPlant">The plant to calculate the damage factor for.</param>
        /// <returns>1 (at full health) and 5 (at very low health).</returns>
        public static float TryGetDamageFactorForPlantHealth(Plant targetPlant)
        {
            if (targetPlant is not { HitPoints: > 0 } || targetPlant.MaxHitPoints <= 0)
                return 5f;

            float healthPct = (float)targetPlant.HitPoints / targetPlant.MaxHitPoints;
            float factor = Mathf.Lerp(5f, 1f, Mathf.Log10(1f + healthPct * 9f));
            return factor;
        }
    }
}
